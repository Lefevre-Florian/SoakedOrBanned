using System;
using System.Collections.Generic;
using Godot;

// Author : Lefevre Florian
namespace Com.IsartDigital.Sokoban.Architectures {
	/// <summary>
	/// Struct used to represent all the users stored in the database
	/// </summary>
	public struct User
	{
		private string _username;
		private string _password;

		public int levelUnlocked;

		public List<int> scores;

		private float _sfxVolume;
		private float _musicVolume;

		private int _langage;

		public string Username
        {
			get { return _username; }
			private set { _username = value; }
        }

		public string Password
        {
			get { return _password; }
			private set { _password = value; }
        }
		
		public float SFXVolume
		{
			get { return _sfxVolume; }
			private set { _sfxVolume = value; }
		}

		public float MusicVolume
        {
			get { return _musicVolume; }
			private set { _musicVolume = value; }
        }

		public int Langage
        {
			get { return _langage; }
			private set { _langage = value; }
        }

		public User(string pUsername, string pPassword, int pLevelUnlocked, List<int> pScores)
        {
			_username = pUsername;
			_password = pPassword;

			levelUnlocked = pLevelUnlocked;
			scores = pScores;

			_langage = 0;
			_sfxVolume = _musicVolume = 0;
        }

		public User(string pUsername, string pPassword, int pLevelUnlocked, Godot.Collections.Array pScores)
        {
			_username = pUsername;
			_password = pPassword;

			levelUnlocked = pLevelUnlocked;
			scores = new List<int>();

			int pLength = pScores.Count;
			for (int i = 0; i < pLength; i++)
				scores.Add(Convert.ToInt32(pScores[i]));

			_langage = 0;
			_sfxVolume = _musicVolume = 0;
		}

		public User(string pUsername, string pPassword, int pLevelUnlocked, Godot.Collections.Array pScores, float pSFX = 0, float pMusic = 0, int pLangage = 0)
        {
			_username = pUsername;
			_password = pPassword;

			levelUnlocked = pLevelUnlocked;
			scores = new List<int>();

			int pLength = pScores.Count;
			for (int i = 0; i < pLength; i++)
				scores.Add(Convert.ToInt32(pScores[i]));

			_langage = pLangage;
			_sfxVolume = pSFX;
			_musicVolume = pMusic;
        }

		public void SetConfiguration(float pSFX, float pMusic, int pLangage)
        {
			_sfxVolume = pSFX;
			_musicVolume = pMusic;
			_langage = pLangage;
        }

		public void SetVolume(float pVolume) => _musicVolume = pVolume;

		public void SetSFX(float pVolume) => _sfxVolume = pVolume;

		public void SetLangage(int pLangageID) => _langage = pLangageID;

		public Godot.Collections.Array GetScoresToArray()
        {
			Godot.Collections.Array lArray = new Godot.Collections.Array();

			int lLength = scores.Count;
			for (int i = 0; i < lLength; i++)
				lArray.Add(scores[i]);
			return lArray;
        }

	}

}