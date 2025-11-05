using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Repositories.User;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;

namespace UseCases.Test.User.Register
{
    public class RegisterUserUseCaseTest
    {
        [Fact]
        public async Task UseCase_Should_ReturnsRegisteredUserResponse_When_RequestIsValid()
        {
            
            var request = RequestRegisterUserJsonBuilder.Build();

            var useCase = CreateUseCase();

            var result = await useCase.Execute(request);

            result.ShouldNotBeNull();
            result.Name.ShouldBe(request.Name);

        }

        [Fact]
        public async Task UseCase_Should_ReturnsOneError_When_EmailIsAlreadyRegistered()
         {

            var request = RequestRegisterUserJsonBuilder.Build();

            var useCase = CreateUseCase(request.Email);

            Func<Task> action = async () => await useCase.Execute(request);

            (await action.ShouldThrowAsync<ErrorOnValidationException>()).ShouldSatisfyAllConditions
                (
                    e => e.ErrorMessages.ShouldHaveSingleItem(),
                    e => e.ErrorMessages.ShouldContain(ResourceMessageException.EMAIL_ALREADY_REGISTERED)
                );
        }

        [Fact]
        public async Task UseCase_Should_ReturnsOneError_When_NameIsEmpty()
        {
            var request = RequestRegisterUserJsonBuilder.Build();
            request.Name = string.Empty;

            var useCase = CreateUseCase();

            Func<Task> action = async () => await useCase.Execute(request);

            (await action.ShouldThrowAsync<ErrorOnValidationException>()).ShouldSatisfyAllConditions
                (
                    e => e.ErrorMessages.ShouldHaveSingleItem(),
                    e => e.ErrorMessages.ShouldContain(ResourceMessageException.NAME_EMPTY)
                );
        }



        private RegisterUserUseCase CreateUseCase(string? email = null)
        {
            var mapper = MapperBuilder.Build();

            var passwordEncrypter = PasswordEncripterBuilder.Build();

            var readOnlyRepositoryBuilder = new UserReadOnlyRepositoryBuilder();

            if(string.IsNullOrEmpty(email) == false)
            {
                readOnlyRepositoryBuilder.ExistActiveUserWithEmail(email);
            }

            var writeOnlyRepository = UserWriteOnlyRepositoryBuilder.Build();

            var unitOfWork = UnitOfWorkBuilder.Build();

            return new RegisterUserUseCase(writeOnlyRepository, readOnlyRepositoryBuilder.Build(), unitOfWork, passwordEncrypter, mapper);
        }
    }
}
