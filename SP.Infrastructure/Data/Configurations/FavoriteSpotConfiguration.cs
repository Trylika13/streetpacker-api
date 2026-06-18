using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SP.Domain.Entities;

namespace SP.Infrastructure.Data.Configurations;

public class FavoriteSpotConfiguration : IEntityTypeConfiguration<FavoriteSpot>
{
    public void Configure(EntityTypeBuilder<FavoriteSpot> builder)
    {
        // 1. Nom de la table pivot en DB
        builder.ToTable("FavoriteSpots");

        // 2. Clé primaire composite (UserId + SpotId)
        builder.HasKey(fs => new { fs.UserId, fs.SpotsId });

        // 3. Configuration des relations (Clés étrangères)
        builder.HasOne(fs => fs.User)
            .WithMany() // Ou .WithMany(u => u.FavoriteSpots) si tu ajoutes la collection dans ton entité User
            .HasForeignKey(fs => fs.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Si l'utilisateur est supprimé, on vire ses favoris

        builder.HasOne(fs => fs.Spot)
            .WithMany()
            .HasForeignKey(fs => fs.SpotsId)
            .OnDelete(DeleteBehavior.Cascade); // Si le spot est supprimé, on nettoie la table pivot
    }
}