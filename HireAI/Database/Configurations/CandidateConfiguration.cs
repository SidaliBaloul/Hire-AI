using HireAI.Features.Candidates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireAI.Database.Configurations;

internal sealed class CandidateConfiguration
    : IEntityTypeConfiguration<Candidate>
{
    public void Configure(EntityTypeBuilder<Candidate> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasMany(x => x.Skills)
            .WithOne(x => x.Candidate)
            .HasForeignKey(x => x.CandidateId);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Cadidates)
            .HasForeignKey(x => x.userId);

        builder.HasMany(x => x.Experiences)
            .WithOne(x => x.Candidate)
            .HasForeignKey(x => x.CandidateId);
    }
}
