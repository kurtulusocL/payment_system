using System.Linq.Expressions;
using AutoMapper;
using PaymentSystem.Application.Constants.Messages;
using PaymentSystem.Application.Constants.Services.Abstract;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Infrastructure.GenericRepository;
using PaymentSystem.Infrastructure.Repositories.Abstract;
using PaymentSystem.Shared.Dtos.MappingDtos.AppRoleDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Infrastructure.Services.Concrete
{
    public class RoleManager:IRoleService
    {
        private readonly IAppRoleRepository _roleRepository;
        private readonly IAzureService _azureService;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        private const string CacheKeyAll = "roles:all";
        private const string CacheKeyAdmin = "roles:admin";
        private const int CacheMinutes = 30;

        public RoleManager(IAppRoleRepository roleRepository, IAzureService azureService, ICacheService cacheService, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _azureService = azureService;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        public IQueryable<AppRoleGetDto> GetAll()
        {
            try
            {
                var cached = _cacheService.Get<List<AppRoleGetDto>>(CacheKeyAll);
                if (cached != null) return cached.AsQueryable();

                var data = _roleRepository.GetAllInclude(
                    new Expression<Func<AppRole, bool>>[]
                    {
                    i => i.IsActive == true,
                    i => i.IsDeleted == false
                    }, null);

                var result = _mapper.Map<List<AppRoleGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
                _cacheService.Set(CacheKeyAll, result, TimeSpan.FromMinutes(CacheMinutes));
                return result.AsQueryable();
            }
            catch (Exception)
            {
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<AppRole>(
                        i => i.IsActive == true && i.IsDeleted == false)
                        .GetAwaiter().GetResult();

                    return _mapper.Map<List<AppRoleGetDto>>(
                        azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception)
                {
                    return Enumerable.Empty<AppRoleGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<AppRoleGetDto> GetAllForAdmin()
        {
            try
            {
                var cached = _cacheService.Get<List<AppRoleGetDto>>(CacheKeyAdmin);
                if (cached != null) return cached.AsQueryable();

                var data = _roleRepository.GetAllInclude(null, null);

                var result = _mapper.Map<List<AppRoleGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
                _cacheService.Set(CacheKeyAdmin, result, TimeSpan.FromMinutes(CacheMinutes));
                return result.AsQueryable();
            }
            catch (Exception)
            {
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<AppRole>()
                        .GetAwaiter().GetResult();

                    return _mapper.Map<List<AppRoleGetDto>>(
                        azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception)
                {
                    return Enumerable.Empty<AppRoleGetDto>().AsQueryable();
                }
            }
        }

        public async Task<AppRoleGetDto> GetByIdAsync(string? id)
        {
            try
            {
                var cacheKey = $"role:{id}";
                var cached = _cacheService.Get<AppRoleGetDto>(cacheKey);
                if (cached != null) return cached;

                var data = await _roleRepository.GetAsync(i => i.Id == id);
                var result = _mapper.Map<AppRoleGetDto>(data);

                if (result != null)
                    _cacheService.Set(cacheKey, result, TimeSpan.FromMinutes(CacheMinutes));

                return result;
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetFromAzureAsync<AppRole>(id);
                    return _mapper.Map<AppRoleGetDto>(azureData);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public async Task<AppRoleUpdateDto> GetForEditAsync(string id)
        {
            try
            {
                var cacheKey = $"role:edit:{id}";
                var cached = _cacheService.Get<AppRoleUpdateDto>(cacheKey);
                if (cached != null) return cached;

                var data = await _roleRepository.GetAsync(i => i.Id == id);
                var result = _mapper.Map<AppRoleUpdateDto>(data);

                if (result != null)
                    _cacheService.Set(cacheKey, result, TimeSpan.FromMinutes(CacheMinutes));

                return result;
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetFromAzureAsync<AppRole>(id);
                    return _mapper.Map<AppRoleUpdateDto>(azureData);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public async Task<Result<AppRoleCreateDto>> CreateAsync(AppRoleCreateDto dto)
        {
            try
            {
                var entity = _mapper.Map<AppRole>(dto);
                await _roleRepository.AddAsync(entity);
                InvalidateRoleCaches();
                return Result<AppRoleCreateDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return Result<AppRoleCreateDto>.Failure(ex.Message);
            }
        }

        public async Task<Result<AppRoleUpdateDto>> UpdateAsync(AppRoleUpdateDto dto)
        {
            try
            {
                var data = await _roleRepository.GetAsync(i => i.Id == dto.Id);
                if (data == null)
                    return Result<AppRoleUpdateDto>.Failure("Role not found.");

                _mapper.Map(dto, data);
                data.UpdatedDate = DateTime.UtcNow;
                await _roleRepository.UpdateAsync(data);
                InvalidateRoleCaches();
                _cacheService.Remove($"role:{dto.Id}");
                _cacheService.Remove($"role:edit:{dto.Id}");
                return Result<AppRoleUpdateDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return Result<AppRoleUpdateDto>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> DeleteAsync(string id)
        {
            try
            {
                var data = await _roleRepository.GetAsync(i => i.Id == id);
                if (data == null)
                    return Result<bool>.Failure("Role not found.");

                await _roleRepository.DeleteAsync(data);
                InvalidateRoleCaches();
                _cacheService.Remove($"role:{id}");
                _cacheService.Remove($"role:edit:{id}");
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> DeleteByIdAsync(List<string> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return Result<bool>.Failure("Id list was null or empty.");

                await _roleRepository.DeleteByIdsAsync(ids.Cast<object>());
                InvalidateRoleCaches();
                foreach (var id in ids)
                {
                    _cacheService.Remove($"role:{id}");
                    _cacheService.Remove($"role:edit:{id}");
                }
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> SetActiveAsync(string id)
        {
            try
            {
                var entity = await _roleRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = true;
                entity.SuspendedDate = null;

                var result = await _roleRepository.SetActiveAsync(entity);
                if (result)
                {
                    InvalidateRoleCaches();
                    _cacheService.Remove($"role:{id}");
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsActiveError);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> SetInActiveAsync(string id)
        {
            try
            {
                var entity = await _roleRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = false;
                entity.SuspendedDate = DateTime.UtcNow;

                var result = await _roleRepository.SetDeActiveAsync(entity);

                if (result)
                {
                    InvalidateRoleCaches();
                    _cacheService.Remove($"role:{id}");
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsActiveError);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> SetDeletedAsync(string id)
        {
            try
            {
                var entity = await _roleRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = true;
                entity.DeletedDate = DateTime.UtcNow;

                var result = await _roleRepository.SetDeletedAsync(entity);
                if (result == true)
                {
                    InvalidateRoleCaches();
                    _cacheService.Remove($"role:{id}");
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.IsDeletedError);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> SetNotDeletedAsync(string id)
        {
           
            try
            {
                var entity = await _roleRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = false;
                entity.DeletedDate = null;

                var result = await _roleRepository.SetNotDeletedAsync(entity);
                if (result == true)
                {
                    InvalidateRoleCaches();
                    _cacheService.Remove($"role:{id}");
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.NotDeleteError);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(ex.Message);
            }
        }

        private void InvalidateRoleCaches()
        {
            _cacheService.Remove(CacheKeyAll);
            _cacheService.Remove(CacheKeyAdmin);
        }
    }
}
