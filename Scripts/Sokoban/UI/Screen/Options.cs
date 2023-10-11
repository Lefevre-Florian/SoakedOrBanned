using Godot;
using System;
using Com.IsartDigital.Sokoban.Localization;
using Com.IsartDigital.Sokoban.Managers;
using Com.IsartDigital.Utils.Events;
using Com.IsartDigital.Sokoban.Architectures;

//Author : VERDIER Thomas 
namespace Com.IsartDigital.Sokoban.UI {

	public class Options : Control
	{
		private const string EVENT_OPTION_BUTTON_SELECTION = "item_selected";
		private const string EVENT_HSLIDER_VALUE_CHANGED = "value_changed";

		[Export] NodePath volumeSliderPath = default;
		[Export] NodePath volumeSFXSliderPath = default;
		[Export] NodePath optionButtonLanguagePath = default;
		[Export] NodePath closeButtonPath = default;

		//private const float DB_CONVERTER = 100f;
		
		private HSlider volumeSlider = null;
		private HSlider volumeSFXSlider = null;
		
		private OptionButton optionButtonLanguage = null;
		
		private Button closeButton = null;

		private DatabaseManager databaseManager = null;
		
		public override void _Ready()
		{
			volumeSlider = GetNode<HSlider>(volumeSliderPath);
			volumeSFXSlider = GetNode<HSlider>(volumeSFXSliderPath);

			volumeSlider.Value = SoundManager.globalMusic;
			volumeSFXSlider.Value = SoundManager.globalSFX;

			volumeSlider.Connect(EVENT_HSLIDER_VALUE_CHANGED, this, nameof(ChangeVolume));
			volumeSFXSlider.Connect(EVENT_HSLIDER_VALUE_CHANGED, this, nameof(ChangeVolumeSFX));

			optionButtonLanguage = GetNode<OptionButton>(optionButtonLanguagePath);
			AddLanguages();
			optionButtonLanguage.Connect(EVENT_OPTION_BUTTON_SELECTION, this, nameof(OnItemSelection));
			
			closeButton = GetNode<Button>(closeButtonPath);
			closeButton.Connect(EventButton.PRESSED, this, nameof(CloseAndDie));

			databaseManager = DatabaseManager.GetInstance();

			Connect(EventNode.TREE_EXITING, this, nameof(Destructor));
		}

		private void AddLanguages()
        {
			foreach (string lName in Enum.GetNames(typeof(Languages)))
				optionButtonLanguage.AddItem(lName);
        }

		private void OnItemSelection(int pIndex) => LocalizationManager.GetInstance().SaveLanguage((Languages)pIndex + 1);

		private void ChangeVolume(float pValue) => SoundManager.GetInstance().ChangeAudioPlayerMusicVolume(pValue);

		private void ChangeVolumeSFX(float pValue) => SoundManager.GetInstance().ChangeAudioPlayersVFXVolume(pValue);

		private void CloseAndDie()
        {
			if(DatabaseManager.User != null)
            {
				User lUser = DatabaseManager.User.Value;
				lUser.SetSFX((float)SoundManager.globalSFX);
				lUser.SetVolume((float)SoundManager.globalMusic);
				lUser.SetLangage((int)LocalizationManager.Languages);
				DatabaseManager.User = lUser;
            }
			GetTree().Paused = false;
			databaseManager.SaveDatabase();
			QueueFree();
        }

		private void Destructor()
        {
			optionButtonLanguage.Disconnect(EVENT_OPTION_BUTTON_SELECTION, this, nameof(OnItemSelection));

			volumeSlider.Disconnect(EVENT_HSLIDER_VALUE_CHANGED, this, nameof(ChangeVolume));
			volumeSFXSlider.Disconnect(EVENT_HSLIDER_VALUE_CHANGED, this, nameof(ChangeVolumeSFX));

			closeButton.Disconnect(EventButton.PRESSED, this, nameof(CloseAndDie));

			Disconnect(EventNode.TREE_EXITING, this, nameof(Destructor));
		}
	}

}