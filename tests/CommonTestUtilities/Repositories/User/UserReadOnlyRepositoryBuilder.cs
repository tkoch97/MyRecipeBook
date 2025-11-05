

using Moq;
using MyRecipeBook.Domain.Repositories.User;

namespace CommonTestUtilities.Repositories.User
{
    public class UserReadOnlyRepositoryBuilder
    {
        private readonly Mock<IUserReadOnlyRepository> _repository;

        public UserReadOnlyRepositoryBuilder() => _repository = new Mock<IUserReadOnlyRepository>();

        public void ExistActiveUserWithEmail(string email)
        {
            _repository.Setup(_repository => _repository.ExistActiveUserWithEmail(email)).ReturnsAsync(true);
        }

        public IUserReadOnlyRepository Build()
        {
            return _repository.Object;
        }
    }
}
