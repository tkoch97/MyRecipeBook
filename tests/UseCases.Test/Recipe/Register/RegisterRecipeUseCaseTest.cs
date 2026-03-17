using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Repositories.Recipe;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;

namespace UseCases.Test.Recipe.Register
{
    public class RegisterRecipeUseCaseTest
    {
        [Fact]
        public async Task UseCase_Should_Returns_ResponseRegisteredRecipeJson_When_RequestIsValid()
        {
            var request = RequestRecipeJsonBuilder.Build();
            (var user, _) = UserBuilder.Build();
            var useCase = CreateUseCase(user);

            var result = await useCase.Execute(request);

            result.ShouldNotBeNull();
            result.Id.ShouldNotBeNull();
            result.Title.ShouldBe(request.Title);
        }

        [Fact]
        public async Task UseCase_Should_Returns_AnError_When_TitleIsEmpty()
        {
            var request = RequestRecipeJsonBuilder.Build();
            request.Title = string.Empty;
            (var user, _) = UserBuilder.Build();
            var useCase = CreateUseCase(user);


            Func<Task> action = async () => await useCase.Execute(request);

            (await action.ShouldThrowAsync<ErrorOnValidationException>()).ShouldSatisfyAllConditions
                (
                 e => e.ErrorMessages.ShouldHaveSingleItem(),
                 e => e.ErrorMessages.ShouldContain(ResourceMessageException.RECIPE_TITLE_EMPTY)
                );
        }

        private static RegisterRecipeUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
        {
            var mapper = MapperBuilder.Build();
            var recipeWriteOnlyRepository = RecipeWriteOnlyRepositoryBuilder.Build();
            var unitOfwork = UnitOfWorkBuilder.Build();
            var loggedUser = LoggedUserBuilder.Build(user);

            return new RegisterRecipeUseCase(mapper, recipeWriteOnlyRepository, unitOfwork, loggedUser);
        }
    }
}
