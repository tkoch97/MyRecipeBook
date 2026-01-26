using FluentValidation;
using MyRecipeBook.Application.SharedValidators;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.Application.UseCases.User.ChangePassword
{
    public class ChangePasswordValidator : AbstractValidator<RequestChangePasswordJson>
    {
        public ChangePasswordValidator()
        {
            RuleFor(request => request.NewPassword).SetValidator(new PasswordValidator<RequestChangePasswordJson>());
            RuleFor(request => request.CurrentPassword).SetValidator(new PasswordValidator<RequestChangePasswordJson>());

            When(request => request.CurrentPassword == request.NewPassword, () =>
            {
                RuleFor(request => request.NewPassword)
                .NotEqual(request => request.CurrentPassword)
                .WithMessage(ResourceMessageException.PASSWORDS_MUST_BE_DIFFERENT);
            });
        }
    }
}
