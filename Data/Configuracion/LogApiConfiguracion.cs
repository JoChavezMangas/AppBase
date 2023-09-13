using Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Configuracion
{
    internal class LogApiConfiguracion : IEntityTypeConfiguration<LogApi>
    {
        public void Configure(EntityTypeBuilder<LogApi> builder)
        {
            builder.Property(t => t.status).IsRequired();
            builder.Property(t => t.Message).IsRequired();
            builder.Property(t => t.urlEnpoint).IsRequired();
        }
    }
}
