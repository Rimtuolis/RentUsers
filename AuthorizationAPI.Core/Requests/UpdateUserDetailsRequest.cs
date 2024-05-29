using FluentValidation;

namespace AuthorizationAPI.Core.Requests
{
    public class UpdateUserDetailsRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? PasswordRepeat { get; set; }
        public List<string>? Permissions { get; set; }
        public bool IsAccessTerminated { get; set; }
        public string? Company {  get; set; }
    }

    public class UpdateUserDetailsRequestValidator : AbstractValidator<UpdateUserDetailsRequest>
    {
        public UpdateUserDetailsRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Nenurodytas el.paštas")
                .EmailAddress().WithMessage("Netinkamai nurodytas el.paštas");
            RuleFor(x => x.Password)
                .Equal(x => x.PasswordRepeat).WithMessage("Slaptažodžiai nesutampa");
            RuleFor(x => x.Permissions)
                .NotEmpty().WithMessage("Prieigos teisės negali būti tuščios");
        }
    }
}
