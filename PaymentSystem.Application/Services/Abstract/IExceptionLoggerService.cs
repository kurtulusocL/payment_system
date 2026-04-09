using PaymentSystem.Shared.Dtos.MappingDtos.ExceptionLoggerDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Application.Services.Abstract
{
    public interface IExceptionLoggerService
    {
        IQueryable<ExceptionLoggerGetDto> GetAll();
        IQueryable<ExceptionLoggerGetDto> GetAllForAdmin();
        Task<ExceptionLoggerGetDto> GetByIdAsync(int? id);
        Task<Result<bool>> DeleteAsync(int id);
        Task<Result<bool>> DeleteByIdAsync(List<int> ids);
        Task<Result<bool>> SetActiveAsync(int id);
        Task<Result<bool>> SetInActiveAsync(int id);
        Task<Result<bool>> SetDeletedAsync(int id);
        Task<Result<bool>> SetNotDeletedAsync(int id);
    }
}
