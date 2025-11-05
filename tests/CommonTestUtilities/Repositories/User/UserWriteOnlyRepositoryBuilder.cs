using MyRecipeBook.Domain.Repositories.User;

namespace CommonTestUtilities.Repositories.User
{
    public class UserWriteOnlyRepositoryBuilder
    {
        public static IUserWriteOnlyRepository Build()
        {
            var mock = new Moq.Mock<IUserWriteOnlyRepository>();
            return mock.Object;
        }
    }
}
