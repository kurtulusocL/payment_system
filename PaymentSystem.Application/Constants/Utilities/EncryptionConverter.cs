using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PaymentSystem.Application.Constants.Services.Abstract;

namespace PaymentSystem.Application.Constants.Utilities
{
    public class EncryptionConverter : ValueConverter<string, string>
    {
        public EncryptionConverter(IEncryptionService encryptionService) : base
            (
                plainText => encryptionService.Encrypt(plainText),
                cipherText => encryptionService.Decrypt(cipherText)
            )
        { }
    }
}
