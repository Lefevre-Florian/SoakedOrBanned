using Godot;
using System;
using Com.IsartDigital.Tools.Resources;
using Com.IsartDigital.Sokoban.Abstracts;
using Com.IsartDigital.Sokoban.Managers;
using Com.IsartDigital.Utils.Events;

// Author : BENCTEUX Pierre-Antoine
namespace Com.IsartDigital.Sokoban.Objects
{
    public class PushableBox : IndexableTile
    {
        // Parameters
        [Export] private Vector2 idleRange = new Vector2(0.95f, 1.05f);
        [Export] private float durationIdle = 0.5f;
        [Export] private float durationLerp = 0.1f;

        // Objects
        protected SceneTreeTween tweenIdle;
        protected SceneTreeTween tweenLerp;

        protected MovementsManager movementsManager = null;
        protected Texture myTexture;

        // Properties
        private const int TILE_SKIPPING = 2;
        protected bool isMoving = false;

        private bool isAnimated = true;


        public override void _Ready()
        {
            base._Ready();
            movementsManager = MovementsManager.GetInstance();
            IdleAnimation();

            movementsManager.Connect(nameof(MovementsManager.IsRetrying), this, nameof(SetAnimationState));
            Connect(EventNode.TREE_EXITING, this, nameof(Destructor));

            isAnimated = true;
        }

        public override void _Process(float delta)
        {
        }

        public virtual void IsMoving(Vector2 pMovementDirection)
        {
            if(!isAnimated)
                GlobalPosition = GlobalPosition + pMovementDirection * tileSize;
            else 
                Lerp(GlobalPosition + pMovementDirection * tileSize);

            //GlobalPosition += pMovementDirection * tileSize;

            _gridPosition += new Vector2(pMovementDirection.x, pMovementDirection.y);
            ZIndex += _gridPosition.y != _gridPosition.y + pMovementDirection.y ? (_gridPosition.y > pMovementDirection.y + _gridPosition.y ? -GridManager.TILE_SKIPPING : GridManager.TILE_SKIPPING) : 0;
        }

        public virtual void IsMovingPathfinding(Vector2 pMovementDirection)
        {
            GlobalPosition += pMovementDirection * tileSize;

            _gridPosition += new Vector2(pMovementDirection.x, pMovementDirection.y);
            ZIndex += _gridPosition.y != _gridPosition.y + pMovementDirection.y ? (_gridPosition.y > pMovementDirection.y + _gridPosition.y ? -GridManager.TILE_SKIPPING : GridManager.TILE_SKIPPING) : 0;
        }

        public virtual void Move() 
        {
            Vector2 lDirection = Controller.Move();
            movementsManager.AddToUndo(this, lDirection * -1);
            IsMoving(lDirection);
        }

        protected void NotMovingAnymore() => isMoving = false;

        protected void Lerp(Vector2 pNewPosition)
        {
            tweenLerp = CreateTween();
            tweenLerp.Connect(Resources.FINISHED, this, nameof(NotMovingAnymore));
            tweenLerp.TweenProperty(this, Resources.GLOBAL_POSITION, pNewPosition, durationLerp);
        }

        protected virtual void IdleAnimation()
        {
            if(GetTree().CurrentScene.Name != Resources.LEVELCREATOR_SCREEN_NAME)
            {
                Vector2 lFirstMove = new Vector2(Scale.x, Mathf.Sin(idleRange.y));
                Vector2 lSecondMove = new Vector2(Scale.x, Mathf.Sin(idleRange.x));

                tweenIdle = CreateTween();

                tweenIdle.SetLoops();
                tweenIdle.TweenProperty(this, Resources.SCALE, lFirstMove, durationIdle);
                tweenIdle.TweenProperty(this, Resources.SCALE, lSecondMove, durationIdle);
            }
        }

        private void SetAnimationState(bool pState)
        {
            isMoving = !pState;
            isAnimated = pState;
        }

        private void Destructor()
        {
            movementsManager.Disconnect(nameof(MovementsManager.IsRetrying), this, nameof(SetAnimationState));
            Disconnect(EventNode.TREE_EXITING, this, nameof(Destructor));
        }
    }
}
