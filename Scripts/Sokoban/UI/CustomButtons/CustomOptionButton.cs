using Godot;
using System;
using Com.IsartDigital.Utils.Events;
//Author : VERDIER Thomas 

namespace Com.IsartDigital.Sokoban.UI {

	public class CustomOptionButton : TranslableButton
	{
		[Export(PropertyHint.File)] string optionsPath;

		public override void _Ready()
		{
			base._Ready();
			Connect(EventButton.BUTTON_UP, this, nameof(openOptions));
		}

		private void openOptions()
        {
			GetParent().GetParent().GetParent().AddChild(((PackedScene)GD.Load(optionsPath)).Instance());
			GetTree().Paused = true;
        }

	}

}