using Godot;
using System;
using Com.IsartDigital.Tools.Resources;

// Author : BENCTEUX Pierre-Antoine
namespace Com.IsartDigital.Sokoban.Juiciness
{
	public class StarLoader : VBoxContainer
	{
		// References
		[Export(PropertyHint.File)] private string starPath = "";
		[Export] private NodePath starContainerPath;
		[Export] private NodePath sideParticlesContainerPath = "";
		[Export] private int deltaToSecondConvertValue = 2;

		// Parameters
		[Export] private int starsNb;
		[Export] private float partDelay = 1f;

		// Objects
		private SceneTreeTween tweenParticles;

		private HBoxContainer starContainer;
		private Node2D sideParticlesContainer;

		// Properties
		private float count = 0f;
		int entirePart;
		int lastEntire = -1;

		public override void _Ready()
		{
			starContainer = GetNode<HBoxContainer>(starContainerPath);
			sideParticlesContainer = GetNode<Node2D>(sideParticlesContainerPath);

			SetDelayOnParticles();
		}

		public override void _Process(float pDelta)
		{
			if (entirePart % deltaToSecondConvertValue == 0 && entirePart != lastEntire && starContainer.GetChildCount() < starsNb)
			{
				starContainer.AddChild(GD.Load<PackedScene>(starPath).Instance());
				lastEntire = entirePart;

				if (starContainer.GetChildCount() == starsNb)
					SetProcess(false);
			}
			// Below : Delta * 2 so 1second is equal count = 2, makes easier the division condition to generate a star each second
			count += deltaToSecondConvertValue * pDelta;	
			entirePart = (int)count;
		}

		private void SetDelayOnParticles()
        {
			tweenParticles = CreateTween();

            foreach (Particles2D particles in sideParticlesContainer.GetChildren())
            {
				tweenParticles.TweenProperty(particles, Resources.EMITTING, true, 0).SetDelay(partDelay);
            }
        }
	}
}