using MyRecipeBook.Application.Services.Cryptography;

namespace CommonTestUtilities.Cryptography
{
    public class PasswordEncripterBuilder
    {
        public static PasswordEncrypter Build()
        {
            return new PasswordEncrypter("ABC");
        }
    }
}
