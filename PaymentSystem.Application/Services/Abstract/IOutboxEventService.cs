using PaymentSystem.Shared.Dtos.MappingDtos.OutboxEventDto;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Application.Services.Abstract
{
    public interface IOutboxEventService
    {
        IQueryable<OutboxEventGetDto> GetAll();
        IQueryable<OutboxEventGetDto> GetAllBySuccessfullProcess();
        IQueryable<OutboxEventGetDto> GetAllByErrorProcess();
        IQueryable<OutboxEventGetDto> GetAllForAdmin();
        Task<OutboxEventGetDto> GetByIdAsync(int? id);
        Task<Result<bool>> DeleteAsync(int id);
        Task<Result<bool>> DeleteByIdAsync(List<int> ids);
        Task<Result<bool>> SetActiveAsync(int id);
        Task<Result<bool>> SetInActiveAsync(int id);
        Task<Result<bool>> SetDeletedAsync(int id);
        Task<Result<bool>> SetNotDeletedAsync(int id);
    }
}
