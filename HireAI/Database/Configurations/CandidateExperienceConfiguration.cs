using HireAI.Features.Candidates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireAI.Database.Configurations;

internal sealed class ExperienceConfiguration
    : IEntityTypeConfiguration<CandidateExperience>
{
    public void Configure(EntityTypeBuilder<CandidateExperience> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Company)
            .IsRequired();

        builder.Property(x => x.JobTitle)
            .IsRequired();

        builder.HasOne(x => x.Candidate)
            .WithMany(x => x.Experiences)
            .HasForeignKey(x => x.CandidateId);
    }
}
