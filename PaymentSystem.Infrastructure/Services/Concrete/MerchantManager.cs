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
using PaymentSystem.Shared.Dtos.MappingDtos.MerchantDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Infrastructure.Services.Concrete
{
    public class MerchantManager : IMerchantService
    {
        private readonly IMerchantRepository _merchantRepository;
        private readonly ICacheService _cacheService;
        private readonly IAzureService _azureService;
        private readonly IMapper _mapper;
        private readonly ILogger<MerchantManager> _logger;
        private readonly IHtmlSanitizer _htmlSanitizer;

        private const string CacheKeyAll = "merchants:all";
        private const string CacheKeyAdmin = "merchants:admin";
        private const string CacheKeyStatusPrefix = "merchants:status:";
        private const string CacheKeyNullTask = "merchants:nulltask";
        private const string CacheKeyByPayment = "merchants:bypayment";
        private const string CacheKeyItemPrefix = "merchant:";
        private const string CacheKeyEditPrefix = "merchant:edit:";
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(30);

        public MerchantManager(IMerchantRepository merchantRepository, ICacheService cacheService, IAzureService azureService, IMapper mapper,
            ILogger<MerchantManager> logger, IHtmlSanitizer htmlSanitizer)
        {
            _merchantRepository = merchantRepository;
            _cacheService = cacheService;
            _azureService = azureService;
            _mapper = mapper;
            _logger = logger;
            _htmlSanitizer = htmlSanitizer;
        }

        public IQueryable<MerchantGetDto> GetAllIncluding()
        {
            try
            {
                var cached = _cacheService.Get<List<MerchantGetDto>>(CacheKeyAll);
                if (cached != null) return cached.AsQueryable();

                var data = _merchantRepository.GetAllInclude(new Expression<Func<Merchant, bool>>[]
                {
                    i => i.IsActive == true,
                    i => i.IsDeleted == false
                }, null, y => y.MerchantStatus, y => y.Payments).ProjectTo<MerchantGetDto>(_mapper.ConfigurationProvider).OrderByDescending(i => i.CreatedDate);

                var list = data.ToList();
                _cacheService.Set(CacheKeyAll, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MerchantManager.GetAllIncluding — local DB failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<Merchant>(
                        i => i.IsActive == true && i.IsDeleted == false,
                        y => y.MerchantStatus,
                        y => y.Payments).GetAwaiter().GetResult();

                    return _mapper.Map<List<MerchantGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "MerchantManager.GetAllIncluding — Azure fallback failed");
                    return Enumerable.Empty<MerchantGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<MerchantGetDto> GetAllIncludingByStatusId(int merchantStatusId)
        {
            var cacheKey = $"{CacheKeyStatusPrefix}{merchantStatusId}";
            try
            {
                var cached = _cacheService.Get<List<MerchantGetDto>>(cacheKey);
                if (cached != null) return cached.AsQueryable();

                var data = _merchantRepository.GetAllInclude(new Expression<Func<Merchant, bool>>[]
                    {
                        i => i.MerchantStatusId == merchantStatusId,
                        i => i.IsActive == true,
                        i => i.IsDeleted == false
                    }, y => y.MerchantStatus, y => y.Payments).ProjectTo<MerchantGetDto>(_mapper.ConfigurationProvider).OrderByDescending(i => i.CreatedDate);

                var list = data.ToList();
                _cacheService.Set(cacheKey, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MerchantManager.GetAllIncludingByStatusId({Id})", merchantStatusId);
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<Merchant>(
                        i => i.MerchantStatusId == merchantStatusId && i.IsActive == true && i.IsDeleted == false,
                        y => y.MerchantStatus,
                        y => y.Payments).GetAwaiter().GetResult();

                    return _mapper.Map<List<MerchantGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "MerchantManager.GetAllIncludingByStatusId — Azure fallback failed");
                    return Enumerable.Empty<MerchantGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<MerchantGetDto> GetAllIncludingByNullTaskNumber()
        {
            try
            {
                var cached = _cacheService.Get<List<MerchantGetDto>>(CacheKeyNullTask);
                if (cached != null) return cached.AsQueryable();

                var data = _merchantRepository.GetAllInclude(new Expression<Func<Merchant, bool>>[]
                {
                    i=>i.IsActive==true,
                    i=>i.IsDeleted==false,
                    i=>i.TaxNumber==null,
                    i => !i.Payments.Any(),
                    i => i.IsActive == true,
                    i => i.IsDeleted == false
                }, null, y => y.MerchantStatus, y => y.Payments).ProjectTo<MerchantGetDto>(_mapper.ConfigurationProvider).OrderByDescending(i => i.CreatedDate);

                var list = data.ToList();
                _cacheService.Set(CacheKeyNullTask, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MerchantManager.GetAllIncludingByNullTaskNumber — local DB failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<Merchant>(
                        i => i.IsActive == true && i.IsDeleted == false,
                        i => i.TaxNumber == null,
                        y => y.MerchantStatus,
                        y => y.Payments).GetAwaiter().GetResult();

                    return _mapper.Map<List<MerchantGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "MerchantManager.GetAllIncludingByNullTaskNumber — Azure fallback failed");
                    return Enumerable.Empty<MerchantGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<MerchantGetDto> GetAllIncludingByPayment()
        {
            try
            {
                var cached = _cacheService.Get<List<MerchantGetDto>>(CacheKeyByPayment);
                if (cached != null) return cached.AsQueryable();

                var data = _merchantRepository.GetAllInclude(new Expression<Func<Merchant, bool>>[]
                {
                    i => i.Payments.Any(),
                    i => i.IsActive == true,
                    i => i.IsDeleted == false
                }, null, y => y.MerchantStatus, y => y.Payments).ProjectTo<MerchantGetDto>(_mapper.ConfigurationProvider).OrderByDescending(i => i.PaymentCount);

                var list = data.ToList();
                _cacheService.Set(CacheKeyByPayment, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MerchantManager.GetAllIncludingByPayment — local DB failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<Merchant>(
                        i => i.IsActive == true && i.IsDeleted == false,
                        y => y.MerchantStatus,
                        y => y.Payments).GetAwaiter().GetResult();

                    return _mapper.Map<List<MerchantGetDto>>(azureData.OrderByDescending(i => i.Payments.Count()).ToList()).AsQueryable();
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "MerchantManager.GetAllIncludingByPayment — Azure fallback failed");
                    return Enumerable.Empty<MerchantGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<MerchantGetDto> GetAllIncludingForAdmin()
        {
            try
            {
                var cached = _cacheService.Get<List<MerchantGetDto>>(CacheKeyAdmin);
                if (cached != null) return cached.AsQueryable();

                var data = _merchantRepository.GetAllInclude(null, null, y => y.MerchantStatus, y => y.Payments).ProjectTo<MerchantGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate);

                var list = data.ToList();
                _cacheService.Set(CacheKeyAdmin, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MerchantManager.GetAllIncludingForAdmin — local DB failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<Merchant>(
                        null,
                        y => y.MerchantStatus,
                        y => y.Payments).GetAwaiter().GetResult();

                    return _mapper.Map<List<MerchantGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "MerchantManager.GetAllIncludingForAdmin — Azure fallback failed");
                    return Enumerable.Empty<MerchantGetDto>().AsQueryable();
                }
            }
        }

        public async Task<IEnumerable<MerchantGetDto>> GetByIdAsync(int? id)
        {
            if (id == null) return Enumerable.Empty<MerchantGetDto>();
            var cacheKey = $"{CacheKeyItemPrefix}{id}";
            try
            {
                var cached = _cacheService.Get<List<MerchantGetDto>>(cacheKey);
                if (cached != null) return cached;

                var entity = await _merchantRepository.GetIncludeAsync(
                    i => i.Id == id,
                    y => y.MerchantStatus,
                    y => y.Payments);

                if (entity == null) return Enumerable.Empty<MerchantGetDto>();

                var dto = _mapper.Map<MerchantGetDto>(entity);
                var result = new List<MerchantGetDto> { dto };
                _cacheService.Set(cacheKey, result, CacheExpiry);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MerchantManager.GetByIdAsync({Id})", id);
                try
                {
                    var azureData = await _azureService.GetFromAzureWithIncludesAsync<Merchant>(
                        i => i.Id == id,
                        y => y.MerchantStatus,
                        y => y.Payments);

                    return azureData != null
                        ? new List<MerchantGetDto> { _mapper.Map<MerchantGetDto>(azureData) }
                        : Enumerable.Empty<MerchantGetDto>();
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "MerchantManager.GetByIdAsync — Azure fallback failed");
                    return Enumerable.Empty<MerchantGetDto>();
                }
            }
        }

        public async Task<MerchantUpdateDto> GetByIdForUpdateAsync(int id)
        {
            var cacheKey = $"{CacheKeyEditPrefix}{id}";
            try
            {
                var cached = _cacheService.Get<MerchantUpdateDto>(cacheKey);
                if (cached != null) return cached;

                var entity = await _merchantRepository.GetAsync(i => i.Id == id);
                if (entity == null) return null!;

                var dto = _mapper.Map<MerchantUpdateDto>(entity);
                _cacheService.Set(cacheKey, dto, CacheExpiry);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MerchantManager.GetByIdForUpdateAsync({Id})", id);
                try
                {
                    var azureData = await _azureService.GetFromAzureAsync<Merchant>(id);
                    return azureData != null ? _mapper.Map<MerchantUpdateDto>(azureData) : null!;
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "MerchantManager.GetByIdForUpdateAsync — Azure fallback failed");
                    return null!;
                }
            }
        }

        public async Task<Result<bool>> CreateAsync(MerchantCreateDto merchantCreateDto)
        {
            try
            {
                var safeTaxNumber = _htmlSanitizer.Sanitize(merchantCreateDto.TaxNumber ?? string.Empty);

                var entity = _mapper.Map<Merchant>(merchantCreateDto);
                entity.TaxNumber = safeTaxNumber;
                entity.ApiKey = Guid.NewGuid();
                entity.IsActive = true;
                entity.IsDeleted = false;
                entity.CreatedDate = DateTime.UtcNow;

                var result = await _merchantRepository.AddAsync(entity);
                if (result)
                {
                    InvalidateMerchantCaches();
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.AddError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MerchantManager.CreateAsync failed");
                return Result<bool>.Failure("Merchant oluşturulamadı: " + ex.Message);
            }
        }

        public async Task<Result<bool>> UpdateAsync(MerchantUpdateDto merchantUpdateDto)
        {
            try
            {
                var entity = await _merchantRepository.GetAsync(x => x.Id == merchantUpdateDto.Id);
                if (entity == null) return Result<bool>.Failure(MessageConstants.NotFound);

                var safeTaxNumber = _htmlSanitizer.Sanitize(merchantUpdateDto.TaxNumber ?? string.Empty);

                _mapper.Map(merchantUpdateDto, entity);
                entity.TaxNumber = safeTaxNumber;
                entity.UpdatedDate = DateTime.UtcNow;

                var result = await _merchantRepository.UpdateAsync(entity);
                if (result)
                {
                    InvalidateMerchantCaches(merchantUpdateDto.Id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.UpdateError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MerchantManager.UpdateAsync failed");
                return Result<bool>.Failure("Merchant güncellenemedi: " + ex.Message);
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _merchantRepository.GetAsync(x => x.Id == id);
                if (entity == null) return Result<bool>.Failure(MessageConstants.NotFound);
               
                var result=await _merchantRepository.DeleteAsync(entity);
                if (result)
                {
                    InvalidateMerchantCaches(id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.DeleteError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MerchantManager.DeleteAsync({Id}) failed", id);
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
                    var entity = await _merchantRepository.GetAsync(x => x.Id == id);
                    if (entity == null)
                    {
                        notFound.Add(id);
                        continue;
                    }

                    await _merchantRepository.DeleteAsync(entity);
                    InvalidateMerchantCaches(id);
                }

                if (notFound.Count == ids.Count)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MerchantManager.DeleteByIdAsync failed");
                return Result<bool>.Failure("Toplu silme başarısız: " + ex.Message);
            }
        }

        public async Task<Result<bool>> SetActiveAsync(int id)
        {
            try
            {
                var entity = await _merchantRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = true;
                entity.SuspendedDate = null;

                var result = await _merchantRepository.SetActiveAsync(entity);
                if (result)
                {
                    InvalidateMerchantCaches(id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsActiveError);
            }
            catch (Exception ex) { _logger.LogError(ex, "MerchantManager.SetActiveAsync({Id})", id); return Result<bool>.Failure(ex.Message); }
        }

        public async Task<Result<bool>> SetInActiveAsync(int id)
        {
            try
            {

                var entity = await _merchantRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = false;
                entity.SuspendedDate = DateTime.UtcNow;

                var result = await _merchantRepository.SetDeActiveAsync(entity);

                if (result)
                {
                    InvalidateMerchantCaches(id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsActiveError);
            }
            catch (Exception ex) { _logger.LogError(ex, "MerchantManager.SetInActiveAsync({Id})", id); return Result<bool>.Failure(ex.Message); }
        }

        public async Task<Result<bool>> SetDeletedAsync(int id)
        {
            try
            {
                var entity = await _merchantRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = true;
                entity.DeletedDate = DateTime.UtcNow;

                var result = await _merchantRepository.SetDeletedAsync(entity);
                if (result == true)
                {
                    InvalidateMerchantCaches(id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsDeletedError);
            }
            catch (Exception ex) { _logger.LogError(ex, "MerchantManager.SetDeletedAsync({Id})", id); return Result<bool>.Failure(ex.Message); }
        }

        public async Task<Result<bool>> SetNotDeletedAsync(int id)
        {
            try
            {
                var entity = await _merchantRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = false;
                entity.DeletedDate = null;

                var result = await _merchantRepository.SetNotDeletedAsync(entity);
                if (result == true)
                {
                    InvalidateMerchantCaches(id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.NotDeleteError);
            }
            catch (Exception ex) { _logger.LogError(ex, "MerchantManager.SetNotDeletedAsync({Id})", id); return Result<bool>.Failure(ex.Message); }
        }

        private void InvalidateMerchantCaches(int? id = null)
        {
            _cacheService.Remove(CacheKeyAll);
            _cacheService.Remove(CacheKeyAdmin);
            _cacheService.Remove(CacheKeyNullTask);
            _cacheService.Remove(CacheKeyByPayment);
            if (id.HasValue)
            {
                _cacheService.Remove($"{CacheKeyItemPrefix}{id}");
                _cacheService.Remove($"{CacheKeyEditPrefix}{id}");
            }
        }
    }
}
