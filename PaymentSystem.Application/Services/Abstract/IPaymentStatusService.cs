using PaymentSystem.Shared.Dtos.MappingDtos.PaymentStatusDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Application.Services.Abstract
{
    public interface IPaymentStatusService
    {
        IQueryable<PaymentStatusGetDto> GetAllIncluding();
        IQueryable<PaymentStatusGetDto> GetAllIncludingOrderByPayments();
        IQueryable<PaymentStatusGetDto> GetAllIncludingForAdmin();
        Task<PaymentStatusGetDto> GetByIdAsync(int? id);
        Task<Result<bool>> CreateAsync(PaymentStatusCreateDto paymentStatusCreateDto);
        Task<PaymentStatusGetDto> GetByIdForUpdateAsync(int id);
        Task<Result<bool>> UpdateAsync(PaymentStatusUpdateDto paymentStatusUpdateDto);
        Task<Result<bool>> DeleteAsync(int id);
        Task<Result<bool>> DeleteByIdAsync(List<int> ids);
        Task<Result<bool>> SetActiveAsync(int id);
        Task<Result<bool>> SetInActiveAsync(int id);
        Task<Result<bool>> SetDeletedAsync(int id);
        Task<Result<bool>> SetNotDeletedAsync(int id);
    }
}
