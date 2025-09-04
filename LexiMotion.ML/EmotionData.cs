using Microsoft.ML.Data;

namespace LexiMotion.ML
{
    public class EmotionData
    {
        [LoadColumn(0)] // adjust this if "text" is at column A (zero-based index)
        public string Text { get; set; } = string.Empty;
        [LoadColumn(37)] // adjust this if "label" is at column AL (zero-based index)
        public string Label { get; set; } = string.Empty;
    }

    public class EmotionPrediction
    {
        [ColumnName("PredictedLabel")]
        public string PredictedEmotion { get; set; } = string.Empty;
        public float[] Score { get; set; } = Array.Empty<float>();
    }
}