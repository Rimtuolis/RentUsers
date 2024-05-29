using FluentValidation;

namespace AuthorizationAPI.Core.Requests;

public class TokenRefreshRequest
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}

public class TokenRefreshRequestValidator : AbstractValidator<TokenRefreshRequest>
{
    public TokenRefreshRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Prieigos atkūrimo žetonas nepateiktas");
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("Prieigos žetonas nepateiktas");
    }
}