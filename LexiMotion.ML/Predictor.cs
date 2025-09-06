using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Linq;
using System.IO;

namespace LexiMotion.ML
{
    public static class Predictor
    {
        private static readonly string _modelPath = "EmotionModel.zip";
        private static MLContext _mlContext = new MLContext();
        private static PredictionEngine<EmotionData, EmotionPrediction>? _predictionEngine;
        private static readonly string[] labels = File.ReadAllLines("labels.txt");

        static Predictor()
        {
            DataViewSchema modelSchema;
            var trainedModel = _mlContext.Model.Load(_modelPath, out modelSchema);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<EmotionData, EmotionPrediction>(trainedModel);
        }

        public static void TestPrediction(string text)
        {
            if (_predictionEngine == null)
            {
                Console.WriteLine("The system prediction engine not initialized.");
                return;
            }

            // Detect negation
            bool isNegated = TextPreprocessor.ContainsNegation(text);

            // Predict emotion
            var input = new EmotionData { Text = text };
            var result = _predictionEngine.Predict(input);

            // Predict sarcasm
            bool isSarcastic = SarcasmPredictor.PredictSarcasm(text);

            // Final interpretation
            string finalEmotion = InterpretationHelper.Interpret(result.PredictedEmotion, isNegated, isSarcastic);

            Console.WriteLine($"Input: {text}");
            Console.WriteLine($"Predicted Emotion: {result.PredictedEmotion}");
            Console.WriteLine("Emotion Confidence Scores:");
            for (int i = 0; i < result.Score.Length && i < labels.Length; i += 2)
            {
                string left = $"{labels[i]}: {result.Score[i]:F4}";
                string right = (i + 1 < labels.Length) ? $"{labels[i + 1]}: {result.Score[i + 1]:F4}" : "";
                Console.WriteLine($"  {left,-25}{right}");
            }
            Console.WriteLine($"Negation Detected: {isNegated}");
            Console.WriteLine($"Sarcasm Detected: {isSarcastic}");
            Console.WriteLine($"Final Interpreted Emotion: {finalEmotion}");        
        }
    }
}
