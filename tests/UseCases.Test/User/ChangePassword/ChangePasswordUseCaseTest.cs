using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Repositories.User;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.ChangePassword;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;

namespace UseCases.Test.User.ChangePassword
{
    public class ChangePasswordUseCaseTest
    {

        [Fact]
        public async Task UseCase_Should_ReturnsNoContentAndChangedPassword_When_RequestIsValid()
        {
            var (user, registeredPassword) = UserBuilder.Build();
            var request = RequestChangePasswordJsonBuilder.Build();
            request.CurrentPassword = registeredPassword;

            var useCase = CreateUseCase(user);

            Func<Task> action = async () => await useCase.Execute(request);

            var passwordEncrypter = PasswordEncripterBuilder.Build();

            await action.ShouldNotThrowAsync();
            user.Password.ShouldBe(passwordEncrypter.Encrypt(request.NewPassword));

        }


        [Fact]
        public async Task UseCase_Should_ReturnsError_When_RequestCurrentPassword_NotEqual_RegisteredPassword()
        {
            var (user, registeredPassword) = UserBuilder.Build();
            var request = RequestChangePasswordJsonBuilder.Build();

            var useCase = CreateUseCase(user);

            Func<Task> action = async () => await useCase.Execute(request);

            var passwordEncrypter = PasswordEncripterBuilder.Build();

            (await action.ShouldThrowAsync<ErrorOnValidationException>()).ShouldSatisfyAllConditions
            (
                e => e.ErrorMessages.ShouldHaveSingleItem(),
                e => e.ErrorMessages.ShouldContain(ResourceMessageException.CURRENT_PASSWORD_DIFFERENT_REGISTERED_PASSWORD)
            );
            user.Password.ShouldBe(passwordEncrypter.Encrypt(registeredPassword));
        }

        [Fact]
        public async Task UseCase_Should_ReturnsError_When_RequestNewPassword_IsEmpty()
        {
            var (user, registeredPassword) = UserBuilder.Build();
            var request = RequestChangePasswordJsonBuilder.Build();
            request.CurrentPassword = registeredPassword;
            request.NewPassword = string.Empty;

            var useCase = CreateUseCase(user);

            Func<Task> action = async () => await useCase.Execute(request);

            var passwordEncrypter = PasswordEncripterBuilder.Build();

            (await action.ShouldThrowAsync<ErrorOnValidationException>()).ShouldSatisfyAllConditions
                (
                    e => e.ErrorMessages.ShouldHaveSingleItem(),
                    e => e.ErrorMessages.ShouldContain(ResourceMessageException.PASSWORD_EMPTY)
                );
            user.Password.ShouldBe(passwordEncrypter.Encrypt(registeredPassword));
        }

        [Fact]
        public async Task UseCase_Should_ReturnsError_When_NewPassword_IsEqual_To_CurrentPassword()
        {
            var (user, registeredPassword) = UserBuilder.Build();
            var request = RequestChangePasswordJsonBuilder.Build();
            request.CurrentPassword = registeredPassword;
            request.NewPassword = request.CurrentPassword;

            var useCase = CreateUseCase(user);

            Func<Task> action = async () => await useCase.Execute(request);

            var passwordEncrypter = PasswordEncripterBuilder.Build();

            (await action.ShouldThrowAsync<ErrorOnValidationException>()).ShouldSatisfyAllConditions
                (
                    e => e.ErrorMessages.ShouldHaveSingleItem(),
                    e => e.ErrorMessages.ShouldContain(ResourceMessageException.PASSWORDS_MUST_BE_DIFFERENT)
                );
            user.Password.ShouldBe(passwordEncrypter.Encrypt(registeredPassword));
;
        }


        private static ChangePasswordUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
        {
            var loggedUserMock = LoggedUserBuilder.Build(user);
            var userUpdateOnlyRepository= new UserUpdateOnlyRepositoryBuilder().GetById(user).Build();
            var unitOfWork = UnitOfWorkBuilder.Build();
            var passwordEncrypter = PasswordEncripterBuilder.Build();

            return new ChangePasswordUseCase(loggedUserMock, userUpdateOnlyRepository, unitOfWork, passwordEncrypter);
        }
    }
}
