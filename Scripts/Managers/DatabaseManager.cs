using Godot;
using System;
using System.Collections.Generic;
using Com.IsartDigital.Utils.Events;
using Com.IsartDigital.Sokoban.Architectures;
using Com.IsartDigital.Security;
using Com.IsartDigital.Sokoban.Localization;

// Author : Lefevre Florian
namespace Com.IsartDigital.Sokoban.Managers {
	
    public class DatabaseManager : Node
    {
        private static DatabaseManager instance;

        private const string DATABASE_FILE_PATH = "res://Ressources/database.json";

        #region JSON Keys
        // Global scop
        private const string KEY_MAIN = "users";
        // Main informations
        private const string KEY_USERNAME = "name";
        private const string KEY_PASSWORD = "pwd";
        private const string KEY_LEVEL_UNLOCKED = "level_unlocked";
        private const string KEY_SCORES = "scores";
        // Configuration informations
        private const string KEY_CONFIG = "Configuration";
        private const string KEY_LANGAGE = "Langage";
        private const string KEY_SFX = "SFX";
        private const string KEY_MUSIC = "Music";
        #endregion

        // This represent all the players in the database 
        private List<User> users = new List<User>();

        // This represent the player connected (Session cookie)
        private static User? _user = null;
        public static User? User
        {
            get { return _user; }
            set { _user = value; }
        }

        public List<User> Users
        {
            get { return users; }
            private set { users = value; }
        }

        // Signal emitted when a player login or logout
        [Signal]
        public delegate void CookieUpdated();

        private DatabaseManager ():base() {}

        public override void _Ready()
        {
            if (instance != null){  
                QueueFree();
                return;
            }
            instance = this;

            Connect(EventNode.TREE_EXITING, this, nameof(Destructor));

            ReadDatabase();
        }

        public static DatabaseManager GetInstance()
        {
            if (instance == null) instance = new DatabaseManager();
            return instance;
        }

        public void ReadDatabase() 
        {
            File lFile = new File();
            if (lFile.FileExists(DATABASE_FILE_PATH))
            {
                lFile.Open(DATABASE_FILE_PATH, File.ModeFlags.Read);
                object lJSON = JSON.Parse(lFile.GetAsText()).Result;
                lFile.Close();
                if(lJSON != null && lJSON is Godot.Collections.Dictionary)
                {
                    User lUser;
                    Godot.Collections.Dictionary lInfo;
                    foreach (Godot.Collections.Dictionary lItem in (Godot.Collections.Array)((Godot.Collections.Dictionary)lJSON)[KEY_MAIN])
                    {
                        lInfo = (Godot.Collections.Dictionary)lItem[KEY_CONFIG];
                        lUser = new User((string)lItem[KEY_USERNAME],
                                         (string)lItem[KEY_PASSWORD],
                                         Convert.ToInt32(lItem[KEY_LEVEL_UNLOCKED]),
                                         (Godot.Collections.Array)lItem[KEY_SCORES],
                                         (float)lInfo[KEY_SFX],
                                         (float)lInfo[KEY_MUSIC],
                                         Convert.ToInt32(lInfo[KEY_LANGAGE]));
                        users.Add(lUser);
                    }
                }
            }
        }

        public User? SelectUserFromDatabase(string pUsername, string pPassword) 
        {
            pPassword = Hasher.HashPassword(pPassword);
            User? lUser = null;
            if (users.Exists(lItem => lItem.Username == pUsername && lItem.Password == pPassword))
                lUser = users.Find(lItem => lItem.Username == pUsername && lItem.Password == pPassword);
            return lUser;
        }

        public void InitCookie(User pUser) {
            _user = pUser;
            LocalizationManager.GetInstance().SaveLanguage((Languages)pUser.Langage);
            SoundManager.GetInstance().SaveVolumeConfiguration(pUser.SFXVolume, pUser.MusicVolume);
            EmitSignal(nameof(CookieUpdated));
        }

        public void FreeCookie()
        {
            if(_user != null)
            {
                LocalizationManager.GetInstance().SaveLanguage(Languages.English);
                SoundManager.GetInstance().SaveVolumeConfiguration((float)SoundManager.BASE_VOLUME_SFX,
                                                                   (float)SoundManager.BASE_VOLUME_MUSIC);
                _user = null;
                EmitSignal(nameof(CookieUpdated));
            }
        }

        public void InsertIntoDatabase() { }

        public void UpdateValuesFromDatabase() { }

        public void SaveDatabase()
        {
            if (_user == null) return;
            Godot.Collections.Array lUsers = new Godot.Collections.Array();

            users[users.FindIndex(lItem => lItem.Username == ((User)_user).Username)] = (User)_user;

            Godot.Collections.Dictionary lStructure;
            Godot.Collections.Dictionary lInfo;
            foreach (User lItem in users)
            {
                lStructure = new Godot.Collections.Dictionary();
                lStructure.Add(KEY_USERNAME, lItem.Username);
                lStructure.Add(KEY_PASSWORD, lItem.Password);
                lStructure.Add(KEY_LEVEL_UNLOCKED, lItem.levelUnlocked);
                lStructure.Add(KEY_SCORES, lItem.GetScoresToArray());

                lInfo = new Godot.Collections.Dictionary();
                lInfo.Add(KEY_SFX, lItem.SFXVolume);
                lInfo.Add(KEY_MUSIC, lItem.MusicVolume);
                lInfo.Add(KEY_LANGAGE, (int)lItem.Langage);

                lStructure.Add(KEY_CONFIG, lInfo);

                lUsers.Add(lStructure);
            }
            lStructure = new Godot.Collections.Dictionary();
            lStructure.Add(KEY_MAIN, lUsers);

            File lFile = new File();
            lFile.Open(DATABASE_FILE_PATH, File.ModeFlags.Write);
            lFile.StoreLine(JSON.Print(lStructure));
            lFile.Close();
        }

        /// <summary>
        /// Call when the node exit the Godot tree (in our case when the game is closing)
        /// Used to detect when to save the data in the database
        /// </summary>
        private void Destructor()
        {
            Disconnect(EventNode.TREE_EXITING, this, nameof(Destructor));
            if(_user != null)
                SaveDatabase();
            _user = null;
        }

        protected override void Dispose(bool pDisposing)
        {
            if (pDisposing && instance == this) instance = null;
            base.Dispose(pDisposing);
        }
    }
}