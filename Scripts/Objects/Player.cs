using Godot;
using System;
using Com.IsartDigital.Sokoban.Abstracts;
using Com.IsartDigital.Sokoban.Managers;
using System.Collections.Generic;
using Com.IsartDigital.Tools.Resources;
using Com.IsartDigital.Sokoban.Pathfindings;

//Author : BENCTEUX Pierre-Antoine & BOULIN Valère
namespace Com.IsartDigital.Sokoban.Objects
{
    public class Player : PushableBox
    {
        // Parameters
        [Export] private List<Texture> directionsTextures = new List<Texture>();
        [Export] private PackedScene walkParticlesPath = default;

        private const string PATH_ARROWINDICATOR = "res://Scenes/Objects/Arrow.tscn";
        private const string PROPERTY_DIRECTION = "direction";

        // Objects
        private UIManager UIManager = null;
        private TipsManager tipsManager = null;

        // Properties
        private Vector2 direction;
        private int leftTextureIndex = 0;
        private int upTextureIndex = 1;
        private int rightTextureIndex = 2;
        private int downTextureIndex = 3;

        public List<Vector2> pathfindingPath = new List<Vector2>();
        public static Vector2 pathfindingDestination = Vector2.Zero;

        // Juiciness
        private Particles2D walkParticles = null;

        public override void _Ready()
        {
            base._Ready();

            ArrowIndicator lArrow = GD.Load<PackedScene>(PATH_ARROWINDICATOR).Instance<ArrowIndicator>();
            AddChild(lArrow);
            lArrow.Init(this);

            ResetSprite();
            UIManager = UIManager.GetInstance();
            tipsManager = TipsManager.GetInstance();
        }

        public override void _Process(float pDelta)
        {
            base._Process(pDelta);

            if (Controller.Move() != Vector2.Zero)
                Move();

            if(pathfindingDestination != Vector2.Zero)
            {
                pathfindingPath = Pathfinding.GetInstance().GetPath((int)GridPosition.x, (int)GridPosition.y, (int)pathfindingDestination.x, (int)pathfindingDestination.y, IsWaterType);
                pathfindingPath = ConvertPathfindingIntoDirections(pathfindingPath);
                pathfindingDestination = Vector2.Zero;
            }

            if (pathfindingPath.Count > 0)
            {
                movementsManager.AddToUndo(this, pathfindingPath[0] * -1);
                movementsManager.ClearRedoList();

                IsMovingPathfinding(pathfindingPath[0]);
                movementsManager.IncrementAction();
                pathfindingPath.RemoveAt(0);
            }
        }

        override public void Move()
        {
            if (!isMoving)
            {
                // Animation movement
                isMoving = true;

                // Movement logic
                direction = Controller.Move();
                if (movementsManager.CheckWishedMove(this, direction))
                {
                    if (tipsManager.AreTipsShown)
                        tipsManager.ShowTipsPanel();
                    // Particles
                    if (walkParticlesPath != null && direction != Vector2.Down)
                    {
                        walkParticles = walkParticlesPath.Instance<Particles2D>();
                        AddChild(walkParticles);
                        walkParticles.ZIndex = ZIndex-1;
                        walkParticles.GlobalPosition = (GlobalPosition + Offset) + ((direction*-1) * Texture.GetSize().x / 2);
                        if (direction != Vector2.Up)
                            walkParticles.GlobalPosition += new Vector2(0f, Texture.GetSize().y / 4);
                        walkParticles.ProcessMaterial.Set(PROPERTY_DIRECTION, new Vector3(direction.x*-1, direction.y*-1, 0f));
                    }
                    movementsManager.AddToUndo(this, direction * -1);
                    movementsManager.ClearRedoList();

                    IsMoving(direction);
                    movementsManager.IncrementAction();
                }
                else
                    isMoving = false;
            }
        }

        public override void IsMoving(Vector2 pMovementDirection)
        {
            ChangeSprite(pMovementDirection);
            base.IsMoving(pMovementDirection);
        }

        private List<Vector2> ConvertPathfindingIntoDirections(List<Vector2> pList)
        {
            Vector2 lLastPathfindingPosition;
            List<Vector2> lDirectionList = new List<Vector2>();

            for (int i = 1; i < pList.Count; i++)
            {
                lLastPathfindingPosition = pList[i - 1];
                if (pathfindingPath[i].x < lLastPathfindingPosition.x)
                    lDirectionList.Add(Vector2.Left);
                else if (pathfindingPath[i].x > lLastPathfindingPosition.x)
                    lDirectionList.Add(Vector2.Right);
                else if (pathfindingPath[i].y > lLastPathfindingPosition.y)
                    lDirectionList.Add(Vector2.Down);
                else if (pathfindingPath[i].y < lLastPathfindingPosition.y)
                    lDirectionList.Add(Vector2.Up);
            }

            return lDirectionList;
        }

        private void ChangeSprite(Vector2 pDirection)
        {
            if (pDirection == Vector2.Left)
                Texture = directionsTextures[leftTextureIndex];
            else if (pDirection == Vector2.Up)
                Texture = directionsTextures[upTextureIndex];
            else if (pDirection == Vector2.Right)
                Texture = directionsTextures[rightTextureIndex];
            else if (pDirection == Vector2.Down)
                Texture = directionsTextures[downTextureIndex];
        }

        public void ResetSprite() => Texture = directionsTextures[downTextureIndex];
    }
}
