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
using PaymentSystem.Shared.Dtos.MappingDtos.TransactionTypeDto;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Infrastructure.Services.Concrete
{
    public class TransactionTypeManager:ITransactionTypeService
    {
        private readonly ITransactionTypeRepository _transactionTypeRepository;
        private readonly ICacheService _cacheService;
        private readonly IAzureService _azureService;
        private readonly IMapper _mapper;
        private readonly ILogger<TransactionTypeManager> _logger;
        private readonly IHtmlSanitizer _htmlSanitizer;

        private const string CacheKeyAll = "transactiontypes:all";
        private const string CacheKeyAdmin = "transactiontypes:admin";
        private const string CacheKeyByTransactions = "transactiontypes:bytransactions";
        private const string CacheKeyItemPrefix = "transactiontype:";
        private const string CacheKeyEditPrefix = "transactiontype:edit:";
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(30);

        public TransactionTypeManager(ITransactionTypeRepository transactionTypeRepository, ICacheService cacheService, IAzureService azureService, IMapper mapper, ILogger<TransactionTypeManager> logger, IHtmlSanitizer htmlSanitizer)
        {
            _transactionTypeRepository = transactionTypeRepository;
            _cacheService = cacheService;
            _azureService = azureService;
            _mapper = mapper;
            _logger = logger;
            _htmlSanitizer = htmlSanitizer;
        }

        public IQueryable<TransactionTypeGetDto> GetAllIncluding()
        {
            try
            {
                var cached = _cacheService.Get<List<TransactionTypeGetDto>>(CacheKeyAll);
                if (cached != null) return cached.AsQueryable();

                var data = _transactionTypeRepository.GetAllInclude(
                    new Expression<Func<TransactionType, bool>>[] { i => i.IsActive == true, i => i.IsDeleted == false },
                    null, y => y.Transactions)
                    .ProjectTo<TransactionTypeGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate).ToList();

                _cacheService.Set(CacheKeyAll, data, CacheExpiry);
                return data.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TransactionTypeManager.GetAllIncluding failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<TransactionType>(
                        i => i.IsActive == true && i.IsDeleted == false, y => y.Transactions).GetAwaiter().GetResult();
                    return _mapper.Map<List<TransactionTypeGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch
                {
                    return Enumerable.Empty<TransactionTypeGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<TransactionTypeGetDto> GetAllIncludingOrderByTransactions()
        {
            try
            {
                var cached = _cacheService.Get<List<TransactionTypeGetDto>>(CacheKeyByTransactions);
                if (cached != null) return cached.AsQueryable();

                var data = _transactionTypeRepository.GetAllInclude(
                    new Expression<Func<TransactionType, bool>>[] { i => i.IsActive == true, i => i.IsDeleted == false },
                    null, y => y.Transactions)
                    .ProjectTo<TransactionTypeGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.TransactionCount).ToList();

                _cacheService.Set(CacheKeyByTransactions, data, CacheExpiry);
                return data.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TransactionTypeManager.GetAllIncludingOrderByTransactions failed");
                return Enumerable.Empty<TransactionTypeGetDto>().AsQueryable();
            }
        }

        public IQueryable<TransactionTypeGetDto> GetAllIncludingForAdmin()
        {
            try
            {
                var cached = _cacheService.Get<List<TransactionTypeGetDto>>(CacheKeyAdmin);
                if (cached != null) return cached.AsQueryable();

                var data = _transactionTypeRepository.GetAllInclude(null, null, y => y.Transactions)
                    .ProjectTo<TransactionTypeGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate).ToList();

                _cacheService.Set(CacheKeyAdmin, data, CacheExpiry);
                return data.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TransactionTypeManager.GetAllIncludingForAdmin failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<TransactionType>(null, y => y.Transactions).GetAwaiter().GetResult();
                    return _mapper.Map<List<TransactionTypeGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch
                {
                    return Enumerable.Empty<TransactionTypeGetDto>().AsQueryable();
                }
            }
        }

        public async Task<TransactionTypeGetDto> GetByIdAsync(int? id)
        {
            if (id == null) return null!;
            var cacheKey = $"{CacheKeyItemPrefix}{id}";
            try
            {
                var cached = _cacheService.Get<TransactionTypeGetDto>(cacheKey);
                if (cached != null) return cached;

                var entity = await _transactionTypeRepository.GetIncludeAsync(i => i.Id == id, y => y.Transactions);
                if (entity == null) return null!;

                var dto = _mapper.Map<TransactionTypeGetDto>(entity);
                _cacheService.Set(cacheKey, dto, CacheExpiry);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TransactionTypeManager.GetByIdAsync({Id})", id);
                return null!;
            }
        }

        public async Task<TransactionTypeGetDto> GetByIdForUpdateAsync(int id)
        {
            var cacheKey = $"{CacheKeyEditPrefix}{id}";
            try
            {
                var cached = _cacheService.Get<TransactionTypeGetDto>(cacheKey);
                if (cached != null) return cached;

                var entity = await _transactionTypeRepository.GetAsync(i => i.Id == id);
                if (entity == null) return null!;

                var dto = _mapper.Map<TransactionTypeGetDto>(entity);
                _cacheService.Set(cacheKey, dto, CacheExpiry);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TransactionTypeManager.GetByIdForUpdateAsync({Id})", id);
                return null!;
            }
        }

        public async Task<Result<bool>> CreateAsync(TransactionTypeCreateDto transactionTypeCreateDto)
        {
            try
            {
                if (transactionTypeCreateDto == null) return Result<bool>.Failure("Veri boş olamaz.");

                var safeDescription = _htmlSanitizer.Sanitize(transactionTypeCreateDto.Description ?? string.Empty);

                var entity = _mapper.Map<TransactionType>(transactionTypeCreateDto);
                entity.Description = safeDescription;

                entity.IsActive = true;
                entity.IsDeleted = false;
                entity.CreatedDate = DateTime.UtcNow;

                await _transactionTypeRepository.AddAsync(entity);
                InvalidateCaches();
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TransactionTypeManager.CreateAsync failed");
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> UpdateAsync(TransactionTypeUpdateDto transactionTypeUpdateDto)
        {
            try
            {
                var entity = await _transactionTypeRepository.GetAsync(x => x.Id == transactionTypeUpdateDto.Id);
                if (entity == null) return Result<bool>.Failure("İşlem tipi bulunamadı.");

                var safeDescription = _htmlSanitizer.Sanitize(transactionTypeUpdateDto.Description ?? string.Empty);

                _mapper.Map(transactionTypeUpdateDto, entity);
                entity.Description = safeDescription;
                entity.UpdatedDate = DateTime.UtcNow;

                await _transactionTypeRepository.UpdateAsync(entity);
                InvalidateCaches(transactionTypeUpdateDto.Id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TransactionTypeManager.UpdateAsync failed");
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _transactionTypeRepository.GetAsync(x => x.Id == id);
                if (entity == null) return Result<bool>.Failure("İşlem tipi bulunamadı.");
                await _transactionTypeRepository.DeleteAsync(entity);
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
                    var entity = await _transactionTypeRepository.GetAsync(x => x.Id == id);
                    if (entity != null)
                    {
                        await _transactionTypeRepository.DeleteAsync(entity);
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
                var entity = await _transactionTypeRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = true;
                entity.SuspendedDate = null;

                var result = await _transactionTypeRepository.SetActiveAsync(entity);
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
                var entity = await _transactionTypeRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = false;
                entity.SuspendedDate = DateTime.UtcNow;

                var result = await _transactionTypeRepository.SetDeActiveAsync(entity);

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
                var entity = await _transactionTypeRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = true;
                entity.DeletedDate = DateTime.UtcNow;

                var result = await _transactionTypeRepository.SetDeletedAsync(entity);
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
                var entity = await _transactionTypeRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = false;
                entity.DeletedDate = null;

                var result = await _transactionTypeRepository.SetNotDeletedAsync(entity);
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
            _cacheService.Remove(CacheKeyByTransactions);
            if (id.HasValue)
            {
                _cacheService.Remove($"{CacheKeyItemPrefix}{id}");
                _cacheService.Remove($"{CacheKeyEditPrefix}{id}");
            }
        }
    }
}
