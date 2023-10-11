using Godot;
using System;

//Author : VERDIER Thomas 

namespace Com.IsartDigital.Sokoban.Objects {

	public class Chain : Node2D
	{
        [Export] int length = 2;

        [Export] PackedScene sideChainFactory;
        [Export] PackedScene frontChainFactory;

        Node2D spawnPoint;
        ChainLink focusedLink;
        ChainLink nextLink;

        [Export] NodePath spawnPointPath;
        [Export] NodePath ballPath;
        RigidBody2D ball;
        PinJoint2D pin;
        [Export] NodePath mouseCollisionPath;
        KinematicBody2D mouseCollision;
		public override void _Ready()
		{
            spawnPoint = GetNode<Node2D>(spawnPointPath);
            ball = GetNode<RigidBody2D>(ballPath);
            for (int i = 0; i < length; i++)
            {
                if (i == 0)
                {
                    focusedLink = (ChainLink)frontChainFactory.Instance();
                    AddChild(focusedLink);
                    focusedLink.GlobalPosition = spawnPoint.GlobalPosition;
                    pin = new PinJoint2D();
                    AddChild(pin);
                    pin.GlobalPosition = focusedLink.GlobalPosition;
                    pin.NodeA = focusedLink.GetPath();
                    pin.NodeB = spawnPoint.GetPath();
                    
                }
                else
                {
                    if (i % 2 == 0)
                    {
                        SpawnChainLink(frontChainFactory);
                    }
                    else
                    {
                        SpawnChainLink(sideChainFactory);
                    }
                }
            }
            ball.GlobalPosition = focusedLink.bottomPos.GlobalPosition;
            pin = new PinJoint2D();
            AddChild(pin);
            pin.GlobalPosition = ball.GlobalPosition;
            pin.NodeA = ball.GetPath();
            pin.NodeB = focusedLink.GetPath();
            mouseCollision = GetNode<KinematicBody2D>(mouseCollisionPath);
		}

        private void SpawnChainLink(PackedScene pPackedScene)
        {
            nextLink = (ChainLink)pPackedScene.Instance();
            AddChild(nextLink);
            nextLink.GlobalPosition = focusedLink.bottomPos.GlobalPosition;
            pin = new PinJoint2D();
            AddChild(pin);
            pin.GlobalPosition = nextLink.GlobalPosition;
            pin.NodeA = nextLink.GetPath();
            pin.NodeB = focusedLink.GetPath();
            focusedLink = nextLink;
        }

        public override void _Process(float delta)
        {
            mouseCollision.GlobalPosition = GetGlobalMousePosition();
        }

    }

}