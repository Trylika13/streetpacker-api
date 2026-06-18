using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SP.Domain.Entities;

namespace SP.Infrastructure.Data.Configurations;

public class FavoriteAdConfiguration : IEntityTypeConfiguration<FavoriteAd>
{
    public void Configure(EntityTypeBuilder<FavoriteAd> builder)
    {
        // 1. Nom de la table pivot en DB
        builder.ToTable("FavoriteAds");

        // 2. Clé primaire composite (UserId + AdId)
        builder.HasKey(fa => new { fa.UserId, fa.AdId });

        // 3. Configuration des relations (Clés étrangères)
        builder.HasOne(fa => fa.User)
            .WithMany()
            .HasForeignKey(fa => fa.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(fa => fa.Ad)
            .WithMany()
            .HasForeignKey(fa => fa.AdId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}