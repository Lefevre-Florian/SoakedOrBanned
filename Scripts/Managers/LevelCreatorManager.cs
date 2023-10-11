using Godot;
using System;
using System.Collections.Generic;
using Com.IsartDigital.Sokoban.Objects;
using Com.IsartDigital.Tools.Resources;
using Com.IsartDigital.Sokoban.TerrainGeneration;
using Com.IsartDigital.Sokoban.Managers;

//Author : BOULIN Valère
namespace Com.IsartDigital.Sokoban.Managers
{
    public class LevelCreatorManager : Node2D
    {
        [Export] private readonly int gridWidth = 9;
        [Export] private readonly int gridHeight = 9;
        [Export] private readonly float TILE_APPARITION_DURATION = .5f;
        [Export] private readonly float TILE_APPARITION_SCALE_MULTIPLIER = 1.5f;
        [Export] private readonly int JSON_LEVEL_LINE = 178;
        [Export] private readonly int JSON_LEVEL_LINE_PAR_OFFSET = 3;
        [Export] private PackedScene levelPackagePackedScene;

        private bool dirtPlayerPlaced = false;
        private bool waterPlayerPlaced = false;

        [Export] private readonly NodePath gridContainerPath;
        private Node2D gridContainer;

        [Export] private readonly NodePath buttonContainerPath;
        private GridContainer buttonContainer;

        [Export] private readonly NodePath saveButtonPath;
        private Button saveButton;

        [Export] private readonly NodePath playButtonPath;
        private Button playButton;

        [Export] private readonly NodePath saveButtonTweenPath;
        private Tween saveButtonTween;

        [Export] private readonly NodePath parSpinboxPath;
        private SpinBox parSpinbox;

        [Export] private PackedScene activeTile;

        [Export] private readonly PackedScene dirtTile;
        [Export] private readonly PackedScene waterTile;
        [Export] private readonly PackedScene waterBoxTile;
        [Export] private readonly PackedScene dirtBoxTile;
        [Export] private readonly PackedScene dirtPlayerTile;
        [Export] private readonly PackedScene waterPlayerTile;
        [Export] private readonly PackedScene targetDirtTile;
        [Export] private readonly PackedScene targetWaterTile;
        [Export] private readonly PackedScene wallTile;

        private const string SAVE_LEVEL_OFFSET = "				\"#";
        private const string SAVE_PAR_OFFSET = "			\"par\": ";
        private const string SAVE_PAR_END = ",    ";

        private const string CHAR_TO_STRIP = "0123456789";
        private const string CHAR_TO_REPLACE = "@";

        #region Tiles Identifiers
        private Dictionary<string, string> tilesNames = new Dictionary<string, string>();
        private const string DIRT_CRATE_NAME = "DirtCrate";
        private const string DIRT_CRATE_SYMBOL = "b";
        private const string WATER_CRATE_NAME = "WaterCrate";
        private const string WATER_CRATE_SYMBOL = "B";
        private const string DIRT_BLOCK_NAME = "TileDirtBlock";
        private const string DIRT_BLOCK_SYMBOL = "-";
        private const string WATER_BLOCK_NAME = "TileWaterBlock";
        private const string WATER_BLOCK_SYMBOL = "+";
        private const string DIRT_TARGET_NAME = "TargetDirtBlock";
        private const string DIRT_TARGET_SYMBOL = "a";
        private const string WATER_TARGET_NAME = "TargetWaterBlock";
        private const string WATER_TARGET_SYMBOL = "A";
        private const string DIRT_PLAYER_NAME = "LandPlayer";
        private const string DIRT_PLAYER_SYMBOL = "p";
        private const string WATER_PLAYER_NAME = "WaterPlayer";
        private const string WATER_PLAYER_SYMBOL = "P";
        private const string WALL_NAME = "Wall";
        private const string WALL_SYMBOL = "#";
        #endregion

        private float tileSize = 100;
        private float tileScaleDividor = 1f;

        private const string PATH_LEVEL_JSON = "res://Ressources/leveldesign.json";

        public List<Node2D> tileList = new List<Node2D>();

        private static LevelCreatorManager instance;

        static public LevelCreatorManager GetInstance()
        {
            if (instance == null) instance = new LevelCreatorManager();
            return instance;
        }

        public override void _Ready()
        {
            if (instance != null)
            {
                QueueFree();
                return;
            }
            instance = this;

            #region Dictionnary Append

            tilesNames.Add(DIRT_CRATE_NAME, DIRT_CRATE_SYMBOL);
            tilesNames.Add(WATER_CRATE_NAME, WATER_CRATE_SYMBOL);
            tilesNames.Add(DIRT_BLOCK_NAME, DIRT_BLOCK_SYMBOL);
            tilesNames.Add(WATER_BLOCK_NAME, WATER_BLOCK_SYMBOL);
            tilesNames.Add(DIRT_TARGET_NAME, DIRT_TARGET_SYMBOL);
            tilesNames.Add(WATER_TARGET_NAME, WATER_TARGET_SYMBOL);
            tilesNames.Add(DIRT_PLAYER_NAME, DIRT_PLAYER_SYMBOL);
            tilesNames.Add(WATER_PLAYER_NAME, WATER_PLAYER_SYMBOL);
            tilesNames.Add(WALL_NAME, WALL_SYMBOL);

            #endregion

            saveButtonTween = GetNode<Tween>(saveButtonTweenPath);
            playButton = GetNode<Button>(playButtonPath);
            gridContainer = GetNode<Node2D>(gridContainerPath);
            buttonContainer = GetNode<GridContainer>(buttonContainerPath);
            saveButton = GetNode<Button>(saveButtonPath);
            parSpinbox = GetNode<SpinBox>(parSpinboxPath);

            saveButton.Connect(Resources.PRESSED, this, nameof(SaveLevel));
            playButton.Connect(Resources.PRESSED, this, nameof(PlayLevel));

            foreach (AnimatedButtons pButton in buttonContainer.GetChildren())
            {
                pButton.Connect(Resources.PRESSED, this, nameof(ButtonPressed), new Godot.Collections.Array() { pButton.Name.ToInt() });
            }

            GenerateTile();
        }

        public void SwapTile(Node2D pTile)
        {
            Node2D lNewTile = activeTile.Instance<Node2D>();

            if (!((lNewTile is Player && ((Player)lNewTile).IsWaterType && waterPlayerPlaced) || (lNewTile is Player && !((Player)lNewTile).IsWaterType && dirtPlayerPlaced)))
            {
                Tween lAnimationTween = new Tween();

                gridContainer.AddChild(lNewTile);
                lNewTile.Position = new Vector2(pTile.Position);

                lAnimationTween.InterpolateProperty(lNewTile, Resources.SCALE, lNewTile.Scale * TILE_APPARITION_SCALE_MULTIPLIER, lNewTile.Scale /= tileScaleDividor, TILE_APPARITION_DURATION, Tween.TransitionType.Expo, Tween.EaseType.Out);
                lNewTile.AddChild(lAnimationTween);
                lAnimationTween.Start();

                if (pTile is Player)
                {
                    if (((Player)pTile).IsWaterType)
                        waterPlayerPlaced = false;
                    else
                        dirtPlayerPlaced = false;
                }

                if (lNewTile is Player)
                {
                    if (((Player)lNewTile).IsWaterType)
                    {
                        waterPlayerPlaced = true;
                    }
                    else
                    {
                        dirtPlayerPlaced = true;
                    }
                }

                tileList.Insert(tileList.IndexOf(pTile), lNewTile);
                tileList.Remove(pTile);
                pTile.QueueFree();
            }
        }

        private void GenerateTile()
        {
            Tile lTile;

            for (float i = 0; i < tileSize * gridHeight; i += tileSize)
            {
                for (float j = 0; j < tileSize * gridWidth; j += tileSize)
                {
                    lTile = activeTile.Instance<Tile>();
                    lTile.Scale /= tileScaleDividor;
                    gridContainer.AddChild(lTile);
                    lTile.Position = new Vector2(j, i);
                    tileList.Add(lTile);
                }
            }
        }

        private void ButtonPressed(int pButtonId)
        {
            #region ButtonIdentifiers
            switch (pButtonId)
            {
                case 1:
                    activeTile = dirtTile;
                    break;
                case 2:
                    activeTile = waterTile;
                    break;
                case 4:
                    activeTile = dirtBoxTile;
                    break;
                case 3:
                    activeTile = waterBoxTile;
                    break;
                case 5:
                    activeTile = dirtPlayerTile;
                    break;
                case 6:
                    activeTile = waterPlayerTile;
                    break;
                case 7:
                    activeTile = targetDirtTile;
                    break;
                case 8:
                    activeTile = targetWaterTile;
                    break;
                case 9:
                    activeTile = wallTile;
                    break;
                default:
                    break;
            }
            #endregion

            foreach (Node2D pChild in TilePlacer.GetInstance().GetChildren())
                pChild.QueueFree();

            TilePlacer.GetInstance().AddChild(activeTile.Instance());
        }

        private void SaveLevel()
        {
            if (CanSave())
            {
                File lFile = new File();

                int lIndex = 0;
                string lText;
                string lTempChar;

                lFile.Open(PATH_LEVEL_JSON, File.ModeFlags.ReadWrite);

                FileSkipLine(lFile, JSON_LEVEL_LINE);
                lFile.StoreString(SAVE_PAR_OFFSET + parSpinbox.Value.ToString() + SAVE_PAR_END);
                FileSkipLine(lFile, JSON_LEVEL_LINE_PAR_OFFSET);

                lText = SAVE_LEVEL_OFFSET;
                for (int i = 0; i < gridHeight; i++)
                {
                    for (int j = 0; j < gridWidth; j++)
                    {
                        lTempChar = "";
                        tilesNames.TryGetValue(tileList[lIndex].Name.Replace(CHAR_TO_REPLACE, "").RStrip(CHAR_TO_STRIP), out lTempChar);
                        lText += lTempChar;
                        lIndex++;
                    }
                    lFile.StoreString(lText);
                    lFile.GetLine();
                    lText = SAVE_LEVEL_OFFSET;
                }
                lFile.Close();
                saveButtonTween.InterpolateProperty(saveButton, "modulate", new Color(0, 1, 0, 1), new Color(1, 1, 1, 1), .5f);
                saveButtonTween.Start();

                //GetTree().Quit();
            }
            else
            {
                saveButtonTween.InterpolateProperty(saveButton, "modulate", new Color(1, 0, 0, 1), new Color(1, 1, 1, 1), .5f);
                saveButtonTween.Start();
            }
            
        }

        bool CanSave()
        {
            bool lDirtTargetPlaced = false;
            bool lWaterargetPlaced = false;

            bool lDirtCratePlaced = false;
            bool lWaterCratePlaced = false;

            foreach (Node2D pTile in tileList)
            {
                if (pTile is Goal && ((Goal)pTile).IsWaterType)
                    lWaterargetPlaced = true;
                else if (pTile is Goal && !((Goal)pTile).IsWaterType)
                    lDirtTargetPlaced = true;
                else if (pTile is PushableBox && ((PushableBox)pTile).IsWaterType)
                    lWaterCratePlaced = true;
                else if (pTile is PushableBox && !((PushableBox)pTile).IsWaterType)
                    lDirtCratePlaced = true;
            }
            if (waterPlayerPlaced && dirtPlayerPlaced && lWaterargetPlaced && lDirtTargetPlaced && lDirtCratePlaced && lWaterCratePlaced)
                return true;
            else
                return false;
        } 

        void PlayLevel()
        {
            SceneChanger.GetInstance().ChangeScene(levelPackagePackedScene);
            GridManager.GetInstance().SetLevelIndex(9);
        }

        void FileSkipLine(File pFile, int pNumberOfLines)
        {
            for (int i = 0; i < pNumberOfLines; i++)
            {
                pFile.GetLine();
            }
        }

        protected override void Dispose(bool pDisposing)
        {
            if (pDisposing && instance == this) instance = null;
            base.Dispose(pDisposing);
        }
    }
}

