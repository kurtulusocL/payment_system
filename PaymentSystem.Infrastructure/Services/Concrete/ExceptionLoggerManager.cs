using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Logging;
using PaymentSystem.Application.Constants.Messages;
using PaymentSystem.Application.Constants.Services.Abstract;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Infrastructure.GenericRepository;
using PaymentSystem.Infrastructure.Repositories.Abstract;
using PaymentSystem.Shared.Dtos.MappingDtos.ExceptionLoggerDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Infrastructure.Services.Concrete
{
    public class ExceptionLoggerManager : IExceptionLoggerService
    {
        private readonly IExceptionLoggerRepository _exceptionLoggerRepository;
        private readonly ICacheService _cacheService;
        private readonly IAzureService _azureService;
        private readonly IMapper _mapper;
        private readonly ILogger<ExceptionLoggerManager> _logger;

        private const string CacheKeyAll = "exceptions:all";
        private const string CacheKeyAdmin = "exceptions:admin";
        private const string CacheKeyItemPrefix = "exception:";
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(30);

        public ExceptionLoggerManager(IExceptionLoggerRepository exceptionLoggerRepository, ICacheService cacheService, IAzureService azureService, IMapper mapper, ILogger<ExceptionLoggerManager> logger)
        {
            _exceptionLoggerRepository = exceptionLoggerRepository;
            _cacheService = cacheService;
            _azureService = azureService;
            _mapper = mapper;
            _logger = logger;
        }

        public IQueryable<ExceptionLoggerGetDto> GetAll()
        {
            try
            {
                var cached = _cacheService.Get<List<ExceptionLoggerGetDto>>(CacheKeyAll);
                if (cached != null) return cached.AsQueryable();

                var data = _exceptionLoggerRepository.GetAllInclude(new Expression<Func<ExceptionLogger, bool>>[]
                {
                    i => i.IsActive == true,
                    i => i.IsDeleted == false
                }, null).ProjectTo<ExceptionLoggerGetDto>(_mapper.ConfigurationProvider).OrderByDescending(i => i.CreatedDate);

                var list = data.ToList();
                _cacheService.Set(CacheKeyAll, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExceptionLoggerManager.GetAll — local DB failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<ExceptionLogger>(
                        i => i.IsActive == true && i.IsDeleted == false).GetAwaiter().GetResult();

                    return _mapper.Map<List<ExceptionLoggerGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "ExceptionLoggerManager.GetAll — Azure fallback failed");
                    return Enumerable.Empty<ExceptionLoggerGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<ExceptionLoggerGetDto> GetAllForAdmin()
        {
            try
            {
                var cached = _cacheService.Get<List<ExceptionLoggerGetDto>>(CacheKeyAdmin);
                if (cached != null) return cached.AsQueryable();

                var data = _exceptionLoggerRepository.GetAllInclude(null, null).ProjectTo<ExceptionLoggerGetDto>(_mapper.ConfigurationProvider).OrderByDescending(i => i.CreatedDate);

                var list = data.ToList();
                _cacheService.Set(CacheKeyAdmin, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExceptionLoggerManager.GetAllForAdmin — local DB failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<ExceptionLogger>(null).GetAwaiter().GetResult();
                    return _mapper.Map<List<ExceptionLoggerGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "ExceptionLoggerManager.GetAllForAdmin — Azure fallback failed");
                    return Enumerable.Empty<ExceptionLoggerGetDto>().AsQueryable();
                }
            }
        }

        public async Task<ExceptionLoggerGetDto> GetByIdAsync(int? id)
        {
            if (id == null) return null!;
            var cacheKey = $"{CacheKeyItemPrefix}{id}";
            try
            {
                var cached = _cacheService.Get<ExceptionLoggerGetDto>(cacheKey);
                if (cached != null) return cached;

                var entity = await _exceptionLoggerRepository.GetAsync(i => i.Id == id);
                if (entity == null) return null!;

                var dto = _mapper.Map<ExceptionLoggerGetDto>(entity);
                _cacheService.Set(cacheKey, dto, CacheExpiry);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExceptionLoggerManager.GetByIdAsync({Id})", id);
                try
                {
                    var azureData = await _azureService.GetFromAzureAsync<ExceptionLogger>(id.Value);
                    return azureData != null ? _mapper.Map<ExceptionLoggerGetDto>(azureData) : null!;
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "ExceptionLoggerManager.GetByIdAsync — Azure fallback failed");
                    return null!;
                }
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _exceptionLoggerRepository.GetAsync(x => x.Id == id);
                if (entity == null) return Result<bool>.Failure(MessageConstants.NotFound);

                var result = await _exceptionLoggerRepository.DeleteAsync(entity);
                if (result)
                {
                    InvalidateExceptionCaches(id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.DeleteError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExceptionLoggerManager.DeleteAsync({Id}) failed", id);
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> DeleteByIdAsync(List<int> ids)
        {
            if (ids == null || !ids.Any())
                return Result<bool>.Failure(MessageConstants.NotFound);

            try
            {
                var notFound = new List<int>();
                
                foreach (var id in ids)
                {
                    var entity = await _exceptionLoggerRepository.GetAsync(x => x.Id == id);
                    if (entity == null)
                    {
                        notFound.Add(id);
                        continue;
                    }
                    
                    await _exceptionLoggerRepository.DeleteAsync(entity);
                    InvalidateExceptionCaches(id);
                }

                if (notFound.Count == ids.Count)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExceptionLoggerManager.DeleteByIdAsync failed");
                return Result<bool>.Failure("Toplu silme başarısız: " + ex.Message);
            }
        }

        public async Task<Result<bool>> SetActiveAsync(int id)
        {
            try
            {
                var entity = await _exceptionLoggerRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = true;
                entity.SuspendedDate = null;

                var result = await _exceptionLoggerRepository.SetActiveAsync(entity);
                if (result)
                {
                    InvalidateExceptionCaches(id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsActiveError);
            }
            catch (Exception ex) { _logger.LogError(ex, "ExceptionLoggerManager.SetActiveAsync({Id})", id); return Result<bool>.Failure(ex.Message); }
        }

        public async Task<Result<bool>> SetInActiveAsync(int id)
        {
            try
            {
                var entity = await _exceptionLoggerRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = false;
                entity.SuspendedDate = DateTime.UtcNow;

                var result = await _exceptionLoggerRepository.SetDeActiveAsync(entity);

                if (result)
                {
                    InvalidateExceptionCaches(id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsActiveError);
            }
            catch (Exception ex) { _logger.LogError(ex, "ExceptionLoggerManager.SetInActiveAsync({Id})", id); return Result<bool>.Failure(ex.Message); }
        }

        public async Task<Result<bool>> SetDeletedAsync(int id)
        {
            try
            {
                var entity = await _exceptionLoggerRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = true;
                entity.DeletedDate = DateTime.UtcNow;

                var result = await _exceptionLoggerRepository.SetDeletedAsync(entity);
                if (result == true)
                {
                    InvalidateExceptionCaches(id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsDeletedError);
            }
            catch (Exception ex) { _logger.LogError(ex, "ExceptionLoggerManager.SetDeletedAsync({Id})", id); return Result<bool>.Failure(ex.Message); }
        }

        public async Task<Result<bool>> SetNotDeletedAsync(int id)
        {
            try
            {
                var entity = await _exceptionLoggerRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = false;
                entity.DeletedDate = null;

                var result = await _exceptionLoggerRepository.SetNotDeletedAsync(entity);
                if (result == true)
                {
                    InvalidateExceptionCaches(id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.NotDeleteError);
            }
            catch (Exception ex) { _logger.LogError(ex, "ExceptionLoggerManager.SetNotDeletedAsync({Id})", id); return Result<bool>.Failure(ex.Message); }
        }

        private void InvalidateExceptionCaches(int? id = null)
        {
            _cacheService.Remove(CacheKeyAll);
            _cacheService.Remove(CacheKeyAdmin);
            if (id.HasValue)
            {
                _cacheService.Remove($"{CacheKeyItemPrefix}{id}");
            }
        }
    }
}
