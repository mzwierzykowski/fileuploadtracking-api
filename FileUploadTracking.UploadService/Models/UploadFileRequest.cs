using FluentValidation;

namespace FileUploadTracking.UploadService.Models;

public record UploadFileRequest(string FileName);

public class UploadFileRequestValidator : AbstractValidator<UploadFileRequest>
{
    public UploadFileRequestValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("FileName is required.")
            .Must(name => name.EndsWith(".pdf") || name.EndsWith(".jpg") || name.EndsWith(".png"))
            .WithMessage("Only PDF, JPG, and PNG files are allowed.");
    }
}