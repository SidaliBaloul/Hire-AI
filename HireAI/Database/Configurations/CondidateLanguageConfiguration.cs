using HireAI.Features.Candidates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireAI.Database.Configurations;

internal sealed class CandidateLanguageConfiguration
    : IEntityTypeConfiguration<CandidateLanguage>
{
    public void Configure(EntityTypeBuilder<CandidateLanguage> builder)
    {
        builder.HasKey(x => new
        {
            x.CandidateId,
            x.Language
        });

        builder.Property(x => x.Language)
            .IsRequired();

        builder.Property(x => x.Level);

        builder.HasOne(x => x.Candidate)
            .WithMany(x => x.Languages)
            .HasForeignKey(x => x.CandidateId);
    }
}
