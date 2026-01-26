using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.ChangePassword
{
    public class ChangePasswordUseCase : IChangePasswordUseCase
    {
        private readonly ILoggedUser _loggedUser;
        private readonly IUserUpdateOnlyRepository _userUpdateOnlyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordEncrypter _passwordEncrypter;

        public ChangePasswordUseCase
            (
                ILoggedUser loggedUser,
                IUserUpdateOnlyRepository userUpdateOnlyRepository,
                IUnitOfWork unitOfWork,
                IPasswordEncrypter passwordEncrypter
            )
        {
            _loggedUser = loggedUser;
            _userUpdateOnlyRepository = userUpdateOnlyRepository;
            _unitOfWork = unitOfWork;
            _passwordEncrypter = passwordEncrypter;
        }

        public async Task Execute(RequestChangePasswordJson request)
        {
            var loggedUser = await _loggedUser.User();

            Validate(request, loggedUser);

            var userToUpdate = await _userUpdateOnlyRepository.GetById(loggedUser.Id);
            var newPasswordEncrypted = _passwordEncrypter.Encrypt(request.NewPassword);

            userToUpdate.Password = newPasswordEncrypted;

            _userUpdateOnlyRepository.Update(userToUpdate);

            await _unitOfWork.Commit();
        }

        private void Validate(RequestChangePasswordJson request, Domain.Entities.User loggedUser)
        {
            var result = new ChangePasswordValidator().Validate(request);

            var requestCurrentPasswordEncrypted = _passwordEncrypter.Encrypt(request.CurrentPassword);

            if (!requestCurrentPasswordEncrypted.Equals(loggedUser.Password))
            {
                result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, ResourceMessageException.CURRENT_PASSWORD_DIFFERENT_REGISTERED_PASSWORD));
            }
            if (!result.IsValid)
            {
                throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).ToList());
            }
        }
    }
}
