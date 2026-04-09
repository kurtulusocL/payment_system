using PaymentSystem.Shared.Dtos.MappingDtos.SecuritySettingDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Application.Services.Abstract
{
    public interface ISecuritySettingService
    {
        IQueryable<SecuritySettingGetDto> GetAll();
        IQueryable<SecuritySettingGetDto> GetAllForAdmin();
        Task<SecuritySettingGetDto> GetByIdAsync(int? id);
        Task<Result<bool>> CreateAsync(SecuritySettingCreateDto securitySettingCreateDto);
        Task<SecuritySettingGetDto> GetByIdForUpdateAsync(int id);
        Task<Result<bool>> UpdateAsync(SecuritySettingUpdateDto securitySettingUpdateDto);
        Task<Result<bool>> DeleteAsync(int id);
        Task<Result<bool>> DeleteByIdAsync(List<int> ids);
        Task<Result<bool>> SetActiveAsync(int id);
        Task<Result<bool>> SetInActiveAsync(int id);
        Task<Result<bool>> SetDeletedAsync(int id);
        Task<Result<bool>> SetNotDeletedAsync(int id);
    }
}
