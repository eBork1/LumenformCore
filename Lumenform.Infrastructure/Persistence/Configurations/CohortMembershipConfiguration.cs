using Lumenform.Domain.Entities;
using Lumenform.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lumenform.Infrastructure.Persistence.Configurations;

public class CohortMembershipConfiguration : IEntityTypeConfiguration<CohortMembership>
{
    public void Configure(EntityTypeBuilder<CohortMembership> builder)
    {
        builder.ToTable("CohortMemberships");
        
        builder.HasKey(cm => cm.Id);
        
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(cm => cm.CohortId)
            .IsRequired();
        
        builder.Property(cm => cm.UserId)
            .IsRequired();
        
        builder.Property(cm => cm.Role)
            .IsRequired()
            .HasConversion<int>();  // Store enum as int
        
        builder.Property(cm => cm.Status)
            .IsRequired()
            .HasConversion<int>();
        
        builder.Property(cm => cm.ParticipantType)
            .HasConversion<int?>();
        
        builder.Property(cm => cm.JoinedDate)
            .IsRequired();
        
        builder.Property(cm => cm.CreatedAt)
            .IsRequired();
        
        builder.Property(cm => cm.UpdatedAt)
            .IsRequired();
        
        // Indexes
        builder.HasIndex(cm => cm.UserId);
        builder.HasIndex(cm => new { cm.CohortId, cm.UserId })
            .IsUnique();  // User can only be in a cohort once
        builder.HasIndex(cm => cm.Status);
    }
}