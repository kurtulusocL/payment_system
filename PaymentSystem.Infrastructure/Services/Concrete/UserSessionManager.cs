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
using PaymentSystem.Infrastructure.Repositories.Abstract;
using PaymentSystem.Shared.Dtos.MappingDtos.UserSessionDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Infrastructure.Services.Concrete
{
    public class UserSessionManager : IUserSessionService
    {
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserSessionManager> _logger;

        private const string CacheKeyAll = "usersessions:all";
        private const string CacheKeyAdmin = "usersessions:admin";
        private const string CacheKeyOnline = "usersessions:online";
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(10);

        public UserSessionManager(
            IUserSessionRepository userSessionRepository,
            IUserService userService,
            IHttpContextAccessor httpContextAccessor,
            ICacheService cacheService,
            IMapper mapper,
            ILogger<UserSessionManager> logger)
        {
            _userSessionRepository = userSessionRepository;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _cacheService = cacheService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<bool>> CreateAsync(string username, DateTime loginDate, string userId)
        {
            var httpContextUserId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (httpContextUserId == null)
                return Result<bool>.Failure("User is not authenticated.");

            try
            {
                var entity = new UserSession
                {
                    Username = username,
                    LoginDate = loginDate,
                    UserId = httpContextUserId,
                    IsOnline = true,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false
                };

                await _userSessionRepository.AddAsync(entity);
                InvalidateCaches();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserSessionManager.CreateAsync failed for user: {UserId}", httpContextUserId);
                return Result<bool>.Failure(ex.Message);
            }
        }

        public IQueryable<UserSessionGetDto> GetAllIncluding()
        {
            var cached = _cacheService.Get<List<UserSessionGetDto>>(CacheKeyAll);
            if (cached != null) return cached.AsQueryable();

            var data = _userSessionRepository.GetAllInclude(
                new Expression<Func<UserSession, bool>>[] { i => i.IsActive == true, i => i.IsDeleted == false },
                null, x => x.User)
                .ProjectTo<UserSessionGetDto>(_mapper.ConfigurationProvider)
                .OrderByDescending(i => i.LoginDate).ToList();

            _cacheService.Set(CacheKeyAll, data, CacheExpiry);
            return data.AsQueryable();
        }

        public IQueryable<UserSessionGetDto> GetAllIncludingByOnline()
        {
            var cached = _cacheService.Get<List<UserSessionGetDto>>(CacheKeyOnline);
            if (cached != null) return cached.AsQueryable();

            var data = _userSessionRepository.GetAllInclude(
                new Expression<Func<UserSession, bool>>[] { i => i.IsOnline == true && i.IsDeleted == false },
                null, x => x.User)
                .ProjectTo<UserSessionGetDto>(_mapper.ConfigurationProvider)
                .OrderByDescending(i => i.LoginDate).ToList();

            _cacheService.Set(CacheKeyOnline, data, CacheExpiry);
            return data.AsQueryable();
        }

        public IQueryable<UserSessionGetDto> GetAllIncludingByUserId(string userId)
        {
            return _userSessionRepository.GetAllInclude(
                new Expression<Func<UserSession, bool>>[] { i => i.UserId == userId && i.IsDeleted == false }, x => x.User)
                .ProjectTo<UserSessionGetDto>(_mapper.ConfigurationProvider)
                .OrderByDescending(i => i.LoginDate);
        }

        public IQueryable<UserSessionGetDto> GetAllIncludingByLoginDate()
        {
            return GetAllIncluding().OrderByDescending(x => x.LoginDate);
        }

        public IQueryable<UserSessionGetDto> GetAllIncludingByOnlineDurationTime()
        {
            return GetAllIncluding().OrderByDescending(x => x.OnlineDurationSeconds);
        }

        public IQueryable<UserSessionGetDto> GetAllIncludingForAdmin()
        {
            var cached = _cacheService.Get<List<UserSessionGetDto>>(CacheKeyAdmin);
            if (cached != null) return cached.AsQueryable();

            var data = _userSessionRepository.GetAllInclude(null, null, x => x.User)
                .ProjectTo<UserSessionGetDto>(_mapper.ConfigurationProvider)
                .OrderByDescending(i => i.CreatedDate).ToList();

            _cacheService.Set(CacheKeyAdmin, data, CacheExpiry);
            return data.AsQueryable();
        }

        public async Task<UserSessionGetDto?> GetByIdAsync(int? id)
        {
            if (id == null) return null;
            try
            {
                var entity = await _userSessionRepository.GetIncludeAsync(i => i.Id == id, x => x.User);
                if (entity == null) return null;
                return _mapper.Map<UserSessionGetDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserSessionManager.GetByIdAsync failed for id: {Id}", id);
                return null;
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _userSessionRepository.GetAsync(x => x.Id == id);
                if (entity == null) return Result<bool>.Failure("Oturum bulunamadı.");
                await _userSessionRepository.DeleteAsync(entity);
                InvalidateCaches();
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
                    var entity = await _userSessionRepository.GetAsync(x => x.Id == id);
                    if (entity != null) await _userSessionRepository.DeleteAsync(entity);
                }
                InvalidateCaches();
                return Result<bool>.Success(true);
            }
            catch (Exception ex) { return Result<bool>.Failure(ex.Message); }
        }

        public async Task<Result<bool>> SetActiveAsync(int id)
        {
            var entity = await _userSessionRepository.GetAsync(x => x.Id == id);
            if (entity == null)
                return Result<bool>.Failure(MessageConstants.NotFound);

            entity.IsActive = true;
            entity.SuspendedDate = null;

            var result = await _userSessionRepository.SetActiveAsync(entity);
            if (result)
            {
                InvalidateCaches();
                return Result<bool>.Success(true);
            }
            return Result<bool>.Failure(MessageConstants.IsActiveError);
        }

        public async Task<Result<bool>> SetInActiveAsync(int id)
        {

            var entity = await _userSessionRepository.GetAsync(x => x.Id == id);
            if (entity == null)
                return Result<bool>.Failure(MessageConstants.NotFound);

            entity.IsActive = false;
            entity.SuspendedDate = DateTime.UtcNow;

            var result = await _userSessionRepository.SetDeActiveAsync(entity);

            if (result)
            {
                InvalidateCaches();
                return Result<bool>.Success(true);
            }
            return Result<bool>.Failure(MessageConstants.IsActiveError);
        }

        public async Task<Result<bool>> SetDeletedAsync(int id)
        {
            var entity = await _userSessionRepository.GetAsync(x => x.Id == id);
            if (entity == null)
                return Result<bool>.Failure(MessageConstants.NotFound);

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;

            var result = await _userSessionRepository.SetDeletedAsync(entity);
            if (result == true)
            {
                InvalidateCaches();
                return Result<bool>.Success(true);
            }
            return Result<bool>.Failure(MessageConstants.IsDeletedError);
        }

        public async Task<Result<bool>> SetNotDeletedAsync(int id)
        {
            var entity = await _userSessionRepository.GetAsync(x => x.Id == id);
            if (entity == null)
                return Result<bool>.Failure(MessageConstants.NotFound);

            entity.IsDeleted = false;
            entity.DeletedDate = null;

            var result = await _userSessionRepository.SetNotDeletedAsync(entity);
            if (result == true)
            {
                InvalidateCaches();
                return Result<bool>.Success(true);
            }
            return Result<bool>.Failure(MessageConstants.NotDeleteError);
        }

        private void InvalidateCaches()
        {
            _cacheService.Remove(CacheKeyAll);
            _cacheService.Remove(CacheKeyAdmin);
            _cacheService.Remove(CacheKeyOnline);
        }
    }
}
