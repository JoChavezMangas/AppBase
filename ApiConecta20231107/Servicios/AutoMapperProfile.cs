using API.DTOs;
using AutoMapper;
using Entidades;

namespace API.Servicios
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<Empleado, EmpleadoDTO>();
            CreateMap<EmpleadoDTO, Empleado>();
        }
    }
}
