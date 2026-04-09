using System.Linq.Expressions;
using AutoMapper;
using PaymentSystem.Application.Constants.Messages;
using PaymentSystem.Application.Constants.Services.Abstract;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Infrastructure.GenericRepository;
using PaymentSystem.Infrastructure.Repositories.Abstract;
using PaymentSystem.Shared.Dtos.MappingDtos.AppUserDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Infrastructure.Services.Concrete
{
    public class UserManager : IUserService
    {
        private readonly IAppUserRepository _userRepository;
        private readonly IAzureService _azureService;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        private const string CacheKeyAll = "users:all";
        private const string CacheKeyActive = "users:active";
        private const string CacheKeyDeActive = "users:deactive";
        private const string CacheKeyDeleted = "users:deleted";
        private const string CacheKeyAdmin = "users:admin";
        private const int CacheMinutes = 30;

        public UserManager(IAppUserRepository userRepository, IAzureService azureService, ICacheService cacheService, IMapper mapper)
        {
            _userRepository = userRepository;
            _azureService = azureService;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        public IQueryable<AppUserGetDto> GetAllIncluding()
        {
            try
            {
                var cached = _cacheService.Get<List<AppUserGetDto>>(CacheKeyAll);
                if (cached != null) return cached.AsQueryable();

                var data = _userRepository.GetAllInclude(
                    new Expression<Func<AppUser, bool>>[]
                    {
                        i => i.IsActive == true,
                        i => i.IsDeleted == false
                    }, null,
                    y => y.Audits,
                    y => y.Payments,
                    y => y.UserSessions,
                    y => y.Wallet);

                var result = _mapper.Map<List<AppUserGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
                _cacheService.Set(CacheKeyAll, result, TimeSpan.FromMinutes(CacheMinutes));
                return result.AsQueryable();
            }
            catch (Exception)
            {
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<AppUser>(
                        i => i.IsActive == true && i.IsDeleted == false,
                        y => y.Audits,
                        y => y.Payments,
                        y => y.UserSessions,
                        y => y.Wallet).GetAwaiter().GetResult();

                    return _mapper.Map<List<AppUserGetDto>>(
                        azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception)
                {
                    return Enumerable.Empty<AppUserGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<AppUserGetDto> GetAllIncludingDeActiveUser()
        {
            try
            {
                var cached = _cacheService.Get<List<AppUserGetDto>>(CacheKeyDeActive);
                if (cached != null) return cached.AsQueryable();

                var data = _userRepository.GetAllInclude(
                    new Expression<Func<AppUser, bool>>[]
                    {
                    i => i.IsActive == false,
                    i => i.IsDeleted == false
                    }, null,
                    y => y.Audits,
                    y => y.Payments,
                    y => y.UserSessions,
                    y => y.Wallet);

                var result = _mapper.Map<List<AppUserGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
                _cacheService.Set(CacheKeyDeActive, result, TimeSpan.FromMinutes(CacheMinutes));
                return result.AsQueryable();
            }
            catch (Exception)
            {
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<AppUser>(
                        i => i.IsActive == false && i.IsDeleted == false,
                        y => y.Audits,
                        y => y.Payments,
                        y => y.UserSessions,
                        y => y.Wallet).GetAwaiter().GetResult();

                    return _mapper.Map<List<AppUserGetDto>>(
                        azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception)
                {
                    return Enumerable.Empty<AppUserGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<AppUserGetDto> GetAllIncludingDeletedUser()
        {
            try
            {
                var cached = _cacheService.Get<List<AppUserGetDto>>(CacheKeyDeleted);
                if (cached != null) return cached.AsQueryable();

                var data = _userRepository.GetAllInclude(
                    new Expression<Func<AppUser, bool>>[]
                    {
                    i => i.IsDeleted == true
                    }, null,
                    y => y.Audits,
                    y => y.Payments,
                    y => y.UserSessions,
                    y => y.Wallet);

                var result = _mapper.Map<List<AppUserGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
                _cacheService.Set(CacheKeyDeleted, result, TimeSpan.FromMinutes(CacheMinutes));
                return result.AsQueryable();
            }
            catch (Exception)
            {
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<AppUser>(
                        i => i.IsDeleted == true,
                        y => y.Audits,
                        y => y.Payments,
                        y => y.UserSessions,
                        y => y.Wallet).GetAwaiter().GetResult();

                    return _mapper.Map<List<AppUserGetDto>>(
                        azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception)
                {
                    return Enumerable.Empty<AppUserGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<AppUserGetDto> GetAllIncludingActiveUser()
        {
            try
            {
                var cached = _cacheService.Get<List<AppUserGetDto>>(CacheKeyActive);
                if (cached != null) return cached.AsQueryable();

                var data = _userRepository.GetAllInclude(
                    new Expression<Func<AppUser, bool>>[]
                    {
                    i => i.IsActive == true,
                    i => i.IsDeleted == false
                    }, null,
                    y => y.Audits,
                    y => y.Payments,
                    y => y.UserSessions,
                    y => y.Wallet);

                var result = _mapper.Map<List<AppUserGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
                _cacheService.Set(CacheKeyActive, result, TimeSpan.FromMinutes(CacheMinutes));
                return result.AsQueryable();
            }
            catch (Exception)
            {
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<AppUser>(
                        i => i.IsActive == true && i.IsDeleted == false,
                        y => y.Audits,
                        y => y.Payments,
                        y => y.UserSessions,
                        y => y.Wallet).GetAwaiter().GetResult();

                    return _mapper.Map<List<AppUserGetDto>>(
                        azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception)
                {
                    return Enumerable.Empty<AppUserGetDto>().AsQueryable();
                }
            }
        }

        public IQueryable<AppUserGetDto> GetAllIncludingForAdmin()
        {
            try
            {
                var cached = _cacheService.Get<List<AppUserGetDto>>(CacheKeyAdmin);
                if (cached != null) return cached.AsQueryable();

                var data = _userRepository.GetAllInclude(null, null,
                    y => y.Audits,
                    y => y.Payments,
                    y => y.UserSessions,
                    y => y.Wallet);

                var result = _mapper.Map<List<AppUserGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
                _cacheService.Set(CacheKeyAdmin, result, TimeSpan.FromMinutes(CacheMinutes));
                return result.AsQueryable();
            }
            catch (Exception)
            {
                try
                {
                    var azureData = _azureService.GetAllFromAzureAsync<AppUser>(null,
                        y => y.Audits,
                        y => y.Payments,
                        y => y.UserSessions,
                        y => y.Wallet).GetAwaiter().GetResult();

                    return _mapper.Map<List<AppUserGetDto>>(
                        azureData.OrderByDescending(i => i.CreatedDate).ToList()).AsQueryable();
                }
                catch (Exception)
                {
                    return Enumerable.Empty<AppUserGetDto>().AsQueryable();
                }
            }
        }

        public async Task<AppUserGetDto> GetByIdAsync(string? id)
        {
            try
            {
                var cacheKey = $"user:{id}";
                var cached = _cacheService.Get<AppUserGetDto>(cacheKey);
                if (cached != null) return cached;

                var data = await _userRepository.GetIncludeAsync(
                    i => i.Id == id,
                    y => y.Audits,
                    y => y.Payments,
                    y => y.UserSessions,
                    y => y.Wallet);

                var result = _mapper.Map<AppUserGetDto>(data);
                if (result != null)
                    _cacheService.Set(cacheKey, result, TimeSpan.FromMinutes(CacheMinutes));

                return result;
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetFromAzureWithIncludesAsync<AppUser>(
                        i => i.Id == id,
                        y => y.Audits,
                        y => y.Payments,
                        y => y.UserSessions,
                        y => y.Wallet);

                    return _mapper.Map<AppUserGetDto>(azureData);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public async Task<Result<bool>> DeleteAsync(string id)
        {
            try
            {
                var data = await _userRepository.GetAsync(i => i.Id == id);
                if (data == null)
                    return Result<bool>.Failure("User not found.");

                await _userRepository.DeleteAsync(data);
                InvalidateUserCaches(id);
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

                await _userRepository.DeleteByIdsAsync(ids.Cast<object>());
                foreach (var id in ids)
                    InvalidateUserCaches(id);

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
                var entity = await _userRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = true;
                entity.SuspendedDate = null;

                var result = await _userRepository.SetActiveAsync(entity);
                if (result)
                {
                    InvalidateUserCaches(id);
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
                var entity = await _userRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsActive = false;
                entity.SuspendedDate = DateTime.UtcNow;

                var result = await _userRepository.SetDeActiveAsync(entity);

                if (result)
                {
                    InvalidateUserCaches(id);
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
                var entity = await _userRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = true;
                entity.DeletedDate = DateTime.UtcNow;

                var result = await _userRepository.SetDeletedAsync(entity);
                if (result == true)
                {
                    InvalidateUserCaches(id);
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
                var entity = await _userRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return Result<bool>.Failure(MessageConstants.NotFound);

                entity.IsDeleted = false;
                entity.DeletedDate = null;

                var result = await _userRepository.SetNotDeletedAsync(entity);
                if (result == true)
                {
                    InvalidateUserCaches(id);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure(MessageConstants.NotDeleteError);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(ex.Message);
            }
        }

        private void InvalidateUserCaches(string id)
        {
            _cacheService.Remove(CacheKeyAll);
            _cacheService.Remove(CacheKeyActive);
            _cacheService.Remove(CacheKeyDeActive);
            _cacheService.Remove(CacheKeyDeleted);
            _cacheService.Remove(CacheKeyAdmin);
            _cacheService.Remove($"user:{id}");
        }
    }
}
