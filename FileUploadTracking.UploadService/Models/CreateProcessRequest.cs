using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace FileUploadTracking.UploadService.Models;

public record CreateProcessRequest(Guid UserId, string CustomerId, string CallbackUrl);

public class CreateProcessRequestValidator : AbstractValidator<CreateProcessRequest>
{
    public CreateProcessRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage("UserId must be provided and not be empty.");

        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.")
            .MinimumLength(3).WithMessage("CustomerId must be at least 3 characters long.");

        RuleFor(x => x.CallbackUrl)
            .NotEmpty().WithMessage("CallbackUrl is required.")
            .Must(IsValidUri).WithMessage("Url {PropertyValue} must be a valid Uri.");
            
    }

    private static bool IsValidUri(string uri)
    {
        if (string.IsNullOrWhiteSpace(uri))
        {
            return false;
        }
        return Uri.TryCreate(uri, UriKind.Absolute, out Uri outUri)
               && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps);
    }
}
