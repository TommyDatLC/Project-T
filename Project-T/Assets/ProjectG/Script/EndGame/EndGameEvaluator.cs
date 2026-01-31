using UnityEngine;

namespace Script.EndGame
{
    public static class EndgameEvaluator
    {
        public enum EndingType { SecretFamily, Frightening, Questioning, Nonchalant, Failure }

        public struct EndgameResult
        {
            public EndingType type;
            public string endingTitle;
            public string description;
            public Color themeColor;
        }

        public static EndgameResult EvaluateGame(float drawingScore, float elapsed, float limit, bool metSpecialCondition)
        {
            // 1. Secret Ending: The bird doesn't leave; it joins the family.
            if (metSpecialCondition)
            {
                return CreateResult(EndingType.SecretFamily, "Ending: Kinship", 
                    "The predator didn't see a threat... it saw a long-lost relative.", new Color(1f, 0.6f, 0.2f));
            }

            bool isTimeValid = elapsed < limit;
            float timePercentRemaining = (limit - elapsed) / limit;

            // 2. Frightening: High accuracy (The mask is terrifyingly realistic/detailed)
            if (isTimeValid && drawingScore > 75f)
            {
                return CreateResult(EndingType.Frightening, "Ending: Terrific", 
                    "That bird will be in therapy for years. A masterpiece of horror.", Color.magenta);
            }

            // 3. Questioning: Finished fast, but the mask looks like nothing.
            // The bird is just confused why you're waving a scribble at it.
            if (timePercentRemaining > 0.75f && drawingScore < 50f)
            {
                return CreateResult(EndingType.Questioning, "Ending: Confusion", 
                    "The bird stopped out of pure bewilderment. Is that... a face?", Color.yellow);
            }

            // 4. Nonchalant: Valid score. It worked, but it wasn't a spectacle.
            if (isTimeValid && drawingScore >= 50f)
            {
                return CreateResult(EndingType.Nonchalant, "Ending: Shooed", 
                    "The bird left, mostly because your mask was too annoying to look at.", Color.cyan);
            }

            // 5. Failure: Too slow or the mask was too poorly drawn to be scary.
            string failReason = isTimeValid ? "The bird laughed at your 'art' and attacked." : "The bird reached you before you finished.";
            return CreateResult(EndingType.Failure, "Ending: Prey", failReason, Color.red);
        }

        // Helper method to keep the main logic clean
        private static EndgameResult CreateResult(EndingType type, string title, string desc, Color color)
        {
            return new EndgameResult { type = type, endingTitle = title, description = desc, themeColor = color };
        }
    }
}