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

        if (File.Exists(modelPath))
        {
            Console.WriteLine("The system found an existing model. Skipping training...");
        }
        else
        {
            Console.WriteLine("The system is training a new model...");
            TrainAndSaveModel(mlContext, modelPath);
        }

        // Test prediction (whether trained new or loaded existing model)
        Predictor.TestPrediction("I am sick of your bullshit");
    }

    static void TrainAndSaveModel(MLContext mlContext, string modelPath)
    {
        // 1. Load Dataset
        string dataPath = Path.Combine(Environment.CurrentDirectory, "datasets", "GoEmotion-English-Context-Reddit.csv");
        var data = mlContext.Data.LoadFromTextFile<EmotionData>(
            path: dataPath,
            hasHeader: true,
            separatorChar: ','
        );

        // 2. Split data
        var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);

        // 3. Define pipeline
        var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", "Text")
            .Append(mlContext.Transforms.Conversion.MapValueToKey("Label", "Label"))
            .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
            .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        // 4. Train
        var model = pipeline.Fit(split.TrainSet);

        // Extract label names from the pipeline schema
        var labelColumn = model.GetOutputSchema(split.TrainSet.Schema)["Label"];

        if (labelColumn.HasKeyValues())
        {
            VBuffer<ReadOnlyMemory<char>> keys = default;
            labelColumn.GetKeyValues(ref keys);

            var labels = keys.DenseValues().Select(x => x.ToString()).ToArray();

            // Save labels to file
            File.WriteAllLines("labels.txt", labels);

            Console.WriteLine("Labels successfully saved to labels.txt");
        }

        // 5. Evaluate
        var predictions = model.Transform(split.TestSet);
        var metrics = mlContext.MulticlassClassification.Evaluate(predictions);
        Console.WriteLine($"MacroAccuracy: {metrics.MacroAccuracy:P2}");
        Console.WriteLine($"MicroAccuracy: {metrics.MicroAccuracy:P2}");

        // 6. Save
        mlContext.Model.Save(model, split.TrainSet.Schema, modelPath);
        Console.WriteLine($"The trained model has been successfully saved to {modelPath}.");
    }
}
