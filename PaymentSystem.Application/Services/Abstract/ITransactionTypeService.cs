using PaymentSystem.Shared.Dtos.MappingDtos.TransactionTypeDto;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Application.Services.Abstract
{
    public interface ITransactionTypeService
    {
        IQueryable<TransactionTypeGetDto> GetAllIncluding();
        IQueryable<TransactionTypeGetDto> GetAllIncludingOrderByTransactions();
        IQueryable<TransactionTypeGetDto> GetAllIncludingForAdmin();
        Task<TransactionTypeGetDto> GetByIdAsync(int? id);
        Task<Result<bool>> CreateAsync(TransactionTypeCreateDto transactionTypeCreateDto);
        Task<TransactionTypeGetDto> GetByIdForUpdateAsync(int id);
        Task<Result<bool>> UpdateAsync(TransactionTypeUpdateDto transactionTypeUpdateDto);
        Task<Result<bool>> DeleteAsync(int id);
        Task<Result<bool>> DeleteByIdAsync(List<int> ids);
        Task<Result<bool>> SetActiveAsync(int id);
        Task<Result<bool>> SetInActiveAsync(int id);
        Task<Result<bool>> SetDeletedAsync(int id);
        Task<Result<bool>> SetNotDeletedAsync(int id);
    }
}
