using Godot;
using System;

// Author : Lefevre Florian
namespace Com.IsartDigital.Sokoban.Objects {

	public class Crate : PushableBox
	{
        [Signal]
        public delegate void Moving(Vector2 pPosition);
        public override void IsMoving(Vector2 pMovementDirection)
        {
            base.IsMoving(pMovementDirection);
            EmitSignal(nameof(Moving), _gridPosition);
            
        }

        protected override void IdleAnimation()
        {
            
        }
    }
}