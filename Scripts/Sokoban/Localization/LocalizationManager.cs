using Godot;
using System.Collections.Generic;

// Author : Lefevre Florian
namespace Com.IsartDigital.Sokoban.Localization
{

    public class LocalizationManager : Node
	{
		private static LocalizationManager instance = null;

		private const string PATH_LOCALIZATION_FILE = "res://Ressources/Localization.csv";
		private const string COMMA_SPERATOR = ";";

		private static Languages _language = Languages.English; // Default language is English

		public static Languages Languages
        {
			get { return _language; }
			private set { _language = value; }
        }

		// Load the dictionary in static to keep it in memory
		private static Dictionary<Languages, Dictionary<string, string>> translations = new Dictionary<Languages, Dictionary<string, string>>();

		// Signal to emit when the langage of the game is changed
		[Signal]
		public delegate void LangageChanged();

		private LocalizationManager() : base() { }

		public override void _Ready()
		{
			if(instance != null)
            {
				QueueFree();
				return;
            }
			if (translations.Count <= 0)
				InitTranslationServer();	
		}

		public static LocalizationManager GetInstance()
        {
			if(instance == null) instance = new LocalizationManager();
			return instance;
        }

		/// <summary>
		/// Initialize translation library (Dictionary) reading the CSV file containing every text and keys
		/// </summary>
		private void InitTranslationServer()
        {
			File lFile = new File();
			if (lFile.FileExists(PATH_LOCALIZATION_FILE))
			{
				lFile.Open(PATH_LOCALIZATION_FILE, File.ModeFlags.Read);

				Dictionary<string, string> lFrenchDictionary = new Dictionary<string, string>();
				Dictionary<string, string> lEnglishDictionary = new Dictionary<string, string>();

				int lEnglishIndex = (int)Languages.English;
				int lFrenchIndex = (int)Languages.Francais;

				string[] lLine;
				int lLineLength = lFile.GetCsvLine(COMMA_SPERATOR).Length;
				while (!lFile.EofReached())
				{
					lLine = lFile.GetCsvLine(COMMA_SPERATOR);
					if (lLine.Length < lLineLength)
						continue;
					lFrenchDictionary.Add(lLine[0], lLine[lFrenchIndex]);
					lEnglishDictionary.Add(lLine[0], lLine[lEnglishIndex]);
				}
				translations.Add(Languages.English, lEnglishDictionary);
				translations.Add(Languages.Francais, lFrenchDictionary);

				lFile.Close();
			}
		}

		/// <summary>
		/// Change the server global language + emit signal to update all text in the game scene
		/// </summary>
		/// <param name="pLanguage"></param>
		public void SaveLanguage(Languages pLanguage)
        {
			_language = pLanguage;
			EmitSignal(nameof(LangageChanged));
        }

		/// <summary>
		/// Get a text translation by passing the key to this method
		/// </summary>
		/// <returns>The translated text corresponding to the key</returns>
		public string GetTranslation(string pKey)
        {
			return translations[_language].ContainsKey(pKey) ? translations[_language][pKey] : null;
        }

        protected override void Dispose(bool pDisposing)
        {
			if (pDisposing && instance != null) instance = null;
            base.Dispose(pDisposing);
        }

    }

}