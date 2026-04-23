using SP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SP.Infrastructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        // Nom de la table dans DBeaver
        builder.ToTable("refresh_tokens");

        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.Id).HasColumnName("id");

        builder.Property(rt => rt.Token).HasColumnName("token").IsRequired();
        builder.Property(rt => rt.CreatedAt).HasColumnName("created");
        builder.Property(rt => rt.ExpiresAt).HasColumnName("expires");
        builder.Property(rt => rt.IsRevoked).HasColumnName("revoked");
        
        // Clé étrangère vers Users
        builder.Property(rt => rt.UserId).HasColumnName("user_id");

        // Configuration de la relation (Un utilisateur a plusieurs tokens)
        builder.HasOne(rt => rt.User)
            .WithMany(u => u.RefreshToken)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}