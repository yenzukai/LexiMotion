using Microsoft.ML.Data;

namespace LexiMotion.ML
{
    public class SarcasmData
    {
        [LoadColumn(0)]
        public string Text { get; set; } = string.Empty;

        [LoadColumn(1)]
        public float Label { get; set; }
    }

    public class SarcasmPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool IsSarcastic { get; set; }

        public float Probability { get; set; }

        public float Score { get; set; }
    }
}