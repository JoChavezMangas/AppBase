using API.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name ="ObtenerRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatoHATEOAS>>>Get()
        {

            var datosHateoas=new List<DatoHATEOAS>();

            var esAdmin = await authorizationService.AuthorizeAsync(User, "EsAdmin");

            datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("ObtenerRoot", new { }), descripcion: "self", metodo: "GET"));

            datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("Listar", new { }), descripcion: "Listar-TipoEmpresa", metodo: "GET"));


            if(esAdmin.Succeeded) 
            {
                datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("Agregar", new { }), descripcion: "Agregar-TipoEmpresa", metodo: "POST"));
            }
            

            return datosHateoas;
        }
    }
}
