using Godot;
using System;
using System.Collections.Generic;
using Com.IsartDigital.Sokoban.Managers;
using Com.IsartDigital.Utils.Events;

// Author : Lefevre Florian
namespace Com.IsartDigital.Sokoban.Objects {

	public class Goal : IndexableTile
	{
        // Properties
        private GameManager gameManager = null;
        private List<Crate> boxes = new List<Crate>();
        private bool _isFull = false;

        // Getters / Setters
        public bool IsFull
        {
            get { return _isFull; }
            private set { _isFull = value; }
        }

        public override void _Ready()
        {
            gameManager = GameManager.GetInstance();
            Connect(EventNode.TREE_EXITING, this, nameof(Destructor));
        }

        public void ConnectBoxes(List<Crate> pBoxes)
        {
            foreach (Crate lBox in pBoxes)
            {
                lBox.Connect(nameof(Crate.Moving), this, nameof(BoxObserver));
                boxes.Add(lBox);
            }
        }

        private void BoxObserver(Vector2 pBoxPosition)
        {
            foreach (Crate lBox in boxes)
            {
                if (lBox.GridPosition == _gridPosition)
                {
                    _isFull = true;
                    gameManager.EndGame();
                    return;
                }
            }
            _isFull = false;
            gameManager.EndGame();
        }

        /// <summary>
        /// Called when the node is destroy or exiting godot tree (used to clean the node)
        /// </summary>
        private void Destructor()
        {
            if (boxes.Count > 0)
            {
                foreach (Crate lBox in boxes)
                    lBox.Disconnect(nameof(Crate.Moving), this, nameof(BoxObserver));
                boxes.Clear();
            }
            Disconnect(EventNode.TREE_EXITING, this, nameof(Destructor));
        }
    }
}