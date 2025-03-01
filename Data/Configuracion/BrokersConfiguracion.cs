using Entidades;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Configuracion
{
    public class BrokersConfiguracion : IEntityTypeConfiguration<Brokers>
    {
        public void Configure(EntityTypeBuilder<Brokers> builder)
        {
            builder.Property(t => t.RazonSocial).IsRequired().HasMaxLength(50);
            builder.Property(t => t.NombreComercial).IsRequired().HasMaxLength(50);
        }
    }
}
