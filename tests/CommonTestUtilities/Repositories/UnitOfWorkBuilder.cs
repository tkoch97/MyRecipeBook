using MyRecipeBook.Domain.Repositories;

namespace CommonTestUtilities.Repositories
{
    public class UnitOfWorkBuilder
    {
        public static IUnitOfWork Build()
        { 
            var mock = new Moq.Mock<IUnitOfWork>();

            return mock.Object;
        }
    }
}
