using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Repositories.User;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.Update;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;

namespace UseCases.Test.User.Update
{
    public class UpdateUserUseCaseTest
    {
        [Fact]
        public async Task UseCase_Should_ReturnsNoContentAndUpdateUser_When_RequestIsValid()
        {
            var (user, _) = UserBuilder.Build();
            var request = RequestUpdateUserJsonBuilder.Build();

            var useCase = CreateUseCase(user);

            Func<Task> action = async () => await useCase.Execute(request);

            await action.ShouldNotThrowAsync();
            user.Name.ShouldBe(request.Name);
            user.Email.ShouldBe(request.Email);
        }

        [Fact]
        public async Task UseCase_Should_ThrowsException_When_EmailIsAlreadyInUseByAnotherUser()
        {
            var (user, _) = UserBuilder.Build();
            var request = RequestUpdateUserJsonBuilder.Build();
            var useCase = CreateUseCase(user, request.Email);

            Func<Task> action = async () => await useCase.Execute(request);

            (await action.ShouldThrowAsync<ErrorOnValidationException>()).ShouldSatisfyAllConditions
            (
                e => e.ErrorMessages.ShouldHaveSingleItem(),
                e => e.ErrorMessages.ShouldContain(ResourceMessageException.EMAIL_ALREADY_REGISTERED)
            );
            user.Name.ShouldNotBe(request.Name);
            user.Email.ShouldNotBe(request.Email);
        }

        [Fact]
        public async Task UseCase_Should_ThrowsException_When_NameIsEmpty()
        {
            var (user, _) = UserBuilder.Build();
            var request = RequestUpdateUserJsonBuilder.Build();
            request.Name = string.Empty;
            var useCase = CreateUseCase(user);

            Func<Task> action = async () => await useCase.Execute(request);

            (await action.ShouldThrowAsync<ErrorOnValidationException>()).ShouldSatisfyAllConditions
            (
                e => e.ErrorMessages.ShouldHaveSingleItem(),
                e => e.ErrorMessages.ShouldContain(ResourceMessageException.NAME_EMPTY)
            );
            user.Name.ShouldNotBe(request.Name);
            user.Email.ShouldNotBe(request.Email);
        }

        private static UpdateUserUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user, string? email = null)
        {
            var loggedUserMock = LoggedUserBuilder.Build(user);
            var userReadOnlyRepositoryBuilder = new UserReadOnlyRepositoryBuilder();
            var userUpdateOnlyRepositoryBuilder = new UserUpdateOnlyRepositoryBuilder().GetById(user).Build(); 
            //o GetByID retorna o proprio builder para permitir o encadeamento e depois o Build retorna o repositorio
            var unitOfWorkBuilder = UnitOfWorkBuilder.Build();

            if (!string.IsNullOrEmpty(email))
            {
                userReadOnlyRepositoryBuilder.ExistActiveUserWithEmail(email);
            }

            return new UpdateUserUseCase
                (
                    loggedUserMock, userReadOnlyRepositoryBuilder.Build(), 
                    userUpdateOnlyRepositoryBuilder, unitOfWorkBuilder
                );
        }
    }
}
