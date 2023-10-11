using Godot;
using System;
using Com.IsartDigital.Utils.Events;

// Author : Lefevre Florian
namespace Com.IsartDigital.Sokoban.Objects {

	public class Particles : Particles2D
	{

		public override void _Ready()
		{
			Emitting = true;
			SceneTreeTimer lTimer = GetTree().CreateTimer(Lifetime);
			lTimer.Connect(EventTimer.TIMEOUT, this, nameof(Destructor));
		}

		private void Destructor() => QueueFree();

	}

}