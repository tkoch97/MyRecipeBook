using MyRecipeBook.Domain.Repositories.Recipe;

namespace CommonTestUtilities.Repositories.Recipe
{
    public class RecipeWriteOnlyRepositoryBuilder
    {
        public static IRecipeWriteOnlyRepository Build()
        {
            var mock = new Moq.Mock<IRecipeWriteOnlyRepository>();
            return mock.Object;
        }
    }
}
