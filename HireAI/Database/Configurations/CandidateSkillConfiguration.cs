using HireAI.Features.Candidates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireAI.Database.Configurations;

internal sealed class CandidateSkillConfiguration
    : IEntityTypeConfiguration<CandidateSkill>
{
    public void Configure(EntityTypeBuilder<CandidateSkill> builder)
    {
        builder.HasKey(x => new { x.CandidateId, x.Skill });

        builder.Property(x => x.Skill)
            .IsRequired();
    }
}
