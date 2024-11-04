using FluentValidation;
using PhotoGallery.Models.VMs;

namespace PhotoGallery.Validators;

public class RefreshTokenVMValidator : AbstractValidator<RefreshTokenVM>
{
    public RefreshTokenVMValidator()
    {
        RuleFor(x => x.AccessToken)
           .NotEmpty().WithMessage("AccessToken is required.");
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("RefreshToken is required");
    }
}
