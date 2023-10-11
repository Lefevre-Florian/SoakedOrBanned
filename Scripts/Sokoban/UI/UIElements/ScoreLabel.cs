using Godot;
using System;

// Author : Lefevre Florian
namespace Com.IsartDigital.Sokoban.UI.UIElements {

	public class ScoreLabel : Panel
	{

		[Export] private NodePath rankLabelPath = default;
		[Export] private NodePath usernameLabelPath = default;
		[Export] private NodePath scoreLabelPath = default;

		[Export] private float labelPrintDuration;
		[Export] private Tween.TransitionType labelTransition = default;
		[Export] private Tween.EaseType labelEaseType = default;

		private const string PROPERTY_LABEL_FONT_COLOR = "font_color";
		private const string PROPERTY_LABEL_PERCENT_VISIBLE = "percent_visible";

		private Label rankLabel, userNameLabel, scoreLabel;

        public override void _Ready()
        {
			RectPivotOffset = new Vector2(RectSize.x / 2, RectSize.y / 2);
        }

		public void Init(string pRank, int pScore, string pName, Color pFontColor = default, bool pIsAnimated = false)
        {
			rankLabel = SetupLabel(rankLabelPath, pRank);
			userNameLabel = SetupLabel(usernameLabelPath, pName, pFontColor);
			scoreLabel = SetupLabel(scoreLabelPath, pScore.ToString(), pFontColor);

			Panel lRankBG = rankLabel.GetParent<Panel>();
			lRankBG.SelfModulate = pFontColor;

			if (pIsAnimated)
				userNameLabel.PercentVisible = scoreLabel.PercentVisible = 0f;
        }

		private Label SetupLabel(NodePath pPath, string pValue, Color pFontColor = default)
        {
			Label lLabel = GetNode<Label>(pPath);
			lLabel.Text = pValue;
			if(pFontColor != default)
				lLabel.AddColorOverride(PROPERTY_LABEL_FONT_COLOR, pFontColor);
			return lLabel;
        }

		public void PlayAnimation()
        {
			SceneTreeTween lTween = GetTree().CreateTween();
			lTween.Chain();
			lTween.TweenProperty(userNameLabel, PROPERTY_LABEL_PERCENT_VISIBLE, 1f, labelPrintDuration)
				  .SetTrans(labelTransition)
				  .SetEase(labelEaseType);
			lTween.TweenProperty(scoreLabel, PROPERTY_LABEL_PERCENT_VISIBLE, 1f, labelPrintDuration)
				  .SetTrans(labelTransition)
				  .SetEase(labelEaseType);
			lTween.Play();
		}

	}

}