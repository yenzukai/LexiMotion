using Microsoft.ML;
using Microsoft.ML.Data;
using LexiMotion.ML;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        var mlContext = new MLContext();
        string modelPath = "EmotionModel.zip";
        string sarcasmModelPath = "SarcasmModel.zip";

        if (File.Exists(modelPath))
        {
            Console.WriteLine("The system found an existing emotion model. Skipping training...");
        }
        else
        {
            Console.WriteLine("The system is training a new emotion model...");
            EmotionTrainer.TrainAndSaveModel(mlContext, modelPath);
        }

        if (File.Exists(sarcasmModelPath))
        {
            Console.WriteLine("The system found an existing sarcasm model. Skipping training...");
        }
        else
        {
            Console.WriteLine("The system is training a new sarcasm model...");
            SarcasmTrainer.TrainAndSaveModel(mlContext, sarcasmModelPath);
        }
        // Test emotion and sarcasm prediction (whether trained new or loaded existing model)
        string inputText = "I like long walks, especially when they are taken by people who annoy me.";
        Predictor.TestPrediction(inputText);
    }
}
