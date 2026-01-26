using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.Update
{
    public class UpdateUserUseCase : IUpdateUserUseCase
    {
        private readonly ILoggedUser _loggedUser;
        private readonly IUserReadOnlyRepository _userReadOnlyRepository;
        private readonly IUserUpdateOnlyRepository _userUpdateOnlyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserUseCase
            (
                ILoggedUser loggedUser,
                IUserReadOnlyRepository userReadOnlyRepository,
                IUserUpdateOnlyRepository userUpdateOnlyRepository,
                IUnitOfWork unitOfWork
            )
        {
            _loggedUser = loggedUser;
            _userReadOnlyRepository = userReadOnlyRepository;
            _userUpdateOnlyRepository = userUpdateOnlyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Execute(RequestUpdateUserJson request)
        {
            var loggedUser = await _loggedUser.User();

            await Validate(request, loggedUser.Email);

            var userToUpdate = await _userUpdateOnlyRepository.GetById(loggedUser.Id);

            userToUpdate.Name = request.Name;
            userToUpdate.Email = request.Email;

            _userUpdateOnlyRepository.Update(userToUpdate);

            await _unitOfWork.Commit();
        }

        private async Task Validate(RequestUpdateUserJson request, string currentEmail)
        {
            var validator = new UpdateUserValidator();
            var result = validator.Validate(request);

            if(!string.Equals(currentEmail, request.Email))
            {
                var userEmailExist = await _userReadOnlyRepository.ExistActiveUserWithEmail(request.Email);
                if (userEmailExist)
                {
                    result.Errors.Add(new FluentValidation.Results.ValidationFailure("email", ResourceMessageException.EMAIL_ALREADY_REGISTERED));
                }
            }

            if (!result.IsValid)
            {
                var errorsMessagens = result.Errors.Select(e => e.ErrorMessage).ToList();
                throw new ErrorOnValidationException(errorsMessagens);
            }
        }
    }
}
