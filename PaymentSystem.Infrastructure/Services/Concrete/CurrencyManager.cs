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
using PaymentSystem.Shared.Dtos.MappingDtos.CurrencyDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Infrastructure.Services.Concrete
{
    public class CurrencyManager : ICurrencyService
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly ICacheService _cacheService;
        private readonly IAzureService _azureService;
        private readonly IMapper _mapper;
        private readonly ILogger<CurrencyManager> _logger;
        private readonly IHtmlSanitizer _htmlSanitizer;

        private const string CacheKeyAll = "currencies:all";
        private const string CacheKeyAdmin = "currencies:admin";
        private const string CacheKeyByPayment = "currencies:bypayment";
        private const string CacheKeyByWallet = "currencies:bywallet";
        private const string CacheKeyByTransaction = "currencies:bytransaction";
        private const string CacheKeyItemPrefix = "currency:";
        private const string CacheKeyEditPrefix = "currency:edit:";
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(30);

        public CurrencyManager(ICurrencyRepository currencyRepository, ICacheService cacheService, IAzureService azureService, IMapper mapper, ILogger<CurrencyManager> logger, IHtmlSanitizer htmlSanitizer)
        {
            _currencyRepository = currencyRepository;
            _cacheService = cacheService;
            _azureService = azureService;
            _mapper = mapper;
            _logger = logger;
            _htmlSanitizer = htmlSanitizer;
        }

        public IQueryable<CurrencyGetDto> GetAllIncluding()
        {
            try
            {
                var cached = _cacheService.Get<List<CurrencyGetDto>>(CacheKeyAll);
                if (cached != null) return cached.AsQueryable();

                var data = _currencyRepository.GetAllInclude(new Expression<Func<Currency, bool>>[]
                {
                    i => i.IsActive == true,
                    i => i.IsDeleted == false
                }, null, y => y.Payments, y => y.Wallets, y => y.Transactions).ProjectTo<CurrencyGetDto>(_mapper.ConfigurationProvider).OrderByDescending(i => i.CreatedDate).ToList();

                _cacheService.Set(CacheKeyAll, data, CacheExpiry);
                return data.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CurrencyManager.GetAllIncluding failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<Currency>(
                    i => i.IsActive == true &&
                    i.IsDeleted == false, y => y.Payments, y => y.Wallets, y => y.Transactions).GetAwaiter().GetResult();
                    return _mapper.Map<List<CurrencyGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch
                {
                    return Enumerable.Empty<CurrencyGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<CurrencyGetDto> GetAllIncludingOrderByPayment()
        {
            try
            {
                var cached = _cacheService.Get<List<CurrencyGetDto>>(CacheKeyByPayment);
                if (cached != null) return cached.AsQueryable();

                var data = _currencyRepository.GetAllInclude(new Expression<Func<Currency, bool>>[]
                {
                    i => i.IsActive == true,
                    i => i.IsDeleted == false
                }, null, y => y.Payments).ProjectTo<CurrencyGetDto>(_mapper.ConfigurationProvider).OrderByDescending(i => i.PaymentCount).ToList();

                _cacheService.Set(CacheKeyByPayment, data, CacheExpiry);
                return data.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CurrencyManager.GetAllIncludingOrderBypayment failed");
                return Enumerable.Empty<CurrencyGetDto>().AsQueryable();
            }
        }

        public IQueryable<CurrencyGetDto> GetAllIncludingOrderByWallet()
        {
            try
            {
                var cached = _cacheService.Get<List<CurrencyGetDto>>(CacheKeyByWallet);
                if (cached != null) return cached.AsQueryable();

                var data = _currencyRepository.GetAllInclude(new Expression<Func<Currency, bool>>[]
                {
                    i => i.IsActive == true,
                    i => i.IsDeleted == false
                }, null, y => y.Wallets).ProjectTo<CurrencyGetDto>(_mapper.ConfigurationProvider).OrderByDescending(i => i.WalletCount).ToList();

                _cacheService.Set(CacheKeyByWallet, data, CacheExpiry);
                return data.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CurrencyManager.GetAllIncludingOrderByWallet failed");
                return Enumerable.Empty<CurrencyGetDto>().AsQueryable();
            }
        }

        public IQueryable<CurrencyGetDto> GetAllIncludingOrderByTransaction()
        {
            try
            {
                var cached = _cacheService.Get<List<CurrencyGetDto>>(CacheKeyByTransaction);
                if (cached != null) return cached.AsQueryable();

                var data = _currencyRepository.GetAllInclude(new Expression<Func<Currency, bool>>[]
                {
                    i => i.IsActive == true,
                    i => i.IsDeleted == false
                }, null, y => y.Transactions).ProjectTo<CurrencyGetDto>(_mapper.ConfigurationProvider).OrderByDescending(i => i.TransactionCount).ToList();

                _cacheService.Set(CacheKeyByTransaction, data, CacheExpiry);
                return data.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CurrencyManager.GetAllIncludingOrderByTransaction failed");
                return Enumerable.Empty<CurrencyGetDto>().AsQueryable();
            }
        }

        public IQueryable<CurrencyGetDto> GetAllIncludingForAdmin()
        {
            try
            {
                var cached = _cacheService.Get<List<CurrencyGetDto>>(CacheKeyAdmin);
                if (cached != null) return cached.AsQueryable();

                var data = _currencyRepository.GetAllInclude(null, null, y => y.Payments, y => y.Wallets, y => y.Transactions)
                    .ProjectTo<CurrencyGetDto>(_mapper.ConfigurationProvider).OrderByDescending(i => i.CreatedDate).ToList();

                _cacheService.Set(CacheKeyAdmin, data, CacheExpiry);
                return data.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CurrencyManager.GetAllIncludingForAdmin failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<Currency>(null, y => y.Payments, y => y.Wallets, y => y.Transactions).GetAwaiter().GetResult();
                    return _mapper.Map<List<CurrencyGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch
                {
                    return Enumerable.Empty<CurrencyGetDto>().AsQueryable();
                }
            }
        }

        public async Task<CurrencyGetDto> GetByIdAsync(int? id)
        {
            if (id == null) return null!;
            var cacheKey = $"{CacheKeyItemPrefix}{id}";
            try
            {
                var cached = _cacheService.Get<CurrencyGetDto>(cacheKey);
                if (cached != null) return cached;

                var entity = await _currencyRepository.GetIncludeAsync(i => i.Id == id, y => y.Payments, y => y.Wallets, y => y.Transactions);
                if (entity == null) return null!;

                var dto = _mapper.Map<CurrencyGetDto>(entity);
                _cacheService.Set(cacheKey, dto, CacheExpiry);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CurrencyManager.GetByIdAsync({Id})", id);
                return null!;
            }
        }

        public async Task<CurrencyGetDto> GetByIdForUpdate(int id)
        {
            var cacheKey = $"{CacheKeyEditPrefix}{id}";
            try
            {
                var cached = _cacheService.Get<CurrencyGetDto>(cacheKey);
                if (cached != null) return cached;

                var entity = await _currencyRepository.GetAsync(i => i.Id == id);
                if (entity == null) return null!;

                var dto = _mapper.Map<CurrencyGetDto>(entity);
                _cacheService.Set(cacheKey, dto, CacheExpiry);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CurrencyManager.GetByIdForUpdate({Id})", id);
                return null!;
            }
        }

        public async Task<Result<bool>> CreateAsync(CurrencyCreateDto currencyCreateDto)
        {
            try
            {
                if (currencyCreateDto == null) return Result<bool>.Failure(MessageConstants.NotFound);

                var safeDescription = _htmlSanitizer.Sanitize(currencyCreateDto.Description ?? string.Empty);
                var safeSymbol = _htmlSanitizer.Sanitize(currencyCreateDto.Symbol ?? string.Empty);

                var entity = _mapper.Map<Currency>(currencyCreateDto);
                entity.Description = safeDescription;
                entity.Symbol = safeSymbol;

                entity.IsActive = true;
                entity.IsDeleted = false;
                entity.CreatedDate = DateTime.UtcNow;

                var result = await _currencyRepository.AddAsync(entity);
                if (result)
                {
                    InvalidateCurrencyCaches();
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.AddError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CurrencyManager.CreateAsync failed");
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> UpdateAsync(CurrencyUpdateDto currencyUpdateDto)
        {
            try
            {
                var entity = await _currencyRepository.GetAsync(x => x.Id == currencyUpdateDto.Id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                var safeDescription = _htmlSanitizer.Sanitize(currencyUpdateDto.Description ?? string.Empty);
                var safeSymbol = _htmlSanitizer.Sanitize(currencyUpdateDto.Symbol ?? string.Empty);

                _mapper.Map(currencyUpdateDto, entity);
                entity.Description = safeDescription;
                entity.Symbol = safeSymbol;
                entity.UpdatedDate = DateTime.UtcNow;

                var result = await _currencyRepository.UpdateAsync(entity);
                if (result)
                {
                    InvalidateCurrencyCaches(currencyUpdateDto.Id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.UpdateError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CurrencyManager.UpdateAsync failed");
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _currencyRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                var result = await _currencyRepository.DeleteAsync(entity);
                if (result)
                {
                    InvalidateCurrencyCaches(id);
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
                bool allDeleted = true;
                foreach (var id in ids)
                {
                    var entity = await _currencyRepository.GetAsync(x => x.Id == id);
                    if (entity != null)
                    {
                        var result = await _currencyRepository.DeleteAsync(entity);
                        if (result)
                        {
                            InvalidateCurrencyCaches(id);
                        }
                        else
                        {
                            allDeleted = false;
                        }
                    }
                    else
                    {
                        allDeleted = false;
                    }
                }
                return allDeleted ? Result<bool>.Success(true) : Result<bool>.Failure(MessageConstants.DeleteError);
            }
            catch (Exception ex) 
            { 
                _logger.LogError(ex, "CurrencyManager.DeleteByIdAsync failed for ids: {Ids}", string.Join(", ", ids));
                return Result<bool>.Failure(ex.Message); 
            }
        }

        public async Task<Result<bool>> SetActiveAsync(int id)
        {
            try
            {
                var entity = await _currencyRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = true;
                entity.SuspendedDate = null;

                var result = await _currencyRepository.SetActiveAsync(entity);
                if (result)
                {
                    InvalidateCurrencyCaches(id);
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
                var entity = await _currencyRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = false;
                entity.SuspendedDate = DateTime.UtcNow;

                var result = await _currencyRepository.SetDeActiveAsync(entity);

                if (result)
                {
                    InvalidateCurrencyCaches(id);
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
                var entity = await _currencyRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = true;
                entity.DeletedDate = DateTime.UtcNow;

                var result = await _currencyRepository.SetDeletedAsync(entity);
                if (result == true)
                {
                    InvalidateCurrencyCaches(id);
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
                var entity = await _currencyRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = false;
                entity.DeletedDate = null;

                var result = await _currencyRepository.SetNotDeletedAsync(entity);
                if (result == true)
                {
                    InvalidateCurrencyCaches(id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.NotDeleteError);
            }
            catch (Exception ex) { return Result<bool>.Failure(ex.Message); }
        }

        private void InvalidateCurrencyCaches(int? id = null)
        {
            _cacheService.Remove(CacheKeyAll);
            _cacheService.Remove(CacheKeyAdmin);
            _cacheService.Remove(CacheKeyByPayment);
            _cacheService.Remove(CacheKeyByWallet);
            _cacheService.Remove(CacheKeyByTransaction);
            if (id.HasValue)
            {
                _cacheService.Remove($"{CacheKeyItemPrefix}{id}");
                _cacheService.Remove($"{CacheKeyEditPrefix}{id}");
            }
        }
    }
}
