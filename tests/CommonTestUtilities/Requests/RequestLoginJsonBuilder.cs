using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommonTestUtilities.Requests
{
    public class RequestLoginJsonBuilder
    {
        public static RequestLoginUserJson Build()
        {
            return new Faker<RequestLoginUserJson>("pt_BR")
                .RuleFor(u => u.Email, (f) => f.Internet.Email())
                .RuleFor(u => u.Password, (f) => f.Internet.Password());
        }

    }
}
