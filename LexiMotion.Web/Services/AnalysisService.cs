using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Data.SqlTypes;
using System.Text.Json;

namespace LexiMotion.Web.Services
{
    public class AnalysisService
    {
        private readonly IWebHostEnvironment _env;
        private readonly MLContext _ml = new();

        private readonly object _emotionLock = new();
        private ITransformer? _emotionModel;
        private string[] _emotionLabels = Array.Empty<string>();

        private readonly object _sarcasmLock = new();
        private ITransformer? _sarcasmModel;

        public AnalysisService(IWebHostEnvironment env)
        {
            _env = env;
            LoadModels();
        }

        private string ResolvePath(params string[] parts)
        {
            var baseDir = Path.Combine(_env.ContentRootPath, "MLModels");
            var path = Path.Combine([.. new[] { baseDir }, .. parts]);
            if (File.Exists(path))
                return path;

            //Fallback: root/ (helpful during dev)
            path = Path.Combine(_env.ContentRootPath, Path.Combine(parts));
            return path;
        }

        private void LoadModels()
        {
            // Emotion model + labels
            lock (_emotionLock)
            {
                var modelPath = ResolvePath("EmotionModel.zip");
                if (File.Exists(modelPath))
                {
                    _emotionModel = _ml.Model.Load(modelPath, out _);
                }
                var labelsPath = ResolvePath("labels.txt");
                if (File.Exists(labelsPath))
                {
                    _emotionLabels = File.ReadAllLines(labelsPath);
                }
            }

            // Sarcasm model
            lock (_sarcasmLock)
            {
                var sarcasmPath = ResolvePath("SarcasmModel.zip");
                if (File.Exists(sarcasmPath))
                {
                    _sarcasmModel = _ml.Model.Load(sarcasmPath, out _);
                }
            }
        }

        // Lightweight negation check
        public static bool ContainsNegation(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            string[] negs = new[] { "not", "no", "nor", "neither", "never", "don't", "can't", "won't",
            "isn't", "wasn't", "shouldn't", "couldn't" };
            var tokens = text.ToLowerInvariant().Split(new[] { ' ', '\t', '\n', '\r', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            return tokens.Intersect(negs).Any();
        }

        //Simple interpretation layer
        public static string Interpret(string emotion, bool negated, bool sarcastic)
        {
            string e = emotion;
            if (negated)
            {
                var flip = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["joy"] = "sadness",
                    ["sadness"] = "relief",
                    ["anger"] = "calm",
                    ["fear"] = "courage",
                    ["admiration"] = "contempt",
                    ["love"] = "indifference",
                    ["gratitude"] = "resentment",
                    ["pride"] = "shame",
                    ["optimism"] = "pessimism",
                    ["approval"] = "disapproval",
                    ["disapproval"] = "approval",
                };
                if (flip.TryGetValue(emotion, out var mapped))
                    e = mapped;
            }
            if (sarcastic) e = e + "_sarcastic";
            return e;
        }

        // Strongly-typed schema for emotion model
        private class EmotionInput
        {
            public string Text { get; set; } = string.Empty;
            public string Label { get; set; } = string.Empty;
        }
        private class EmotionOutput
        {
            [ColumnName("PredictedLabel")]
            public string PredictedLabel { get; set; } = string.Empty;
            public float[] Score { get; set; } = Array.Empty<float>();
        }

        // Strongly-typed schema for sarcasm model
        private class SarcasmInput
        {
            public string Text { get; set; } = string.Empty;
            public string Label { get; set; } = string.Empty;
        }
        private class SarcasmOutput
        {
            [ColumnName("PredictedLabel")]
            public bool PredictedLabel { get; set; }
            public float Probability { get; set; }
            public float Score { get; set; }
        }

        public async Task<AnalysisResult> AnalyzeAsync(string inputType, string rawInput, CancellationToken ct = default)
        {
            // In case inputType=="url", you'd fetch the content here.
            var text = rawInput?.Trim() ?? string.Empty;

            // 1) Negation
            bool neg = ContainsNegation(text);

            // 2) Emotion
            string predicted = "";
            Dictionary<string, float> scores = new();
            if (_emotionModel is not null)
            {
                var engine = _ml.Model.CreatePredictionEngine<EmotionInput, EmotionOutput>(_emotionModel);
                var emo = engine.Predict(new EmotionInput { Text = text });
                predicted = emo.PredictedLabel;

                // align scores with labels
                if (_emotionLabels.Length == emo.Score.Length)
                {
                    for (int i = 0; i < _emotionLabels.Length; i++)
                        scores[_emotionLabels[i]] = emo.Score[i];
                }
                else
                {
                    for (int i = 0; i < emo.Score.Length; i++)
                        scores[$"Class_{i}"] = emo.Score[i];
                }
            }

            // 3) Sarcasm
            bool sarc = false;
            float sarcProb = 0f;

            if (_sarcasmModel is not null)
            {
                var engine = _ml.Model.CreatePredictionEngine<SarcasmInput, SarcasmOutput>(_sarcasmModel);
                var s = engine.Predict(new SarcasmInput { Text = text });
                sarc = s.PredictedLabel;
                sarcProb = s.Probability;
            }

            // 4) Interpretation
            string final = Interpret(predicted, neg, sarc);

            var result = new AnalysisResult
            {
                InputType = inputType,
                InputText = text,
                PredictedEmotion = predicted,
                ConfidenceScores = scores,
                NegationDetected = neg,
                SarcasmDetected = sarc,
                SarcasmProbability = sarcProb,
                FinalEmotion = final
            };

            // Simulate async work (if you later fetch external content)
            await Task.CompletedTask;
            return result;
        }
    }

    public class AnalysisResult
    {
        public string InputType { get; set; } = "text"; // or "url"
        public string InputText { get; set; } = string.Empty;
        public string PredictedEmotion { get; set; } = string.Empty;
        public Dictionary<string, float> ConfidenceScores { get; set; } = new();
        public bool NegationDetected { get; set; }
        public bool SarcasmDetected { get; set; }
        public float SarcasmProbability { get; set; }
        public string FinalEmotion { get; set; } = string.Empty;

        public string ConfidenceScoresJson => JsonSerializer.Serialize(ConfidenceScores);
        public IEnumerable<KeyValuePair<string, float>> ScoresSortedDesc => ConfidenceScores.OrderByDescending(kv => kv.Value);
    }
}
