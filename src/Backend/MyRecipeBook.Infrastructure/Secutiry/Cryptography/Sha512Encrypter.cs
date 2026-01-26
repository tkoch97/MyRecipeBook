using MyRecipeBook.Domain.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace MyRecipeBook.Infrastructure.Secutiry.Cryptography
{
    public class Sha512Encrypter : IPasswordEncrypter
    {
        private readonly string _additionalKey;

        public Sha512Encrypter(string additionalKey)
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
