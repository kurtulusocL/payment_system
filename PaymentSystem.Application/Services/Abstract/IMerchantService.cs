using PaymentSystem.Shared.Dtos.MappingDtos.MerchantDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Application.Services.Abstract
{
    public interface IMerchantService
    {
        IQueryable<MerchantGetDto> GetAllIncluding();
        IQueryable<MerchantGetDto> GetAllIncludingByStatusId(int merchantStatusId);
        IQueryable<MerchantGetDto> GetAllIncludingByNullTaskNumber();
        IQueryable<MerchantGetDto> GetAllIncludingByPayment();
        IQueryable<MerchantGetDto> GetAllIncludingForAdmin();
        Task<IEnumerable<MerchantGetDto>> GetByIdAsync(int? id);
        Task<Result<bool>> CreateAsync(MerchantCreateDto merchantCreateDto);
        Task<MerchantUpdateDto> GetByIdForUpdateAsync(int id);
        Task<Result<bool>> UpdateAsync(MerchantUpdateDto merchantUpdateDto);
        Task<Result<bool>> DeleteAsync(int id);
        Task<Result<bool>> DeleteByIdAsync(List<int> ids);
        Task<Result<bool>> SetActiveAsync(int id);
        Task<Result<bool>> SetInActiveAsync(int id);
        Task<Result<bool>> SetDeletedAsync(int id);
        Task<Result<bool>> SetNotDeletedAsync(int id);
    }
}
