using API.DTOs;
using FluentValidation;
using FluentValidation.Validators;

namespace API.Validador
{
    public class EditarAdminValidador:AbstractValidator<EditarAdminDTO>
    {
        public EditarAdminValidador()
        {
            RuleFor(t=>t.Email).NotEmpty().EmailAddress(EmailValidationMode.Net4xRegex);
        }
    }
}
