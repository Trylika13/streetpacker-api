// using SP.Domain.Entities;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
//
// namespace SP.Infrastructure.Data.Configurations;
//
// public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
// {
//     public void Configure(EntityTypeBuilder<RefreshToken> builder)
//     {
//         // Nom de la table dans DBeaver
//         builder.ToTable("refresh_tokens");
//
//         builder.HasKey(rt => rt.Id);
//         builder.Property(rt => rt.Id).HasColumnName("id");
//
//         builder.Property(rt => rt.Token).HasColumnName("token").IsRequired();
//         builder.Property(rt => rt.Created).HasColumnName("created");
//         builder.Property(rt => rt.Expires).HasColumnName("expires");
//         builder.Property(rt => rt.Revoked).HasColumnName("revoked");
//         
//         // Clé étrangère vers Users
//         builder.Property(rt => rt.UserId).HasColumnName("user_id");
//
//         // Configuration de la relation (Un utilisateur a plusieurs tokens)
//         builder.HasOne(rt => rt.User)
//             .WithMany(u => u.RefreshTokens)
//             .HasForeignKey(rt => rt.UserId)
//             .OnDelete(DeleteBehavior.Cascade);
//     }
// }