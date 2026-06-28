# LexiMotion: Multi-Emotion & Sarcasm-Aware Sentiment Analysis

LexiMotion is a research-based sentiment analysis system designed to go beyond basic polarity classification (positive, negative, neutral). It integrates three key detection layers:  
- **Emotion Prediction** – supports 28 fine-grained emotion categories.  
- **Negation Handling** – flips sentiment when negation cues are present (e.g., *“not bad”* → positive).  
- **Sarcasm Detection** – identifies sarcasm in text, which can reverse the intended emotional meaning.  

The system processes social media text (e.g., Reddit posts and comments), analyzes emotional content, and stores results in a PostgreSQL database. A web dashboard provides visualization of emotional trends, enabling better insights for organizations, researchers, and communities.

---

## Features
- **28 Fine-Grained Emotion Labels**  
  *admiration, amusement, anger, annoyance, approval, caring, confusion, curiosity, desire, disappointment, disapproval, disgust, embarrassment, excitement, fear, gratitude, grief, joy, love, nervousness, optimism, pride, realization, relief, remorse, sadness, surprise,* and *neutral*.  
  > Extendable for Filipino context (e.g., “banat”, “mixed feelings”).  

- **Negation Handler**  
  - Regex + rule-based preprocessing for English Context.

- **Sarcasm Detector**  
  - Binary classification (sarcastic / non-sarcastic).  
  - Trained using SDCA Logistic Regression with high accuracy (83.33%), AUC (91.34%), and F1-score (85.14%).  

- **Data Storage & Visualization**  
  - Stores raw text and processed results in **PostgreSQL**.  
  - Web dashboard built with **ASP.NET Core + Picocss** for graphical summaries.  

---

## Models Used
- **Emotion Prediction:**  
  - Stochastic Dual Coordinated Ascent (SDCA) with Maximum Entropy Loss.  
  - Alternative tests with L-BFGS and LightGBM.  
  - Best performance: SDCA (MacroAccuracy 25.82%, MicroAccuracy 52.56%).  

- **Sarcasm Detection:**  
  - SDCA Logistic Regression.  
  - Achieved Accuracy 83.33%, AUC 91.34%, F1 85.14%.  

- **Negation Handling:**  
  - Preprocessing step before feeding text into models.  
  - Uses regex rules + negation lexicons.  

---

## Data Sources
- [GoEmotions Dataset (Google Research)](https://github.com/google-research/google-research/tree/master/goemotions) – 211k Reddit comments labeled across 27 emotions + neutral.  
- Hugging Face Datasets.  
- Kaggle corpora.

---

## System Workflow
1. User submits text (e.g., Reddit post/comment link).  
2. System fetches raw content via API.  
3. Preprocessing (tokenization, cleaning, negation handling).  
4. Emotion prediction model assigns probabilities to 28 labels.  
5. Sarcasm detector checks for sarcastic tone.  
6. Results stored in PostgreSQL.  
7. Dashboard visualizes outputs in charts and summaries.  

---

## Tech Stack
- **Backend:** ASP.NET Core, ML.NET  
- **Frontend:** Picocss, Chart.js  
- **Database:** PostgreSQL  
- **Machine Learning:**  
  - SDCA Logistic Regression (ML.NET)  
  - Regex-based preprocessing  

---

## Privacy & Ethics
LexiMotion prioritizes **privacy and ethical use**:  
- Only processes publicly available or user-submitted text.  
- No personal data collection.  
- Results are used solely for research and educational purposes.  
- See the [Privacy Policy](./Pages/Privacy.cshtml) for details.  

---

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/)  
- [PostgreSQL](https://www.postgresql.org/) installed and running  

### Installation
```bash
# Clone the repository
git clone https://github.com/yenzukai/LexiMotion.git
cd LexiMotion

# Restore dependencies
dotnet restore

# Run the application
dotnet run
