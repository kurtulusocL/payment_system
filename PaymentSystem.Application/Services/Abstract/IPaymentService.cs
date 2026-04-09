using PaymentSystem.Shared.Dtos.MappingDtos.PaymentDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Application.Services.Abstract
{
    public interface IPaymentService
    {
        IQueryable<PaymentGetDto> GetAllIncluding();
        IQueryable<PaymentGetDto> GetAllIncludingByUserId(string userId);
        IQueryable<PaymentGetDto> GetAllIncludingByPaymentStatusId(int paymentStatusId);
        IQueryable<PaymentGetDto> GetAllIncludingByCurrencyId(int currencyId);
        IQueryable<PaymentGetDto> GetAllIncludingByMerchantId(int merchantId);
        IQueryable<PaymentGetDto> GetAllIncludingForAdmin();
        Task<IEnumerable<PaymentGetDto>> GetByIdAsync(int? id);
        Task<Result<bool>> CreateAsync(PaymentCreateDto dto);
        Task<PaymentGetDto> GetByIdForUpdateAsync(int id);
        Task<Result<bool>> UpdateAsync(PaymentUpdateDto dto);
        Task<Result<bool>> DeleteAsync(int id);
        Task<Result<bool>> DeleteByIdAsync(List<int> ids);
        Task<Result<bool>> SetActiveAsync(int id);
        Task<Result<bool>> SetInActiveAsync(int id);
        Task<Result<bool>> SetDeletedAsync(int id);
        Task<Result<bool>> SetNotDeletedAsync(int id);
    }
}
