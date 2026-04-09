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
using PaymentSystem.Shared.Dtos.MappingDtos.SecuritySettingDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Infrastructure.Services.Concrete
{
    public class SecuritySettingManager : ISecuritySettingService
    {
        private readonly ISecuritySettingRepository _securitySettingRepository;
        private readonly ICacheService _cacheService;
        private readonly IAzureService _azureService;
        private readonly IMapper _mapper;
        private readonly ILogger<SecuritySettingManager> _logger;

        private const string CacheKeyAll = "securitysettings:all";
        private const string CacheKeyAdmin = "securitysettings:admin";
        private const string CacheKeyItemPrefix = "securitysetting:";
        private const string CacheKeyEditPrefix = "securitysetting:edit:";
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(30);

        public SecuritySettingManager(ISecuritySettingRepository securitySettingRepository, ICacheService cacheService, IAzureService azureService, IMapper mapper, ILogger<SecuritySettingManager> logger)
        {
            _securitySettingRepository = securitySettingRepository;
            _cacheService = cacheService;
            _azureService = azureService;
            _mapper = mapper;
            _logger = logger;
        }

        public IQueryable<SecuritySettingGetDto> GetAll()
        {
            try
            {
                var cached = _cacheService.Get<List<SecuritySettingGetDto>>(CacheKeyAll);
                if (cached != null) return cached.AsQueryable();

                var data = _securitySettingRepository.GetAllInclude(
                    new Expression<Func<SecuritySetting, bool>>[] { i => i.IsActive == true, i => i.IsDeleted == false },
                    null)
                    .ProjectTo<SecuritySettingGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate).ToList();

                _cacheService.Set(CacheKeyAll, data, CacheExpiry);
                return data.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SecuritySettingManager.GetAll failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<SecuritySetting>(
                        i => i.IsActive == true && i.IsDeleted == false).GetAwaiter().GetResult();
                    return _mapper.Map<List<SecuritySettingGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch
                {
                    return Enumerable.Empty<SecuritySettingGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<SecuritySettingGetDto> GetAllForAdmin()
        {
            try
            {
                var cached = _cacheService.Get<List<SecuritySettingGetDto>>(CacheKeyAdmin);
                if (cached != null) return cached.AsQueryable();

                var data = _securitySettingRepository.GetAllInclude(null, null)
                    .ProjectTo<SecuritySettingGetDto>(_mapper.ConfigurationProvider)
                    .OrderByDescending(i => i.CreatedDate).ToList();

                _cacheService.Set(CacheKeyAdmin, data, CacheExpiry);
                return data.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SecuritySettingManager.GetAllForAdmin failed");
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<SecuritySetting>(null).GetAwaiter().GetResult();
                    return _mapper.Map<List<SecuritySettingGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch
                {
                    return Enumerable.Empty<SecuritySettingGetDto>().AsQueryable();
                }
            }
        }

        public async Task<SecuritySettingGetDto> GetByIdAsync(int? id)
        {
            if (id == null) return null!;
            var cacheKey = $"{CacheKeyItemPrefix}{id}";
            try
            {
                var cached = _cacheService.Get<SecuritySettingGetDto>(cacheKey);
                if (cached != null) return cached;

                var entity = await _securitySettingRepository.GetAsync(i => i.Id == id);
                if (entity == null) return null!;

                var dto = _mapper.Map<SecuritySettingGetDto>(entity);
                _cacheService.Set(cacheKey, dto, CacheExpiry);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SecuritySettingManager.GetByIdAsync({Id})", id);
                return null!;
            }
        }

        public async Task<SecuritySettingGetDto> GetByIdForUpdateAsync(int id)
        {
            var cacheKey = $"{CacheKeyEditPrefix}{id}";
            try
            {
                var cached = _cacheService.Get<SecuritySettingGetDto>(cacheKey);
                if (cached != null) return cached;

                var entity = await _securitySettingRepository.GetAsync(i => i.Id == id);
                if (entity == null) return null!;

                var dto = _mapper.Map<SecuritySettingGetDto>(entity);
                _cacheService.Set(cacheKey, dto, CacheExpiry);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SecuritySettingManager.GetByIdForUpdateAsync({Id})", id);
                return null!;
            }
        }

        public async Task<Result<bool>> CreateAsync(SecuritySettingCreateDto securitySettingCreateDto)
        {
            try
            {
                if (securitySettingCreateDto == null) return Result<bool>.Failure("Veri boş olamaz.");

                var entity = _mapper.Map<SecuritySetting>(securitySettingCreateDto);

                entity.IsActive = true;
                entity.IsDeleted = false;
                entity.CreatedDate = DateTime.UtcNow;

                var result = await _securitySettingRepository.AddAsync(entity);
                if (result)
                {
                    InvalidateCaches();
                    return Result<bool>.Success(true);
                }
                
                return Result<bool>.Failure("Güvenlik ayarı oluşturulamadı.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SecuritySettingManager.CreateAsync failed");
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> UpdateAsync(SecuritySettingUpdateDto securitySettingUpdateDto)
        {
            try
            {
                var entity = await _securitySettingRepository.GetAsync(x => x.Id == securitySettingUpdateDto.Id);
                if (entity == null) return Result<bool>.Failure("Güvenlik ayarı bulunamadı.");

                _mapper.Map(securitySettingUpdateDto, entity);
                entity.UpdatedDate = DateTime.UtcNow;

                await _securitySettingRepository.UpdateAsync(entity);
                InvalidateCaches(securitySettingUpdateDto.Id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SecuritySettingManager.UpdateAsync failed");
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _securitySettingRepository.GetAsync(x => x.Id == id);
                if (entity == null) return Result<bool>.Failure("Güvenlik ayarı bulunamadı.");
                
                var result = await _securitySettingRepository.DeleteAsync(entity);
                if (result)
                {
                    InvalidateCaches(id);
                    return Result<bool>.Success(true);
                }
                
                return Result<bool>.Failure("Güvenlik ayarı silinemedi.");
            }
            catch (Exception ex) 
            { 
                _logger.LogError(ex, "SecuritySettingManager.DeleteAsync({Id}) failed", id);
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
                    var entity = await _securitySettingRepository.GetAsync(x => x.Id == id);
                    if (entity == null)
                    {
                        notFound.Add(id);
                        continue;
                    }
                    
                    await _securitySettingRepository.DeleteAsync(entity);
                    InvalidateCaches(id);
                }

                if (notFound.Count == ids.Count)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                return Result<bool>.Success(true);
            }
            catch (Exception ex) 
            { 
                return Result<bool>.Failure(ex.Message); 
            }
        }

        public async Task<Result<bool>> SetActiveAsync(int id)
        {
            try
            {
                var entity = await _securitySettingRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = true;
                entity.SuspendedDate = null;

                var result = await _securitySettingRepository.SetActiveAsync(entity);
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
                var entity = await _securitySettingRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = false;
                entity.SuspendedDate = DateTime.UtcNow;

                var result = await _securitySettingRepository.SetDeActiveAsync(entity);

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
                var entity = await _securitySettingRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = true;
                entity.DeletedDate = DateTime.UtcNow;

                var result = await _securitySettingRepository.SetDeletedAsync(entity);
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
                var entity = await _securitySettingRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = false;
                entity.DeletedDate = null;

                var result = await _securitySettingRepository.SetNotDeletedAsync(entity);
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
            if (id.HasValue)
            {
                _cacheService.Remove($"{CacheKeyItemPrefix}{id}");
                _cacheService.Remove($"{CacheKeyEditPrefix}{id}");
            }
        }
    }
}
