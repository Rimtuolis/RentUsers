using FluentValidation;

namespace AuthorizationAPI.Core.Requests
{
    public class CreateUserRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string PasswordRepeat { get; set; }
        public required List<string> Permissions { get; set; }
        public string CompanyId { get; set; }

    }

    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Nenurodytas el.paštas")
                .EmailAddress().WithMessage("Netinkamai nurodytas el.paštas");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Nenurodytas slaptažodis")
                .Equal(x => x.PasswordRepeat).WithMessage("Slaptažodžiai nesutampa");
            RuleFor(x => x.PasswordRepeat)
                .NotEmpty().WithMessage("Nenurodytas pakartotinas slaptažodis");
            RuleFor(x => x.Permissions)
                .NotEmpty().WithMessage("Prieigos teisės negali būti tuščios");
        }
    }
}
