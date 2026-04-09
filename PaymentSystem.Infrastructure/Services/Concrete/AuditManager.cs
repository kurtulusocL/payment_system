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
using PaymentSystem.Shared.Dtos.MappingDtos.AuditDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Infrastructure.Services.Concrete
{
    public class AuditManager : IAuditService
    {
        private readonly IAuditRepository _auditRepository;
        private readonly ICacheService _cacheService;
        private readonly IAzureService _azureService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuditManager> _logger;

        private const string CacheKeyAll = "audits:all";
        private const string CacheKeyAdmin = "audits:admin";
        private const string CacheKeyUserPrefix = "audits:user:";
        private const string CacheKeyItemPrefix = "audit:";
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(30);

        public AuditManager(IAuditRepository auditRepository, ICacheService cacheService, IAzureService azureService, IMapper mapper, ILogger<AuditManager> logger)
        {
            _auditRepository = auditRepository;
            _cacheService = cacheService;
            _azureService = azureService;
            _mapper = mapper;
            _logger = logger;
        }

        public IQueryable<AuditGetDto> GetAllIncluding()
        {
            try
            {
                var cached = _cacheService.Get<List<AuditGetDto>>(CacheKeyAll);
                if (cached != null) return cached.AsQueryable();

                var data = _auditRepository.GetAllInclude(new Expression<Func<Audit, bool>>[]
                    {
                        i => i.IsActive == true,
                        i => i.IsDeleted == false
                    }, null, y => y.AppUser)
                    .ProjectTo<AuditGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate)
                    .ToList();

                _cacheService.Set(CacheKeyAll, data, CacheExpiry);
                return data.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuditManager.GetAllIncluding — local DB failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<Audit>(
                        i => i.IsActive == true && i.IsDeleted == false,
                        y => y.AppUser).GetAwaiter().GetResult();

                    return _mapper.Map<List<AuditGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "AuditManager.GetAllIncluding — Azure fallback failed");
                    return Enumerable.Empty<AuditGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<AuditGetDto> GetAllIncludingByUserId(string userId)
        {
            var cacheKey = $"{CacheKeyUserPrefix}{userId}";
            try
            {
                var cached = _cacheService.Get<List<AuditGetDto>>(cacheKey);
                if (cached != null) return cached.AsQueryable();

                var data = _auditRepository.GetAllInclude(new Expression<Func<Audit, bool>>[]
                    {
                        i => i.AppUserId == userId,
                        i => i.IsActive == true,
                        i => i.IsDeleted == false
                    }, y => y.AppUser)
                    .ProjectTo<AuditGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate)
                    .ToList();

                _cacheService.Set(cacheKey, data, CacheExpiry);
                return data.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuditManager.GetAllIncludingByUserId({UserId})", userId);
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<Audit>(
                        i => i.AppUserId == userId && i.IsActive == true && i.IsDeleted == false,
                        y => y.AppUser).GetAwaiter().GetResult();

                    return _mapper.Map<List<AuditGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "AuditManager.GetAllIncludingByUserId — Azure fallback failed");
                    return Enumerable.Empty<AuditGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<AuditGetDto> GetAllIncludingForAdmin()
        {
            try
            {
                var cached = _cacheService.Get<List<AuditGetDto>>(CacheKeyAdmin);
                if (cached != null) return cached.AsQueryable();

                var data = _auditRepository.GetAllInclude(null, null, y => y.AppUser)
                    .ProjectTo<AuditGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate)
                    .ToList();

                _cacheService.Set(CacheKeyAdmin, data, CacheExpiry);
                return data.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuditManager.GetAllIncludingForAdmin — local DB failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<Audit>(null, y => y.AppUser).GetAwaiter().GetResult();
                    return _mapper.Map<List<AuditGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "AuditManager.GetAllIncludingForAdmin — Azure fallback failed");
                    return Enumerable.Empty<AuditGetDto>().AsQueryable();
                }
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _auditRepository.GetAsync(x => x.Id == id);
                if (entity == null) return Result<bool>.Failure(MessageConstants.NotFound);

                var result = await _auditRepository.DeleteAsync(entity);
                if (result)
                {
                    InvalidateAuditCaches(id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.DeleteError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuditManager.DeleteAsync({Id}) failed", id);
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> DeleteByIdAsync(List<int> ids)
        {
            try
            {
                var notFound = new List<int>();
                foreach (var id in ids)
                {
                    var entity = await _auditRepository.GetAsync(x => x.Id == id);
                    if (entity == null)
                    {
                        notFound.Add(id);
                        continue;
                    }
                    await _auditRepository.DeleteAsync(entity);
                    InvalidateAuditCaches(id);
                }

                if (notFound.Count == ids.Count)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuditManager.DeleteByIdAsync failed");
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> SetActiveAsync(int id)
        {
            try
            {
                var entity = await _auditRepository.GetAsync(x => x.Id == id);
                if (entity == null) return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = true;

                var result = await _auditRepository.SetActiveAsync(entity);
                if (result)
                {
                    InvalidateAuditCaches(id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsActiveError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuditManager.SetActiveAsync({Id})", id);
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> SetInActiveAsync(int id)
        {
            try
            {
                var entity = await _auditRepository.GetAsync(x => x.Id == id);
                if (entity == null) return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = false;
                entity.SuspendedDate = DateTime.UtcNow;

                var result = await _auditRepository.SetDeActiveAsync(entity);
                if (result)
                {
                    InvalidateAuditCaches(id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsActiveError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuditManager.SetInActiveAsync({Id})", id);
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> SetDeletedAsync(int id)
        {
            try
            {
                var entity = await _auditRepository.GetAsync(x => x.Id == id);
                if (entity == null) return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = true;
                entity.DeletedDate = DateTime.UtcNow;
                entity.UpdatedDate = DateTime.UtcNow;

                var result = await _auditRepository.SetDeletedAsync(entity);
                if (result)
                {
                    InvalidateAuditCaches(id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsDeletedError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuditManager.SetDeletedAsync({Id})", id);
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> SetNotDeletedAsync(int id)
        {
            try
            {
                var entity = await _auditRepository.GetAsync(x => x.Id == id);
                if (entity == null) return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = false;
                entity.DeletedDate = null;
                entity.UpdatedDate = DateTime.UtcNow;

                var result = await _auditRepository.SetNotDeletedAsync(entity);
                if (result)
                {
                    InvalidateAuditCaches(id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.NotDeleteError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuditManager.SetNotDeletedAsync({Id})", id);
                return Result<bool>.Failure(ex.Message);
            }
        }

        private void InvalidateAuditCaches(int? id = null, string? userId = null)
        {
            _cacheService.Remove(CacheKeyAll);
            _cacheService.Remove(CacheKeyAdmin);

            if (id.HasValue)
                _cacheService.Remove($"{CacheKeyItemPrefix}{id}");

            if (!string.IsNullOrEmpty(userId))
                _cacheService.Remove($"{CacheKeyUserPrefix}{userId}");
        }

        public async Task<Result<AuditGetDto>> GetByIdAsync(int id)
        {
            var cacheKey = $"{CacheKeyItemPrefix}{id}";
            try
            {
                var cached = _cacheService.Get<AuditGetDto>(cacheKey);
                if (cached != null) return Result<AuditGetDto>.Success(cached);

                var entity = await _auditRepository.GetIncludeAsync(i => i.Id == id, y => y.AppUser);
                if (entity == null) return Result<AuditGetDto>.Failure(MessageConstants.NotFound);

                var dto = _mapper.Map<AuditGetDto>(entity);
                _cacheService.Set(cacheKey, dto, CacheExpiry);
                return Result<AuditGetDto>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuditManager.GetByIdAsync({Id})", id);
                try
                {
                    var azureData = await _azureService.GetFromAzureWithIncludesAsync<Audit>(i => i.Id == id, y => y.AppUser);
                    if (azureData == null) return Result<AuditGetDto>.Failure(MessageConstants.NotFound);

                    return Result<AuditGetDto>.Success(_mapper.Map<AuditGetDto>(azureData));
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "AuditManager.GetByIdAsync — Azure fallback failed");
                    return Result<AuditGetDto>.Failure(ex.Message);
                }
            }
        }
    }
}
