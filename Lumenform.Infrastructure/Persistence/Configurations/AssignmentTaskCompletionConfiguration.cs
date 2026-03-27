using Lumenform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lumenform.Infrastructure.Persistence.Configurations;

public class AssignmentTaskCompletionConfiguration : IEntityTypeConfiguration<AssignmentTaskCompletion>
{
    public void Configure(EntityTypeBuilder<AssignmentTaskCompletion> builder)
    {
        builder.ToTable("AssignmentTaskCompletions");
        
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(c => c.AssignmentTaskId)
            .IsRequired();
        
        builder.Property(c => c.UserId)
            .IsRequired();
        
        builder.Property(c => c.CompletedAt)
            .IsRequired();
        
        builder.Property(c => c.CreatedAt)
            .IsRequired();
        
        builder.Property(c => c.UpdatedAt)
            .IsRequired();
        
        // Relationship
        builder.HasOne(c => c.AssignmentTask)
            .WithMany(t => t.Completions)
            .HasForeignKey(c => c.AssignmentTaskId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Indexes
        builder.HasIndex(c => c.AssignmentTaskId);
        builder.HasIndex(c => c.UserId);
        builder.HasIndex(c => new { c.AssignmentTaskId, c.UserId })
            .IsUnique();  // User can only complete a task once
    }
}