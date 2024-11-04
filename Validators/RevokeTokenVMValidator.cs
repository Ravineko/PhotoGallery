using FluentValidation;
using PhotoGallery.Models.VMs;

namespace PhotoGallery.Validators;

public class RevokeTokenVMValidator : AbstractValidator<RevokeTokenVM>
{
    public RevokeTokenVMValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Token is required");
    }
}
