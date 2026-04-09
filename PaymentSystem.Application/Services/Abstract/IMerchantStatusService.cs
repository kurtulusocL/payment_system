using PaymentSystem.Shared.Dtos.MappingDtos.MerchantStatusDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Application.Services.Abstract
{
    public interface IMerchantStatusService
    {
        IQueryable<MerchantStatusGetDto> GetAllIncluding();
        IQueryable<MerchantStatusGetDto> GetAllIncludingOrderByMerchants();
        IQueryable<MerchantStatusGetDto> GetAllIncludingForAdmin();
        Task<MerchantStatusGetDto> GetByIdAsync(int? id);
        Task<Result<bool>> CreateAsync(MerchantStatusCreateDto merchantStatusCreateDto);
        Task<MerchantStatusGetDto> GetByIdForUpdateAsync(int id);
        Task<Result<bool>> UpdateAsync(MerchantStatusUpdateDto merchantStatusUpdateDto);
        Task<Result<bool>> DeleteAsync(int id);
        Task<Result<bool>> DeleteByIdAsync(List<int> ids);
        Task<Result<bool>> SetActiveAsync(int id);
        Task<Result<bool>> SetInActiveAsync(int id);
        Task<Result<bool>> SetDeletedAsync(int id);
        Task<Result<bool>> SetNotDeletedAsync(int id);
    }
}
