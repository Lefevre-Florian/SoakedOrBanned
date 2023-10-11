using Godot;
using System;
using Com.IsartDigital.Utils.Events;
using Com.IsartDigital.Sokoban.Managers;

// Author : Lefevre Florian
namespace Com.IsartDigital.Sokoban.Objects {

	public class ArrowIndicator : Polygon2D
	{

		[Export (PropertyHint.Range,"0f,100f,1f,or_greater")] private float movementYVelocity = 0f;
		[Export(PropertyHint.Range, "0f,100f,1f,or_greater")] private float hoverDisplacement = 0f;
		[Export] private float movementDuration;
		[Export] private Tween.TransitionType movementTransition = default;
		[Export] private Tween.EaseType movementEaseType = default;

		private const string PROPERTY_POSITION = "position";

		private Vector2 initialPosition;
		private Tween tween = new Tween();

		private Player owner = null;
		private GameManager gameManager = null;

		public void Init(Player pOwner)
        {
			Vector2 lPivot = pOwner.Texture.GetSize();
			GlobalPosition = pOwner.GlobalPosition + new Vector2(lPivot.x / 2, -lPivot.y/2) - new Vector2(0f, hoverDisplacement);
			owner = pOwner;
			gameManager = GameManager.GetInstance();

			gameManager.Connect(nameof(GameManager.Switch), this, nameof(SwitchPlayer));

			initialPosition = Position;
			AddChild(tween);
			tween.Connect(EventTween.TWEEN_ALL_COMPLETED, this, nameof(IdleMove));
			Connect(EventNode.TREE_EXITING, this, nameof(Destructor));
			IdleMove();
		}

		private void SwitchPlayer(Player pPlayer) => Visible = pPlayer == owner ? true : false; 

		private void IdleMove()
        {
			tween.Start();
			tween.InterpolateProperty(this, PROPERTY_POSITION,
									  initialPosition,
									  new Vector2(Position.x, Position.y - movementYVelocity),
									  movementDuration,
									  movementTransition,
									  movementEaseType);
			tween.InterpolateProperty(this, PROPERTY_POSITION,
									  new Vector2(Position.x, Position.y - movementYVelocity),
									  initialPosition,
									  movementDuration,
									  movementTransition,
									  movementEaseType);
		}

		private void Destructor()
        {
			tween.Disconnect(EventTween.TWEEN_ALL_COMPLETED, this, nameof(IdleMove));
			gameManager.Disconnect(nameof(GameManager.Switch), this, nameof(SwitchPlayer));
			Disconnect(EventNode.TREE_EXITING, this, nameof(Destructor));
		}


	}

}