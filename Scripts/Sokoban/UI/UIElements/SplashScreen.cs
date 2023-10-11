using Godot;
using System;

// Author : Lefevre Florian
namespace Com.IsartDigital.Sokoban.UI{

	public class SplashScreen : Control
	{
		[Export] private NodePath logoPath = default;

		[Export(PropertyHint.File)] private string mainScenePath = "";

		[Export] private float printingDuration;

		[Export] private float openingDuration;
		[Export] private Tween.TransitionType openingTransition = default;
		[Export] private Tween.EaseType openingEase = default;

		[Export] private float closingDuration;
		[Export] private Tween.TransitionType closingTransition = default;
		[Export] private Tween.EaseType closingEase = default;

		private const string PROPERTY_MODULATE = "modulate";

		private const float MIN = 0f;
		private const float MAX = 1f;

		public override void _Ready()
		{
			SceneTreeTween lTween = GetTree().CreateTween();
			TextureRect lLogo = GetNode<TextureRect>(logoPath);

			Color lStart = new Color(lLogo.Modulate.r, lLogo.Modulate.g, lLogo.Modulate.b, MIN);
			Color lFinal = new Color(lLogo.Modulate.r, lLogo.Modulate.g, lLogo.Modulate.b, MAX);

			lLogo.Modulate = lStart;

			lTween.Chain();

			lTween.TweenProperty(lLogo, PROPERTY_MODULATE, lFinal, openingDuration)
				  .SetTrans(openingTransition)
				  .SetEase(openingEase)
				  .SetDelay(printingDuration);
			lTween.TweenProperty(lLogo, PROPERTY_MODULATE, lStart, closingDuration)
				  .SetTrans(closingTransition)
				  .SetEase(closingEase);

			lTween.TweenCallback(this, nameof(LoadMainMenu));
		}

		private void LoadMainMenu() => GetTree().ChangeScene(mainScenePath);

	}

}