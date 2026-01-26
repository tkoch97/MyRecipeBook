using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories.User;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Application.UseCases.Login.DoLogin;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;



namespace UseCases.Test.Login.DoLogin;

public class DoLoginUseCaseTest
{
    [Fact]
    public async Task UseCase_Should_ReturnsOkResponse_When_ActiveUserIsFound()
    {
        var (user, password) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        var result = await useCase.Execute(new RequestLoginUserJson
        {
            Email = user.Email,
            Password = password
        });

        result.ShouldNotBeNull();
        result.Name.ShouldSatisfyAllConditions
            (
                r => r.ShouldNotBeNullOrWhiteSpace(),
                r => r.ShouldBe(user.Name)
            );
        result.Tokens.ShouldNotBeNull();
        result.Tokens.AccessToken.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task UseCase_Should_ThrowsException_When_ActiveUserIsNotFound()
    {
        var request = RequestLoginJsonBuilder.Build();

        var useCase = CreateUseCase();

        Func<Task> action = async () => await useCase.Execute(request);

        (await action.ShouldThrowAsync<InvalidLoginException>())
            .Message.ShouldBe(ResourceMessageException.EMAIL_OR_PASSWORD_INVALID);
    }

    private static DoLoginUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User? user = null)
    {
        var userReadOnlyRepositoryBuilder = new UserReadOnlyRepositoryBuilder();
        var accessTokenGenerator = JwtTokenGeneratorBuilder.Build();

        if (user is not null)
        {
            userReadOnlyRepositoryBuilder.GetByEmailAndPassword(user);
        }

        return new DoLoginUseCase(userReadOnlyRepositoryBuilder.Build(), PasswordEncripterBuilder.Build(), accessTokenGenerator);
    }
}
