using Godot;
using System;
using Com.IsartDigital.Sokoban.TerrainGeneration;
using System.Collections.Generic;

//Author : VERDIER Thomas 
namespace Com.IsartDigital.Sokoban.UI.LevelSelector {

	public class ButtonContainer : GridContainer
	{
		public static ButtonContainer Instance;

		[Export] int buttonPerColumns;

		[Export] PackedScene buttonFactory;

		List<LevelPattern> lLevels;
		public override void _Ready()
		{

			if (Instance != null)
            {
				QueueFree();
				return;
            }
			Instance = this;
			Columns = buttonPerColumns;
			lLevels = LevelLoader.levelPatterns;
            for (int i = 0; i < lLevels.Count; i++)
            {
				ButtonSelector lButton = (ButtonSelector)buttonFactory.Instance();
				lButton.indexLevel = i;
				AddChild(lButton);
				if (lLevels[i].locked) lButton.Disabled = true;
            }
		}

		public void SetButtons()
        {
			for (int i = 0; i < GetChildCount(); i++)
			{
				if (lLevels[i].locked) ((Button)GetChild(i)).Disabled = true;
				else ((Button)GetChild(i)).Disabled = false;
			}
		}

        protected override void Dispose(bool disposing)
        {
			Instance = null;
            base.Dispose(disposing);
        }

    }

	

}