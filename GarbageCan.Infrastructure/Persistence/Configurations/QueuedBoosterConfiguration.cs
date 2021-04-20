﻿using GarbageCan.Domain.Entities.Boosters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class QueuedBoosterConfiguration : IEntityTypeConfiguration<QueuedBooster>
    {
        public void Configure(EntityTypeBuilder<QueuedBooster> builder)
        {
            builder.HasKey(t => t.position);
            builder.Property(t => t.position).ValueGeneratedNever();
            builder.ToTable("xpQueuedBoosters");
        }
    }
}