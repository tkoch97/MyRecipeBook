using Bogus;
using CommonTestUtilities.Cryptography;
using MyRecipeBook.Domain.Entities;

namespace CommonTestUtilities.Entities
{
    public class UserBuilder
    {
        public static (User user, string password) Build()
        {
            var passwordEncrypter = PasswordEncripterBuilder.Build();

            var password = new Faker().Internet.Password();

            var user = new Faker<User>()
                .RuleFor(u => u.Id, () => 1)
                .RuleFor(u => u.Name, (f) => f.Name.FullName())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.Name))
                .RuleFor(u => u.UserIdentifier, () => Guid.NewGuid())
                .RuleFor(u => u.Password, (f) => passwordEncrypter.Encrypt(password));

            return (user, password);
        }
    }
}
