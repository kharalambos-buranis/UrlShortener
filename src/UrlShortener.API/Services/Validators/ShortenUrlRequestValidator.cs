using FluentValidation;
using UrlShortener.API.Models.Requests;

namespace UrlShortener.API.Services.Validators
{
    public class ShortenUrlRequestValidator : AbstractValidator<ShortenUrlRequest>
    {
        public ShortenUrlRequestValidator()
        {
            RuleFor(x => x.OriginalUrl)
            .NotEmpty().WithMessage("Original URL is required.")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("Original URL must be a valid absolute URI.");

            RuleFor(x => x.ExpiresAt)
                .GreaterThan(DateTimeOffset.UtcNow)
                .When(x => x.ExpiresAt.HasValue)
                .WithMessage("Expiration date must be in the future.");

            RuleFor(x => x.Alias)
                .Matches("^[a-zA-Z0-9_-]*$")
                .MaximumLength(50)
                .WithMessage("Alias can only contain alphanumeric characters, dashes or underscores and must be 50 characters or fewer.")
                .When(x => !string.IsNullOrWhiteSpace(x.Alias));

        }


    }
}
