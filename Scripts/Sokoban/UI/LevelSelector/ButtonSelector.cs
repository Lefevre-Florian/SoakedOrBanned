using Godot;
using System;
using Com.IsartDigital.Sokoban.TerrainGeneration;
using Com.IsartDigital.Utils.Events;

//Author : VERDIER Thomas 
namespace Com.IsartDigital.Sokoban.UI.LevelSelector {

	public class ButtonSelector : Button
	{
		private const string PROPERTY_GLOBAL_POSITION = "global_position";

		public int indexLevel;

		private Tween tween = new Tween();

		public override void _Ready()
		{
			Text = indexLevel.ToString();
			Connect(EventButton.PRESSED, this, nameof(TpOrNotTp));
			Connect(EventNode.TREE_EXITING, this, nameof(Destructor));
			AddChild(tween);
		}

		private void TpOrNotTp()
        {
            foreach (LevelZone lZone in Map.GetInstance().levelZoneHandler.GetChildren())
            {
				if (lZone.numLevel == indexLevel)
                {
					if (!LevelLoader.levelPatterns[indexLevel].locked)
					{
						tween.InterpolateProperty(SelectorPlayer.GetInstance(), PROPERTY_GLOBAL_POSITION, SelectorPlayer.GetInstance().GlobalPosition,lZone.GlobalPosition,0.1f);
						tween.Start();
					}
                }
            }
        }

		private void Destructor()
        {
			Disconnect(EventButton.PRESSED, this, nameof(TpOrNotTp));
			Disconnect(EventNode.TREE_EXITING, this, nameof(Destructor));
        }

	}

}