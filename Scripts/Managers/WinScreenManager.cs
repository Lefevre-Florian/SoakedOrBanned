using Godot;
using System;
using System.Collections.Generic;
using Com.IsartDigital.Tools.Resources;

//Author : BENCTEUX Pierre-Antoine
namespace Com.IsartDigital.Sokoban.Managers
{
    public class WinScreenManager : Control
    {
        // References
        [Export(PropertyHint.File)] private string threeStarsScenePath = "";
        [Export(PropertyHint.File)] private string twoStarsScenePath = "";
        [Export(PropertyHint.File)] private string oneStarScenePath = "";
        [Export(PropertyHint.File)] private string mapScenePath = "";
        [Export] private PackedScene worldPackedScene;
        [Export] private NodePath[] dynamicLabelsList;
        [Export] private NodePath starSceneContainerPath;
        [Export] private NodePath toMapButtonPath;

        // Objects
        private VBoxContainer starSceneContainer;
        private Label levelNumber;
        private Label scoreNumber;
        private Label actionsNumber;
        private Button toMapButton;

        private GameManager gameManager;
        private GridManager gridManager;
        private UIManager UIManager;

        private List<string> starsScenesList = new List<string>();

        // Properties
        private int levelNumberIndex = 0;
        private int scoreNumberIndex = 1;
        private int actionsNumberIndex = 2;

        // Singleton instance
        static private WinScreenManager instance;
		
		static public WinScreenManager GetInstance () {
			if (instance == null) instance = new WinScreenManager();
		    return instance;
		}

        private WinScreenManager ():base() {}

        private void Init()
        {
            gameManager = GameManager.GetInstance();
            gridManager = GridManager.GetInstance();
            UIManager = UIManager.GetInstance();

            if (TipsManager.GetInstance().AreTipsShown)
                TipsManager.GetInstance().ShowTipsPanel();

            starSceneContainer = GetNode<VBoxContainer>(starSceneContainerPath);

            levelNumber = GetNode<Label>(dynamicLabelsList[levelNumberIndex]);
            scoreNumber = GetNode<Label>(dynamicLabelsList[scoreNumberIndex]);
            actionsNumber = GetNode<Label>(dynamicLabelsList[actionsNumberIndex]);
            toMapButton = GetNode<Button>(toMapButtonPath);

            starsScenesList.Add(threeStarsScenePath);
            starsScenesList.Add(twoStarsScenePath);
            starsScenesList.Add(oneStarScenePath);

            toMapButton.Connect(Resources.BUTTON_UP, this, nameof(SwitchScene));
        }

        public override void _Ready()
        {
            if (instance != null){  
                QueueFree();
                return;
            }
            instance = this;

            Init();
            LoadStarsScene();
            GetFinishedLevelDatas();
        }

        private void GetFinishedLevelDatas()
        {
            levelNumber.Text = gameManager._levelIndex.ToString();
            scoreNumber.Text = gameManager.Score.ToString();
            actionsNumber.Text = (MovementsManager.GetInstance().ActionsCount + 1).ToString();
        }

        private void LoadStarsScene() => starSceneContainer.AddChild(GD.Load<PackedScene>(starsScenesList[gameManager.ScoreRankIndex]).Instance());

        private void SwitchScene() => SceneChanger.GetInstance().ChangeScene(worldPackedScene);

        protected override void Dispose(bool pDisposing)
        {
            if (pDisposing && instance == this) instance = null;
            base.Dispose(pDisposing);
        }
    }
}