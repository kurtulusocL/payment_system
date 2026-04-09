using PaymentSystem.Shared.Dtos.MappingDtos.WalletDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Application.Services.Abstract
{
    public interface IWalletService
    {
        IQueryable<WalletGetDto> GetAllIncluding();
        IQueryable<WalletGetDto> GetAllIncludingByUserId(string userId);
        IQueryable<WalletGetDto> GetAllIncludingByCurrencyId(int currencyId);
        IQueryable<WalletGetDto> GetAllIncludingForAdmin();
        Task<WalletGetDto> GetByIdAsync(int? id);
        Task<Result<bool>> CreateAsync(WalletCreateDto dto);
        Task<WalletGetDto> GetByIdForUpdateAsync(int id);
        Task<Result<bool>> UpdateAsync(WalletUpdateDto dto);
        Task<Result<bool>> DeleteAsync(int id);
        Task<Result<bool>> DeleteByIdAsync(List<int> ids);
        Task<Result<bool>> SetActiveAsync(int id);
        Task<Result<bool>> SetInActiveAsync(int id);
        Task<Result<bool>> SetDeletedAsync(int id);
        Task<Result<bool>> SetNotDeletedAsync(int id);
    }
}
