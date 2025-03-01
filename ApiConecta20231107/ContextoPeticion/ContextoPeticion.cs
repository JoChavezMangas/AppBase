using System.Security.Claims;

namespace ApiBase.Auxiliares
{
    public class ContextoPeticion: IContextoPeticion
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        

        public ContextoPeticion(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string empleadoId { 
            get 
            { 
                return _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(z => z.Type == "empleadoId")?.Value;
            } 
        }
        public string role
        {
            get
            {
                return _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(z => z.Type == "role")?.Value;
            }
        }
        
        public string email
        {
            get
            {
                return _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(z => z.Type == "email")?.Value;
            }
        }

        public string empresa
        {
            get
            {
                return _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(z => z.Type == "empresa")?.Value;
            }
        }

    }
}
