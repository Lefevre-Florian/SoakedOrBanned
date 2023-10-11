using Godot;
using System;
using Com.IsartDigital.Sokoban.Objects;
using Com.IsartDigital.Tools.Resources;
using Com.IsartDigital.Sokoban.Managers;


//Author : BOULIN Valère
namespace Com.IsartDigital.Sokoban.Objects
{
    public class TilePlacer : Node2D
    {
        [Export] private readonly float MINIMUM_SNAP_DISTANCE;
        [Export] private readonly float BLINK_FACTOR = .5f;

        private Vector2 snappingTarget;
        private Node2D activeTile;

        private float blinkTime;

        private static TilePlacer instance;

        static public TilePlacer GetInstance()
        {
            if (instance == null) instance = new TilePlacer();
            return instance;
        }

        public override void _Ready()
        {
            if (instance != null)
            {
                QueueFree();
                return;
            }
            instance = this;
        }

        public override void _Process(float delta)
        {
            ChooseWhoToSnap();
            Position = snappingTarget;

            if (Input.IsActionPressed(Resources.LEFT_CLICK) && activeTile != null)
                LevelCreatorManager.GetInstance().SwapTile(activeTile);

            Modulate = new Color(1, 1, 1, Mathf.Abs(Mathf.Sin(blinkTime)));
            blinkTime += BLINK_FACTOR;
        }

        private void ChooseWhoToSnap()
        {
            snappingTarget = GetGlobalMousePosition();
            activeTile = null;
            Vector2 lMinDistance = new Vector2(MINIMUM_SNAP_DISTANCE, MINIMUM_SNAP_DISTANCE);

            foreach (Node2D pTile in LevelCreatorManager.GetInstance().tileList)
            {
                Vector2 lDistanceToTile = GetGlobalMousePosition() - pTile.GlobalPosition;

                if (Mathf.Abs(lDistanceToTile.x) < lMinDistance.x && Mathf.Abs(lDistanceToTile.y) < lMinDistance.y)
                {
                    lMinDistance = lDistanceToTile;
                    snappingTarget = pTile.GlobalPosition;
                    activeTile = pTile;
                }
            }
        }

        protected override void Dispose(bool pDisposing)
        {
            if (pDisposing && instance == this) instance = null;
            base.Dispose(pDisposing);
        }
    }

}
