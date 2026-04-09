using PaymentSystem.Shared.Dtos.MappingDtos.UserSessionDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Application.Services.Abstract
{
    public interface IUserSessionService
    {
        IQueryable<UserSessionGetDto> GetAllIncluding();
        IQueryable<UserSessionGetDto> GetAllIncludingByUserId(string userId);
        IQueryable<UserSessionGetDto> GetAllIncludingByOnline();
        IQueryable<UserSessionGetDto> GetAllIncludingByLoginDate();
        IQueryable<UserSessionGetDto> GetAllIncludingByOnlineDurationTime();
        IQueryable<UserSessionGetDto> GetAllIncludingForAdmin();
        Task<UserSessionGetDto> GetByIdAsync(int? id);
        Task<Result<bool>> CreateAsync(string username, DateTime loginDate, string userId);
        Task<Result<bool>> DeleteAsync(int id);
        Task<Result<bool>> DeleteByIdAsync(List<int> ids);
        Task<Result<bool>> SetActiveAsync(int id);
        Task<Result<bool>> SetInActiveAsync(int id);
        Task<Result<bool>> SetDeletedAsync(int id);
        Task<Result<bool>> SetNotDeletedAsync(int id);
    }
}
