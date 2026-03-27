using Lumenform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lumenform.Infrastructure.Persistence.Configurations;

public class CohortConfiguration : IEntityTypeConfiguration<Cohort>
{
    public void Configure(EntityTypeBuilder<Cohort> builder)
    {
        builder.ToTable("Cohorts");
        
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(c => c.CreatedByUserId)
            .IsRequired();
        
        builder.Property(c => c.ParishName)
            .HasMaxLength(200);
        
        builder.Property(c => c.IsActive)
            .IsRequired();
        
        builder.Property(c => c.CreatedAt)
            .IsRequired();
        
        builder.Property(c => c.UpdatedAt)
            .IsRequired();
        
        // Relationships
        builder.HasMany(c => c.Memberships)
            .WithOne(m => m.Cohort)
            .HasForeignKey(m => m.CohortId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(c => c.Assignments)
            .WithOne(a => a.Cohort)
            .HasForeignKey(a => a.CohortId)
            .OnDelete(DeleteBehavior.SetNull);  // Templates should remain if cohort deleted
        
        builder.HasMany(c => c.Events)
            .WithOne(e => e.Cohort)
            .HasForeignKey(e => e.CohortId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Indexes
        builder.HasIndex(c => c.IsActive);
        builder.HasIndex(c => c.CreatedAt);
        builder.HasIndex(c => c.CreatedByUserId);
    }
}