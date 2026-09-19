using HireAI.Features.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireAI.Database.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(refreshToken => refreshToken.Token).HasMaxLength(200);

        builder.HasIndex(x => x.Token).IsUnique();

        builder.HasOne(x => x.User)
            .WithMany().HasForeignKey(x => x.UserId);
        
    }
}

