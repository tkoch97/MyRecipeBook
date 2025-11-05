using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommonTestUtilities.Requests
{
    public static class RequestRegisterUserJsonBuilder
    {
        public static RequestRegisterUserJson Build(int passwordLength = 8) 
        {
            return new Faker<RequestRegisterUserJson>("pt_BR")
                .RuleFor(u => u.Name, (f) => f.Person.FirstName)
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.Name))
                .RuleFor(u => u.Password, (f) => f.Internet.Password(passwordLength));
        }
    }
}
