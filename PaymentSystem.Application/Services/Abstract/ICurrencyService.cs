using PaymentSystem.Shared.Dtos.MappingDtos.CurrencyDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Application.Services.Abstract
{
    public interface ICurrencyService
    {
        IQueryable<CurrencyGetDto> GetAllIncluding();
        IQueryable<CurrencyGetDto> GetAllIncludingOrderByPayment();
        IQueryable<CurrencyGetDto> GetAllIncludingOrderByWallet();
        IQueryable<CurrencyGetDto> GetAllIncludingOrderByTransaction();
        IQueryable<CurrencyGetDto> GetAllIncludingForAdmin();
        Task<CurrencyGetDto> GetByIdAsync(int? id);
        Task<Result<bool>> CreateAsync(CurrencyCreateDto currencyCreateDto);
        Task<CurrencyGetDto> GetByIdForUpdate(int id);
        Task<Result<bool>> UpdateAsync(CurrencyUpdateDto currencyUpdateDto);
        Task<Result<bool>> DeleteAsync(int id);
        Task<Result<bool>> DeleteByIdAsync(List<int> ids);
        Task<Result<bool>> SetActiveAsync(int id);
        Task<Result<bool>> SetInActiveAsync(int id);
        Task<Result<bool>> SetDeletedAsync(int id);
        Task<Result<bool>> SetNotDeletedAsync(int id);
    }
}
