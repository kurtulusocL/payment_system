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
using PaymentSystem.Shared.Dtos.MappingDtos.PaymentStatusDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Infrastructure.Services.Concrete
{
    public class PaymentStatusManager:IPaymentStatusService
    {
        private readonly IPaymentStatusRepository _paymentStatusRepository;
        private readonly ICacheService _cacheService;
        private readonly IAzureService _azureService;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentStatusManager> _logger;
        private readonly IHtmlSanitizer _htmlSanitizer;

        private const string CacheKeyAll = "paymentstatuses:all";
        private const string CacheKeyAdmin = "paymentstatuses:admin";
        private const string CacheKeyByPayments = "paymentstatuses:bypayments";
        private const string CacheKeyItemPrefix = "paymentstatus:";
        private const string CacheKeyEditPrefix = "paymentstatus:edit:";
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(30);

        public PaymentStatusManager(IPaymentStatusRepository paymentStatusRepository, ICacheService cacheService, IAzureService azureService, IMapper mapper, ILogger<PaymentStatusManager> logger, IHtmlSanitizer htmlSanitizer)
        {
            _paymentStatusRepository = paymentStatusRepository;
            _cacheService = cacheService;
            _azureService = azureService;
            _mapper = mapper;
            _logger = logger;
            _htmlSanitizer = htmlSanitizer;
        }

        public IQueryable<PaymentStatusGetDto> GetAllIncluding()
        {
            try
            {
                var cached = _cacheService.Get<List<PaymentStatusGetDto>>(CacheKeyAll);
                if (cached != null) return cached.AsQueryable();

                var data = _paymentStatusRepository.GetAllInclude(
                    new Expression<Func<PaymentStatus, bool>>[] { i => i.IsActive == true, i => i.IsDeleted == false },
                    null, y => y.Payments)
                    .ProjectTo<PaymentStatusGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate).ToList();

                _cacheService.Set(CacheKeyAll, data, CacheExpiry);
                return data.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PaymentStatusManager.GetAllIncluding failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<PaymentStatus>(
                        i => i.IsActive == true && i.IsDeleted == false, y => y.Payments).GetAwaiter().GetResult();
                    return _mapper.Map<List<PaymentStatusGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch
                {
                    return Enumerable.Empty<PaymentStatusGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<PaymentStatusGetDto> GetAllIncludingOrderByPayments()
        {
            try
            {
                var cached = _cacheService.Get<List<PaymentStatusGetDto>>(CacheKeyByPayments);
                if (cached != null) return cached.AsQueryable();

                var data = _paymentStatusRepository.GetAllInclude(
                    new Expression<Func<PaymentStatus, bool>>[] { i => i.IsActive == true, i => i.IsDeleted == false },
                    null, y => y.Payments)
                    .ProjectTo<PaymentStatusGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.PaymentCount).ToList();

                _cacheService.Set(CacheKeyByPayments, data, CacheExpiry);
                return data.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PaymentStatusManager.GetAllIncludingOrderByPayments failed");
                return Enumerable.Empty<PaymentStatusGetDto>().AsQueryable();
            }
        }

        public IQueryable<PaymentStatusGetDto> GetAllIncludingForAdmin()
        {
            try
            {
                var cached = _cacheService.Get<List<PaymentStatusGetDto>>(CacheKeyAdmin);
                if (cached != null) return cached.AsQueryable();

                var data = _paymentStatusRepository.GetAllInclude(null, null, y => y.Payments)
                    .ProjectTo<PaymentStatusGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate).ToList();

                _cacheService.Set(CacheKeyAdmin, data, CacheExpiry);
                return data.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PaymentStatusManager.GetAllIncludingForAdmin failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<PaymentStatus>(null, y => y.Payments).GetAwaiter().GetResult();
                    return _mapper.Map<List<PaymentStatusGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch
                {
                    return Enumerable.Empty<PaymentStatusGetDto>().AsQueryable();
                }
            }
        }

        public async Task<PaymentStatusGetDto> GetByIdAsync(int? id)
        {
            if (id == null) return null!;
            var cacheKey = $"{CacheKeyItemPrefix}{id}";
            try
            {
                var cached = _cacheService.Get<PaymentStatusGetDto>(cacheKey);
                if (cached != null) return cached;

                var entity = await _paymentStatusRepository.GetIncludeAsync(i => i.Id == id, y => y.Payments);
                if (entity == null) return null!;

                var dto = _mapper.Map<PaymentStatusGetDto>(entity);
                _cacheService.Set(cacheKey, dto, CacheExpiry);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PaymentStatusManager.GetByIdAsync({Id})", id);
                return null!;
            }
        }

        public async Task<PaymentStatusGetDto> GetByIdForUpdateAsync(int id)
        {
            var cacheKey = $"{CacheKeyEditPrefix}{id}";
            try
            {
                var cached = _cacheService.Get<PaymentStatusGetDto>(cacheKey);
                if (cached != null) return cached;

                var entity = await _paymentStatusRepository.GetAsync(i => i.Id == id);
                if (entity == null) return null!;

                var dto = _mapper.Map<PaymentStatusGetDto>(entity);
                _cacheService.Set(cacheKey, dto, CacheExpiry);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PaymentStatusManager.GetByIdForUpdateAsync({Id})", id);
                return null!;
            }
        }

        public async Task<Result<bool>> CreateAsync(PaymentStatusCreateDto paymentStatusCreateDto)
        {
            try
            {
                if (paymentStatusCreateDto == null) return Result<bool>.Failure("Veri boş olamaz.");

                var safeDescription = _htmlSanitizer.Sanitize(paymentStatusCreateDto.Description ?? string.Empty);

                var entity = _mapper.Map<PaymentStatus>(paymentStatusCreateDto);
                entity.Description = safeDescription;

                entity.IsActive = true;
                entity.IsDeleted = false;
                entity.CreatedDate = DateTime.UtcNow;

                await _paymentStatusRepository.AddAsync(entity);
                InvalidateCaches();
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PaymentStatusManager.CreateAsync failed");
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> UpdateAsync(PaymentStatusUpdateDto paymentStatusUpdateDto)
        {
            try
            {
                var entity = await _paymentStatusRepository.GetAsync(x => x.Id == paymentStatusUpdateDto.Id);
                if (entity == null) return Result<bool>.Failure("Ödeme durumu bulunamadı.");

                var safeDescription = _htmlSanitizer.Sanitize(paymentStatusUpdateDto.Description ?? string.Empty);

                _mapper.Map(paymentStatusUpdateDto, entity);
                entity.Description = safeDescription;
                entity.UpdatedDate = DateTime.UtcNow;

                await _paymentStatusRepository.UpdateAsync(entity);
                InvalidateCaches(paymentStatusUpdateDto.Id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PaymentStatusManager.UpdateAsync failed");
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _paymentStatusRepository.GetAsync(x => x.Id == id);
                if (entity == null) return Result<bool>.Failure("Ödeme durumu bulunamadı.");
                await _paymentStatusRepository.DeleteAsync(entity);
                InvalidateCaches(id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex) { return Result<bool>.Failure(ex.Message); }
        }

        public async Task<Result<bool>> DeleteByIdAsync(List<int> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    var entity = await _paymentStatusRepository.GetAsync(x => x.Id == id);
                    if (entity != null)
                    {
                        await _paymentStatusRepository.DeleteAsync(entity);
                        InvalidateCaches(id);
                    }
                }
                return Result<bool>.Success(true);
            }
            catch (Exception ex) { return Result<bool>.Failure(ex.Message); }
        }

        public async Task<Result<bool>> SetActiveAsync(int id)
        {
            try
            {
                var entity = await _paymentStatusRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = true;
                entity.SuspendedDate = null;

                var result = await _paymentStatusRepository.SetActiveAsync(entity);
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
                var entity = await _paymentStatusRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = false;
                entity.SuspendedDate = DateTime.UtcNow;

                var result = await _paymentStatusRepository.SetDeActiveAsync(entity);

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
                var entity = await _paymentStatusRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = true;
                entity.DeletedDate = DateTime.UtcNow;

                var result = await _paymentStatusRepository.SetDeletedAsync(entity);
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
                var entity = await _paymentStatusRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = false;
                entity.DeletedDate = null;

                var result = await _paymentStatusRepository.SetNotDeletedAsync(entity);
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
            _cacheService.Remove(CacheKeyByPayments);
            if (id.HasValue)
            {
                _cacheService.Remove($"{CacheKeyItemPrefix}{id}");
                _cacheService.Remove($"{CacheKeyEditPrefix}{id}");
            }
        }
    }
}
