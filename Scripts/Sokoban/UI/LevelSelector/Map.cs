using Godot;
using System;
using Com.IsartDigital.Utils.Events;
using Com.IsartDigital.Sokoban.TerrainGeneration;
using Com.IsartDigital.Sokoban.Architectures;
using Com.IsartDigital.Sokoban.Managers;

//Author : VERDIER Thomas 
namespace Com.IsartDigital.Sokoban.UI.LevelSelector {

	public class Map : Node2D
	{
		private static Map instance;

		//Max unlocked level
		public static int unlockedLevel = -1;

		[Export] NodePath levelZoneHandlerPath;
		public Node2D levelZoneHandler;

		[Export] NodePath unlockAllButtonPath;
		Button unlockAllButton;

		[Export] NodePath selectorPlayerPath;
		SelectorPlayer selectorPlayer;

		public override void _Ready()
		{

			if (instance != null )
            {
				QueueFree();
				return;
            }
			instance = this;

			levelZoneHandler = GetNode<Node2D>(levelZoneHandlerPath);
			selectorPlayer = GetNode<SelectorPlayer>(selectorPlayerPath);
			
			if (DatabaseManager.User != null)
			{
				User lUser;
				lUser = DatabaseManager.User.Value;
				unlockedLevel = lUser.levelUnlocked;
			}

			if (unlockedLevel <= 0)
				unlockedLevel = 0;
			else
				selectorPlayer.GlobalPosition = levelZoneHandler.GetChild<LevelZone>(0).GlobalPosition;

			
			if (unlockedLevel < levelZoneHandler.GetChildCount())
			{
				levelZoneHandler.GetChild<LevelZone>(unlockedLevel).ClearSequence();
			}
			unlockAllButton = GetNode<Button>(unlockAllButtonPath);
			unlockAllButton.Connect(EventButton.BUTTON_UP, this, nameof(UnlockAll));
		}

		public static Map GetInstance()
        {
			return instance;
        }

        protected override void Dispose(bool disposing)
        {
			instance = null;
            base.Dispose(disposing);
        }

        private void UnlockAll()
        {
            foreach (LevelZone lItem in levelZoneHandler.GetChildren())
				lItem.ClearCells();
			unlockedLevel = levelZoneHandler.GetChildCount();
			if (DatabaseManager.User != null)
			{
				User lUser;
				lUser = DatabaseManager.User.Value;
				lUser.levelUnlocked = unlockedLevel;
				DatabaseManager.User = lUser;
			}
			foreach (LevelPattern lPattrern in LevelLoader.levelPatterns)
			{
				lPattrern.locked = false;
			}
			ButtonContainer.Instance.SetButtons();
		}

	}

}