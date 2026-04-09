
namespace PaymentSystem.Application.Constants.Services.Abstract
{
    public interface IEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}
