using FluentValidation;
using PhotoGallery.Models.VMs;

namespace PhotoGallery.Validators;

public class VerifyTwoFactorVMValidator : AbstractValidator<VerifyTwoFactorVM>
{
    public VerifyTwoFactorVMValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("2FA code is required");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");
    }
}
