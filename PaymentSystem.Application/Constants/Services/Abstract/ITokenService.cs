using PaymentSystem.Domain.Entities;

namespace PaymentSystem.Application.Constants.Services.Abstract
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}
