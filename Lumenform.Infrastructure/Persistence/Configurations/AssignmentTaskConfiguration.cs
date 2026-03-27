using Lumenform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lumenform.Infrastructure.Persistence.Configurations;

public class AssignmentTaskConfiguration : IEntityTypeConfiguration<AssignmentTask>
{
    public void Configure(EntityTypeBuilder<AssignmentTask> builder)
    {
        builder.ToTable("AssignmentTasks");
        
        builder.HasKey(t => t.Id);
        
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(t => t.AssignmentId)
            .IsRequired();  // FK to Assignment
        
        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(1000);  // Tasks can be longer than titles
        
        builder.Property(t => t.Order)
            .IsRequired();
        
        builder.Property(t => t.CreatedAt)
            .IsRequired();
        
        builder.Property(t => t.UpdatedAt)
            .IsRequired();
        
        // Relationship - already defined in AssignmentConfiguration
        // but can be explicit here too
        builder.HasOne(t => t.Assignment)
            .WithMany(a => a.Tasks)
            .HasForeignKey(t => t.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);  // Delete tasks when assignment deleted
        
        builder.HasMany(t => t.Completions)
            .WithOne(c => c.AssignmentTask)
            .HasForeignKey(c => c.AssignmentTaskId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Indexes
        builder.HasIndex(t => t.AssignmentId);
        builder.HasIndex(t => new { t.AssignmentId, t.Order });  // Query tasks in order
    }
}