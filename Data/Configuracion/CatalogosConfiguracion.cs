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

    public class CatalogosConfiguracion : IEntityTypeConfiguration<Catalogos>
    {
        public void Configure(EntityTypeBuilder<Catalogos> builder)
        {
            builder.Property(t => t.Nombre).IsRequired().HasMaxLength(50);
        }
    }
}
