using Godot;
using System;
using Com.IsartDigital.Utils.Events;
using Com.IsartDigital.Sokoban.Managers;
using Com.IsartDigital.Sokoban.Architectures;

// Author : Lefevre Florian
namespace Com.IsartDigital.Sokoban.UI.Screen {
	
    public class Login : Control
    {

        [Export] private NodePath logoutPanelPath = default;
        [Export] private NodePath loginPanelPath = default;

        /// Login
        [Export] private NodePath usernameLabelPath = default;
        [Export] private NodePath passwordLabelPath = default;
        [Export] private NodePath passwordHideBtnPath = default;

        [Export] private NodePath loginBtnPath = default;
        [Export] private NodePath quitBtnPath = default;

        [Export] private NodePath errorLabelPath = default;

        /// Logout
        [Export] private NodePath logoutBtnPath = default;
    
        private LineEdit passwordLabel = new LineEdit();

        private Button loginButton = null;
        private Button quitButton = null;
        private Button logoutButton = null;
        private CheckButton checkButton = null;

        private Label errorLabel = null;

        private DatabaseManager databaseManager = null;

        public override void _Ready()
        {

            databaseManager = DatabaseManager.GetInstance();
            if (DatabaseManager.User != null)
            {
                GetNode<Panel>(logoutPanelPath).Visible = true;
                GetNode<Panel>(loginPanelPath).Visible = false;
            }
            else
            {
                GetNode<Panel>(logoutPanelPath).Visible = false;
                GetNode<Panel>(loginPanelPath).Visible = true;
            }

            passwordLabel = GetNode<LineEdit>(passwordLabelPath);

            loginButton = GetNode<Button>(loginBtnPath);
            logoutButton = GetNode<Button>(logoutBtnPath);
            quitButton = GetNode<Button>(quitBtnPath);
            checkButton = GetNode<CheckButton>(passwordHideBtnPath);

            checkButton.Pressed = passwordLabel.Secret;

            loginButton.Connect(EventButton.PRESSED, this, nameof(ActivateLogin));
            logoutButton.Connect(EventButton.PRESSED, this, nameof(ActivateLogout));
            quitButton.Connect(EventButton.PRESSED, this, nameof(Exit));
            checkButton.Connect(EventButton.PRESSED, this, nameof(TriggerPasswordLabelState));

            errorLabel = GetNode<Label>(errorLabelPath);
            errorLabel.Visible = false;

            Connect(EventNode.TREE_EXITING, this, nameof(Destructor));
        }

        /// <summary>
        /// Check if the couple Username/Password exist in the database if so create a "Session cookie" containing
        /// the player datas stored in the database
        /// else display an error message
        /// </summary>
        private void ActivateLogin()
        {
            string lUsername = GetNode<LineEdit>(usernameLabelPath).Text;
            string lPassword = passwordLabel.Text;

            User? lUser =  databaseManager.SelectUserFromDatabase(lUsername, lPassword);
            if (lUser == null && !errorLabel.Visible) errorLabel.Visible = true;
            else if(lUser != null)
            {
                databaseManager.InitCookie((User)lUser);
                Exit();
            }
        }

        private void Exit() => QueueFree();

        /// <summary>
        /// Unable the possibility for the player to display is password (Text format => 123 or Password format => ***)
        /// </summary>
        private void TriggerPasswordLabelState() => passwordLabel.Secret = checkButton.Pressed;

        private void ActivateLogout() 
        {
            databaseManager.FreeCookie();
            Exit();
        }

        private void Destructor()
        {
            if(DatabaseManager.User == null)
            {
                loginButton.Disconnect(EventButton.PRESSED, this, nameof(ActivateLogin));
                quitButton.Disconnect(EventButton.PRESSED, this, nameof(Exit));
                checkButton.Disconnect(EventButton.PRESSED, this, nameof(TriggerPasswordLabelState));
            }
            else
            {
                logoutButton.Disconnect(EventButton.PRESSED, this, nameof(ActivateLogout));
            }

            Disconnect(EventNode.TREE_EXITING, this, nameof(Destructor));
        }

    }
}