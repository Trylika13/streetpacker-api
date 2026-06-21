using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SP.Domain.Entities;

namespace SP.Infrastructure.Data.Configurations;

public class AdConfiguration : IEntityTypeConfiguration<Ad>
{
    public void Configure(EntityTypeBuilder<Ad> builder)
    {
        // 👑 Ta clé primaire de base (ajuste le nom si c'est Id ou AdId)
        builder.HasKey(a => a.AdId); 

        // 👑 La configuration de la table pivot Many-to-Many pour les annonces
        builder.HasMany(a => a.Tags)
            .WithMany(t => t.Ads)
            .UsingEntity(
                "Ad_Tags", // Le nom exact de ta table pivot en DB
                l => l.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagId"),
                r => r.HasOne(typeof(Ad)).WithMany().HasForeignKey("AdId") 
            );
    }
}