using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Ganss.Xss;
using Microsoft.Extensions.Logging;
using PaymentSystem.Application.Constants.Messages;
using PaymentSystem.Application.Constants.Services.Abstract;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Infrastructure.GenericRepository;
using PaymentSystem.Infrastructure.Repositories.Abstract;
using PaymentSystem.Shared.Dtos.MappingDtos.MerchantStatusDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Infrastructure.Services.Concrete
{
    public class MerchantStatusManager : IMerchantStatusService
    {
        private readonly IMerchantStatusRepository _merchantStatusRepository;
        private readonly ICacheService _cacheService;
        private readonly IAzureService _azureService;
        private readonly IMapper _mapper;
        private readonly ILogger<MerchantStatusManager> _logger;
        private readonly IHtmlSanitizer _htmlSanitizer;

        private const string CacheKeyAll = "merchantstatuses:all";
        private const string CacheKeyAdmin = "merchantstatuses:admin";
        private const string CacheKeyByMerchants = "merchantstatuses:bymerchants";
        private const string CacheKeyItemPrefix = "merchantstatus:";
        private const string CacheKeyEditPrefix = "merchantstatus:edit:";
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(30);

        public MerchantStatusManager(IMerchantStatusRepository merchantStatusRepository, ICacheService cacheService, IAzureService azureService, IMapper mapper, ILogger<MerchantStatusManager> logger, IHtmlSanitizer htmlSanitizer)
        {
            _merchantStatusRepository = merchantStatusRepository;
            _cacheService = cacheService;
            _azureService = azureService;
            _mapper = mapper;
            _logger = logger;
            _htmlSanitizer = htmlSanitizer;
        }

        public IQueryable<MerchantStatusGetDto> GetAllIncluding()
        {
            try
            {
                var cached = _cacheService.Get<List<MerchantStatusGetDto>>(CacheKeyAll);
                if (cached != null) return cached.AsQueryable();

                var data = _merchantStatusRepository.GetAllInclude(new Expression<Func<MerchantStatus, bool>>[]
                {
                    i => i.IsActive == true,
                    i => i.IsDeleted == false
                }, null, y => y.Merchants).ProjectTo<MerchantStatusGetDto>(_mapper.ConfigurationProvider).OrderByDescending(i => i.CreatedDate).ToList();

                _cacheService.Set(CacheKeyAll, data, CacheExpiry);
                return data.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MerchantStatusManager.GetAllIncluding failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<MerchantStatus>(
                        i => i.IsActive == true && i.IsDeleted == false, y => y.Merchants).GetAwaiter().GetResult();
                    return _mapper.Map<List<MerchantStatusGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch
                {
                    return Enumerable.Empty<MerchantStatusGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<MerchantStatusGetDto> GetAllIncludingOrderByMerchants()
        {
            try
            {
                var cached = _cacheService.Get<List<MerchantStatusGetDto>>(CacheKeyByMerchants);
                if (cached != null) return cached.AsQueryable();

                var data = _merchantStatusRepository.GetAllInclude(
                    new Expression<Func<MerchantStatus, bool>>[]
                    {
                        i => i.IsActive == true,
                        i => i.IsDeleted == false
                    }, null, y => y.Merchants).ProjectTo<MerchantStatusGetDto>(_mapper.ConfigurationProvider).OrderByDescending(i => i.MerchantCount).ToList();

                _cacheService.Set(CacheKeyByMerchants, data, CacheExpiry);
                return data.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MerchantStatusManager.GetAllIncludingOrderByMerchants failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<MerchantStatus>(null, y => y.Merchants).GetAwaiter().GetResult();
                    return _mapper.Map<List<MerchantStatusGetDto>>(azureData.OrderByDescending(i => i.Merchants.Count()).ToList()).AsQueryable();
                }
                catch
                {
                    return Enumerable.Empty<MerchantStatusGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<MerchantStatusGetDto> GetAllIncludingForAdmin()
        {
            try
            {
                var cached = _cacheService.Get<List<MerchantStatusGetDto>>(CacheKeyAdmin);
                if (cached != null) return cached.AsQueryable();

                var data = _merchantStatusRepository.GetAllInclude(null, null, y => y.Merchants)
                    .ProjectTo<MerchantStatusGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate).ToList();

                _cacheService.Set(CacheKeyAdmin, data, CacheExpiry);
                return data.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MerchantStatusManager.GetAllIncludingForAdmin failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<MerchantStatus>(null, y => y.Merchants).GetAwaiter().GetResult();
                    return _mapper.Map<List<MerchantStatusGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch
                {
                    return Enumerable.Empty<MerchantStatusGetDto>().AsQueryable();
                }
            }
        }

        public async Task<MerchantStatusGetDto> GetByIdAsync(int? id)
        {
            if (id == null) return null!;
            var cacheKey = $"{CacheKeyItemPrefix}{id}";
            try
            {
                var cached = _cacheService.Get<MerchantStatusGetDto>(cacheKey);
                if (cached != null) return cached;

                var entity = await _merchantStatusRepository.GetIncludeAsync(i => i.Id == id, y => y.Merchants);
                if (entity == null) return null!;

                var dto = _mapper.Map<MerchantStatusGetDto>(entity);
                _cacheService.Set(cacheKey, dto, CacheExpiry);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MerchantStatusManager.GetByIdAsync({Id})", id);
                return null!;
            }
        }

        public async Task<MerchantStatusGetDto> GetByIdForUpdateAsync(int id)
        {
            var cacheKey = $"{CacheKeyEditPrefix}{id}";
            try
            {
                var cached = _cacheService.Get<MerchantStatusGetDto>(cacheKey);
                if (cached != null) return cached;

                var entity = await _merchantStatusRepository.GetAsync(i => i.Id == id);
                if (entity == null) return null!;

                var dto = _mapper.Map<MerchantStatusGetDto>(entity);
                _cacheService.Set(cacheKey, dto, CacheExpiry);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MerchantStatusManager.GetByIdForUpdateAsync({Id})", id);
                return null!;
            }
        }

        public async Task<Result<bool>> CreateAsync(MerchantStatusCreateDto merchantStatusCreateDto)
        {
            try
            {
                if (merchantStatusCreateDto == null) return Result<bool>.Failure("Veri boş olamaz.");

                var safeDescription = _htmlSanitizer.Sanitize(merchantStatusCreateDto.Description ?? string.Empty);

                var entity = _mapper.Map<MerchantStatus>(merchantStatusCreateDto);
                entity.Description = safeDescription;

                entity.IsActive = true;
                entity.IsDeleted = false;
                entity.CreatedDate = DateTime.UtcNow;

                var result = await _merchantStatusRepository.AddAsync(entity);
                if (result)
                {
                    InvalidateCaches();
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.AddError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MerchantStatusManager.CreateAsync failed");
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> UpdateAsync(MerchantStatusUpdateDto merchantStatusUpdateDto)
        {
            try
            {
                var entity = await _merchantStatusRepository.GetAsync(x => x.Id == merchantStatusUpdateDto.Id);
                if (entity == null) return Result<bool>.Failure(MessageConstants.NotFound);

                var safeDescription = _htmlSanitizer.Sanitize(merchantStatusUpdateDto.Description ?? string.Empty);

                _mapper.Map(merchantStatusUpdateDto, entity);
                entity.Description = safeDescription;
                entity.UpdatedDate = DateTime.UtcNow;

                var result = await _merchantStatusRepository.UpdateAsync(entity);
                if (result)
                {
                    InvalidateCaches(merchantStatusUpdateDto.Id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.UpdateError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MerchantStatusManager.UpdateAsync failed");
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _merchantStatusRepository.GetAsync(x => x.Id == id);
                if (entity == null) return Result<bool>.Failure(MessageConstants.NotFound);

                var result = await _merchantStatusRepository.DeleteAsync(entity);
                if (result)
                {
                    InvalidateCaches(id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.DeleteError);
            }
            catch (Exception ex) { return Result<bool>.Failure(ex.Message); }
        }

        public async Task<Result<bool>> DeleteByIdAsync(List<int> ids)
        {
            try
            {
                var notFound = new List<int>();
                foreach (var id in ids)
                {
                    var entity = await _merchantStatusRepository.GetAsync(x => x.Id == id);
                    if (entity == null) { notFound.Add(id); continue; }

                    await _merchantStatusRepository.DeleteAsync(entity);
                    InvalidateCaches(id);
                }

                if (notFound.Count == ids.Count)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                InvalidateCaches();
                return Result<bool>.Success(true);
            }
            catch (Exception ex) { return Result<bool>.Failure(ex.Message); }
        }

        public async Task<Result<bool>> SetActiveAsync(int id)
        {
            try
            {
                var entity = await _merchantStatusRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = true;
                entity.SuspendedDate = null;

                var result = await _merchantStatusRepository.SetActiveAsync(entity);
                if (result)
                {
                    InvalidateCaches(id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsActiveError);
            }
            catch (Exception ex) { return Result<bool>.Failure(ex.Message); }
        }

        public async Task<Result<bool>> SetInActiveAsync(int id)
        {
            try
            {
                var entity = await _merchantStatusRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = false;
                entity.SuspendedDate = DateTime.UtcNow;

                var result = await _merchantStatusRepository.SetDeActiveAsync(entity);

                if (result)
                {
                    InvalidateCaches(id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsActiveError);
            }
            catch (Exception ex) { return Result<bool>.Failure(ex.Message); }
        }

        public async Task<Result<bool>> SetDeletedAsync(int id)
        {
            try
            {
                var entity = await _merchantStatusRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = true;
                entity.DeletedDate = DateTime.UtcNow;

                var result = await _merchantStatusRepository.SetDeletedAsync(entity);
                if (result == true)
                {
                    InvalidateCaches(id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsDeletedError);
            }
            catch (Exception ex) { return Result<bool>.Failure(ex.Message); }
        }

        public async Task<Result<bool>> SetNotDeletedAsync(int id)
        {
            try
            {
                var entity = await _merchantStatusRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = false;
                entity.DeletedDate = null;

                var result = await _merchantStatusRepository.SetNotDeletedAsync(entity);
                if (result == true)
                {
                    InvalidateCaches(id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.NotDeleteError);
            }
            catch (Exception ex) { return Result<bool>.Failure(ex.Message); }
        }

        private void InvalidateCaches(int? id = null)
        {
            _cacheService.Remove(CacheKeyAll);
            _cacheService.Remove(CacheKeyAdmin);
            _cacheService.Remove(CacheKeyByMerchants);
            if (id.HasValue)
            {
                _cacheService.Remove($"{CacheKeyItemPrefix}{id}");
                _cacheService.Remove($"{CacheKeyEditPrefix}{id}");
            }
        }
    }
}
