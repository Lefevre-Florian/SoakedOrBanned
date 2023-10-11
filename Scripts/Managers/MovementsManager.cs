using Godot;
using System;
using System.Collections.Generic;
using Com.IsartDigital.Sokoban.Objects;
using Com.IsartDigital.Utils.Events;
using Com.IsartDigital.Sokoban.Abstracts;

//Author : BENCTEUX Pierre-Antoine
namespace Com.IsartDigital.Sokoban.Managers
{
    public class MovementsManager : Node
    {

        private struct UndoElement
        {
            public PushableBox entity;
            public Vector2 position;
            public Vector2 scale;

            public UndoElement(PushableBox pEntity, Vector2 pPosition, Vector2 pScale)
            {
                entity = pEntity;
                position = pPosition;
                scale = pScale;
            }

            public UndoElement(PushableBox pEntity, Vector2 pPosition)
            {
                entity = pEntity;
                position = pPosition;
                scale = Vector2.One;
            }
        }

        [Export] private float reduction = 0.25f;

        // Objects
        private List<PushableBox> _pushableBoxesList = new List<PushableBox>();
        private List<Stack<PushableBox>> stacksList;
        private Stack<PushableBox> stackOne = new Stack<PushableBox>();
        private Stack<PushableBox> stackTwo = new Stack<PushableBox>();

        private List<List<UndoElement>> undoList = new List<List<UndoElement>>();
        private List<List<UndoElement>> redoList = new List<List<UndoElement>>();

        

        private GameManager gameManager = null;

        private int _actionsCount = 0;

        // Getters/Setters
        public List<PushableBox> BoxesList
        {
            get { return _pushableBoxesList; }
            private set { _pushableBoxesList = value; }
        }

        public int ActionsCount
        {
            get { return _actionsCount; }
            private set { _actionsCount = value; }
        }

        // Signals
        [Signal]
        public delegate void UpdateActionCount(int pActionCount);

        [Signal]
        public delegate void IsRetrying(bool pState);

        // Singleton instance
        static private MovementsManager instance;
		
	    static public MovementsManager GetInstance () {
		    if (instance == null) instance = new MovementsManager();
		    return instance;
	    }

        private MovementsManager ():base() {}

        public override void _Ready()
        {
            if (instance != null){  
                QueueFree();
                return;
            }
            instance = this;

            Init();
        }
        
        private void Init()
        {
            gameManager = GameManager.GetInstance();
            stacksList = new List<Stack<PushableBox>>() { stackOne, stackTwo };
        }

        public void AddToBoxesList(PushableBox pPushableBoxSprite) => _pushableBoxesList.Add(pPushableBoxSprite);

        public bool CheckWishedMove(Player pWishToMovePlayer, Vector2 pWishedDirection, bool pLockBox = false)
        {
            // Local objects and parameters
            Vector2 lTargetedPosition = pWishToMovePlayer.GridPosition + pWishedDirection;
            Vector2 lBoxDestinationPosition = lTargetedPosition + pWishedDirection;

            Sprite lTargetedPositionTile = GridManager.GetInstance().SpriteList[(int)lTargetedPosition.y, (int)lTargetedPosition.x];
            Sprite lBoxDestinationTile = GridManager.GetInstance().SpriteList[(int)pWishToMovePlayer.GridPosition.y, (int)pWishToMovePlayer.GridPosition.x];
            
            PushableBox lBoxToPush = BoxesList[0];
            PushableBox lBoxOnDest = BoxesList[0];

            Stack<PushableBox> lBoxTargetStack = stacksList[0];
            Stack<PushableBox> lBoxDestStack = stacksList[0];

            bool lIsBoxOnTargetedTile = false;
            bool lIsBoxOnDestTile = false;
            bool lIsBoxInAStack = false;
            bool lIsDestBoxInAStack = false;
            int lStackListCount = stacksList.Count;
            int lRelevantStackMin = 2;

            // Check if there's box on targeted tile and destination (targeted +1) tile
            foreach (PushableBox lBoxSprite in BoxesList)
                if (lTargetedPosition == lBoxSprite.GridPosition)
                {
                    lIsBoxOnTargetedTile = true;
                    lBoxToPush = lBoxSprite;
                }
                else if (lBoxDestinationPosition == lBoxSprite.GridPosition)
                {
                    lIsBoxOnDestTile = true;
                    lBoxOnDest = lBoxSprite;
                }

            // If there's a box on targeted tile
            if (lIsBoxOnTargetedTile)
            {
                if (pLockBox) return false;
                for (int i = 0; i < lStackListCount; i++)
                {
                    // Getting box destination's tile if actually is a tile
                    if (GridManager.GetInstance().SpriteList[(int)lBoxDestinationPosition.y, (int)lBoxDestinationPosition.x] is Tile lBoxNextTile)
                        lBoxDestinationTile = lBoxNextTile;
                    else if (GridManager.GetInstance().SpriteList[(int)lBoxDestinationPosition.y, (int)lBoxDestinationPosition.x] is Wall lWallNextTile)
                        lBoxDestinationTile = lWallNextTile;

                    // Manage the box and the potentially 2nd box according to stacks
                    if (stacksList[i].Count != 0)
                    {
                        if (stacksList[i].Peek().GridPosition == lTargetedPosition)
                        {
                            lIsBoxInAStack = true;
                            lBoxTargetStack = stacksList[i];
                            lBoxToPush = stacksList[i].Peek();
                        }
                        else if (lIsBoxOnDestTile)
                            if (stacksList[i].Peek().GridPosition == lBoxDestinationPosition)
                            {
                                lIsDestBoxInAStack = true;
                                lBoxDestStack = stacksList[i];
                                lBoxOnDest = stacksList[i].Peek();
                            }
                    }
                }

                // Movement of player on opposite type box
                if (lBoxToPush.IsWaterType != pWishToMovePlayer.IsWaterType)
                    return true;

                if (lIsBoxOnDestTile)
                    if (lBoxToPush.IsWaterType != lBoxOnDest.IsWaterType)
                    {
                        // If targeted box and box on its destination are in stacks
                        if (lIsBoxInAStack && lIsDestBoxInAStack)
                        {
                            lBoxTargetStack.Pop();
                            UpdateBoxStateOnStack(lBoxToPush, lBoxDestStack);
                            lBoxDestStack.Push(lBoxToPush);

                            if (lBoxTargetStack.Count < lRelevantStackMin)
                                lBoxTargetStack.Clear();
                        }
                        // If targeted box isn't in stack but box on dest is
                        else if (!lIsBoxInAStack && lIsDestBoxInAStack)
                        {
                            UpdateBoxStateOnStack(lBoxToPush, lBoxDestStack);
                            lBoxDestStack.Push(lBoxToPush);
                        }
                        // If targegeted box is in stack but box on dest isn't 
                        else if (lIsBoxInAStack && !lIsDestBoxInAStack)
                        {
                            lBoxTargetStack.Pop();

                            if (lBoxTargetStack.Count < lRelevantStackMin)
                                lBoxTargetStack.Clear();

                            CheckStacks(lBoxToPush, lBoxOnDest);
                        }
                        // If both boxes aren't in stacks
                        else if (!lIsBoxInAStack && !lIsDestBoxInAStack)
                            CheckStacks(lBoxToPush, lBoxOnDest);

                        PlayerOnPushedBoxCheck(pWishToMovePlayer, lBoxToPush, pWishedDirection);
                        lBoxToPush.Move();

                        return true;
                    }
                    else
                        return false;
                else if (!lIsBoxOnDestTile && lBoxDestinationTile is Tile lWalkableTile)
                {
                    if (lBoxToPush.IsWaterType == lWalkableTile.IsWaterType)
                    {
                        lBoxToPush.ZIndex = ((int)lBoxToPush.GridPosition.y) * GridManager.TILE_SKIPPING + 1 ;
                        lBoxToPush.Scale = Vector2.One;
                        PlayerOnPushedBoxCheck(pWishToMovePlayer, lBoxToPush, pWishedDirection);
                        lBoxToPush.Move();

                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            // Manage movements if there's no box on next tile
            else if (lTargetedPositionTile is Tile lWalkableTile)
            {
                if (lWalkableTile.IsWaterType == pWishToMovePlayer.IsWaterType)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        private void PlayerOnPushedBoxCheck(Player pControlledPlayer, PushableBox pPushedBox, Vector2 pMoveDirection)
        {
            // Check if the other player is on the pushed box
            if (pControlledPlayer == GameManager.GetInstance().LandPlayer && GameManager.GetInstance().WaterPlayer.GridPosition == pPushedBox.GridPosition)
            {
                gameManager.WaterPlayer.IsMoving(pMoveDirection);
                AddToUndo(gameManager.WaterPlayer, pMoveDirection * -1);
            }
            else if (pControlledPlayer == GameManager.GetInstance().WaterPlayer && GameManager.GetInstance().LandPlayer.GridPosition == pPushedBox.GridPosition)
            {
                 gameManager.LandPlayer.IsMoving(pMoveDirection);
                AddToUndo(gameManager.LandPlayer, pMoveDirection * -1);
            }  
        }

        private void CheckStacks(PushableBox lPushedBox, PushableBox lDestBox)
        {
            if (stackOne.Count == 0)
            {
                stackOne.Push(lDestBox);
                UpdateBoxStateOnStack(lPushedBox, stackOne);
                stackOne.Push(lPushedBox);
            }
            else
            {
                stackTwo.Push(lDestBox);
                UpdateBoxStateOnStack(lPushedBox, stackTwo);
                stackTwo.Push(lPushedBox);
            }
        }

        public void UpdateBoxStateOnStack(PushableBox pPushedBox, Stack<PushableBox> pStack)
        {
            if (pStack.Count == 0) return;
            pPushedBox.Scale = pStack.Peek().Scale - new Vector2(reduction, reduction);
            pPushedBox.ZIndex = pStack.Peek().ZIndex + 1;
        }

        // ========= Undo / Redo ========= //

        public void AddToUndo(PushableBox pEntity, Vector2 pPosition)
        {
            if(undoList.Count > _actionsCount)
                undoList[_actionsCount].Add(new UndoElement(pEntity, pPosition, pEntity.Scale));
            else
                undoList.Add(new List<UndoElement>() { new UndoElement(pEntity, pPosition, pEntity.Scale) });
        }

        public void UndoAction()
        {
            if(undoList.Count > 0)
            {
                DecrementAction();
                redoList.Add(undoList[_actionsCount]);
                foreach (UndoElement lActions in undoList[_actionsCount])
                {
                    lActions.entity.IsMoving(lActions.position);
                    lActions.entity.Scale = lActions.scale;
                }
                undoList.RemoveAt(_actionsCount);   
            }
        }

        public void RedoAction()
        {
            if (redoList.Count - 1 >= 0)
            {
                IncrementAction();
                undoList.Add(redoList[redoList.Count - 1]);
                foreach (UndoElement lActions in redoList[redoList.Count - 1])
                {
                    lActions.entity.IsMoving(lActions.position * -1);
                    lActions.entity.Scale = lActions.scale;
                }
                redoList.RemoveAt(redoList.Count - 1);
            }
        }

        public void RetryLoop(float pTime, Timer pTimer = null)
        {
            if(undoList.Count == 0)
            {
                if(pTimer != null)
                {
                    pTimer.Disconnect(EventTimer.TIMEOUT, this, nameof(RetryLoop));
                    pTimer.QueueFree();
                }
                undoList.Clear();
                redoList.Clear();

                if (gameManager.IsWaterPlayerSelected) gameManager.ChangeCharacterSelected();

                gameManager.LandPlayer.ResetSprite();
                gameManager.WaterPlayer.ResetSprite();
                Controller.canPlay = true;
                EmitSignal(nameof(IsRetrying), true);
                return;
            }

            if (pTimer == null) 
            {
                EmitSignal(nameof(IsRetrying), false);
                Controller.canPlay = false;
                pTimer = new Timer();
                pTimer.WaitTime = pTime / undoList.Count;
                pTimer.Connect(EventTimer.TIMEOUT, this, nameof(RetryLoop), new Godot.Collections.Array(pTime, pTimer));
                AddChild(pTimer);
                pTimer.Start();
            }
            else 
                pTimer.WaitTime = pTime / undoList.Count;

            // Undo the action (Part of the code that is really important)
            UndoAction();
        }

        public void IncrementAction() => EmitSignal(nameof(UpdateActionCount), ++_actionsCount);

        public void DecrementAction() => EmitSignal(nameof(UpdateActionCount), --_actionsCount);

        public void ClearRedoList() => redoList.Clear();

        protected override void Dispose(bool pDisposing)
        {
            if (pDisposing && instance == this) instance = null;
            base.Dispose(pDisposing);
        }
    }
}