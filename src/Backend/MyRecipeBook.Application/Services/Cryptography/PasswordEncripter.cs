using System.Security.Cryptography;
using System.Text;

namespace MyRecipeBook.Application.Services.Cryptography
{
    public class PasswordEncrypter
    {   
        private readonly string _additionalKey;

        public PasswordEncrypter(string additionalKey)
        {
            _additionalKey = additionalKey;
        }

        public string Encrypt(string password)
        {
            var newPassword = $"{password}{_additionalKey}";

            // Precisamos pegar a senha e transformar em um array de bytes
            var bytes = Encoding.UTF8.GetBytes(newPassword);
            var hashBytes = SHA512.HashData(bytes);

            return ConvertByteArrayToString(hashBytes);
        }


        private static string ConvertByteArrayToString(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }

    }
}
