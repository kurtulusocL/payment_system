using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Logging;
using PaymentSystem.Application.Constants.Messages;
using PaymentSystem.Application.Constants.Services.Abstract;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Infrastructure.GenericRepository;
using PaymentSystem.Infrastructure.Repositories.Abstract;
using PaymentSystem.Shared.Dtos.MappingDtos.OutboxEventDto;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Infrastructure.Services.Concrete
{
    public class OutboxEventManager : IOutboxEventService
    {
        private readonly IOutboxEventRepository _outboxRepository;
        private readonly ICacheService _cacheService;
        private readonly IAzureService _azureService;
        private readonly IMapper _mapper;
        private readonly ILogger<OutboxEventManager> _logger;

        private const string CacheKeyAll = "outboxevents:all";
        private const string CacheKeyAdmin = "outboxevents:admin";
        private const string CacheKeySuccess = "outboxevents:success";
        private const string CacheKeyError = "outboxevents:error";
        private const string CacheKeyItemPrefix = "outboxevent:";
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(15);

        public OutboxEventManager(IOutboxEventRepository outboxRepository, ICacheService cacheService, IAzureService azureService, IMapper mapper,
            ILogger<OutboxEventManager> logger)
        {
            _outboxRepository = outboxRepository;
            _cacheService = cacheService;
            _azureService = azureService;
            _mapper = mapper;
            _logger = logger;
        }

        public IQueryable<OutboxEventGetDto> GetAll()
        {
            try
            {
                var cached = _cacheService.Get<List<OutboxEventGetDto>>(CacheKeyAll);
                if (cached != null) return cached.AsQueryable();

                var data = _outboxRepository.GetAll(
                    i => i.IsActive == true && i.IsDeleted == false)
                    .ProjectTo<OutboxEventGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate);

                var list = data.ToList();
                _cacheService.Set(CacheKeyAll, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OutboxEventManager.GetAll — local DB failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<OutboxEvent>(
                        i => i.IsActive == true && i.IsDeleted == false).GetAwaiter().GetResult();

                    return _mapper.Map<List<OutboxEventGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "OutboxEventManager.GetAll — Azure fallback failed");
                    return Enumerable.Empty<OutboxEventGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<OutboxEventGetDto> GetAllBySuccessfullProcess()
        {
            try
            {
                var cached = _cacheService.Get<List<OutboxEventGetDto>>(CacheKeySuccess);
                if (cached != null) return cached.AsQueryable();

                var data = _outboxRepository.GetAll(
                    i => i.IsProcessed == true && i.IsActive == true && i.IsDeleted == false)
                    .ProjectTo<OutboxEventGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate);

                var list = data.ToList();
                _cacheService.Set(CacheKeySuccess, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OutboxEventManager.GetAllBySuccessfullProcess — local DB failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<OutboxEvent>(
                        i => i.IsProcessed == true && i.IsActive == true && i.IsDeleted == false).GetAwaiter().GetResult();

                    return _mapper.Map<List<OutboxEventGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "OutboxEventManager.GetAllBySuccessfullProcess — Azure fallback failed");
                    return Enumerable.Empty<OutboxEventGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<OutboxEventGetDto> GetAllByErrorProcess()
        {
            try
            {
                var cached = _cacheService.Get<List<OutboxEventGetDto>>(CacheKeyError);
                if (cached != null) return cached.AsQueryable();

                var data = _outboxRepository.GetAll(
                    i => i.IsProcessed == false && i.IsActive == true && i.IsDeleted == false)
                    .ProjectTo<OutboxEventGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate);

                var list = data.ToList();
                _cacheService.Set(CacheKeyError, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OutboxEventManager.GetAllByErrorProcess — local DB failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<OutboxEvent>(
                        i => i.IsProcessed == false && i.IsActive == true && i.IsDeleted == false).GetAwaiter().GetResult();

                    return _mapper.Map<List<OutboxEventGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "OutboxEventManager.GetAllByErrorProcess — Azure fallback failed");
                    return Enumerable.Empty<OutboxEventGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<OutboxEventGetDto> GetAllForAdmin()
        {
            try
            {
                var cached = _cacheService.Get<List<OutboxEventGetDto>>(CacheKeyAdmin);
                if (cached != null) return cached.AsQueryable();

                var data = _outboxRepository.GetAll(null)
                    .ProjectTo<OutboxEventGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate);

                var list = data.ToList();
                _cacheService.Set(CacheKeyAdmin, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OutboxEventManager.GetAllForAdmin — local DB failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<OutboxEvent>(
                        null).GetAwaiter().GetResult();

                    return _mapper.Map<List<OutboxEventGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "OutboxEventManager.GetAllForAdmin — Azure fallback failed");
                    return Enumerable.Empty<OutboxEventGetDto>().AsQueryable();
                }
            }
        }

        public async Task<OutboxEventGetDto> GetByIdAsync(int? id)
        {
            if (id == null) return null!;
            var cacheKey = $"{CacheKeyItemPrefix}{id}";
            try
            {
                var cached = _cacheService.Get<OutboxEventGetDto>(cacheKey);
                if (cached != null) return cached;

                var entity = await _outboxRepository.GetAsync(i => i.Id == id);
                if (entity == null) return null!;

                var dto = _mapper.Map<OutboxEventGetDto>(entity);
                _cacheService.Set(cacheKey, dto, CacheExpiry);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OutboxEventManager.GetByIdAsync({Id})", id);
                try
                {
                    var azureData = await _azureService.GetFromAzureAsync<OutboxEvent>(id);
                    return azureData != null ? _mapper.Map<OutboxEventGetDto>(azureData) : null!;
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "OutboxEventManager.GetByIdAsync — Azure fallback failed");
                    return null!;
                }
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _outboxRepository.GetAsync(x => x.Id == id);
                if (entity == null) return Result<bool>.Failure("OutboxEvent bulunamadı.");
                await _outboxRepository.DeleteAsync(entity);
                InvalidateOutboxCaches(id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OutboxEventManager.DeleteAsync({Id}) failed", id);
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> DeleteByIdAsync(List<int> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    var entity = await _outboxRepository.GetAsync(x => x.Id == id);
                    if (entity != null)
                    {
                        await _outboxRepository.DeleteAsync(entity);
                        InvalidateOutboxCaches(id);
                    }
                }
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OutboxEventManager.DeleteByIdAsync failed");
                return Result<bool>.Failure("Toplu silme başarısız: " + ex.Message);
            }
        }

        public async Task<Result<bool>> SetActiveAsync(int id)
        {
            try
            {
                var entity = await _outboxRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = true;
                entity.SuspendedDate = null;

                var result = await _outboxRepository.SetActiveAsync(entity);
                if (result)
                {
                    InvalidateOutboxCaches(id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsActiveError);
            }
            catch (Exception ex) { _logger.LogError(ex, "OutboxEventManager.SetActiveAsync({Id})", id); return Result<bool>.Failure(ex.Message); }
        }

        public async Task<Result<bool>> SetInActiveAsync(int id)
        {
            try
            {
                var entity = await _outboxRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = false;
                entity.SuspendedDate = DateTime.UtcNow;

                var result = await _outboxRepository.SetDeActiveAsync(entity);

                if (result)
                {
                    InvalidateOutboxCaches(id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsActiveError);
            }
            catch (Exception ex) { _logger.LogError(ex, "OutboxEventManager.SetInActiveAsync({Id})", id); return Result<bool>.Failure(ex.Message); }
        }

        public async Task<Result<bool>> SetDeletedAsync(int id)
        {
            try
            {
                var entity = await _outboxRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = true;
                entity.DeletedDate = DateTime.UtcNow;

                var result = await _outboxRepository.SetDeletedAsync(entity);
                if (result == true)
                {
                    InvalidateOutboxCaches(id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsDeletedError);
            }
            catch (Exception ex) { _logger.LogError(ex, "OutboxEventManager.SetDeletedAsync({Id})", id); return Result<bool>.Failure(ex.Message); }
        }

        public async Task<Result<bool>> SetNotDeletedAsync(int id)
        {
            try
            {
                var entity = await _outboxRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = false;
                entity.DeletedDate = null;

                var result = await _outboxRepository.SetNotDeletedAsync(entity);
                if (result == true)
                {
                    InvalidateOutboxCaches(id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.NotDeleteError);
            }
            catch (Exception ex) { _logger.LogError(ex, "OutboxEventManager.SetNotDeletedAsync({Id})", id); return Result<bool>.Failure(ex.Message); }
        }

        private void InvalidateOutboxCaches(int? id = null)
        {
            _cacheService.Remove(CacheKeyAll);
            _cacheService.Remove(CacheKeyAdmin);
            _cacheService.Remove(CacheKeySuccess);
            _cacheService.Remove(CacheKeyError);
            if (id.HasValue)
                _cacheService.Remove($"{CacheKeyItemPrefix}{id}");
        }

    }
}
