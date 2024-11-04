using FluentValidation;
using PhotoGallery.Models.VMs;

namespace PhotoGallery.Validators;

public class RegistrationVMValidator : AbstractValidator<RegistrationVM>
{
    public RegistrationVMValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
    }
}
