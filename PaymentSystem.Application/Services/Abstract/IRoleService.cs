using PaymentSystem.Shared.Dtos.MappingDtos.AppRoleDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Application.Services.Abstract
{
    public interface IRoleService
    {
        IQueryable<AppRoleGetDto> GetAll();
        IQueryable<AppRoleGetDto> GetAllForAdmin();
        Task<AppRoleGetDto> GetByIdAsync(string? id);
        Task<Result<AppRoleCreateDto>> CreateAsync(AppRoleCreateDto dto);
        Task<AppRoleUpdateDto> GetForEditAsync(string id);
        Task<Result<AppRoleUpdateDto>> UpdateAsync(AppRoleUpdateDto dto);
        Task<Result<bool>> DeleteAsync(string id);
        Task<Result<bool>> DeleteByIdAsync(List<string> ids);
        Task<Result<bool>> SetActiveAsync(string id);
        Task<Result<bool>> SetInActiveAsync(string id);
        Task<Result<bool>> SetDeletedAsync(string id);
        Task<Result<bool>> SetNotDeletedAsync(string id);
    }
}
