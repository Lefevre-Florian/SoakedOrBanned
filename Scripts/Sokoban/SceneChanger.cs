using Godot;
using System;
using Com.IsartDigital.Sokoban.UI.LevelSelector;

//Author : VERDIER Thomas 
namespace Com.IsartDigital.Sokoban {

	public class SceneChanger : Node
	{
		private static SceneChanger instance;

		const string INPUT_ESCAPE = "esc";
		const string MAP_STRING = "Com.IsartDigital.Sokoban.UI.LevelSelector.Map";
		const string TITLE_STRING = "Com.IsartDigital.Sokoban.UI.Screen.TitleCard";

		[Export(PropertyHint.File)] string baseScene;
		[Export] private PackedScene titlePackedScene;
		[Export] private PackedScene worldPackedScene;

		[Export] NodePath childHandlerPath;
		Node childHandler;

		private SceneChanger() : base() { }

		public override void _Ready()
		{
            if (instance != null)
            {
				QueueFree();
				return;
            }
			instance = this;
			childHandler = GetNode<Node>(childHandlerPath);
			ChangeScene(titlePackedScene);
		}

        public override void _Process(float delta)
        {
			if (Input.IsActionJustPressed(INPUT_ESCAPE))
            {
				if (childHandler.GetChild(0).GetType().ToString() is MAP_STRING)
					ChangeScene(titlePackedScene);
                else if (!(childHandler.GetChild(0).GetType().ToString() is TITLE_STRING))
					ChangeScene(worldPackedScene);
			}
                

			base._Process(delta);
        }

        public void ClearChildHandler()
        {
			foreach (Node lChild in childHandler.GetChildren())
			{
				lChild.QueueFree();
			}
		}

		public void ChangeScene(PackedScene pPathScene)
        {
			ClearChildHandler();
			PackedScene lScene = pPathScene;
			childHandler.AddChild(lScene.Instance());
        }
		
		public static SceneChanger GetInstance()
        {
			if (instance == null) instance = new SceneChanger();
				return instance;
        }

        protected override void Dispose(bool pDisposing)
        {
			if (pDisposing && instance != null) instance = null;
				base.Dispose(pDisposing);
        }

    }

}