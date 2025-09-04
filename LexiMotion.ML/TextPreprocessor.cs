
namespace LexiMotion.ML
{
    public static class TextPreprocessor
    {
        private static readonly string[] negationWords =
        {
            "not", "no", "never", "don't", "can't", "won't",
            "isn't", "wasn't", "shouldn't", "couldn't"
        };

        // Detects if the input text contains a negation.
        public static bool ContainsNegation(string text)
        {
            var tokens = text.ToLower().Split(' ');

            foreach (var word in tokens)
            {
                if (negationWords.Contains(word))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
