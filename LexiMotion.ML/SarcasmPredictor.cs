using Microsoft.ML;
using System;

namespace LexiMotion.ML
{
    public static class SarcasmPredictor
    {
        private static readonly string _modelPath = "SarcasmModel.zip";
        private static readonly MLContext _mlContext = new MLContext();
        private static PredictionEngine<SarcasmData, SarcasmPrediction>? _predictionEngine;

        static SarcasmPredictor()
        {
            if (System.IO.File.Exists(_modelPath))
            {
                var model = _mlContext.Model.Load(_modelPath, out var schema);
                _predictionEngine = _mlContext.Model.CreatePredictionEngine<SarcasmData, SarcasmPrediction>(model);
            }
        }
        
        /*
        public static void TestSarcasm(string text)
        {
            if (_predictionEngine == null)
            {
                Console.WriteLine("Sarcasm prediction engine not initialized.");
                return;
            }

            var input = new SarcasmData { Text = text };
            var result = _predictionEngine.Predict(input);
            
            Console.WriteLine($"Predicted Sarcasm: {(result.IsSarcastic ? "Sarcastic" : "Not Sarcastic")}");
            Console.WriteLine($"Probability: {result.Probability:P2}");
        }
        */

        public static SarcasmPrediction PredictSarcasm(string text)
        {
            if (_predictionEngine == null)
            {
                Console.WriteLine("Sarcasm prediction engine not initialized.");
                return new SarcasmPrediction { IsSarcastic = false, Probability = 0f, Score = 0f };
            }

            var input = new SarcasmData { Text = text };
            return _predictionEngine.Predict(input);
        }
    }
}
