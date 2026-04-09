using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using PaymentSystem.Application.Constants.Services.Abstract;

namespace PaymentSystem.Application.Constants.Services.Concrete
{
    public class EncryptionService:IEncryptionService
    {
        private readonly byte[] _key;

        public EncryptionService(IConfiguration configuration)
        {
            var base64Key = configuration["Encryption:Key"]
                ?? throw new InvalidOperationException("Encryption key is not configured.");

            _key = Convert.FromBase64String(base64Key);

            if (_key.Length != 32)
                throw new InvalidOperationException("Encryption key must be 32 bytes.");
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;

            var nonce = new byte[12];
            RandomNumberGenerator.Fill(nonce);

            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes = new byte[plainBytes.Length];
            var tag = new byte[16];

            using var aesGcm = new AesGcm(_key, 16);
            aesGcm.Encrypt(nonce, plainBytes, cipherBytes, tag);

            var result = new byte[nonce.Length + cipherBytes.Length + tag.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
            Buffer.BlockCopy(cipherBytes, 0, result, nonce.Length, cipherBytes.Length);
            Buffer.BlockCopy(tag, 0, result, nonce.Length + cipherBytes.Length, tag.Length);

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;

            var fullBytes = Convert.FromBase64String(cipherText);

            var nonce = new byte[12];
            var tag = new byte[16];
            var cipherBytes = new byte[fullBytes.Length - nonce.Length - tag.Length];

            Buffer.BlockCopy(fullBytes, 0, nonce, 0, nonce.Length);
            Buffer.BlockCopy(fullBytes, nonce.Length, cipherBytes, 0, cipherBytes.Length);
            Buffer.BlockCopy(fullBytes, nonce.Length + cipherBytes.Length, tag, 0, tag.Length);

            var plainBytes = new byte[cipherBytes.Length];

            using var aesGcm = new AesGcm(_key, 16);
            aesGcm.Decrypt(nonce, cipherBytes, tag, plainBytes);

            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}
