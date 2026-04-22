using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SP.Domain.Entities;

namespace SP.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.UserId);
        builder.Property(u => u.UserId)
            .HasColumnName("UserId")
            .ValueGeneratedOnAdd();

        builder.Property(u => u.Username)
            .HasColumnName("Username")
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(u => u.Username).IsUnique();
        
        builder.Property(u => u.Email)
            .HasColumnName("Email")
            .IsRequired()
            .HasMaxLength(256);
        
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PasswordHash)
            .HasColumnName("PasswordHash")
            .IsRequired();
        
        builder.Property(u => u.Role)
            .HasColumnName("Role")
            .IsRequired()
            .HasMaxLength(256);
    }
}