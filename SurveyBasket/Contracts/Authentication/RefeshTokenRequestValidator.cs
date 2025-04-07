namespace SurveyBasket.Contracts.Authentication;

public class RefeshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefeshTokenRequestValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty();

        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}