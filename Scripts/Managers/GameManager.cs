using Godot;
using System;
using System.Collections.Generic;
using Com.IsartDigital.Sokoban.Abstracts;
using Com.IsartDigital.Sokoban.Objects;
using Com.IsartDigital.Sokoban.UI.LevelSelector;
using Com.IsartDigital.Sokoban.Architectures;
using Com.IsartDigital.Sokoban.TerrainGeneration;

//Author : BENCTEUX Pierre-Antoine
namespace Com.IsartDigital.Sokoban.Managers
{
    public class GameManager : Node2D
    {
        [Export(PropertyHint.File)] private string winScenePath = "";
        [Export] private NodePath popupContainerPath;

        [Export] private Color blipColor = default;
        [Export] private float blipDuration;
        [Export] private int nBlip;
        [Export] private Tween.TransitionType blipTransition = default;
        [Export] private Tween.EaseType blipEaseType = default;

        // const
        private const int _maxNbPlayers = 2;

        private const int PERFECT_SCORE = 5000;
        private const int MEDIUM_SCORE = 2000;
        private const int LOW_SCORE = 1000;
        private const int MINUS_PER_ACTION_BRONZE = 50;

        private const int BONUS_PER_ACTION_SAVED = 100;
        private const int THREE_STARS_INDEX = 0;
        private const int TWO_STARS_INDEX = 1;
        private const int ONE_STAR_INDEX = 2;

        private const string PROPERTY_MODULATE = "modulate";

        // Objects
        private List<Player> _playerList = new List<Player>();
        private Player _landPlayer;
        private Player _waterPlayer;

        // Properties
        private bool _isWaterPlayerSelected = true;

        private List<Goal> _targets = new List<Goal>();
        public int _levelIndex = 0;

        private int _score = 0;
        private int _scoreRankIndex = 0;

        #region Getters/Setters
        public List<Player> PlayerList
        {
            get { return _playerList; }
            private set { _playerList = value; }
        }

        public Player LandPlayer
        {
            get { return _landPlayer; }
            private set { _landPlayer = value; }
        }

        public Player WaterPlayer
        {
            get { return _waterPlayer; }
            private set { _waterPlayer = value; }
        }

        public bool IsWaterPlayerSelected
        {
            get { return _isWaterPlayerSelected; }
            private set { _isWaterPlayerSelected = value; }
        }

        public List<Goal> Targets
        {
            get { return _targets; }
            private set { _targets = value; }
        }

        public int Score
        {
            get { return _score; }
            private set { _score = value; }
        }

        public int ScoreRankIndex
        {
            get { return _scoreRankIndex; }
            private set { _scoreRankIndex = value; }
        }
        #endregion

        // Singleton instance
        static private GameManager instance;

        // Signals

        [Signal]
        public delegate void Switch(Player pCurrent);

		static public GameManager GetInstance () {
			if (instance == null) instance = new GameManager();
		    return instance;
		}

        private GameManager ():base() {}

        public override void _Ready()
        {
            if (instance != null){  
                QueueFree();
                return;
            }
            instance = this;

            Tsunami.GetInstance().Visible = false;

            // To able/disable players' processes once at start to control only one after everything is loaded
            CallDeferred(nameof(ChangeCharacterSelected));
        }

        public override void _Process(float pDelta)
        {
            Controller.SwapCharacter();
        }

        public void AddToPlayerList(Player pPlayerSprite)
        {
            // Check if _playerList is already filled with the 2 players
            if (_playerList.Count < _maxNbPlayers)
            {
                // Checks which player is given in function and saves him as the player he should be
                if (!pPlayerSprite.IsWaterType)
                    _landPlayer = pPlayerSprite;
                else
                    _waterPlayer = pPlayerSprite;
                _playerList.Add(pPlayerSprite);
            }
        }

        public void ChangeCharacterSelected()
        {
            // Unlock controls
            Controller.canPlay = true;

            // Change player selected
            Player lCharacterSelected;
            _isWaterPlayerSelected = !_isWaterPlayerSelected;

            // Enable/Disable player's process according to current player selected
            if (_isWaterPlayerSelected)
            {
                _waterPlayer.SetProcess(true);
                _landPlayer.SetProcess(false);

                lCharacterSelected = _waterPlayer;
            }
            else
            {
                _waterPlayer.SetProcess(false);
                _landPlayer.SetProcess(true);

                lCharacterSelected = _landPlayer;
            }

            EmitSignal(nameof(Switch), lCharacterSelected);

            SceneTreeTween lTween = GetTree().CreateTween();
            Color lCurrentColor = lCharacterSelected.Modulate;
            lTween.Chain();
            float lDuration = blipDuration / nBlip;
            for (int i = 0; i < nBlip; i++)
            {
                lTween.TweenProperty(lCharacterSelected, PROPERTY_MODULATE, blipColor, lDuration / 2)
                                    .SetTrans(blipTransition)
                                    .SetEase(blipEaseType);
                lTween.TweenProperty(lCharacterSelected, PROPERTY_MODULATE, lCurrentColor, lDuration / 2)
                                    .SetTrans(blipTransition)
                                    .SetEase(blipEaseType);
            }
            lTween.Play();
        }

        public void AddTargetToGoals(Goal pTarget) => _targets.Add(pTarget);

        /// <summary>
        /// Check if there's one target that is not full if it's not the case all
        /// targets are full the game can end if it's not continue
        /// If ending calculate the score of the player based on const - the player par
        /// </summary>
        public void EndGame()
        {
            
            if (_targets.Exists(lGoal => !lGoal.IsFull)) return;
            if (Map.unlockedLevel <= _levelIndex)
            {
                if (DatabaseManager.User != null)
                {
                    User lUser = DatabaseManager.User.Value;
                    lUser.levelUnlocked = _levelIndex + 1;
                    DatabaseManager.User = lUser;
                }
                else
                    Map.unlockedLevel = _levelIndex + 1;
                LevelLoader.levelPatterns[_levelIndex + 1].locked = false;
            }
            int lLevelPar = GridManager.GetInstance().GetPar();
            int lPlayerPar = MovementsManager.GetInstance().ActionsCount + 1;

            if (lPlayerPar <= lLevelPar)
            {
                _score = PERFECT_SCORE;
                _scoreRankIndex = THREE_STARS_INDEX;
            }
            else if (lPlayerPar > lLevelPar && lPlayerPar <= (lLevelPar + (lLevelPar / 2)))
            {
                _score = PERFECT_SCORE - (PERFECT_SCORE - MEDIUM_SCORE) / (lLevelPar / 2) * (lPlayerPar - lLevelPar) / 2;
                _scoreRankIndex = TWO_STARS_INDEX;
            }
            else if (lPlayerPar > (lLevelPar + (lLevelPar / 2)))
            {
                _score = MEDIUM_SCORE - ((lPlayerPar - (lLevelPar + (lLevelPar / 2))) * MINUS_PER_ACTION_BRONZE);
                if (_score < LOW_SCORE) _score = LOW_SCORE;
                _scoreRankIndex = ONE_STAR_INDEX;
            }

            if (DatabaseManager.User != null)
                ScoreManager.GetInstance().AddPlayerScore(_levelIndex, _score, (User)DatabaseManager.User);

            GetNode<Node2D>(popupContainerPath).GetChild(0).AddChild(GD.Load<PackedScene>(winScenePath).Instance());

            Tsunami.GetInstance().Visible = true;
            Tsunami.GetInstance().anim.Play("Come");
        }

        /// <summary>
        /// Method called by the grid Manager to tell to the game manager that he can
        /// start the game (Settup targets blocks)
        /// </summary>
        public void StartGame()
        {
            List<Crate> lBoxes = new List<Crate>();
            foreach (PushableBox lBox in MovementsManager.GetInstance().BoxesList) lBoxes.Add((Crate)lBox);
            foreach (Goal lTarget in _targets) lTarget.ConnectBoxes(lBoxes);
        }

        protected override void Dispose(bool pDisposing)
        {
            if (pDisposing && instance == this) instance = null;
            base.Dispose(pDisposing);
        }
    }
}