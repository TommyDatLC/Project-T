using UnityEngine;

namespace Script
{
    public enum GameRank { S, A, B, C, D }

    public static class RankEvaluator
    {
        public struct RankResult
        {
            public GameRank rank;
            public string message;
            public Color displayColor;
        }

        public static RankResult GetRank(float elapsed, float limit, bool metSpecialCondition)
        {
            if (metSpecialCondition) 
                return new RankResult { rank = GameRank.S, message = "Legendary!", displayColor = Color.yellow };

            float remaining = limit - elapsed;
            float percentRemaining = (remaining / limit) * 100f;

            if (remaining <= 0) 
                return new RankResult { rank = GameRank.D, message = "Too Slow!", displayColor = Color.red };
        
            if (percentRemaining >= 60f) 
                return new RankResult { rank = GameRank.A, message = "Excellent!", displayColor = Color.green };
        
            if (percentRemaining >= 40f) 
                return new RankResult { rank = GameRank.B, message = "Good Job!", displayColor = Color.cyan };
        
            if (percentRemaining >= 20f) 
                return new RankResult { rank = GameRank.C, message = "Keep Practicing!", displayColor = Color.white };

            return new RankResult { rank = GameRank.D, message = "Barely Made It!", displayColor = Color.gray };
        }
    }
}