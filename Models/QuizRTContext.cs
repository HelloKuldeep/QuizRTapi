using Microsoft.EntityFrameworkCore;
namespace QuizRT.Models{
    public class QuizRTContext : DbContext {
        public DbSet<QuizRTTemplate> QuizRTTemplateT { get; set; }
        public DbSet<Questions> QuestionsT { get; set; }
        public DbSet<Options> OptionsT { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
            optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=Quiztest1DB;Trusted_Connection=True;");
        }
        // fluent api
        protected override void OnModelCreating(ModelBuilder modelBuilder){
            modelBuilder.Entity<Questions>().HasMany(n => n.QuestionOptions).WithOne().HasForeignKey(c => c.QuestionsId);
        }
    }
}