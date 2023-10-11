using Godot;
using System;
using Com.IsartDigital.Sokoban.Managers;
using Com.IsartDigital.Sokoban.Localization;

//Author : BENCTEUX Pierre-Antoine
namespace Com.IsartDigital.Sokoban.UI
{
    public class Tips : VBoxContainer
    {
        // References
        [Export] private NodePath levelIndexLabelPath;
        [Export] private NodePath levelTipLabelPath;

        // Parameters

        // Objects
        private Label levelIndexLabel;
        private Label levelTipLabel;

        private GameManager gameManager;

        // Properties
        private const string BASE_TIP_VALUE_ONE = "LEVEL_";
        private const string BASE_TIP_VALUE_TWO = "_TIP_";

        // Singleton instance
        static private Tips instance;
		
		static public Tips GetInstance () {
			if (instance == null) instance = new Tips();
		    return instance;
		}

        private Tips ():base() {}

        private void Init()
        {
            gameManager = GameManager.GetInstance();

            levelIndexLabel = GetNode<Label>(levelIndexLabelPath);
            levelTipLabel = GetNode<Label>(levelTipLabelPath);

            CallDeferred(nameof(GetLevelIndex));
        }

        public override void _Ready()
        {
            if (instance != null){  
                QueueFree();
                return;
            }
            instance = this;

            Init();
        }

        public void LoadInformations(int pTipNumber = 0)
        {
            if (pTipNumber == 0)
                levelTipLabel.Text = "";
            else
                levelTipLabel.Text = LocalizationManager.GetInstance().GetTranslation(BASE_TIP_VALUE_ONE + gameManager._levelIndex.ToString() + BASE_TIP_VALUE_TWO + pTipNumber.ToString());
        }
           
        // Deferred
        private void GetLevelIndex() => levelIndexLabel.Text = gameManager._levelIndex.ToString();

        protected override void Dispose(bool pDisposing)
        {
            if (pDisposing && instance == this) instance = null;
            base.Dispose(pDisposing);
        }
    }
}