using Godot;
using System;
using System.Collections.Generic;
using Com.IsartDigital.Sokoban.Managers;
using Com.IsartDigital.Sokoban.Architectures;
using Com.IsartDigital.Sokoban.UI.UIElements;
using Com.IsartDigital.Utils.Events;

// Author : Lefevre Florian
namespace Com.IsartDigital.Sokoban.UI.Screen {

	public class Scoreboard : Control
	{
		[Export]
		private List<Color> scoreColors = new List<Color>()
		{
			Colors.Gold, Colors.Silver, Colors.Brown, Colors.White
		};

		[Export(PropertyHint.Range, "0,100,1,or_greater")] private int nScore = 10;
		[Export] private NodePath scoreContainerPath = default;
		[Export] private NodePath scorePlayerPath = default;

		[Export] private PackedScene scoreLabelPrefab = null;

		[Export] private float scoreLabelPrintDuration;
		[Export(PropertyHint.Range, "0,1,0.1")] private float scoreLabelPrintModulate;
		[Export] private Tween.TransitionType scoreLabelTransition = default;
		[Export] private Tween.EaseType scoreLabelEaseType = default;
		[Export] private bool animateTextPrinting = false;

		[Export] private List<NodePath> quitButtonPaths = default;

		private const string PROPERTY_PANEL_MODULATE_A = "modulate:a";

		private List<Button> quitButtons = new List<Button>();

		/// <summary>
		/// - If the player is connected check is score and get is position in the leaderboard
		/// - Display the leaderboard
		/// - Potentialy display the player score if he is not in the leaderboard (> 10)
		/// </summary>
		public override void _Ready()
		{
			// Quit buttons declaration + connect
			Button lButton = null;
            foreach (NodePath lPath in quitButtonPaths)
            {
				lButton = GetNode<Button>(lPath);
				lButton.Connect(EventButton.PRESSED, this, nameof(Quit));
				quitButtons.Add(lButton);
            }

			Connect(EventNode.TREE_EXITING, this, nameof(Destructor));
		}

		public void DrawScoreboard(int pLevelIndex)
        {
			ScoreManager lScoreManager = ScoreManager.GetInstance();

			// Print scores
			List<Tuple<string, int>> lScores = lScoreManager.GetScores(pLevelIndex);
			if (lScores != null)
			{
				VBoxContainer lScoreContainer = GetNode<VBoxContainer>(scoreContainerPath);

				int lLength = (lScores.Count < nScore) ? lScores.Count : nScore;
				Color lLabelColor;

				SceneTreeTween lTween = GetTree().CreateTween();
				lTween.Chain();
				for (int i = 0; i < lLength; i++)
				{
					lLabelColor = (i > scoreColors.Count - 1) ? scoreColors[scoreColors.Count - 1] : scoreColors[i];
					lScoreContainer.AddChild(CreateScoreLabel(lScores[i], lTween, lLabelColor, (i + 1).ToString()));
				}

				if (DatabaseManager.User != null && !lScoreManager.IsPlayerInTheLeaderboard(lScores, (User)DatabaseManager.User))
				{
					User? lUser = DatabaseManager.User;
					lScoreContainer = GetNode<VBoxContainer>(scorePlayerPath);
					lScoreContainer.AddChild(CreateScoreLabel(new Tuple<string, int>(((User)lUser).Username, ((User)lUser).scores[pLevelIndex]),
															  lTween,
															  scoreColors[scoreColors.Count - 1],
															  ""));
				}

				lTween.Play();
			}
		}

		private ScoreLabel CreateScoreLabel(Tuple<string, int> pUserScore, SceneTreeTween pTween, Color pColor, string pRank)
        {
			ScoreLabel lScoreLabel = scoreLabelPrefab.Instance<ScoreLabel>();
			lScoreLabel.Init(pRank, pUserScore.Item2, pUserScore.Item1, pColor, animateTextPrinting);

			lScoreLabel.Modulate = new Color(lScoreLabel.Modulate.r, lScoreLabel.Modulate.g, lScoreLabel.Modulate.b, 0f);

			pTween.TweenProperty(lScoreLabel, PROPERTY_PANEL_MODULATE_A, scoreLabelPrintModulate, scoreLabelPrintDuration)
						  .SetTrans(scoreLabelTransition)
						  .SetEase(scoreLabelEaseType);

			if (animateTextPrinting) pTween.TweenCallback(lScoreLabel, nameof(lScoreLabel.PlayAnimation));

			return lScoreLabel;
		}

		private void Quit() => QueueFree();

		/// <summary>
		/// Use this method to clean the object while it's exiting the scene (Free reference / Disconnect signals...)
		/// </summary>
		private void Destructor()
        {
			foreach (Button lButton in quitButtons)
				lButton.Disconnect(EventButton.PRESSED, this, nameof(Quit));
			quitButtons.Clear();
			Disconnect(EventNode.TREE_EXITING, this, nameof(Destructor));
        }

	}

}