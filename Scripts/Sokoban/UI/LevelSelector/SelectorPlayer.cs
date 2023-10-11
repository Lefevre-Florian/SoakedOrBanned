using Godot;
using System;
using Com.IsartDigital.Utils.Events;
using System.Collections.Generic;

//Author : VERDIER Thomas 

namespace Com.IsartDigital.Sokoban.UI.LevelSelector
{
	public enum State
    {
		Water,
		Dirt
    }


	public enum Mode
    {
		Idle, 
		Moving
    }

	public class SelectorPlayer : KinematicBody2D
	{
		[Export(PropertyHint.File)] List<string> dirtPlayerPaths = new List<string>();
		[Export(PropertyHint.File)] List<string> waterPlayerPaths = new List<string>();

		[Export] NodePath lightPath;
		Light2D light;

		const string INPUT_UP = "Up";
		const string INPUT_DOWN = "Down";
		const string INPUT_LEFT = "Left";
		const string INPUT_RIGHT = "Right";

		const string INPUT_LEFT_CLICK = "LeftClick";

		private bool GoingUp = false;
		private bool GoingDown = false;
		private bool GoingLeft = false;
		private bool GoingRight = false;

		private State state = State.Dirt;

		Tween tween;

		[Export(PropertyHint.Range, "0,2000,10")] int speed;

		[Export(PropertyHint.File)] string dirtSpritePath;
		[Export(PropertyHint.File)] string waterSpritePath;

		[Export] NodePath cameraPath;
		public Camera2D camera;

		[Export] NodePath playerSpritePath;
		Sprite playerSprite;

		Mode mode = Mode.Idle;
		Action action;

		Vector2 onMapPos;
		int typeLandOn;

		private static SelectorPlayer instance;

		private Vector2 destination;
		private Vector2 trajectory;
		public override void _Ready()
		{
			if (instance != null)
			{
				QueueFree();
				return;
			}
			//light = GetNode<Light2D>(lightPath);
			tween = new Tween();
			AddChild(tween);
			camera = GetNode<Camera2D>(cameraPath);
			playerSprite = GetNode<Sprite>(playerSpritePath);
			playerSprite.Texture = GD.Load<Texture>(waterSpritePath);
			instance = this;
			state = State.Dirt;
			SetActionMove();
			//IdleJuice();
		}

		public override void _Process(float delta)
		{
			//light.LookAt(GetGlobalMousePosition());
			onMapPos = LandMap.GetInstance().WorldToMap(GlobalPosition);
			typeLandOn = LandMap.GetInstance().GetCell((int)onMapPos.x, (int)onMapPos.y);
			if (typeLandOn == ((int)LandType.Water))
			{
				state = State.Water;
				GD.Print("Goes Booooooats");
			}
			else
				state = State.Dirt;
			action();
			base._Process(delta);
		}

		private void DoActionIdle()
		{
			if (state == State.Water)
			{
				playerSprite.Texture = GD.Load<Texture>(waterSpritePath);
			}
			if (state == State.Dirt)
			{
				playerSprite.Texture = GD.Load<Texture>(dirtSpritePath);
			}
		}

		public void SetActionIdle()
		{
			action = DoActionIdle;
		}

		private void DoActionMove()
		{
			if (state == State.Water)
			{
				playerSprite.Texture = GD.Load<Texture>(waterSpritePath);
			}
			if (state == State.Dirt)
			{
				playerSprite.Texture = GD.Load<Texture>(dirtSpritePath);
			}
			if (Input.IsActionPressed(INPUT_UP))
				GoingUp = true;

			if (Input.IsActionPressed(INPUT_DOWN))
				GoingDown = true;

			if (Input.IsActionPressed(INPUT_LEFT))
				GoingLeft = true;

			if (Input.IsActionPressed(INPUT_RIGHT))
				GoingRight = true;

			if (GoingUp)
			{
				MoveAndSlideAndStopClick(Vector2.Up * speed);
				/*if (state == State.Water)
                {
					playerSprite.Texture = GD.Load<Texture>(waterPlayerPaths[1]);
                }
				if (state == State.Dirt)
				{
					playerSprite.Texture = GD.Load<Texture>(dirtPlayerPaths[1]);
				}*/
			}
			if (GoingDown)
			{
				MoveAndSlideAndStopClick(Vector2.Down * speed);
				/*if (state == State.Water)
				{
					playerSprite.Texture = GD.Load<Texture>(waterPlayerPaths[3]);
				}
				if (state == State.Dirt)
				{
					playerSprite.Texture = GD.Load<Texture>(dirtPlayerPaths[3]);
				}*/
			}
			if (GoingLeft) 
			{ 
				MoveAndSlideAndStopClick(Vector2.Left * speed); 
				/*if (state == State.Water)
				{
					playerSprite.Texture = GD.Load<Texture>(waterPlayerPaths[0]);
				}
				if (state == State.Dirt)
				{
					playerSprite.Texture = GD.Load<Texture>(dirtPlayerPaths[0]);
				}*/
			}
			if (GoingRight) 
			{ 
				MoveAndSlideAndStopClick(Vector2.Right* speed);
				/*if (state == State.Water)
				{
					playerSprite.Texture = GD.Load<Texture>(waterPlayerPaths[2]);
				}
				if (state == State.Dirt)
				{
					playerSprite.Texture = GD.Load<Texture>(dirtPlayerPaths[2]);
				}*/
			}
			
			if (!GoingUp && !GoingDown && !GoingLeft && !GoingRight)
            {
				if (mode != Mode.Idle)
                {
					mode = Mode.Idle;
					IdleJuice();
                }
            }

			GoingUp = false;
			GoingDown = false;
			GoingLeft = false;
			GoingRight = false;

			if ((GlobalPosition - destination).Length() > 10 && destination != Vector2.Zero)
				MoveAndSlide(trajectory.Normalized() * speed);
			else
				destination = Vector2.Zero;
		}

		private void MoveAndSlideAndStopClick(Vector2 direction)
        {
			mode = Mode.Moving;
			tween.StopAll();
			tween.InterpolateProperty(this, "scale", Scale, Vector2.One * 1f, 0.1f);
			tween.Start();
			MoveAndSlide(direction);
			destination = Vector2.Zero;
        }

		public void SetActionMove()
		{
			action = DoActionMove;
			tween.StopAll();
		}

		public static SelectorPlayer GetInstance()
		{
			if (instance == null)
				instance = new SelectorPlayer();
			return instance;
		}

		public void IdleJuice()
        {
				tween.InterpolateProperty(this, "scale", Vector2.One * 1, Vector2.One * 1.1f, 1f);
				tween.InterpolateProperty(this, "scale", Vector2.One * 1.1f, Vector2.One * 1f, 1f, delay: 1f);
				tween.InterpolateCallback(this, 2f, nameof(IdleJuice));
				tween.Start();
			
		}
        protected override void Dispose(bool disposing)
        {
			instance = null;
			base.Dispose(disposing);
        }
    }

}