using Godot;
using System;
using Com.IsartDigital.Tools.Resources;

// Author : BENCTEUX Pierre-Antoine
namespace Com.IsartDigital.Sokoban.Objects
{
	public class Tile : Wall
	{
		// Properties
		[Export] protected bool _isWaterType = false;

		protected Vector2 tileSize = new Vector2(100, 100);

		protected Vector2 _gridPosition;

        public override void _Process(float delta)
        {
            base._Process(delta);

			if(Input.IsActionJustPressed(Resources.LEFT_CLICK) && GetTree().CurrentScene.Name != Resources.LEVELCREATOR_SCREEN_NAME && new Vector2(GlobalPosition.x+50,GlobalPosition.y+50).DistanceTo(GetGlobalMousePosition()) < 50)
            {
				Player.pathfindingDestination = new Vector2(GridPosition.x, GridPosition.y);
			}
        }

        #region Getters/Setters
        public bool IsWaterType
		{
			get { return _isWaterType; }
			private set { _isWaterType = value; }
		}
		#endregion
		public Vector2 GridPosition
		{
			get { return _gridPosition; }
			private set { _gridPosition = value; }
		}

		public void SetIndex(float pX, float pY) => _gridPosition = new Vector2(pX, pY);
	}
}