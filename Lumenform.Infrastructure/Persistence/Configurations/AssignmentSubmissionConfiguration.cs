using Lumenform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lumenform.Infrastructure.Persistence.Configurations;

public class AssignmentSubmissionConfiguration : IEntityTypeConfiguration<AssignmentSubmission>
{
    public void Configure(EntityTypeBuilder<AssignmentSubmission> builder)
    {
        builder.ToTable("AssignmentSubmissions");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(s => s.AssignmentId)
            .IsRequired();
        
        builder.Property(s => s.UserId)
            .IsRequired();
        
        builder.Property(s => s.Content)
            .IsRequired();  // No HasMaxLength - allow unlimited HTML
        
        builder.Property(s => s.SubmittedAt)
            .IsRequired();
        
        builder.Property(s => s.CoordinatorFeedback);  // Nullable, no max length
        
        builder.Property(s => s.CreatedAt)
            .IsRequired();
        
        builder.Property(s => s.UpdatedAt)
            .IsRequired();
        
        // Relationship
        builder.HasOne(s => s.Assignment)
            .WithMany(a => a.Submissions)
            .HasForeignKey(s => s.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Indexes
        builder.HasIndex(s => s.AssignmentId);
        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => new { s.AssignmentId, s.UserId })
            .IsUnique();  // User can only submit once per assignment
    }
}