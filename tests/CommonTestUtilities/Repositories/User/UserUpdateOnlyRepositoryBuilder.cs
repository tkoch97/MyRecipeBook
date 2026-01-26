using Moq;
using MyRecipeBook.Domain.Repositories.User;

namespace CommonTestUtilities.Repositories.User
{
    public class UserUpdateOnlyRepositoryBuilder
    {
        private readonly Mock<IUserUpdateOnlyRepository> _repository;

        public UserUpdateOnlyRepositoryBuilder() => _repository = new Mock<IUserUpdateOnlyRepository>();

        public UserUpdateOnlyRepositoryBuilder GetById(MyRecipeBook.Domain.Entities.User user)
        {
            _repository.Setup(_repository => _repository.GetById(user.Id)).ReturnsAsync(user);
            return this;
        }

        public void Update(MyRecipeBook.Domain.Entities.User user)
        {
            _repository.Setup(_repository => _repository.Update(user));
        }

        public IUserUpdateOnlyRepository Build()
        {
            return _repository.Object;
        }
    }
}
