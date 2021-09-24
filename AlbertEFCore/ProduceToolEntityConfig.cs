﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbertEFCore
{
    public class ProduceToolEntityConfig : IEntityTypeConfiguration<ProduceToolEntity>
    {
        public void Configure(EntityTypeBuilder<ProduceToolEntity> builder)
        {
            builder.ToTable("ProduceToolConfig");
            builder.Property(e => e.ID).HasMaxLength(100);
        }
    }
}