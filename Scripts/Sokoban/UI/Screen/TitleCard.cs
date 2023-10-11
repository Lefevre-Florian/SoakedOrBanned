using Godot;
using System;
using Com.IsartDigital.Utils.Events;
using Com.IsartDigital.Sokoban.Managers;
using Com.IsartDigital.Sokoban.Architectures;

//Author : VERDIER Thomas 

namespace Com.IsartDigital.Sokoban.UI.Screen {

	public class TitleCard : Control
	{
		[Export] private NodePath animationPath;
		private AnimationPlayer animation;
		[Export] private NodePath playButtonPath;
		private Button playButton;
		[Export] private NodePath optionButtonPath;
		private CustomOptionButton optionButton;
		[Export] private NodePath creditsButtonPath;
		private Button creditsButton;
		[Export] private NodePath quitButtonPath;
		private Button quitButton;
		[Export] private NodePath loginButtonPath;
		private Button loginButton;

		[Export] private NodePath loginLabelPath;
		private Label loginLabel;

		[Export(PropertyHint.File)] private string levelSelectorPath;
		[Export(PropertyHint.File)] private string creditsPath;
		[Export(PropertyHint.File)] private string loginPath;
		[Export] private PackedScene worldPackedScene;
		[Export] private PackedScene creditsPackedScene;

		private const string OFFLINE_MSG = "Offline";

		private DatabaseManager databaseManager = null;

		public override void _Ready()
		{
			Connect(EventNode.TREE_EXITING, this, nameof(Destructor));
			ButtonConnection();
			animation.Play("Enter");
		}

		private void ButtonConnection()
		{
			animation = GetNode<AnimationPlayer>(animationPath);
			playButton = GetNode<Button>(playButtonPath);
			ConnectButton(playButton,  nameof(Play)); 
			optionButton = GetNode<CustomOptionButton>(optionButtonPath);
			creditsButton = GetNode<Button>(creditsButtonPath);
			ConnectButton(creditsButton, nameof(Credits)); 
			quitButton = GetNode<Button>(quitButtonPath);
			ConnectButton(quitButton, nameof(Quit));
			loginButton = GetNode<Button>(loginButtonPath);
			ConnectButton(loginButton, nameof(Login));

			loginLabel = GetNode<Label>(loginLabelPath);
			LoginUpdated();
			databaseManager = DatabaseManager.GetInstance();
			databaseManager.Connect(nameof(DatabaseManager.CookieUpdated), this, nameof(LoginUpdated));
		}

		private void ConnectButton(Button pButton, String pMethod)
        {
			pButton.Connect(EventButton.PRESSED, this, pMethod);
        }
		
		private void Play()
        {
			SceneChanger.GetInstance().ChangeScene(worldPackedScene);
        }

		private void Credits()
        {
			SceneChanger.GetInstance().ChangeScene(creditsPackedScene);
		}

		private void Login() => GetParent().AddChild(GD.Load<PackedScene>(loginPath).Instance());

		private void Quit()
		{
			GetTree().Quit();
		}

		private void LoginUpdated()
        {
			if (DatabaseManager.User != null)
				loginLabel.Text = ((User)DatabaseManager.User).Username;
			else
				loginLabel.Text = OFFLINE_MSG;
        }

		/// <summary>
		/// Used to disconnect all the node before the main node (Control) exit the scene
		/// </summary>
		private void Destructor()
        {
			Disconnect(EventNode.TREE_EXITING, this, nameof(Destructor));

			quitButton.Disconnect(EventButton.PRESSED, this, nameof(Quit));
			playButton.Disconnect(EventButton.PRESSED, this, nameof(Play));
			creditsButton.Disconnect(EventButton.PRESSED, this, nameof(Credits));
			loginButton.Disconnect(EventButton.PRESSED, this, nameof(Login));

			if(databaseManager != null)
				databaseManager.Disconnect(nameof(DatabaseManager.CookieUpdated), this, nameof(LoginUpdated));

		}
	
	}

}