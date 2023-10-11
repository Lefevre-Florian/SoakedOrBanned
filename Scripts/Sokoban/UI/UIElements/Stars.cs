using Godot;
using System;
using Com.IsartDigital.Tools.Resources;

// Author : BENCTEUX Pierre-Antoine
namespace Com.IsartDigital.Sokoban.Juiciness
{
	public class Stars : TextureRect
	{
		// Parameters
		[Export] private Vector2 reduceScale = new Vector2(0.2f, 0.2f);
		[Export] private Vector2 baseScale = new Vector2(0.7f, 0.7f);
		[Export] private Vector2 upScale = new Vector2(1f, 1f);
		[Export] private Vector2 randRotationInterval = new Vector2(3f, 5f);
		[Export] private Vector2 delayInterval = new Vector2(1.5f, 2.5f);
		[Export] private float doubleRotationValue = -360f;
		[Export] private float durationAppPartOne = 0.4f;
		[Export] private float durationAppPartTwo = 0.2f;
		[Export] private float durationDoubleRotation = 0.05f;
		[Export] private float durationRotation = 0.8f;

		// Objects
		private RandomNumberGenerator rand = new RandomNumberGenerator();
		private SceneTreeTween tweenApp;
		private SceneTreeTween tweenPermaOne;
		private SceneTreeTween tweenPermaTwo;

		private Particles2D shinyChild;

		// Properties
		private Color baseVisibility;
		private Color reducedVisibility;
		float randRotation;
		private bool isStartAnimationFinished = false;

		private void Init()
		{
			rand.Randomize();

			shinyChild = (Particles2D)GetChild(0);

			baseVisibility = Modulate;
			reducedVisibility = baseVisibility;
			reducedVisibility.a8 = 0;

			randRotation = rand.RandfRange(randRotationInterval.x, randRotationInterval.y);

			Modulate = reducedVisibility;
			RectScale = reduceScale;
			RectRotation += doubleRotationValue;
			RectPivotOffset = shinyChild.Position;
		}
		
		public override void _Ready()
		{
			Init();
			StartAnimation();
		}

        private void StartAnimation()
        {
			tweenApp = CreateTween();

			tweenApp.Connect(Resources.FINISHED, this, nameof(PermanentAnimation));

			tweenApp.TweenProperty(this, Resources.RECT_SCALE, upScale, durationAppPartOne);
			tweenApp.Parallel().TweenProperty(this, Resources.RECT_ROTATION, doubleRotationValue, durationDoubleRotation);
			tweenApp.Parallel().TweenProperty(this, Resources.MODULATE, baseVisibility, durationAppPartTwo);
			tweenApp.Chain().TweenProperty(this, Resources.RECT_SCALE, baseScale, durationAppPartTwo);
        }

		private void PermanentAnimation()
        {
			float lRandDelay;

			tweenPermaOne = CreateTween();
			tweenPermaTwo = CreateTween();

			lRandDelay = rand.RandfRange(delayInterval.x, delayInterval.y);

			tweenPermaOne.SetLoops();
			tweenPermaOne.TweenProperty(this, Resources.RECT_ROTATION, -randRotation, durationRotation);
			tweenPermaOne.Chain().TweenProperty(this, Resources.RECT_ROTATION, randRotation, durationRotation);

			tweenPermaTwo.SetLoops();
			tweenPermaTwo.TweenProperty(this, Resources.RECT_SCALE, upScale, durationAppPartTwo).SetDelay(lRandDelay);
			tweenPermaTwo.TweenProperty(this, Resources.RECT_SCALE, baseScale, durationAppPartTwo);
		}
	}
}