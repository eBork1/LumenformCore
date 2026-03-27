using Lumenform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lumenform.Infrastructure.Persistence.Configurations;

public class CohortEventConfiguration : IEntityTypeConfiguration<CohortEvent>
{
    public void Configure(EntityTypeBuilder<CohortEvent> builder)
    {
        builder.ToTable("CohortEvents");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(e => e.CohortId)
            .IsRequired();
        
        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(e => e.Description)
            .HasMaxLength(1000);
        
        builder.Property(e => e.EventDate)
            .IsRequired();
        
        builder.Property(e => e.Type)
            .IsRequired()
            .HasConversion<int>();
        
        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<int>();
        
        builder.Property(e => e.IsRequired)
            .IsRequired();
        
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        
        builder.Property(e => e.UpdatedAt)
            .IsRequired();
        
        // Relationship
        builder.HasOne(e => e.Cohort)
            .WithMany(c => c.Events)
            .HasForeignKey(e => e.CohortId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Indexes
        builder.HasIndex(e => e.CohortId);
        builder.HasIndex(e => e.EventDate);
        builder.HasIndex(e => e.Status);
    }
}