using HireAI.Features.Candidates;
using HireAI.Features.Users;
using Microsoft.EntityFrameworkCore;

namespace HireAI.Database;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options) 
{
    public DbSet<UserItem> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<CandidateSkill> CandidatesSkills { get; set; }
    public DbSet<CandidateExperience> CandidatesExperiences { get; set; }
    public DbSet<CandidateLanguage> CandidatesLanguages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

       
    }
   
}
