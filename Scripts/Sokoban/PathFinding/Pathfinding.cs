using Godot;
using System;
using System.Collections.Generic;
using Com.IsartDigital.Sokoban.Managers;
using Com.IsartDigital.Sokoban.Objects;

//Author : BOULIN Valère

namespace Com.IsartDigital.Sokoban.Pathfindings
{
    public class Pathfinding : Node
    {
        private static Pathfinding instance;
        public Player playerToPathfind;

        public override void _Ready()
        {
            if (instance != null)
            {
                QueueFree();
                return;
            }
            instance = this;
        }

        public static Pathfinding GetInstance()
        {
            if (instance == null) instance = new Pathfinding();
            return instance;
        }

        public List<Vector2> GetPath(int pStartPosX, int pStartPosY, int pTargetPosX, int pTargetPosY, bool pIsWaterPlayer)
        {
            List<Vector2> lPath = new List<Vector2>();

            PathfindingCell lStartCell = new PathfindingCell();
            lStartCell.posX = pStartPosX;
            lStartCell.posY = pStartPosY;

            PathfindingCell lEndCell = new PathfindingCell();
            lEndCell.posX = pTargetPosX;
            lEndCell.posY = pTargetPosY;

            lStartCell.SetDistance(lEndCell.posX, lEndCell.posY);

            List<PathfindingCell> lActiveCells = new List<PathfindingCell>();
            lActiveCells.Add(lStartCell);

            List<PathfindingCell> lVisitedCells = new List<PathfindingCell>();

            while (lActiveCells.Count > 0)
            {
                lActiveCells.Sort((x, y) => x.costDistance.CompareTo(y.costDistance));
                PathfindingCell lCheckCell = lActiveCells[0];

                if (lCheckCell.posX == lEndCell.posX && lCheckCell.posY == lEndCell.posY)
                {
                    PathfindingCell currentCell = lCheckCell;
                    while (currentCell != null)
                    {
                        lPath.Add(new Vector2(currentCell.posX, currentCell.posY));
                        currentCell = currentCell.lastCell;
                    }
                    lPath.Reverse();
                    return lPath;
                }
                else
                {
                    lVisitedCells.Add(lCheckCell);
                    lActiveCells.RemoveAt(0);

                    List<PathfindingCell> lWalkableCells = GetWalkableCells(lCheckCell, lEndCell, pIsWaterPlayer);
                    foreach (PathfindingCell pCell in lWalkableCells)
                    {
                        if (!TestVisitedCells(pCell, lVisitedCells))
                        {
                            if (TestActiveCells(pCell, lActiveCells))
                            {
                                foreach (PathfindingCell pWalkableCell in lActiveCells)
                                {
                                    if (pCell.posX == pWalkableCell.posX && pWalkableCell.posY == pCell.posY)
                                    {
                                        if (lCheckCell.costDistance < pWalkableCell.costDistance)
                                        {
                                            lActiveCells.Remove(pCell);
                                            lActiveCells.Add(pWalkableCell);
                                        }
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                lActiveCells.Add(pCell);
                            }
                        }
                    }
                }


            }


            return lPath;
        }

        private List<PathfindingCell> GetWalkableCells(PathfindingCell pCurrentCell, PathfindingCell pTargetCell, bool pIsWaterPlayer)
        {
            List<PathfindingCell> lWalkableCells = new List<PathfindingCell>();

            bool lBoxOnPos = false;
            bool lIsBoxWater = false;
            PathfindingCell lUpCell = new PathfindingCell(pCurrentCell);
            lUpCell.posX = pCurrentCell.posX;
            lUpCell.posY = pCurrentCell.posY - 1;
            Sprite lTargetedPositionTile = GridManager.GetInstance().SpriteList[lUpCell.posY, lUpCell.posX];
            if (lTargetedPositionTile is Tile )
            {
                foreach (PushableBox lBoxSprite in MovementsManager.GetInstance().BoxesList)
                {
                    if (new Vector2(lUpCell.posX, lUpCell.posY) == lBoxSprite.GridPosition)
                    {
                        lBoxOnPos = true;
                        lIsBoxWater = lBoxSprite.IsWaterType;
                    }
                }
                   
                if(!lBoxOnPos && ((Tile)lTargetedPositionTile).IsWaterType == pIsWaterPlayer || lBoxOnPos && lIsBoxWater != pIsWaterPlayer)
                {
                    lUpCell.SetDistance(pTargetCell.posX, pTargetCell.posY);
                    lWalkableCells.Add(lUpCell);
                }

                lBoxOnPos = false;
                lIsBoxWater = false;
            }

            PathfindingCell lDownCell = new PathfindingCell(pCurrentCell);
            lDownCell.posX = pCurrentCell.posX;
            lDownCell.posY = pCurrentCell.posY + 1;
            lTargetedPositionTile = GridManager.GetInstance().SpriteList[lDownCell.posY, lDownCell.posX];
            if (lTargetedPositionTile is Tile )
            {
                foreach (PushableBox lBoxSprite in MovementsManager.GetInstance().BoxesList)
                {
                    if (new Vector2(lDownCell.posX, lDownCell.posY) == lBoxSprite.GridPosition)
                    {
                        lBoxOnPos = true;
                        lIsBoxWater = lBoxSprite.IsWaterType;
                    }
                }

                if (!lBoxOnPos && ((Tile)lTargetedPositionTile).IsWaterType == pIsWaterPlayer || lBoxOnPos && lIsBoxWater != pIsWaterPlayer)
                {
                    lDownCell.SetDistance(pTargetCell.posX, pTargetCell.posY);
                    lWalkableCells.Add(lDownCell);
                }

                lBoxOnPos = false;
                lIsBoxWater = false;

            }

            PathfindingCell lLeftCell = new PathfindingCell(pCurrentCell);
            lLeftCell.posX = pCurrentCell.posX - 1;
            lLeftCell.posY = pCurrentCell.posY;
            lTargetedPositionTile = GridManager.GetInstance().SpriteList[lLeftCell.posY, lLeftCell.posX];
            if (lTargetedPositionTile is Tile )
            {
                foreach (PushableBox lBoxSprite in MovementsManager.GetInstance().BoxesList)
                {
                    if (new Vector2(lLeftCell.posX, lLeftCell.posY) == lBoxSprite.GridPosition)
                    {
                        lBoxOnPos = true;
                        lIsBoxWater = lBoxSprite.IsWaterType;
                    }
                }

                if (!lBoxOnPos && ((Tile)lTargetedPositionTile).IsWaterType == pIsWaterPlayer || lBoxOnPos && lIsBoxWater != pIsWaterPlayer)
                {
                    lLeftCell.SetDistance(pTargetCell.posX, pTargetCell.posY);
                    lWalkableCells.Add(lLeftCell);
                }

                lBoxOnPos = false;
                lIsBoxWater = false;
            }

            PathfindingCell lRightCell = new PathfindingCell(pCurrentCell);
            lRightCell.posX = pCurrentCell.posX + 1;
            lRightCell.posY = pCurrentCell.posY;
            lTargetedPositionTile = GridManager.GetInstance().SpriteList[lRightCell.posY, lRightCell.posX];
            if (lTargetedPositionTile is Tile )
            {
                foreach (PushableBox lBoxSprite in MovementsManager.GetInstance().BoxesList)
                {
                    if (new Vector2(lRightCell.posX, lRightCell.posY) == lBoxSprite.GridPosition)
                    {
                        lBoxOnPos = true;
                        lIsBoxWater = lBoxSprite.IsWaterType;
                    }
                }

                if (!lBoxOnPos && ((Tile)lTargetedPositionTile).IsWaterType == pIsWaterPlayer || lBoxOnPos && lIsBoxWater != pIsWaterPlayer)
                {
                    lRightCell.SetDistance(pTargetCell.posX, pTargetCell.posY);
                    lWalkableCells.Add(lRightCell);
                }

                lBoxOnPos = false;
                lIsBoxWater = false;
            }


            return lWalkableCells;
        }

        private bool TestVisitedCells(PathfindingCell pWalkableCell, List<PathfindingCell> pVisitedCells)
        {
            foreach (PathfindingCell pCell in pVisitedCells)
            {

                if (new Vector2(pWalkableCell.posX, pWalkableCell.posY) == new Vector2(pCell.posX, pCell.posY))
                {
                    return true;
                }
            }
            return false;
        }

        private bool TestActiveCells(PathfindingCell pWalkableCell, List<PathfindingCell> pActiveCells)
        {
            foreach (PathfindingCell pCell in pActiveCells)
            {
                if (new Vector2(pWalkableCell.posX, pWalkableCell.posY) == new Vector2(pCell.posX, pCell.posY))
                {
                    return true;
                }
            }
            return false;
        }

        protected override void Dispose(bool pDisposing)
        {
            if (pDisposing && instance == this) instance = null;
            base.Dispose(pDisposing);
        }
    }
}
