using PaymentSystem.Shared.Dtos.MappingDtos.TransactionDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Application.Services.Abstract
{
    public interface ITransactionService
    {
        IQueryable<TransactionGetDto> GetAllIncluding();
        IQueryable<TransactionGetDto> GetAllIncludingByWalletId(int walletId);
        IQueryable<TransactionGetDto> GetAllIncludingByPaymentId(int? paymentId);
        IQueryable<TransactionGetDto> GetAllIncludingByCurrencyId(int currencyId);
        IQueryable<TransactionGetDto> GetAllIncludingByTransactionTypeId(int transactionTypeId);
        IQueryable<TransactionGetDto> GetAllIncludingForAdmin();
        Task<TransactionGetDto> GetByIdAsync(int? id);
        Task<Result<bool>> CreateAsync(TransactionCreateDto dto);
        Task<TransactionGetDto> GetByIdForUpdate(int id);
        Task<Result<bool>> UpdateAsync(TransactionUpdateDto dto);
        Task<Result<bool>> DeleteAsync(int id);
        Task<Result<bool>> DeleteByIdAsync(List<int> ids);
        Task<Result<bool>> SetActiveAsync(int id);
        Task<Result<bool>> SetInActiveAsync(int id);
        Task<Result<bool>> SetDeletedAsync(int id);
        Task<Result<bool>> SetNotDeletedAsync(int id);
    }
}
