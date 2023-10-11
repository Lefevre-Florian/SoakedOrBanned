using Godot;
using System;
using System.Collections.Generic;
using Com.IsartDigital.Sokoban.TerrainGeneration;
using Com.IsartDigital.Utils.Events;

//Author : BENCTEUX Pierre-Antoine
namespace Com.IsartDigital.Sokoban.Managers
{
    public class UIManager : Control
    {
        // References
        [Export] private NodePath levelNumberPath;
        [Export] private NodePath levelParPath;
        [Export] private NodePath actionsCountPath;
        [Export] private NodePath authorNamePath;

        [Export] private NodePath undoButtonPath;
        [Export] private NodePath redoButtonPath;
        [Export] private NodePath retryButtonPath;

        [Export] private float retryTimer = 0f;

        // Objects
        private Label levelNumber;
        private Label levelPar;
        private Label actionsCount;
        private Label authorName;

        private GameManager gameManager;
        private GridManager gridManager;
        private MovementsManager movementsManager;

        private Button undoButton;
        private Button redoButton;
        private Button retryButton;

        // Singleton instance
        static private UIManager instance;
		
		static public UIManager GetInstance () {
			if (instance == null) instance = new UIManager();
		    return instance;
		}

        private UIManager ():base() {}

        private void Init()
        {
            gameManager = GameManager.GetInstance();
            gridManager = GridManager.GetInstance();
            movementsManager = MovementsManager.GetInstance();

            levelNumber = GetNode<Label>(levelNumberPath);
            levelPar = GetNode<Label>(levelParPath);
            actionsCount = GetNode<Label>(actionsCountPath);
            authorName = GetNode<Label>(authorNamePath);

            undoButton = GetNode<Button>(undoButtonPath);
            redoButton = GetNode<Button>(redoButtonPath);
            retryButton = GetNode<Button>(retryButtonPath);

            undoButton.Connect(EventButton.PRESSED, this, nameof(Undo));
            redoButton.Connect(EventButton.PRESSED, this, nameof(Redo));
            retryButton.Connect(EventButton.PRESSED, this, nameof(Retry));
            movementsManager.Connect(nameof(MovementsManager.UpdateActionCount), this, nameof(ManageActionsCount));
            Connect(EventNode.TREE_EXITING, this, nameof(Destructor));
        }

        public override void _Ready()
        {
            if (instance != null){  
                QueueFree();
                return;
            }
            instance = this;

            Init();
            CallDeferred(nameof(SetLevelAndParValues));
        }

        public void ManageActionsCount(int pActionCount) => actionsCount.Text = pActionCount.ToString();

        private void SetLevelAndParValues() // Used in _Ready's CallDeferred
        {
            levelNumber.Text = gameManager._levelIndex.ToString();
            authorName.Text = LevelLoader.levelPatterns[gameManager._levelIndex].Author;
            levelPar.Text = gridManager.GetPar().ToString();
        }

        private void Undo() => movementsManager.UndoAction();

        private void Redo() => movementsManager.RedoAction();

        private void Retry() => movementsManager.RetryLoop(retryTimer);

        private void Destructor()
        {
            movementsManager.Disconnect(nameof(MovementsManager.UpdateActionCount), this, nameof(ManageActionsCount));

            undoButton.Disconnect(EventButton.PRESSED, this, nameof(Undo));
            redoButton.Disconnect(EventButton.PRESSED, this, nameof(Redo));
            retryButton.Disconnect(EventButton.PRESSED, this, nameof(Retry));

            Disconnect(EventNode.TREE_EXITING, this, nameof(Destructor));
        }

        protected override void Dispose(bool pDisposing)
        {
            if (pDisposing && instance == this) instance = null;
            base.Dispose(pDisposing);
        }
    }
}