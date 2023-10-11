using Godot;
using System;
using Com.IsartDigital.Tools.Resources;
using Com.IsartDigital.Sokoban.UI;

//Author : BENCTEUX Pierre-Antoine
namespace Com.IsartDigital.Sokoban.Managers
{
    public class TipsManager : Control
    {
        // References
        [Export] private NodePath globalPanelPath;
        [Export] private NodePath tipButPath;
        [Export] private NodePath prevTipButPath;
        [Export] private NodePath nextTipButPath;
        [Export] private NodePath tipSectionPath;
        [Export] private NodePath rulesPath;

        // Parameters
        [Export] private float tipButOnPanelMargin = 64f;
        [Export] private float xSwtButsMargin = 12f;
        [Export] private float tipSectionTopMargin = 40f;

        // Tween Parameters
        [Export] private float durationGlobalPanelMove = 1f;

        // Objects
        private SceneTreeTween tweenPanelAnim;

        private Panel globalPanel;
        private Button tipBut;
        private Button prevTipBut;
        private Button nextTipBut;
        private Tips tipSection;
        private Control rules;

        // Properties
        private float ySwtButsMargin;
        private int actualTipNb = 0;
        private int minTipNb = 0;
        private int maxTipNb = 3;

        private bool _areTipsShown = false;

        // Tween properties
        private Vector2 globalPanelBasePos;
        private Vector2 globalPanelMovedPos;

        public bool AreTipsShown
        {
            get { return _areTipsShown; }
            private set { _areTipsShown = value; }
        }

        // Singleton instance
        static private TipsManager instance;

		static public TipsManager GetInstance () {
			if (instance == null) instance = new TipsManager();
		    return instance;
		}

        private TipsManager ():base() {}

        private void Init()
        {
            globalPanel = GetNode<Panel>(globalPanelPath);
            tipBut = GetNode<Button>(tipButPath);
            prevTipBut = GetNode<Button>(prevTipButPath);
            nextTipBut = GetNode<Button>(nextTipButPath);
            tipSection = GetNode<Tips>(tipSectionPath);
            rules = GetNode<Control>(rulesPath);

            ySwtButsMargin = prevTipBut.RectSize.y / 2;
            globalPanelBasePos = new Vector2(globalPanel.RectPosition.x - globalPanel.RectSize.x, globalPanel.RectPosition.y);
            globalPanelMovedPos = new Vector2(globalPanelBasePos.x + globalPanel.RectSize.x, globalPanel.RectPosition.y);

            tipBut.RectPosition = new Vector2(globalPanel.RectPosition.x + globalPanel.RectSize.x - tipButOnPanelMargin, 0);
            prevTipBut.RectPosition = new Vector2(xSwtButsMargin, globalPanel.RectSize.y / 2 - ySwtButsMargin);
            nextTipBut.RectPosition = new Vector2(globalPanel.RectSize.x - nextTipBut.RectSize.y , globalPanel.RectSize.y / 2 - ySwtButsMargin);
            globalPanel.RectPosition = globalPanelBasePos;

            tipBut.Connect(Resources.BUTTON_UP, this, nameof(ShowTipsPanel));
            prevTipBut.Connect(Resources.BUTTON_UP, this, nameof(ModifyTipPageIndex), new Godot.Collections.Array { false });
            nextTipBut.Connect(Resources.BUTTON_UP, this, nameof(ModifyTipPageIndex), new Godot.Collections.Array { true });

            ModifyTipPageIndex(false);

            CallDeferred(nameof(CheckIfTutoLevel));
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

        private void CheckIfTutoLevel()
        {
            if (GameManager.GetInstance()._levelIndex == 0) ShowTipsPanel();
        }

        public void ShowTipsPanel()
        {
            tweenPanelAnim = CreateTween();

            if (!_areTipsShown)
                tweenPanelAnim.TweenProperty(globalPanel, Resources.RECT_POSITION, globalPanelMovedPos, durationGlobalPanelMove).SetEase(Tween.EaseType.Out);
            else
                tweenPanelAnim.TweenProperty(globalPanel, Resources.RECT_POSITION, globalPanelBasePos, durationGlobalPanelMove).SetEase(Tween.EaseType.In);

            _areTipsShown = !_areTipsShown;
        }

        private void ModifyTipPageIndex(bool pToNext)
        {
            if (pToNext && actualTipNb < maxTipNb)
                actualTipNb++;
            else if (!pToNext && actualTipNb > minTipNb)
                actualTipNb--;

            tipSection.LoadInformations(actualTipNb);

            if (actualTipNb == maxTipNb)
                nextTipBut.Visible = false;
            else if (actualTipNb == minTipNb)
            {
                prevTipBut.Visible = false;
                rules.Visible = true;
            }
            else
            {
                nextTipBut.Visible = true;
                prevTipBut.Visible = true;
                rules.Visible = false;
            }
        }

        protected override void Dispose(bool pDisposing)
        {
            if (pDisposing && instance == this) instance = null;
            base.Dispose(pDisposing);
        }
    }
}
