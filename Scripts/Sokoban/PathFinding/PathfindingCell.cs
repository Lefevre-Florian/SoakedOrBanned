using Godot;
using System;
using Com.IsartDigital.Sokoban.Objects;

//Author : BOULIN Valère

namespace Com.IsartDigital.Sokoban.Pathfindings
{
    public class PathfindingCell : IndexableTile
    {
        public int posX;
        public int posY;
        public int cost;
        public int distance;
        public float costDistance;
        public PathfindingCell lastCell;

        public override void _Ready()
        {
            costDistance = cost + distance;
        }

        public PathfindingCell(PathfindingCell parent = null)
        {
            lastCell = parent;
            if (lastCell != null)
            {
                cost = lastCell.cost + 1;
            }
            else
            {
                cost = 0;
            }
        }

        public void SetDistance(int pTargetX, int pTargetY)
        {
            distance = Mathf.Abs(pTargetX - posX) + Mathf.Abs(pTargetY - posY);
        }
    }
}

