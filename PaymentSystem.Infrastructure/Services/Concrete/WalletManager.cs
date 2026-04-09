using System.Linq.Expressions;
using System.Security.Claims;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PaymentSystem.Application.Constants.Messages;
using PaymentSystem.Application.Constants.Services.Abstract;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Infrastructure.GenericRepository;
using PaymentSystem.Infrastructure.Repositories.Abstract;
using PaymentSystem.Shared.Dtos.MappingDtos.WalletDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Infrastructure.Services.Concrete
{
    public class WalletManager : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ICacheService _cacheService;
        private readonly IAzureService _azureService;
        private readonly IMapper _mapper;
        private readonly ILogger<WalletManager> _logger;
        readonly IHttpContextAccessor _httpContextAccessor;

        private const string CacheKeyAll = "wallets:all";
        private const string CacheKeyAdmin = "wallets:admin";
        private const string CacheKeyUserPrefix = "wallets:user:";
        private const string CacheKeyCurrencyPrefix = "wallets:currency:";
        private const string CacheKeyItemPrefix = "wallet:";
        private const string CacheKeyEditPrefix = "wallet:edit:";
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(30);

        public WalletManager(IWalletRepository walletRepository, ICacheService cacheService, IAzureService azureService, IMapper mapper, ILogger<WalletManager> logger, IHttpContextAccessor httpContextAccessor)
        {
            _walletRepository = walletRepository;
            _cacheService = cacheService;
            _azureService = azureService;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public IQueryable<WalletGetDto> GetAllIncluding()
        {
            try
            {
                var cached = _cacheService.Get<List<WalletGetDto>>(CacheKeyAll);
                if (cached != null) return cached.AsQueryable();

                var data = _walletRepository.GetAllInclude(new Expression<Func<Wallet, bool>>[]
                {
                    i => i.IsActive == true,
                    i => i.IsDeleted == false
                }, null, y => y.User, y => y.Currency, y => y.Transactions).ProjectTo<WalletGetDto>(_mapper.ConfigurationProvider).OrderByDescending(i => i.CreatedDate);

                var list = data.ToList();
                _cacheService.Set(CacheKeyAll, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WalletManager.GetAllIncluding — local DB failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<Wallet>(
                        i => i.IsActive == true && i.IsDeleted == false,
                        y => y.User,
                        y => y.Currency,
                        y => y.Transactions).GetAwaiter().GetResult();

                    return _mapper.Map<List<WalletGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "WalletManager.GetAllIncluding — Azure fallback failed");
                    return Enumerable.Empty<WalletGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<WalletGetDto> GetAllIncludingByUserId(string userId)
        {
            var cacheKey = $"{CacheKeyUserPrefix}{userId}";
            try
            {
                var cached = _cacheService.Get<List<WalletGetDto>>(cacheKey);
                if (cached != null) return cached.AsQueryable();

                var data = _walletRepository.GetAllInclude(new Expression<Func<Wallet, bool>>[]
                {
                    i => i.UserId == userId,
                    i => i.IsActive == true,
                    i => i.IsDeleted == false
                }, y => y.User, y => y.Currency, y => y.Transactions).ProjectTo<WalletGetDto>(_mapper.ConfigurationProvider).OrderByDescending(i => i.CreatedDate);

                var list = data.ToList();
                _cacheService.Set(cacheKey, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WalletManager.GetAllIncludingByUserId({UserId})", userId);
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<Wallet>(
                        i => i.UserId == userId && i.IsActive == true && i.IsDeleted == false,
                        y => y.User,
                        y => y.Currency,
                        y => y.Transactions).GetAwaiter().GetResult();

                    return _mapper.Map<List<WalletGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "WalletManager.GetAllIncludingByUserId — Azure fallback failed");
                    return Enumerable.Empty<WalletGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<WalletGetDto> GetAllIncludingByCurrencyId(int currencyId)
        {
            var cacheKey = $"{CacheKeyCurrencyPrefix}{currencyId}";
            try
            {
                var cached = _cacheService.Get<List<WalletGetDto>>(cacheKey);
                if (cached != null) return cached.AsQueryable();

                var data = _walletRepository.GetAllInclude(new Expression<Func<Wallet, bool>>[]
                {
                    i => i.CurrencyId == currencyId,
                    i => i.IsActive == true,
                    i => i.IsDeleted == false
                }, y => y.User, y => y.Currency, y => y.Transactions).ProjectTo<WalletGetDto>(_mapper.ConfigurationProvider).OrderByDescending(i => i.CreatedDate);

                var list = data.ToList();
                _cacheService.Set(cacheKey, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WalletManager.GetAllIncludingByCurrencyId({Id})", currencyId);
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<Wallet>(
                        i => i.CurrencyId == currencyId && i.IsActive == true && i.IsDeleted == false,
                        y => y.User,
                        y => y.Currency,
                        y => y.Transactions).GetAwaiter().GetResult();

                    return _mapper.Map<List<WalletGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "WalletManager.GetAllIncludingByCurrencyId — Azure fallback failed");
                    return Enumerable.Empty<WalletGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<WalletGetDto> GetAllIncludingForAdmin()
        {
            try
            {
                var cached = _cacheService.Get<List<WalletGetDto>>(CacheKeyAdmin);
                if (cached != null) return cached.AsQueryable();

                var data = _walletRepository.GetAllInclude(new Expression<Func<Wallet, bool>>[]
                {

                }, null, y => y.User, y => y.Currency, y => y.Transactions).ProjectTo<WalletGetDto>(_mapper.ConfigurationProvider).OrderByDescending(i => i.CreatedDate);

                var list = data.ToList();
                _cacheService.Set(CacheKeyAdmin, list, CacheExpiry);
                return list.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WalletManager.GetAllIncludingForAdmin — local DB failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<Wallet>(
                        null,
                        y => y.User,
                        y => y.Currency,
                        y => y.Transactions).GetAwaiter().GetResult();

                    return _mapper.Map<List<WalletGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "WalletManager.GetAllIncludingForAdmin — Azure fallback failed");
                    return Enumerable.Empty<WalletGetDto>().AsQueryable();
                }
            }
        }

        public async Task<WalletGetDto> GetByIdAsync(int? id)
        {
            if (id == null) return null!;
            var cacheKey = $"{CacheKeyItemPrefix}{id}";
            try
            {
                var cached = _cacheService.Get<WalletGetDto>(cacheKey);
                if (cached != null) return cached;

                var entity = await _walletRepository.GetIncludeAsync(i => i.Id == id, y => y.User, y => y.Currency, y => y.Transactions);
                if (entity == null) return null!;

                var dto = _mapper.Map<WalletGetDto>(entity);
                _cacheService.Set(cacheKey, dto, CacheExpiry);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WalletManager.GetByIdAsync({Id})", id);
                try
                {
                    var azureData = await _azureService.GetFromAzureWithIncludesAsync<Wallet>(
                        i => i.Id == id,
                        y => y.User,
                        y => y.Currency,
                        y => y.Transactions);

                    return azureData != null ? _mapper.Map<WalletGetDto>(azureData) : null!;
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "WalletManager.GetByIdAsync — Azure fallback failed");
                    return null!;
                }
            }
        }

        public async Task<WalletGetDto> GetByIdForUpdateAsync(int id)
        {
            var cacheKey = $"{CacheKeyEditPrefix}{id}";
            try
            {
                var cached = _cacheService.Get<WalletGetDto>(cacheKey);
                if (cached != null) return cached;

                var entity = await _walletRepository.GetAsync(i => i.Id == id);
                if (entity == null) return null!;

                var dto = _mapper.Map<WalletGetDto>(entity);
                _cacheService.Set(cacheKey, dto, CacheExpiry);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WalletManager.GetByIdForUpdateAsync({Id})", id);
                try
                {
                    var azureData = await _azureService.GetFromAzureAsync<Wallet>(id);
                    return azureData != null ? _mapper.Map<WalletGetDto>(azureData) : null!;
                }
                catch (Exception azEx)
                {
                    _logger.LogError(azEx, "WalletManager.GetByIdForUpdateAsync — Azure fallback failed");
                    return null!;
                }
            }
        }

        public async Task<Result<bool>> CreateAsync(WalletCreateDto walletCreateDto)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                    return Result<bool>.Failure("User is not authenticated.");

                var entity = _mapper.Map<Wallet>(walletCreateDto);
                entity.Balance = walletCreateDto.Balance;
                entity.RowVersion = walletCreateDto.RowVersion;
                entity.UserId = userId;
                entity.CurrencyId = walletCreateDto.CurrencyId;
                entity.IsActive = true;
                entity.IsDeleted = false;
                entity.CreatedDate = DateTime.UtcNow;

                await _walletRepository.AddAsync(entity);
                InvalidateWalletCaches(userId, walletCreateDto.CurrencyId);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WalletManager.CreateAsync failed");
                return Result<bool>.Failure("Cüzdan oluşturulamadı: " + ex.Message);
            }
        }

        public async Task<Result<bool>> UpdateAsync(WalletUpdateDto walletUpdateDto)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                    return Result<bool>.Failure("User is not authenticated.");

                var entity = await _walletRepository.GetAsync(x => x.Id == walletUpdateDto.Id);
                if (entity == null) return Result<bool>.Failure("Cüzdan bulunamadı.");

                if (entity.RowVersion != null && walletUpdateDto.RowVersion != null && !entity.RowVersion.SequenceEqual(walletUpdateDto.RowVersion))
                    return Result<bool>.Failure("Concurrency hatası");

                _mapper.Map(walletUpdateDto, entity);
                entity.Balance = walletUpdateDto.Balance;
                entity.UserId = userId;
                entity.CurrencyId = walletUpdateDto.CurrencyId;
                entity.UpdatedDate = DateTime.UtcNow;

                await _walletRepository.UpdateAsync(entity);
                InvalidateWalletCaches(userId, walletUpdateDto.CurrencyId, walletUpdateDto.Id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WalletManager.UpdateAsync({Id}) failed", walletUpdateDto.Id);
                return Result<bool>.Failure("Cüzdan güncellenemedi: " + ex.Message);
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _walletRepository.GetAsync(x => x.Id == id);
                if (entity == null) return Result<bool>.Failure("Cüzdan bulunamadı.");
                await _walletRepository.DeleteAsync(entity);
                InvalidateWalletCaches(entity.UserId, entity.CurrencyId, id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WalletManager.DeleteAsync({Id}) failed", id);
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> DeleteByIdAsync(List<int> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    var entity = await _walletRepository.GetAsync(x => x.Id == id);
                    if (entity != null)
                    {
                        await _walletRepository.DeleteAsync(entity);
                        InvalidateWalletCaches(entity.UserId, entity.CurrencyId, id);
                    }
                }
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WalletManager.DeleteByIdAsync failed");
                return Result<bool>.Failure("Toplu silme başarısız: " + ex.Message);
            }
        }

        public async Task<Result<bool>> SetActiveAsync(int id)
        {
            try
            {
                var entity = await _walletRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = true;
                entity.SuspendedDate = null;

                var result = await _walletRepository.SetActiveAsync(entity);
                if (result)
                {
                    InvalidateWalletCaches(entity.UserId, entity.CurrencyId, id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsActiveError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WalletManager.SetActiveAsync({Id}) failed", id);
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> SetInActiveAsync(int id)
        {
            try
            {
                var entity = await _walletRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = false;
                entity.SuspendedDate = DateTime.UtcNow;

                var result = await _walletRepository.SetDeActiveAsync(entity);

                if (result)
                {
                    InvalidateWalletCaches(entity.UserId, entity.CurrencyId, id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsActiveError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WalletManager.SetInActiveAsync({Id}) failed", id);
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> SetDeletedAsync(int id)
        {
            try
            {
                var entity = await _walletRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = true;
                entity.DeletedDate = DateTime.UtcNow;

                var result = await _walletRepository.SetDeletedAsync(entity);
                if (result == true)
                {
                    InvalidateWalletCaches(entity.UserId, entity.CurrencyId, id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsDeletedError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WalletManager.SetDeletedAsync({Id}) failed", id);
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> SetNotDeletedAsync(int id)
        {
            try
            {
                var entity = await _walletRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = false;
                entity.DeletedDate = null;

                var result = await _walletRepository.SetNotDeletedAsync(entity);
                if (result == true)
                {
                    InvalidateWalletCaches(entity.UserId, entity.CurrencyId, id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.NotDeleteError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WalletManager.SetNotDeletedAsync({Id}) failed", id);
                return Result<bool>.Failure(ex.Message);
            }
        }

        private void InvalidateWalletCaches(string userId, int currencyId, int? id = null)
        {
            _cacheService.Remove(CacheKeyAll);
            _cacheService.Remove(CacheKeyAdmin);
            _cacheService.Remove($"{CacheKeyUserPrefix}{userId}");
            _cacheService.Remove($"{CacheKeyCurrencyPrefix}{currencyId}");
            if (id.HasValue)
            {
                _cacheService.Remove($"{CacheKeyItemPrefix}{id}");
                _cacheService.Remove($"{CacheKeyEditPrefix}{id}");
            }
        }
    }
}
