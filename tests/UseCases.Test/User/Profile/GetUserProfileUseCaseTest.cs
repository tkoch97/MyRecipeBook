using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using MyRecipeBook.Application.UseCases.User.Profile;
using Shouldly;

namespace UseCases.Test.User.Profile
{
    public class GetUserProfileUseCaseTest
    {
        [Fact]
        public async Task UseCase_Should_ReturnsUserEmailAndName_When_LoggedUserExists()
        {
            var (user, _) = UserBuilder.Build();
            var useCase = CreateUseCase(user);

            var result = await useCase.Execute();

            result.ShouldNotBeNull();
            result.Name.ShouldBe(user.Name);
            result.Email.ShouldBe(user.Email);
        }

        private static GetUserProfileUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
        {
            var mapper = MapperBuilder.Build();
            var loggedUserMock = LoggedUserBuilder.Build(user);

            return new GetUserProfileUseCase(loggedUserMock, mapper);
        }
    }
}
