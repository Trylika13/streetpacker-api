using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SP.Domain.Entities;

namespace SP.Infrastructure.Data.Configurations;

public class FavoriteSpotConfiguration : IEntityTypeConfiguration<FavoriteSpot>
{
    public void Configure(EntityTypeBuilder<FavoriteSpot> builder)
    {
        //  Nom de la table pivot en DB
        builder.ToTable("FavoriteSpots");

        // Clé primaire composite (UserId + SpotId)
        builder.HasKey(fs => new { fs.UserId, fs.SpotsId });

        //  Configuration des relations (Clés étrangères)
        builder.HasOne(fs => fs.User)
            .WithMany() 
            .HasForeignKey(fs => fs.UserId)
            .OnDelete(DeleteBehavior.Cascade); 

        builder.HasOne(fs => fs.Spot)
            .WithMany()
            .HasForeignKey(fs => fs.SpotsId)
            .OnDelete(DeleteBehavior.Cascade); // Si le spot est supprimé, on nettoie la table pivot
    }
}