using FluentValidation;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.Application.UseCases.User.Register
{
    internal class RegisterUserValidator : AbstractValidator<RequestRegisterUserJson>
    {
        public RegisterUserValidator() 
        {
            RuleFor(user => user.Name).NotEmpty().WithMessage(ResourceMessageException.NAME_EMPTY);
            RuleFor(user => user.Email)
                .NotEmpty().WithMessage(ResourceMessageException.EMAIL_EMPTY)
                .EmailAddress().WithMessage(ResourceMessageException.EMAIL_VALID);
            RuleFor(user => user.Password.Length)
                .GreaterThanOrEqualTo(6).WithMessage(ResourceMessageException.PASSWORD_LENGTH);
    
        }
    }
}
