using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommonTestUtilities.Requests
{
    public class RequestChangePasswordJsonBuilder
    {
        public static RequestChangePasswordJson Build(int currentPasswordLeght = 6, int newPasswordLenght = 7)
        {
            return new Faker<RequestChangePasswordJson>()
                .RuleFor(r => r.CurrentPassword, (f) => f.Internet.Password(currentPasswordLeght))
                .RuleFor(r => r.NewPassword, (f) => f.Internet.Password(newPasswordLenght));
        }

    }
}
