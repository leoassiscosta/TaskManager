using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Configurations;

public class TaskHistoryConfiguration : IEntityTypeConfiguration<TaskHistory>
{
    public void Configure(EntityTypeBuilder<TaskHistory> builder)
    {
        builder.ToTable("TaskHistories");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.TaskId)
            .IsRequired();

        builder.Property(h => h.UserId)
            .IsRequired();

        builder.Property(h => h.ChangeDescription)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(h => h.PreviousValue)
            .HasMaxLength(1000);

        builder.Property(h => h.NewValue)
            .HasMaxLength(1000);

        builder.Property(h => h.ChangedAt)
            .IsRequired();

        // Relacionamentos
        builder.HasOne(h => h.Task)
            .WithMany(t => t.History)
            .HasForeignKey(h => h.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(h => h.User)
            .WithMany()
            .HasForeignKey(h => h.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(h => h.TaskId);
        builder.HasIndex(h => h.ChangedAt);
    }
}
