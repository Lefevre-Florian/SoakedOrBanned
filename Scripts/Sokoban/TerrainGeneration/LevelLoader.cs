using Godot;
using System;
using System.Collections.Generic;
using Com.IsartDigital.Sokoban.Managers;
using Com.IsartDigital.Sokoban.Architectures;

// Author : Lefevre Florian
namespace Com.IsartDigital.Sokoban.TerrainGeneration {

	public class LevelLoader : Node2D
	{

        private static LevelLoader instance;

        private const string PATH_LEVEL_JSON = "res://Ressources/leveldesign.json";

        #region JSON Keys
        private const string LVL_DESIGN = "levelDesign";
        private const string LVL_PAR = "par";
        private const string LVL_LOCKED = "locked";
        private const string LVL_AUTHOR = "author";
        private const string LVL_MAP = "map";
        #endregion

        public static List<LevelPattern> levelPatterns = new List<LevelPattern>();

        private LevelLoader() : base() { }

        public override void _Ready()
        {
            if (instance != null)
            {
                QueueFree();
                return;
            }
            instance = this;

            LoadLevels();
        }

        public void LoadLevels()
        {
            // Start reading process
            levelPatterns.Clear();

            File lFile = new File();
            if (lFile.FileExists(PATH_LEVEL_JSON))
            {
                lFile.Open(PATH_LEVEL_JSON, File.ModeFlags.Read);

                if (levelPatterns.Count > 0)
                    levelPatterns.Clear();

                // Translating JSON to LevelPattern
                object lDatas = JSON.Parse(lFile.GetAsText()).Result;
                if (lDatas is Godot.Collections.Dictionary)
                {
                    Godot.Collections.Dictionary lDict = lDatas as Godot.Collections.Dictionary;
                    foreach (Godot.Collections.Dictionary lItem in lDict[LVL_DESIGN] as Godot.Collections.Array)
                    {
                        levelPatterns.Add(new LevelPattern(Convert.ToInt32(lItem[LVL_PAR]),
                                                           (bool)lItem[LVL_LOCKED],
                                                           (string)lItem[LVL_AUTHOR],
                                                           (Godot.Collections.Array)lItem[LVL_MAP]));
                    }
                }

                lFile.Close();
            }
        }

        public static LevelLoader GetInstance()
        {
            if (instance == null) instance = new LevelLoader();
            return instance;
        }

        /// <summary>
        /// Level marked with "locked" at false will be return 
        /// or if the player is connected return the number of levels completed
        /// </summary>
        /// <returns></returns>
        public static List<LevelPattern> GetUnlockedLevels()
        {
            List<LevelPattern> lUnlockedLevels = new List<LevelPattern>();
            if (DatabaseManager.User != null)
            {            
                lUnlockedLevels = levelPatterns.GetRange(0, ((User)DatabaseManager.User).levelUnlocked);
            }
            else
                lUnlockedLevels = levelPatterns.FindAll(lLevel => !lLevel.locked);
            return lUnlockedLevels;
        }

        /// <summary>
        /// Level marked with "locked" at true will be return 
        /// or if the player is connected return the number of levels non yet completed
        /// </summary>
        /// <returns></returns>
        public static List<LevelPattern> GetLockedLevels()
        {
            List<LevelPattern> lLockedLevels = new List<LevelPattern>();
            if (DatabaseManager.User != null)
                lLockedLevels = levelPatterns.GetRange(((User)DatabaseManager.User).levelUnlocked, levelPatterns.Count - 1);
            else
                lLockedLevels = levelPatterns.FindAll(lLevel => lLevel.locked);
            return lLockedLevels;
        }

        public static List<LevelPattern> GetAllLevels()
        {
            List<LevelPattern> lLevels = new List<LevelPattern>();
            if (DatabaseManager.User != null)
            {
                int lNLevelUnlocked = ((User)DatabaseManager.User).levelUnlocked;
                int lLength = levelPatterns.Count;
                LevelPattern lLevelPattern;
                for (int i = 0; i < lLength; i++)
                {
                    lLevelPattern = new LevelPattern(levelPatterns[i]);
                    lLevels.Add(lLevelPattern);
                    if (i <= lNLevelUnlocked) lLevelPattern.locked = false;
                    else lLevelPattern.locked = true;
                }
            }
            else lLevels = levelPatterns;
            return lLevels;
        }

        protected override void Dispose(bool pDisposing)
        {
            if (pDisposing && instance == this) instance = null;
            base.Dispose(pDisposing);
        }

    }

}