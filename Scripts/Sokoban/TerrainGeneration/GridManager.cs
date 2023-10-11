using Godot;
using System;
using System.Collections.Generic;
using Com.IsartDigital.Utils.Events;
using Com.IsartDigital.Sokoban.TerrainGeneration;
using Com.IsartDigital.Sokoban.Objects;

// Author : Lefevre Florian
namespace Com.IsartDigital.Sokoban.Managers {
	
    public class GridManager : Node2D
    {
        private static GridManager instance;

        [Export] private Dictionary<string, string> lvlElements = new Dictionary<string, string>();
        [Export] private NodePath gridContainerPath = default;
        [Export] private int _levelIndex = 0;

        [Export(PropertyHint.Range, "0f, 1.5f, 0.1f, or_greater")] private float duration = 0f;
        [Export] private List<float> waveScaleValues = new List<float>() { 0f, 2f, 1f, 0.5f, 1f};
        [Export] private Tween.TransitionType transitionType = default;
        [Export] private Tween.EaseType transitionEase = default;

        [Export(PropertyHint.Range, "0f, 1.5f, 0.1f, or_greater")] private float tokenSpawnDuration = 0f;
        [Export] private float spawnScale = 0f;
        [Export] private Tween.TransitionType tokenTransition = default;
        [Export] private Tween.EaseType tokenEase = default;

        private const float BASE_TILE_SIZE = 100f;

        private const string PROPERY_SCALE = "scale";
        private readonly Vector2 BASE_SCALE = Vector2.One;

        private Vector2 screenSize;
        private Node gridContainer = null;

        private GameManager gameManager = null;
        private MovementsManager movementsManager = null;

        private List<Sprite> tokens = new List<Sprite>();

        public static int TILE_SKIPPING;

        public int LevelIndex
        {
            get { return _levelIndex; }
            private set { _levelIndex = value; }
        }

        private Sprite[,] _spriteList;
        public Sprite[,] SpriteList
        {
            get { return _spriteList; }
            private set { _spriteList = value; }
        }

        private GridManager ():base() {}

        public override void _Ready()
        {
            if (instance != null){  
                QueueFree();
                return;
            }
            instance = this;

            lvlElements = AmbianceManager.GetAnAmbiance().elements;

            gameManager = GameManager.GetInstance();
            movementsManager = MovementsManager.GetInstance();

            screenSize = GetViewportRect().Size;
            gridContainer = GetNode<Node>(gridContainerPath);
        }

        public static GridManager GetInstance()
        {
            if (instance == null) instance = new GridManager();
            return instance;
        }

        public void DrawLevel()
        {
            LevelPattern lLevelPattern = LevelLoader.levelPatterns[_levelIndex];
            if (lLevelPattern == null)
                return;

            int lLength = lLevelPattern.Map.Count;
            _spriteList = new Sprite[lLength, lLevelPattern.Map[0].Length];

            Tween lTween = new Tween();
            float lDelay = 0f;
            AddChild(lTween);

            for (int i = 0; i < lLength; i++)
            {
                for (int j = 0; j < lLevelPattern.Map[i].Length; j++)
                {

                    if (!lvlElements.ContainsKey(lLevelPattern.Map[i][j].ToString()))
                        continue;

                    if (lLevelPattern.Map[i][j] == (int)TileCodex.WATERPLAYER || lLevelPattern.Map[i][j] == (int)TileCodex.WATERBOX)
                        _spriteList[i, j] = TilingProcess(lvlElements[Convert.ToChar((int)TileCodex.WATERTILE).ToString()],
                                      lLength,
                                      new Vector2(j, i));

                    else if (lLevelPattern.Map[i][j] == (int)TileCodex.DIRTPLAYER || lLevelPattern.Map[i][j] == (int)TileCodex.DIRTBOX)
                        _spriteList[i, j] = TilingProcess(lvlElements[Convert.ToChar((int)TileCodex.DIRTTILE).ToString()],
                                      lLength,
                                      new Vector2(j, i));

                    _spriteList[i, j] = TilingProcess(lvlElements[lLevelPattern.Map[i][j].ToString()],
                                                      lLength,
                                                      new Vector2(j, i));

                    if(!(_spriteList[i,j] is PushableBox))
                        SetAnimation(lTween, lDelay, duration/lLength, _spriteList[i,j]);
                }
                lDelay = lTween.GetRuntime();
            }
            DistributingZIndexes();

            lTween.Start();
            lTween.Connect(EventTween.TWEEN_ALL_COMPLETED, this, nameof(SpawnPlayingTile));

            gameManager._levelIndex = LevelIndex;
            gameManager.StartGame();
        }

        private Sprite TilingProcess(string pChar, int pGridLength, Vector2 pGridPosition)
        {
            Sprite lSprite = GD.Load<PackedScene>(pChar).Instance<Sprite>();
            gridContainer.AddChild(lSprite);

            lSprite.Offset = new Vector2(lSprite.Texture.GetSize().x / 2, lSprite.Texture.GetSize().y / 2);

            lSprite.GlobalPosition = new Vector2(screenSize.x / 2 - ((pGridLength) / 2 * BASE_TILE_SIZE) + (BASE_TILE_SIZE * pGridPosition.x),
                                                 screenSize.y / 2 - ((pGridLength) / 2 * BASE_TILE_SIZE) + (BASE_TILE_SIZE * pGridPosition.y));

            // Identifiying if the tile is a player, a goal or a box
            if (lSprite is IndexableTile lTile)
            {
                if(lTile is Goal) gameManager.AddTargetToGoals((Goal)lTile);
                else
                {
                    if (lTile is Player)
                    {

                        gameManager.AddToPlayerList((Player)lTile);
                        lSprite.Offset = new Vector2(lSprite.Offset.x,
                                                lSprite.Offset.y - (lSprite.Texture.GetSize().y - lSprite.Offset.y));
                    }
                    else
                        movementsManager.AddToBoxesList((PushableBox)lTile);
                    if (lTile is Crate)
                    lSprite.Offset = new Vector2(lSprite.Offset.x,
                                                 lSprite.Offset.y - (lSprite.Texture.GetSize().y - 100));

                    lTile.Scale = Vector2.Zero;
                    tokens.Add(lTile);
                }
            }

            if (lSprite is Tile) ((Tile)lSprite).SetIndex(pGridPosition.x, pGridPosition.y);
            return lSprite;
        }

        private void DistributingZIndexes()
        {
            int lRows = _spriteList.GetLength(0);
            int lColumns = _spriteList.GetLength(1);

            int lNBoxes = movementsManager.BoxesList.Count + 1;

            for (int i = 0; i < lRows; i++)
            {
                for (int j = 0; j < lColumns; j++)
                {
                    if (_spriteList[i, j] is Player)
                        _spriteList[i, j].ZIndex = ((i + 1) * (lNBoxes + 1)) - 1; 
                    else if (_spriteList[i, j] is Crate)
                        _spriteList[i, j].ZIndex = (i * (lNBoxes + 1)) + 1;
                    else
                        _spriteList[i, j].ZIndex = i * (lNBoxes + 1);
                }
            }

            TILE_SKIPPING = lNBoxes + 1;
        }

        private void SpawnPlayingTile()
        {
            SceneTreeTween lTween = GetTree().CreateTween();
            
            foreach (Sprite lItem in tokens)
            {
                lTween.Chain();
                lTween.TweenProperty(lItem, PROPERY_SCALE, new Vector2(spawnScale, spawnScale), (tokenSpawnDuration/2) / tokens.Count)
                           .SetTrans(tokenTransition)
                           .SetEase(tokenEase);
                lTween.TweenProperty(lItem, PROPERY_SCALE, BASE_SCALE, (tokenSpawnDuration / 2) / tokens.Count)
                           .SetTrans(tokenTransition)
                           .SetEase(tokenEase);
            }
            lTween.Parallel();
            tokens.Clear();
        }

        private void SetAnimation(Tween pTween, float pDelay, float pDuration, Sprite pTile)
        {
            pTile.Scale = Vector2.Zero;
            int lLength = waveScaleValues.Count;
            for (int i = 0; i < lLength-1; i++)
            {
                pTween.InterpolateProperty(pTile,
                                           PROPERY_SCALE,
                                           BASE_SCALE * waveScaleValues[i],
                                           BASE_SCALE * waveScaleValues[i+1],
                                           pDuration,
                                           transitionType,
                                           transitionEase,
                                           delay: pDelay);
            }
        }

        public int GetPar()
        {
            return LevelLoader.levelPatterns[_levelIndex].Par;
        }

        public void SetLevelIndex(int pLvlIndex)
        {
            if (pLvlIndex < 0 || pLvlIndex > LevelLoader.levelPatterns.Count)
                _levelIndex = LevelLoader.levelPatterns.Count;
            else
                _levelIndex = pLvlIndex;
            DrawLevel();
        }

        protected override void Dispose(bool pDisposing)
        {
            if (pDisposing && instance == this) instance = null;
            base.Dispose(pDisposing);
        }
    }
}