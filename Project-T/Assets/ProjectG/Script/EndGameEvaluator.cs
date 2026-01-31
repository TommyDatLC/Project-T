using UnityEngine;

namespace Script
{
    public static class EndgameEvaluator
    {
        public enum EndingType { SecretFamily, Success, Failure }

        public struct EndgameResult
        {
            public EndingType type;
            public string endingTitle;
            public string description;
            public Color themeColor;
        }

        public static EndgameResult EvaluateGame(float drawingScore, float elapsed, float limit, bool metSpecialCondition)
        {
            // 1. Priority: Secret Ending
            if (metSpecialCondition)
            {
                return new EndgameResult 
                { 
                    type = EndingType.SecretFamily, 
                    endingTitle = "Ending: Family", 
                    description = "You found what truly mattered.", 
                    themeColor = new Color(1f, 0.5f, 0.8f) // Pinkish/Warm
                };
            }

            // 2. Logic: Success vs Failure
            // Condition: Must have time left AND a decent drawing score (e.g., > 50%)
            bool isTimeValid = elapsed < limit;
            bool isDrawingValid = drawingScore >= 50f;

            if (isTimeValid && isDrawingValid)
            {
                return new EndgameResult 
                { 
                    type = EndingType.Success, 
                    endingTitle = "Ending: Success", 
                    description = $"Completed in {elapsed:F1}s with {drawingScore:F1}% accuracy.", 
                    themeColor = Color.green 
                };
            }

            // 3. Default: Failure
            return new EndgameResult 
            { 
                type = EndingType.Failure, 
                endingTitle = "Ending: Failure", 
                description = isTimeValid ? "The drawing was too unstable." : "Time ran out.", 
                themeColor = Color.red 
            };
        }
    }
}