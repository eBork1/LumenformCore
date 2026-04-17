using Lumenform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lumenform.Infrastructure.Persistence.Configurations;

public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        builder.ToTable("Assignments");
        
        builder.HasKey(a => a.Id);
        
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(a => a.Content)
            .IsRequired();  // No max length - allow long HTML content
        
        builder.Property(a => a.CohortId);  // Nullable
        
        builder.Property(a => a.CreatedByUserId)
            .IsRequired();
        
        builder.Property(a => a.IsTemplate)
            .IsRequired();
        
        builder.Property(a => a.CreatedAt)
            .IsRequired();
        
        builder.Property(a => a.UpdatedAt)
            .IsRequired();
        
        builder.Property(a => a.SubmissionRequired)
            .HasDefaultValue(true)
            .IsRequired();
        
        // Relationships
        builder.HasMany(a => a.Tasks)
            .WithOne(t => t.Assignment)
            .HasForeignKey(t => t.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(a => a.Submissions)
            .WithOne(s => s.Assignment)
            .HasForeignKey(s => s.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Indexes
        builder.HasIndex(a => a.CohortId);
        builder.HasIndex(a => a.IsTemplate);
        builder.HasIndex(a => a.CreatedByUserId);
        builder.HasIndex(a => a.DueDate);
    }
}