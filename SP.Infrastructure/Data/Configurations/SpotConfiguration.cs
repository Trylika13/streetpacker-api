using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SP.Domain.Entities;

namespace SP.Infrastructure.Configurations;

public class SpotConfiguration : IEntityTypeConfiguration<Spot>
{
    public virtual void Configure(EntityTypeBuilder<Spot> builder)
    {
        builder.ToTable("Spots");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(100); 

        builder.Property(s => s.Description)
            .HasMaxLength(1000); 

        builder.Property(s => s.Latitude)
            .IsRequired(); 

        builder.Property(s => s.Longitude)
            .IsRequired(); 

        // Jauge de Fraîcheur : Valeur par défaut à 100
        builder.Property(s => s.FreshnessScore)
            .HasDefaultValue(100)
            .IsRequired(); 

        builder.Property(s => s.LastVerifiedAt)
            .IsRequired(); 

        builder.Property(s => s.CreatedAt)
            .IsRequired(); 

        // Relation avec l'utilisateur (Un utilisateur peut créer plusieurs spots)
        builder.HasOne(s => s.User)
            .WithMany() 
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Si l'user supprime son compte, on clean ses spots 

        // Index pour accélérer la recherche 
        builder.HasIndex(s => new { s.Latitude, s.Longitude });
    }
}