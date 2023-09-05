using API.DTOs;
using FluentValidation;
using FluentValidation.Validators;

namespace API.Validador
{
    public class EmpresaValidador:AbstractValidator<EmpresaDTO>
    {
        public EmpresaValidador()
        {
            RuleFor(t => t.RazonComercial).NotEmpty().MinimumLength(2);
            RuleFor(t=>t.RazonSocial).NotEmpty().MinimumLength(2);
            RuleFor(t=>t.RepresentanteLegal).NotEmpty().MinimumLength(5);
            RuleFor(t=>t.Correo).EmailAddress(EmailValidationMode.Net4xRegex);
            RuleFor(t=>t.CodigoPostal).MaximumLength(5).MinimumLength(5);
        }
    }
}
