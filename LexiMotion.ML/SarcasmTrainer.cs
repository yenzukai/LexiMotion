using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.IO;

namespace LexiMotion.ML
{
    public static class SarcasmTrainer
    {
        public static void TrainAndSaveModel(MLContext mlContext, string modelPath)
        {
            // 1. Load merged dataset
            string dataPath = Path.Combine(Environment.CurrentDirectory, "datasets", "sarcasm_combined.csv");
            
            var data = mlContext.Data.LoadFromTextFile<SarcasmData>(
                path: dataPath,
                hasHeader: true,
                separatorChar: ','
            );

            // 2. Split for training/testing
            var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);

            // 3. Define pipeline
            var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(SarcasmData.Text))
                // Convert float Label -> bool
                .Append(mlContext.Transforms.Conversion.ConvertType("Label", outputKind: DataKind.Boolean))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                    labelColumnName: "Label", featureColumnName: "Features"));

            // 4. Train model
            var model = pipeline.Fit(split.TrainSet);

            // 5. Evaluate
            var predictions = model.Transform(split.TestSet);
            var metrics = mlContext.BinaryClassification.Evaluate(predictions, labelColumnName: "Label");

            Console.WriteLine($"   Sarcasm Detection Metrics:");
            Console.WriteLine($"   Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"   AUC: {metrics.AreaUnderRocCurve:P2}");
            Console.WriteLine($"   F1 Score: {metrics.F1Score:P2}");

            // 6. Save model
            mlContext.Model.Save(model, split.TrainSet.Schema, modelPath);
            Console.WriteLine($"The system successfully saved the sarcasm model to {modelPath}");
        }
    }
}
