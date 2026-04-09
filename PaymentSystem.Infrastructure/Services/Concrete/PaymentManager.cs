using System.Linq.Expressions;
using System.Security.Claims;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Ganss.Xss;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PaymentSystem.Application.Constants.Messages;
using PaymentSystem.Application.Constants.Services.Abstract;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Infrastructure.GenericRepository;
using PaymentSystem.Infrastructure.Repositories.Abstract;
using PaymentSystem.Shared.Dtos.MappingDtos.PaymentDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Infrastructure.Services.Concrete
{
    public class PaymentManager : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICacheService _cacheService;
        private readonly IAzureService _azureService;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentManager> _logger;
        readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHtmlSanitizer _htmlSanitizer;

        // Cache key constants
        private const string CacheKeyAll = "payments:all";
        private const string CacheKeyAdmin = "payments:admin";
        private const string CacheKeyUserPrefix = "payments:user:";
        private const string CacheKeyStatusPrefix = "payments:status:";
        private const string CacheKeyCurrencyPrefix = "payments:currency:";
        private const string CacheKeyMerchantPrefix = "payments:merchant:";
        private const string CacheKeyItemPrefix = "payment:";
        private const string CacheKeyEditPrefix = "payment:edit:";
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(30);

        public PaymentManager(IPaymentRepository paymentRepository, ICacheService cacheService, IAzureService azureService, IMapper mapper, ILogger<PaymentManager> logger, IHttpContextAccessor httpContextAccessor, IHtmlSanitizer htmlSanitizer)
        {
            _paymentRepository = paymentRepository;
            _cacheService = cacheService;
            _azureService = azureService;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _htmlSanitizer = htmlSanitizer;
        }

        public IQueryable<PaymentGetDto> GetAllIncluding()
        {
            try
            {
                var cached = _cacheService.Get<List<PaymentGetDto>>(CacheKeyAll);
                if (cached != null) return cached.AsQueryable();

                var data = _paymentRepository.GetAllInclude(
                    new Expression<Func<Payment, bool>>[]
                    {
                        i => i.IsActive == true,
                        i => i.IsDeleted == false
                    }, null,
                    y => y.User,
                    y => y.Merchant,
                    y => y.Currency,
                    y => y.PaymentStatus,
                    y => y.Transactions)
                    .ProjectTo<PaymentGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate);

                var list = data.ToList();
                _cacheService.Set(CacheKeyAll, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception)
            {
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<Payment>(
                        i => i.IsActive == true && i.IsDeleted == false,
                        y => y.User,
                        y => y.Merchant,
                        y => y.Currency,
                        y => y.PaymentStatus,
                        y => y.Transactions).GetAwaiter().GetResult();

                    return _mapper.Map<List<PaymentGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception)
                {
                    return Enumerable.Empty<PaymentGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<PaymentGetDto> GetAllIncludingByUserId(string userId)
        {
            var cacheKey = $"{CacheKeyUserPrefix}{userId}";
            try
            {
                var cached = _cacheService.Get<List<PaymentGetDto>>(cacheKey);
                if (cached != null) return cached.AsQueryable();

                var data = _paymentRepository.GetAllInclude(
                    new Expression<Func<Payment, bool>>[]
                    {
                        i => i.UserId == userId,
                        i => i.IsActive == true,
                        i => i.IsDeleted == false
                    },
                    y => y.User,
                    y => y.Merchant,
                    y => y.Currency,
                    y => y.PaymentStatus,
                    y => y.Transactions)
                    .ProjectTo<PaymentGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate);

                var list = data.ToList();
                _cacheService.Set(cacheKey, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception)
            {
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<Payment>(
                        i => i.UserId == userId && i.IsActive == true && i.IsDeleted == false,
                        y => y.User,
                        y => y.Merchant,
                        y => y.Currency,
                        y => y.PaymentStatus,
                        y => y.Transactions).GetAwaiter().GetResult();

                    return _mapper.Map<List<PaymentGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception)
                {
                    return Enumerable.Empty<PaymentGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<PaymentGetDto> GetAllIncludingByPaymentStatusId(int paymentStatusId)
        {
            var cacheKey = $"{CacheKeyStatusPrefix}{paymentStatusId}";
            try
            {
                var cached = _cacheService.Get<List<PaymentGetDto>>(cacheKey);
                if (cached != null) return cached.AsQueryable();

                var data = _paymentRepository.GetAllInclude(
                    new Expression<Func<Payment, bool>>[]
                    {
                        i => i.PaymentStatusId == paymentStatusId,
                        i => i.IsActive == true,
                        i => i.IsDeleted == false
                    },
                    y => y.User,
                    y => y.Merchant,
                    y => y.Currency,
                    y => y.PaymentStatus,
                    y => y.Transactions)
                    .ProjectTo<PaymentGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate);

                var list = data.ToList();
                _cacheService.Set(cacheKey, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception)
            {
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<Payment>(
                        i => i.PaymentStatusId == paymentStatusId && i.IsActive == true && i.IsDeleted == false,
                        y => y.User,
                        y => y.Merchant,
                        y => y.Currency,
                        y => y.PaymentStatus,
                        y => y.Transactions).GetAwaiter().GetResult();

                    return _mapper.Map<List<PaymentGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception)
                {
                    return Enumerable.Empty<PaymentGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<PaymentGetDto> GetAllIncludingByCurrencyId(int currencyId)
        {
            var cacheKey = $"{CacheKeyCurrencyPrefix}{currencyId}";
            try
            {
                var cached = _cacheService.Get<List<PaymentGetDto>>(cacheKey);
                if (cached != null) return cached.AsQueryable();

                var data = _paymentRepository.GetAllInclude(
                    new Expression<Func<Payment, bool>>[]
                    {
                        i => i.CurrencyId == currencyId,
                        i => i.IsActive == true,
                        i => i.IsDeleted == false
                    },
                    y => y.User,
                    y => y.Merchant,
                    y => y.Currency,
                    y => y.PaymentStatus,
                    y => y.Transactions)
                    .ProjectTo<PaymentGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate);

                var list = data.ToList();
                _cacheService.Set(cacheKey, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception)
            {
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<Payment>(
                        i => i.CurrencyId == currencyId && i.IsActive == true && i.IsDeleted == false,
                        y => y.User,
                        y => y.Merchant,
                        y => y.Currency,
                        y => y.PaymentStatus,
                        y => y.Transactions).GetAwaiter().GetResult();

                    return _mapper.Map<List<PaymentGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception)
                {
                    return Enumerable.Empty<PaymentGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<PaymentGetDto> GetAllIncludingByMerchantId(int merchantId)
        {
            var cacheKey = $"{CacheKeyMerchantPrefix}{merchantId}";
            try
            {
                var cached = _cacheService.Get<List<PaymentGetDto>>(cacheKey);
                if (cached != null) return cached.AsQueryable();

                var data = _paymentRepository.GetAllInclude(
                    new Expression<Func<Payment, bool>>[]
                    {
                        i => i.MerchantId == merchantId,
                        i => i.IsActive == true,
                        i => i.IsDeleted == false
                    },
                    y => y.User,
                    y => y.Merchant,
                    y => y.Currency,
                    y => y.PaymentStatus,
                    y => y.Transactions)
                    .ProjectTo<PaymentGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate);

                var list = data.ToList();
                _cacheService.Set(cacheKey, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception)
            {
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<Payment>(
                        i => i.MerchantId == merchantId && i.IsActive == true && i.IsDeleted == false,
                        y => y.User,
                        y => y.Merchant,
                        y => y.Currency,
                        y => y.PaymentStatus,
                        y => y.Transactions).GetAwaiter().GetResult();

                    return _mapper.Map<List<PaymentGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception)
                {
                    return Enumerable.Empty<PaymentGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<PaymentGetDto> GetAllIncludingForAdmin()
        {
            try
            {
                var cached = _cacheService.Get<List<PaymentGetDto>>(CacheKeyAdmin);
                if (cached != null) return cached.AsQueryable();

                var data = _paymentRepository.GetAllInclude(
                    null, null,
                    y => y.User,
                    y => y.Merchant,
                    y => y.Currency,
                    y => y.PaymentStatus,
                    y => y.Transactions)
                    .ProjectTo<PaymentGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate);

                var list = data.ToList();
                _cacheService.Set(CacheKeyAdmin, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception)
            {
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<Payment>(
                        null,
                        y => y.User,
                        y => y.Merchant,
                        y => y.Currency,
                        y => y.PaymentStatus,
                        y => y.Transactions).GetAwaiter().GetResult();

                    return _mapper.Map<List<PaymentGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception)
                {
                    return Enumerable.Empty<PaymentGetDto>().AsQueryable();
                }
            }
        }

        public async Task<IEnumerable<PaymentGetDto>> GetByIdAsync(int? id)
        {
            if (id == null) return Enumerable.Empty<PaymentGetDto>();
            var cacheKey = $"{CacheKeyItemPrefix}{id}";
            try
            {
                var cached = _cacheService.Get<List<PaymentGetDto>>(cacheKey);
                if (cached != null) return cached;

                var entity = await _paymentRepository.GetIncludeAsync(
                    i => i.Id == id,
                    y => y.User,
                    y => y.Merchant,
                    y => y.Currency,
                    y => y.PaymentStatus,
                    y => y.Transactions);

                if (entity == null) return Enumerable.Empty<PaymentGetDto>();

                var dto = _mapper.Map<PaymentGetDto>(entity);
                var result = new List<PaymentGetDto> { dto };
                _cacheService.Set(cacheKey, result, CacheExpiry);
                return result;
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetFromAzureWithIncludesAsync<Payment>(
                        i => i.Id == id,
                        y => y.User,
                        y => y.Merchant,
                        y => y.Currency,
                        y => y.PaymentStatus,
                        y => y.Transactions);

                    return azureData != null
                        ? new List<PaymentGetDto> { _mapper.Map<PaymentGetDto>(azureData) }
                        : Enumerable.Empty<PaymentGetDto>();
                }
                catch (Exception)
                {
                    return Enumerable.Empty<PaymentGetDto>();
                }
            }
        }

        public async Task<PaymentGetDto> GetByIdForUpdateAsync(int id)
        {
            var cacheKey = $"{CacheKeyEditPrefix}{id}";
            try
            {
                var cached = _cacheService.Get<PaymentGetDto>(cacheKey);
                if (cached != null) return cached;

                var entity = await _paymentRepository.GetAsync(i => i.Id == id);
                if (entity == null) return null!;

                var dto = _mapper.Map<PaymentGetDto>(entity);
                _cacheService.Set(cacheKey, dto, CacheExpiry);
                return dto;
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetFromAzureAsync<Payment>(id);
                    return azureData != null ? _mapper.Map<PaymentGetDto>(azureData) : null!;
                }
                catch (Exception)
                {
                    return null!;
                }
            }
        }

        public async Task<Result<bool>> CreateAsync(PaymentCreateDto paymentCreateDto)
        {
            try
            {
                var payments = await _paymentRepository.GetAllAsync(x => x.IdempotencyKey == paymentCreateDto.IdempotencyKey);
                var exists = payments.Any();
                if (exists)
                    return Result<bool>.Failure("Bu idempotency key ile işlem zaten mevcut.");

                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                    return Result<bool>.Failure("User is not authenticated.");

                var safeIdempotencyKey = _htmlSanitizer.Sanitize(paymentCreateDto.IdempotencyKey);
                var safeDescription = _htmlSanitizer.Sanitize(paymentCreateDto.Description ?? string.Empty);
                var safeMaskedCardNumber = _htmlSanitizer.Sanitize(paymentCreateDto.MaskedCardNumber ?? string.Empty);

                var entity = _mapper.Map<Payment>(paymentCreateDto);
                entity.Amount = paymentCreateDto.Amount;
                entity.IdempotencyKey = safeIdempotencyKey;
                entity.Description = safeDescription;
                entity.MaskedCardNumber = safeMaskedCardNumber;
                entity.UserId = userId;
                entity.MerchantId = paymentCreateDto.MerchantId;
                entity.CurrencyId = paymentCreateDto.CurrencyId;
                entity.PaymentStatusId = paymentCreateDto.PaymentStatusId;
                entity.IsActive = true;
                entity.IsDeleted = false;
                entity.CreatedDate = DateTime.UtcNow;

                await _paymentRepository.AddAsync(entity);
                InvalidatePaymentCaches(userId, paymentCreateDto.MerchantId, paymentCreateDto.CurrencyId, paymentCreateDto.PaymentStatusId);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PaymentManager.CreateAsync failed");
                return Result<bool>.Failure("Ödeme oluşturulamadı: " + ex.Message);
            }
        }

        public async Task<Result<bool>> UpdateAsync(PaymentUpdateDto paymentUpdateDto)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                    return Result<bool>.Failure("User is not authenticated.");

                var entity = await _paymentRepository.GetAsync(x => x.Id == paymentUpdateDto.Id);
                if (entity == null) return Result<bool>.Failure("Ödeme bulunamadı.");

                var safeIdempotencyKey = _htmlSanitizer.Sanitize(paymentUpdateDto.IdempotencyKey);
                var safeDescription = _htmlSanitizer.Sanitize(paymentUpdateDto.Description ?? string.Empty);
                var safeMaskedCardNumber = _htmlSanitizer.Sanitize(paymentUpdateDto.MaskedCardNumber ?? string.Empty);

                _mapper.Map(paymentUpdateDto, entity);
                entity.Amount = paymentUpdateDto.Amount;
                entity.IdempotencyKey = safeIdempotencyKey;
                entity.Description = safeDescription;
                entity.MaskedCardNumber = safeMaskedCardNumber;
                entity.UserId = userId;
                entity.MerchantId = paymentUpdateDto.MerchantId;
                entity.CurrencyId = paymentUpdateDto.CurrencyId;
                entity.PaymentStatusId = paymentUpdateDto.PaymentStatusId;
                entity.UpdatedDate = DateTime.UtcNow;

                await _paymentRepository.UpdateAsync(entity);
                InvalidatePaymentCaches(userId, paymentUpdateDto.MerchantId, paymentUpdateDto.CurrencyId, paymentUpdateDto.PaymentStatusId, paymentUpdateDto.Id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PaymentManager.UpdateAsync({Id}) failed", paymentUpdateDto.Id);
                return Result<bool>.Failure("Ödeme güncellenemedi: " + ex.Message);
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _paymentRepository.GetAsync(x => x.Id == id);
                if (entity == null) return Result<bool>.Failure("Ödeme bulunamadı.");

                await _paymentRepository.DeleteAsync(entity);
                InvalidatePaymentCaches(entity.UserId, entity.MerchantId, entity.CurrencyId, entity.PaymentStatusId, id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PaymentManager.DeleteAsync({Id}) failed", id);
                return Result<bool>.Failure("Ödeme silinemedi: " + ex.Message);
            }
        }

        public async Task<Result<bool>> DeleteByIdAsync(List<int> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    var entity = await _paymentRepository.GetAsync(x => x.Id == id);
                    if (entity != null)
                    {
                        await _paymentRepository.DeleteAsync(entity);
                        InvalidatePaymentCaches(entity.UserId, entity.MerchantId, entity.CurrencyId, entity.PaymentStatusId, id);
                    }
                }
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PaymentManager.DeleteByIdAsync failed");
                return Result<bool>.Failure("Toplu silme başarısız: " + ex.Message);
            }
        }

        public async Task<Result<bool>> SetActiveAsync(int id)
        {

            try
            {
                var entity = await _paymentRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = true;
                entity.SuspendedDate = null;

                var result = await _paymentRepository.SetActiveAsync(entity);
                if (result)
                {
                    InvalidatePaymentCaches(entity.UserId, entity.MerchantId, entity.CurrencyId, entity.PaymentStatusId, id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsActiveError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PaymentManager.SetActiveAsync({Id}) failed", id);
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> SetInActiveAsync(int id)
        {
            try
            {
                var entity = await _paymentRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = false;
                entity.SuspendedDate = DateTime.UtcNow;

                var result = await _paymentRepository.SetDeActiveAsync(entity);

                if (result)
                {
                    InvalidatePaymentCaches(entity.UserId, entity.MerchantId, entity.CurrencyId, entity.PaymentStatusId, id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsActiveError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PaymentManager.SetInActiveAsync({Id}) failed", id);
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> SetDeletedAsync(int id)
        {
            try
            {
                var entity = await _paymentRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = true;
                entity.DeletedDate = DateTime.UtcNow;

                var result = await _paymentRepository.SetDeletedAsync(entity);
                if (result == true)
                {
                    InvalidatePaymentCaches(entity.UserId, entity.MerchantId, entity.CurrencyId, entity.PaymentStatusId, id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsDeletedError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PaymentManager.SetDeletedAsync({Id}) failed", id);
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> SetNotDeletedAsync(int id)
        {
            try
            {
                var entity = await _paymentRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = false;
                entity.DeletedDate = null;

                var result = await _paymentRepository.SetNotDeletedAsync(entity);
                if (result == true)
                {
                    InvalidatePaymentCaches(entity.UserId, entity.MerchantId, entity.CurrencyId, entity.PaymentStatusId, id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.NotDeleteError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PaymentManager.SetNotDeletedAsync({Id}) failed", id);
                return Result<bool>.Failure(ex.Message);
            }
        }

        private void InvalidatePaymentCaches(string userId, int merchantId, int currencyId, int paymentStatusId, int? id = null)
        {
            _cacheService.Remove(CacheKeyAll);
            _cacheService.Remove(CacheKeyAdmin);
            _cacheService.Remove($"{CacheKeyUserPrefix}{userId}");
            _cacheService.Remove($"{CacheKeyMerchantPrefix}{merchantId}");
            _cacheService.Remove($"{CacheKeyCurrencyPrefix}{currencyId}");
            _cacheService.Remove($"{CacheKeyStatusPrefix}{paymentStatusId}");
            if (id.HasValue)
            {
                _cacheService.Remove($"{CacheKeyItemPrefix}{id}");
                _cacheService.Remove($"{CacheKeyEditPrefix}{id}");
            }
        }
    }
}
