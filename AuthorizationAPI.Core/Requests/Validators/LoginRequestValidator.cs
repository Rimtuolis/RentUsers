using FluentValidation;
using Microsoft.AspNetCore.Identity.Data;

namespace AuthorizationAPI.Core.Requests.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Nenurodytas el.paštas")
            .EmailAddress().WithMessage("Neteisingai nurodytas el.paštas");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Nenurodytas slaptažodis");
    }
}