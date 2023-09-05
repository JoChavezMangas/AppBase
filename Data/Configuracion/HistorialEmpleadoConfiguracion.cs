using Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Configuracion
{
    public class HistorialEmpleadoConfiguracion : IEntityTypeConfiguration<HistorialEmpleado>
    {
        public void Configure(EntityTypeBuilder<HistorialEmpleado> builder)
        {
            builder.Property(t => t.EmpleadoId).IsRequired();
            builder.Property(t => t.NuevoCambio).IsRequired();
            builder.Property(t => t.DatoAnterior).IsRequired();
            builder.Property(t => t.Identificador).IsRequired().HasMaxLength(50);
        }

    }
}
