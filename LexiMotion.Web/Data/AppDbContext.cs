    using Microsoft.EntityFrameworkCore;

    namespace LexiMotion.Web.Data
    {
        public class AppDbContext : DbContext
        {
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

            public DbSet<SocialPost> SocialPosts { get; set; }
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<SocialPost>().ToTable("SocialPost");

                modelBuilder.Entity<SocialPost>(entity =>
                {
                    entity.Property(e => e.ConfidenceScores).HasColumnType("jsonb");
                    entity.Property(e => e.SarcasmProbability).HasColumnType("real");
                });

                base.OnModelCreating(modelBuilder);
            }
        }

        public class SocialPost
        {
            public int Id { get; set; }
            public string InputText { get; set; } = string.Empty;
            public string PredictedEmotion { get; set; } = string.Empty;
            public string ConfidenceScores { get; set; } = string.Empty;
            public bool NegationDetected { get; set; }
            public bool SarcasmDetected { get; set; }
            public float SarcasmProbability { get; set; }
            public string FinalEmotion { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }
    }
