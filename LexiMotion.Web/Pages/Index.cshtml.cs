using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using LexiMotion.Web.Data;
using LexiMotion.Web.Services;

namespace LexiMotion.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AnalysisService _analysis;
        private readonly AppDbContext _db;

        public IndexModel(AnalysisService analysis, AppDbContext db)
        {
            _analysis = analysis;
            _db = db;
        }

        [BindProperty]
        public string InputType { get; set; } = "text"; // text | url
        [BindProperty]
        public string InputValue { get; set; } = string.Empty;

        public AnalysisResult? Result { get; set; }
        public void OnGet() { }
        public async Task<IActionResult> OnPostAnalyzeAsync()
        {
            if (string.IsNullOrWhiteSpace(InputValue))
            {
                ModelState.AddModelError(string.Empty, "Please provide input text or URL.");
                return Page();
            }

            Result = await _analysis.AnalyzeAsync(InputType, InputValue);

            // Save full record
            var socialPost = new SocialPost
            {
                InputText = Result.InputText,
                PredictedEmotion = Result.PredictedEmotion,
                NegationDetected = Result.NegationDetected,
                SarcasmDetected = Result.SarcasmDetected,

                // new fields
                ConfidenceScores = Result.ConfidenceScores != null
                    ? JsonConvert.SerializeObject(Result.ConfidenceScores) // serialize Dictionary<string,float>
                    : "{}", 

                SarcasmProbability = Result.SarcasmProbability,
                FinalEmotion = Result.FinalEmotion ?? string.Empty
            };

            _db.SocialPosts.Add(socialPost);
            await _db.SaveChangesAsync();

            return Page();
        }
    }
}