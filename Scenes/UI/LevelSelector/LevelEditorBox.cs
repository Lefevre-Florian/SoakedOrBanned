using Godot;
using System;

//Author : VERDIER Thomas 

namespace Com.IsartDigital.Sokoban.UI.LevelSelector {

	public class LevelEditorBox : Area2D
	{
		[Export(PropertyHint.File)] PackedScene PathToEditor;
		public override void _Ready()
		{
			Connect("area_entered", this, "GoToMapEditor");
		}

		private void GoToMapEditor(Area2D pCollsion)
        {
			SceneChanger.GetInstance().ChangeScene(PathToEditor);
        }

	}

}