using Godot;
using System;
using Com.IsartDigital.Utils.Events;
using Com.IsartDigital.Sokoban.Managers;
using Com.IsartDigital.Sokoban.TerrainGeneration;
using Com.IsartDigital.Sokoban.UI.Screen;

//Author : VERDIER Thomas 

namespace Com.IsartDigital.Sokoban.UI.LevelSelector {

	public class FlagLevel : Area2D
	{
		private const string LABEL_LEVEL_NAME = "LABEL_LEVEL";
		private const string LEVEL_PACKAGE_PATH = "res://Scenes/LevelPackage.tscn";

		// References
		[Export] private PackedScene levelPackagePackedScene;

		//Purely modulable
		[Export] private NodePath buttonPlayPath = default;
		private Button buttonPlay = null;
		[Export] private NodePath infoLevelPanelPath = default;
		private Control infoLevelPanel = null;
		[Export] private NodePath infoLevelLabelPath = default;
		private Label infoLevelLabel = null;
		[Export] private NodePath levelBGLabelPath = default;
		private Label levelBGLabel = null;
		[Export] private NodePath numLevelLabelPath = default;
		private Label numLevelLabel = null;
		[Export] private NodePath buttonScorePath = default;
		private Button buttonScore = null;

		[Export] private PackedScene scoreboardPath = default;

		public int numLevel;
		Tween tween = new Tween();
		private LevelPattern linkedLevel;
		public override void _Ready()
		{
			AddChild(tween);
			linkedLevel = LevelLoader.levelPatterns[numLevel];
			buttonPlay = GetNode<Button>(buttonPlayPath);
			buttonScore = GetNode<Button>(buttonScorePath);
			infoLevelLabel = GetNode<Label>(infoLevelLabelPath);
			levelBGLabel = GetNode<Label>(levelBGLabelPath);
			infoLevelPanel = GetNode<Control>(infoLevelPanelPath);
			numLevelLabel = GetNode<Label>(numLevelLabelPath);

			buttonPlay.Connect(EventButton.BUTTON_UP, this, nameof(GenerateAndSwitchToLevel));
			buttonScore.Connect(EventButton.PRESSED, this, nameof(DisplayScore));

			Connect(EventArea2D.AREA_ENTERED, this, nameof(OnAreaEntered));
			Connect(EventArea2D.AREA_EXITED, this, nameof(OnAreaExited));

			Connect(EventNode.TREE_EXITING, this, nameof(Destructor));
		}

		public void VisualSetUp()
        {
			levelBGLabel.Text = numLevel.ToString();
			infoLevelPanel.Visible = false;
			numLevelLabel.Text = numLevel.ToString();
			if (linkedLevel.locked)
				buttonPlay.Visible = false;
		}

		private void GenerateAndSwitchToLevel()
        {
			SceneChanger.GetInstance().ChangeScene(levelPackagePackedScene);
			GridManager.GetInstance().SetLevelIndex(numLevel);
        }

		private void OnAreaEntered(Area2D pCollisionned)
        {
			levelBGLabel.Visible = false;
			BGPopUp();
        }

		private void OnAreaExited (Area2D pCollisionned)
		{
			levelBGLabel.Visible = true;
			BGPopOut();
		}

		private void BGPopUp()
        {
			infoLevelPanel.Visible = true;
			tween.InterpolateProperty(infoLevelPanel, "rect_scale", Vector2.Zero, Vector2.One, 0.2f);
			tween.Start();
        }

		private void BGPopOut()
		{
			tween.InterpolateProperty(infoLevelPanel, "rect_scale", Vector2.One, Vector2.Zero, 0.2f);
			tween.Start();
		}

		private void DisplayScore()
        {
			Scoreboard lScoreboard = scoreboardPath.Instance<Scoreboard>();
			GetParent().AddChild(lScoreboard);
			lScoreboard.DrawScoreboard(numLevel);
        }

		private void Destructor()
        {
			buttonPlay.Disconnect(EventButton.BUTTON_UP, this, nameof(GenerateAndSwitchToLevel));
			buttonScore.Disconnect(EventButton.PRESSED, this, nameof(DisplayScore));

			Disconnect(EventArea2D.AREA_ENTERED, this, nameof(OnAreaEntered));
			Disconnect(EventArea2D.AREA_EXITED, this, nameof(OnAreaExited));

			Disconnect(EventNode.TREE_EXITING, this, nameof(Destructor));
		}

	}

}