namespace LexiMotion.ML
{
    public static class InterpretationHelper
    {
        public static string Interpret(string predictedEmotion, bool isNegated, bool isSarcastic)
        {
            string finalEmotion = predictedEmotion;

            // Step 1: Flip emotion if negated
            if (isNegated)
            {
                finalEmotion = FlipEmotion(predictedEmotion);
            }

            // Step 2: Adjust emotion if sarcastic
            if (isSarcastic)
            {
                finalEmotion = ApplySarcasm(finalEmotion);
            }

            return finalEmotion;
        }

        private static string FlipEmotion(string emotion)
        {
            // You can expand this mapping according to your emotion labels
            return emotion switch
            {
                "joy" => "sadness",
                "sadness" => "joy",
                "anger" => "calm",
                "fear" => "confidence",
                "disgust" => "approval",
                "surprise" => "neutral",
                _ => emotion,
            };
        }

        private static string ApplySarcasm(string emotion)
        {
            // Simple approach: sarcasm often implies opposite or mocking emotion
            return emotion switch
            {
                "joy" => "mocking_joy",
                "sadness" => "mocking_sadness",
                "anger" => "mocking_anger",
                _ => emotion + "_sarcastic",
            };
        }
    }
}
