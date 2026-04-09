using PaymentSystem.Shared.Dtos.MappingDtos.AuditDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Application.Services.Abstract
{
    public interface IAuditService
    {
        IQueryable<AuditGetDto> GetAllIncluding();
        IQueryable<AuditGetDto> GetAllIncludingByUserId(string userId);
        IQueryable<AuditGetDto> GetAllIncludingForAdmin();
        Task<Result<AuditGetDto>> GetByIdAsync(int id);
        Task<Result<bool>> DeleteAsync(int id);
        Task<Result<bool>> DeleteByIdAsync(List<int> ids);
        Task<Result<bool>> SetActiveAsync(int id);
        Task<Result<bool>> SetInActiveAsync(int id);
        Task<Result<bool>> SetDeletedAsync(int id);
        Task<Result<bool>> SetNotDeletedAsync(int id);
    }
}
