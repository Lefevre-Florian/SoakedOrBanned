using Godot;
using System;

//Author : VERDIER Thomas 

namespace Com.IsartDigital.Sokoban.Objects {

	public class ChainLink : RigidBody2D
	{
		[Export] NodePath topPosPath;
		public Position2D topPos;
		[Export] NodePath bottomPosPath;
		public Position2D bottomPos;
		public override void _Ready()
		{
			topPos = GetNode<Position2D>(topPosPath);
			bottomPos = GetNode<Position2D>(bottomPosPath);
		}

	}

}