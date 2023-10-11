using Godot;
using System;

// Author : Lefevre Florian
namespace Com.IsartDigital.Sokoban.Architectures {

    /// <summary>
    /// Structure used to represent the data for a user (Name + Score + Par) for a specific level
    /// (Structure is used here because it will not benefit of inheritance and for data weight in memory)
    /// </summary>
    public struct LevelScore
	{
        private int _par;
        private int _score;
        private string _userName;

        public int Par
        {
            get { return _par; }
            private set { _par = value; }
        }
        public int Score
        {
            get { return _score; }
            private set { _score = value; }
        }
        public string UserName
        {
            get { return _userName; }
            private set { _userName = value; }
        }

        public LevelScore(int pPar, int pScore, string pUserName)
        {
            _par = pPar;
            _score = pScore;
            _userName = pUserName;
        }

        public void SetScoreAndPar(int pPar, int pScore)
        {
            if (pPar < 0 || pScore < 0)
                return;
            _par = pPar;
            _score = pScore;
        }

    }

}