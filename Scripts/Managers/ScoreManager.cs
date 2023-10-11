using Godot;
using System;
using System.Collections.Generic;
using Com.IsartDigital.Sokoban.Architectures;

// Author : Lefevre Florian
namespace Com.IsartDigital.Sokoban.Managers {

    public class ScoreManager : Node
    {
        private static ScoreManager instance;
        private const int MAX_SCORE = 10;

        private ScoreManager ():base() {}

        public override void _Ready()
        {
            if (instance != null){  
                QueueFree();
                return;
            }
            instance = this;
        }

        public static ScoreManager GetInstance()
        {
            if (instance == null) instance = new ScoreManager();
            return instance;
        }

        public void AddPlayerScore(int pLevelID, int pScore, User pPlayer)
        {
            if (pPlayer.scores.Count - 1 < pLevelID) pPlayer.scores.Add(pScore);
            else if (pPlayer.scores.Count - 1 >= pLevelID
                    && pPlayer.scores[pLevelID] < pScore)
                pPlayer.scores[pLevelID] = pScore;
        }

        public bool IsPlayerInTheLeaderboard(List<Tuple<string, int>> pLeaderboard, User pPlayer)
        {
            if (pLeaderboard.Exists(lUser => lUser.Item1 == pPlayer.Username)) return true;
            else return false;
        }

        public List<Tuple<string, int>> GetScores(int pLevelID)
        {
            List<User> lUsers = DatabaseManager.GetInstance().Users;
            List<Tuple<string, int>> lScores = new List<Tuple<string, int>>();

            foreach (User lUser in lUsers)
                if(lUser.scores.Count > pLevelID) 
                   lScores.Add(new Tuple<string, int>(lUser.Username, lUser.scores[pLevelID]));

            lScores.Sort((lUserA, lUserB) => lUserB.Item2.CompareTo(lUserA.Item2));
            return (lScores.Count > 0) ? (lScores.Count > MAX_SCORE) ? lScores.GetRange(0, MAX_SCORE) : lScores : null;
        }

        protected override void Dispose(bool pDisposing)
        {
            if (pDisposing && instance == this) instance = null;
            base.Dispose(pDisposing);
        }
    }
}