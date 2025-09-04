using Microsoft.EntityFrameworkCore;

namespace LexiMotion.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<SocialPost> SocialPosts { get; set; }
    }

    public class SocialPost
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string PredictedEmotion { get; set; }
        public bool IsNegated { get; set; }
        public bool IsSarcastic { get; set; }
    }
}
