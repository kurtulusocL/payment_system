using PaymentSystem.Shared.Dtos.MappingDtos.AppUserDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Application.Services.Abstract
{
    public interface IUserService
    {
        IQueryable<AppUserGetDto> GetAllIncluding();
        IQueryable<AppUserGetDto> GetAllIncludingDeActiveUser();
        IQueryable<AppUserGetDto> GetAllIncludingDeletedUser();
        IQueryable<AppUserGetDto> GetAllIncludingActiveUser();
        IQueryable<AppUserGetDto> GetAllIncludingForAdmin();
        Task<AppUserGetDto> GetByIdAsync(string? id);
        Task<Result<bool>> DeleteAsync(string id);
        Task<Result<bool>> DeleteByIdAsync(List<string> ids);
        Task<Result<bool>> SetActiveAsync(string id);
        Task<Result<bool>> SetInActiveAsync(string id);
        Task<Result<bool>> SetDeletedAsync(string id);
        Task<Result<bool>> SetNotDeletedAsync(string id);
    }
}
