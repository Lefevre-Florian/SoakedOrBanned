using Godot;
using System;
using Com.IsartDigital.Sokoban.Localization;
using Com.IsartDigital.Utils.Events;

// Author : Lefevre Florian
namespace Com.IsartDigital.Sokoban.UI {

	public class TranslableButton : AnimatedButtons
	{
		/// <summary>
		/// Global summary :
		/// - Use this class on a Button to change the text of the button dynamicaly
		/// - The translationKey is used to get the correct text to print inside the object
		/// (The translation key is trim to avoid blank space problem in the string comparaison)
		/// </summary>

		private string translationKey;
		private LocalizationManager translationServer = null;

		public override void _Ready()
		{
			base._Ready();
			translationServer = LocalizationManager.GetInstance();
			translationKey = Text.Trim();

			Connect(EventNode.TREE_EXITING, this, nameof(Destructor));
			translationServer.Connect(nameof(LocalizationManager.LangageChanged),
									  this,
									  nameof(Translate));
			Translate();
		}

		/// <summary>
		/// Update Text using the translation server and the private key
		/// </summary>
		private void Translate()
        {
			Text = translationServer.GetTranslation(translationKey);
        }

		/// <summary>
		/// Execute this when the object is about to leave the scene
		/// (Can be use to clean the object before final destruction)
		/// </summary>
		private void Destructor()
        {
			Disconnect(EventNode.TREE_EXITING, this, nameof(Destructor));
			if(translationServer != null)
            {
				translationServer.Disconnect(nameof(LocalizationManager.LangageChanged),
									  this,
									  nameof(Translate));
				translationServer = null;
			}

		}

	}

}