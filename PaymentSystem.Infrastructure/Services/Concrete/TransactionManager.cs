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
using PaymentSystem.Shared.Dtos.MappingDtos.TransactionDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Infrastructure.Services.Concrete
{
    public class TransactionManager : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICacheService _cacheService;
        private readonly IAzureService _azureService;
        private readonly IMapper _mapper;
        private readonly ILogger<TransactionManager> _logger;
        private readonly IHtmlSanitizer _htmlSanitizer;

        private const string CacheKeyAll = "transactions:all";
        private const string CacheKeyAdmin = "transactions:admin";
        private const string CacheKeyWalletPrefix = "transactions:wallet:";
        private const string CacheKeyPaymentPrefix = "transactions:payment:";
        private const string CacheKeyCurrencyPrefix = "transactions:currency:";
        private const string CacheKeyTypePrefix = "transactions:type:";
        private const string CacheKeyItemPrefix = "transaction:";
        private const string CacheKeyEditPrefix = "transaction:edit:";
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(30);

        public TransactionManager(ITransactionRepository transactionRepository, ICacheService cacheService, IAzureService azureService, IMapper mapper, ILogger<TransactionManager> logger, IHtmlSanitizer htmlSanitizer)
        {
            _transactionRepository = transactionRepository;
            _cacheService = cacheService;
            _azureService = azureService;
            _mapper = mapper;
            _logger = logger;
            _htmlSanitizer = htmlSanitizer;
        }

        public IQueryable<TransactionGetDto> GetAllIncluding()
        {
            try
            {
                var cached = _cacheService.Get<List<TransactionGetDto>>(CacheKeyAll);
                if (cached != null) return cached.AsQueryable();

                var data = _transactionRepository.GetAllInclude(
                    new Expression<Func<Transaction, bool>>[]
                    {
                i => i.IsActive == true,
                i => i.IsDeleted == false
                    },
                    y => y.Wallet,
                    y => y.Payment,
                    y => y.Currency,
                    y => y.TransactionType)
                    .ProjectTo<TransactionGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate);

                var list = data.ToList();
                _cacheService.Set(CacheKeyAll, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TransactionManager.GetAllIncluding — local DB failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<Transaction>(
                        i => i.IsActive == true && i.IsDeleted == false,
                        y => y.Wallet,
                        y => y.Payment,
                        y => y.Currency,
                        y => y.TransactionType).GetAwaiter().GetResult();

                    return _mapper.Map<List<TransactionGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "TransactionManager.GetAllIncluding — Azure fallback failed");
                    return Enumerable.Empty<TransactionGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<TransactionGetDto> GetAllIncludingByWalletId(int walletId)
        {
            var cacheKey = $"{CacheKeyWalletPrefix}{walletId}";
            try
            {
                var cached = _cacheService.Get<List<TransactionGetDto>>(cacheKey);
                if (cached != null) return cached.AsQueryable();

                var data = _transactionRepository.GetAllInclude(
                    new Expression<Func<Transaction, bool>>[]
                    {
                i => i.WalletId == walletId,
                i => i.IsActive == true,
                i => i.IsDeleted == false
                    },
                    y => y.Wallet,
                    y => y.Payment,
                    y => y.Currency,
                    y => y.TransactionType)
                    .ProjectTo<TransactionGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate);

                var list = data.ToList();
                _cacheService.Set(cacheKey, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TransactionManager.GetAllIncludingByWalletId({Id})", walletId);
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<Transaction>(
                        i => i.WalletId == walletId && i.IsActive == true && i.IsDeleted == false,
                        y => y.Wallet,
                        y => y.Payment,
                        y => y.Currency,
                        y => y.TransactionType).GetAwaiter().GetResult();

                    return _mapper.Map<List<TransactionGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "TransactionManager.GetAllIncludingByWalletId — Azure fallback failed");
                    return Enumerable.Empty<TransactionGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<TransactionGetDto> GetAllIncludingByPaymentId(int? paymentId)
        {
            var cacheKey = $"{CacheKeyPaymentPrefix}{paymentId}";
            try
            {
                var cached = _cacheService.Get<List<TransactionGetDto>>(cacheKey);
                if (cached != null) return cached.AsQueryable();

                var data = _transactionRepository.GetAllInclude(
                    new Expression<Func<Transaction, bool>>[]
                    {
                i => i.PaymentId == paymentId,
                i => i.IsActive == true,
                i => i.IsDeleted == false
                    },
                    y => y.Wallet,
                    y => y.Payment,
                    y => y.Currency,
                    y => y.TransactionType)
                    .ProjectTo<TransactionGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate);

                var list = data.ToList();
                _cacheService.Set(cacheKey, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TransactionManager.GetAllIncludingByPaymentId({Id})", paymentId);
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<Transaction>(
                        i => i.PaymentId == paymentId && i.IsActive == true && i.IsDeleted == false,
                        y => y.Wallet,
                        y => y.Payment,
                        y => y.Currency,
                        y => y.TransactionType).GetAwaiter().GetResult();

                    return _mapper.Map<List<TransactionGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "TransactionManager.GetAllIncludingByPaymentId — Azure fallback failed");
                    return Enumerable.Empty<TransactionGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<TransactionGetDto> GetAllIncludingByCurrencyId(int currencyId)
        {
            var cacheKey = $"{CacheKeyCurrencyPrefix}{currencyId}";
            try
            {
                var cached = _cacheService.Get<List<TransactionGetDto>>(cacheKey);
                if (cached != null) return cached.AsQueryable();

                var data = _transactionRepository.GetAllInclude(
                    new Expression<Func<Transaction, bool>>[]
                    {
                i => i.CurrencyId == currencyId,
                i => i.IsActive == true,
                i => i.IsDeleted == false
                    },
                    y => y.Wallet,
                    y => y.Payment,
                    y => y.Currency,
                    y => y.TransactionType)
                    .ProjectTo<TransactionGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate);

                var list = data.ToList();
                _cacheService.Set(cacheKey, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TransactionManager.GetAllIncludingByCurrencyId({Id})", currencyId);
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<Transaction>(
                        i => i.CurrencyId == currencyId && i.IsActive == true && i.IsDeleted == false,
                        y => y.Wallet,
                        y => y.Payment,
                        y => y.Currency,
                        y => y.TransactionType).GetAwaiter().GetResult();

                    return _mapper.Map<List<TransactionGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "TransactionManager.GetAllIncludingByCurrencyId — Azure fallback failed");
                    return Enumerable.Empty<TransactionGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<TransactionGetDto> GetAllIncludingByTransactionTypeId(int transactionTypeId)
        {
            var cacheKey = $"{CacheKeyTypePrefix}{transactionTypeId}";
            try
            {
                var cached = _cacheService.Get<List<TransactionGetDto>>(cacheKey);
                if (cached != null) return cached.AsQueryable();

                var data = _transactionRepository.GetAllInclude(
                    new Expression<Func<Transaction, bool>>[]
                    {
                i => i.TransactionTypeId == transactionTypeId,
                i => i.IsActive == true,
                i => i.IsDeleted == false
                    },
                    y => y.Wallet,
                    y => y.Payment,
                    y => y.Currency,
                    y => y.TransactionType)
                    .ProjectTo<TransactionGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate);

                var list = data.ToList();
                _cacheService.Set(cacheKey, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TransactionManager.GetAllIncludingByTransactionTypeId({Id})", transactionTypeId);
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<Transaction>(
                        i => i.TransactionTypeId == transactionTypeId && i.IsActive == true && i.IsDeleted == false,
                        y => y.Wallet,
                        y => y.Payment,
                        y => y.Currency,
                        y => y.TransactionType).GetAwaiter().GetResult();

                    return _mapper.Map<List<TransactionGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "TransactionManager.GetAllIncludingByTransactionTypeId — Azure fallback failed");
                    return Enumerable.Empty<TransactionGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<TransactionGetDto> GetAllIncludingForAdmin()
        {
            try
            {
                var cached = _cacheService.Get<List<TransactionGetDto>>(CacheKeyAdmin);
                if (cached != null) return cached.AsQueryable();

                var data = _transactionRepository.GetAllInclude(
                    null, null,
                    y => y.Wallet,
                    y => y.Payment,
                    y => y.Currency,
                    y => y.TransactionType)
                    .ProjectTo<TransactionGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate);

                var list = data.ToList();
                _cacheService.Set(CacheKeyAdmin, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TransactionManager.GetAllIncludingForAdmin — local DB failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<Transaction>(
                        null,
                        y => y.Wallet,
                        y => y.Payment,
                        y => y.Currency,
                        y => y.TransactionType).GetAwaiter().GetResult();

                    return _mapper.Map<List<TransactionGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "TransactionManager.GetAllIncludingForAdmin — Azure fallback failed");
                    return Enumerable.Empty<TransactionGetDto>().AsQueryable();
                }
            }
        }

        public async Task<TransactionGetDto> GetByIdAsync(int? id)
        {
            if (id == null) return null!;
            var cacheKey = $"{CacheKeyItemPrefix}{id}";
            try
            {
                var cached = _cacheService.Get<TransactionGetDto>(cacheKey);
                if (cached != null) return cached;

                var entity = await _transactionRepository.GetIncludeAsync(
                    i => i.Id == id,
                    y => y.Wallet,
                    y => y.Payment,
                    y => y.Currency,
                    y => y.TransactionType);

                if (entity == null) return null!;

                var dto = _mapper.Map<TransactionGetDto>(entity);
                _cacheService.Set(cacheKey, dto, CacheExpiry);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TransactionManager.GetByIdAsync({Id})", id);
                try
                {
                    var azureData = await _azureService.GetFromAzureWithIncludesAsync<Transaction>(
                        i => i.Id == id,
                        y => y.Wallet,
                        y => y.Payment,
                        y => y.Currency,
                        y => y.TransactionType);

                    return azureData != null ? _mapper.Map<TransactionGetDto>(azureData) : null!;
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "TransactionManager.GetByIdAsync — Azure fallback failed");
                    return null!;
                }
            }
        }

        public async Task<TransactionGetDto> GetByIdForUpdate(int id)
        {
            var cacheKey = $"{CacheKeyEditPrefix}{id}";
            try
            {
                var cached = _cacheService.Get<TransactionGetDto>(cacheKey);
                if (cached != null) return cached;

                var entity = await _transactionRepository.GetAsync(i => i.Id == id);
                if (entity == null) return null!;

                var dto = _mapper.Map<TransactionGetDto>(entity);
                _cacheService.Set(cacheKey, dto, CacheExpiry);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TransactionManager.GetByIdForUpdate({Id})", id);
                try
                {
                    var azureData = await _azureService.GetFromAzureAsync<Transaction>(id);
                    return azureData != null ? _mapper.Map<TransactionGetDto>(azureData) : null!;
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "TransactionManager.GetByIdForUpdate — Azure fallback failed");
                    return null!;
                }
            }
        }

        public async Task<Result<bool>> CreateAsync(TransactionCreateDto transactionCreateDto)
        {
            try
            {
                var safeReference = _htmlSanitizer.Sanitize(transactionCreateDto.Reference ?? string.Empty);

                var entity = _mapper.Map<Transaction>(transactionCreateDto);
                entity.Amount = transactionCreateDto.Amount;
                entity.Reference = safeReference;
                entity.WalletId = transactionCreateDto.WalletId;
                entity.PaymentId = transactionCreateDto.PaymentId;
                entity.CurrencyId = transactionCreateDto.CurrencyId;
                entity.TransactionTypeId = transactionCreateDto.TransactionTypeId;
                entity.IsActive = true;
                entity.IsDeleted = false;
                entity.CreatedDate = DateTime.UtcNow;

                await _transactionRepository.AddAsync(entity);
                InvalidateTransactionCaches(transactionCreateDto.WalletId, transactionCreateDto.PaymentId, transactionCreateDto.CurrencyId, transactionCreateDto.TransactionTypeId);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TransactionManager.CreateAsync failed");
                return Result<bool>.Failure("Transaction oluşturulamadı: " + ex.Message);
            }
        }

        public async Task<Result<bool>> UpdateAsync(TransactionUpdateDto transactionUpdateDto)
        {
            try
            {
                var entity = await _transactionRepository.GetAsync(x => x.Id == transactionUpdateDto.Id);
                if (entity == null) return Result<bool>.Failure("Transaction bulunamadı.");

                var safeReference = _htmlSanitizer.Sanitize(transactionUpdateDto.Reference ?? string.Empty);

                _mapper.Map(transactionUpdateDto, entity);
                entity.Amount = transactionUpdateDto.Amount;
                entity.Reference = safeReference;
                entity.WalletId = transactionUpdateDto.WalletId;
                entity.PaymentId = transactionUpdateDto.PaymentId;
                entity.CurrencyId = transactionUpdateDto.CurrencyId;
                entity.TransactionTypeId = transactionUpdateDto.TransactionTypeId;
                entity.UpdatedDate = DateTime.UtcNow;

                await _transactionRepository.UpdateAsync(entity);
                InvalidateTransactionCaches(transactionUpdateDto.WalletId, transactionUpdateDto.PaymentId, transactionUpdateDto.CurrencyId, transactionUpdateDto.TransactionTypeId, transactionUpdateDto.Id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TransactionManager.UpdateAsync({Id}) failed", transactionUpdateDto.Id);
                return Result<bool>.Failure("Transaction güncellenemedi: " + ex.Message);
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _transactionRepository.GetAsync(x => x.Id == id);
                if (entity == null) return Result<bool>.Failure("Transaction bulunamadı.");
                await _transactionRepository.DeleteAsync(entity);
                InvalidateTransactionCaches(entity.WalletId, entity.PaymentId, entity.CurrencyId, entity.TransactionTypeId, id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TransactionManager.DeleteAsync({Id}) failed", id);
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> DeleteByIdAsync(List<int> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    var entity = await _transactionRepository.GetAsync(x => x.Id == id);
                    if (entity != null)
                    {
                        await _transactionRepository.DeleteAsync(entity);
                        InvalidateTransactionCaches(entity.WalletId, entity.PaymentId, entity.CurrencyId, entity.TransactionTypeId, id);
                    }
                }
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TransactionManager.DeleteByIdAsync failed");
                return Result<bool>.Failure("Toplu silme başarısız: " + ex.Message);
            }
        }

        public async Task<Result<bool>> SetActiveAsync(int id)
        {
            try
            {
                var entity = await _transactionRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = true;
                entity.SuspendedDate = null;

                var result = await _transactionRepository.SetActiveAsync(entity);
                if (result)
                {
                    InvalidateTransactionCaches(entity.WalletId, entity.PaymentId, entity.CurrencyId, entity.TransactionTypeId, id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsActiveError);
            }
            catch (Exception ex) { _logger.LogError(ex, "TransactionManager.SetActiveAsync({Id})", id); return Result<bool>.Failure(ex.Message); }
        }

        public async Task<Result<bool>> SetInActiveAsync(int id)
        {
            try
            {
                var entity = await _transactionRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = false;
                entity.SuspendedDate = DateTime.UtcNow;

                var result = await _transactionRepository.SetDeActiveAsync(entity);

                if (result)
                {
                    InvalidateTransactionCaches(entity.WalletId, entity.PaymentId, entity.CurrencyId, entity.TransactionTypeId, id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsActiveError);
            }
            catch (Exception ex) { _logger.LogError(ex, "TransactionManager.SetInActiveAsync({Id})", id); return Result<bool>.Failure(ex.Message); }
        }

        public async Task<Result<bool>> SetDeletedAsync(int id)
        {
            try
            {
                var entity = await _transactionRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = true;
                entity.DeletedDate = DateTime.UtcNow;

                var result = await _transactionRepository.SetDeletedAsync(entity);
                if (result == true)
                {
                    InvalidateTransactionCaches(entity.WalletId, entity.PaymentId, entity.CurrencyId, entity.TransactionTypeId, id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsDeletedError);
            }
            catch (Exception ex) { _logger.LogError(ex, "TransactionManager.SetDeletedAsync({Id})", id); return Result<bool>.Failure(ex.Message); }
        }

        public async Task<Result<bool>> SetNotDeletedAsync(int id)
        {
            try
            {
                var entity = await _transactionRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = false;
                entity.DeletedDate = null;

                var result = await _transactionRepository.SetNotDeletedAsync(entity);
                if (result == true)
                {
                    InvalidateTransactionCaches(entity.WalletId, entity.PaymentId, entity.CurrencyId, entity.TransactionTypeId, id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.NotDeleteError);
            }
            catch (Exception ex) { _logger.LogError(ex, "TransactionManager.SetNotDeletedAsync({Id})", id); return Result<bool>.Failure(ex.Message); }
        }

        private void InvalidateTransactionCaches(int? walletId, int? paymentId, int currencyId, int transactionTypeId, int? id = null)
        {
            _cacheService.Remove(CacheKeyAll);
            _cacheService.Remove(CacheKeyAdmin);
            if (walletId.HasValue) _cacheService.Remove($"{CacheKeyWalletPrefix}{walletId}");
            if (paymentId.HasValue) _cacheService.Remove($"{CacheKeyPaymentPrefix}{paymentId}");
            _cacheService.Remove($"{CacheKeyCurrencyPrefix}{currencyId}");
            _cacheService.Remove($"{CacheKeyTypePrefix}{transactionTypeId}");
            if (id.HasValue)
            {
                _cacheService.Remove($"{CacheKeyItemPrefix}{id}");
                _cacheService.Remove($"{CacheKeyEditPrefix}{id}");
            }
        }
    }
}
