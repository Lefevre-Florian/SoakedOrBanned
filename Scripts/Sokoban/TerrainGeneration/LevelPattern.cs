using System;
using System.Collections.Generic;

// Author : Lefevre Florian
namespace Com.IsartDigital.Sokoban.TerrainGeneration {

	public class LevelPattern
	{

		private int _par;
		private bool _locked;
        private string _author;
		private List<string> _map = new List<string>();

		public int Par
        {
            get { return _par; }
			private set { _par = value; }
        }

		public bool locked
        {
            get { return _locked; }
		    set { _locked = value; }
        }

        public string Author
        {
            get { return _author; }
            private set { _author = value; }
        }

		public List<string> Map
        {
			get { return _map; }
			private set { _map = value; }
        }

        /// <summary>
        /// Simple constructor used to represent a JSON level
        /// </summary>
        /// <param name="pPar"></param>
        /// <param name="pLocked"></param>
        /// <param name="pMap"></param>
		public LevelPattern(int pPar, bool pLocked, string pAuthor, Godot.Collections.Array pMap)
        {
            _par = pPar;
            _locked = pLocked;
            _author = pAuthor;

            foreach (string lItem in pMap)
                _map.Add(lItem);
        }

        public LevelPattern(LevelPattern pLevelPattern)
        {
            _par = pLevelPattern.Par;
            _locked = pLevelPattern._locked;
            _author = pLevelPattern.Author;
            _map = pLevelPattern.Map;
        }

        public override string ToString()
        {
            // Use to debug
            string lText = $"Par : {_par}\nLocked : {_locked}\nMap :\n";
            foreach (string lLine in _map)
            {
                lText += $"{lLine}\n";
            }
            return lText;
        }

        public void Unlock()
        {
            locked = false;
        }
    }

}