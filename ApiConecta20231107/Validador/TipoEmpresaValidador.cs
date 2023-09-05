using API.DTOs;
using FluentValidation;

namespace API.Validador
{
    public class TipoEmpresaValidador : AbstractValidator<TipoEmpresaDTO>
    {
        public TipoEmpresaValidador()
        {
            RuleFor(v => v.Nombre).NotEmpty();
        }
    }
}
