using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Infrastructure.Secutiry.Cryptography;

namespace CommonTestUtilities.Cryptography
{
    public class PasswordEncripterBuilder
    {
        public static IPasswordEncrypter Build()
        {
            return new Sha512Encrypter("ABC");
        }
    }
}
