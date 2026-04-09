using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaymentSystem.Application.Constants.Services.Abstract;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Infrastructure.Data.Context.Azure;
using PaymentSystem.Infrastructure.Repositories.Abstract;
using PaymentSystem.Shared.Dtos.AuthDtos;
using PaymentSystem.Shared.ViewModels;

namespace PaymentSystem.Infrastructure.Services.Concrete
{
    public class AuthManager : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IMailService _mailService;
        private readonly ITokenService _tokenService;
        private readonly ICacheService _cacheService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly AzureDbContext _azureDbContext;
        private readonly IOutboxEventRepository _outboxEventRepository;
        private readonly ILogger<AuthManager> _logger;
        private const string ConfirmCodeCachePrefix = "auth:confirmcode:";
        private static readonly TimeSpan ConfirmCodeExpiry = TimeSpan.FromMinutes(10);

        private static int GenerateSecureConfirmCode()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            var number = Math.Abs(BitConverter.ToInt32(bytes, 0));
            return 100000 + (number % 900000);
        }

        public AuthManager(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<AppRole> roleManager,
            IUserSessionRepository userSessionRepository,
            IMailService mailService,
            ITokenService tokenService,
            ICacheService cacheService,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            AzureDbContext azureDbContext,
            IOutboxEventRepository outboxEventRepository,
            ILogger<AuthManager> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userSessionRepository = userSessionRepository;
            _mailService = mailService;
            _tokenService = tokenService;
            _cacheService = cacheService;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _azureDbContext = azureDbContext;
            _outboxEventRepository = outboxEventRepository;
            _logger = logger;
        }
        private string GetUserIdFromClaims()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated. UserId claim not found.");
            }
            return userId;
        }

        private double GetJwtExpiryMinutes()
        {
            var configValue = _configuration.GetSection("JwtSettings:ExpiryMinutes").Value;
            if (double.TryParse(configValue, out var expiryMinutes))
            {
                return expiryMinutes;
            }
            _logger.LogWarning("Invalid JwtSettings:ExpiryMinutes configuration value '{Value}', using default 60", configValue);
            return 60;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto login)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(login.Email);
                if (user == null || !user.IsActive || user.IsDeleted)
                {
                    _logger.LogWarning("Login attempt failed: User not found or inactive. Email: {Email}", login.Email);
                    return null;
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, login.Password, lockoutOnFailure: true);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Login attempt failed: Invalid password. Email: {Email}", login.Email);
                    return null;
                }

                if (user.IsConfirmCodeActive)
                {
                    var confirmCode = GenerateSecureConfirmCode();
                    var cacheKey = $"{ConfirmCodeCachePrefix}{user.Email}";

                    _cacheService.Set(cacheKey, confirmCode.ToString(), ConfirmCodeExpiry);
                    await _mailService.SendLoginConfirmCodeAsync(user.Email!, confirmCode.ToString());

                    return new AuthResponseDto
                    {
                        Token = "confirm_required",
                        UserId = user.Id,
                        Email = user.Email ?? string.Empty,
                        NameSurname = user.NameSurname
                    };
                }
                var token = await _tokenService.CreateToken(user);
                var userSession = new UserSession
                {
                    UserId = user.Id,
                    Username = user.UserName ?? user.Email ?? string.Empty,
                    LoginDate = DateTime.UtcNow,
                    IsOnline = true,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedDate = DateTime.UtcNow
                };
                await _userSessionRepository.AddAsync(userSession);

                return new AuthResponseDto
                {
                    Token = token,
                    UserId = user.Id,
                    Email = user.Email ?? string.Empty,
                    NameSurname = user.NameSurname,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(GetJwtExpiryMinutes())
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LoginAsync failed for email: {Email}", login.Email);
                throw;
            }
        }

        public async Task<AuthResponseDto?> LoginWithConfirmCodeAsync(LoginDto login)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(login.Email);
                if (user == null || !user.IsActive || user.IsDeleted)
                {
                    _logger.LogWarning("LoginWithConfirmCode attempt failed: User not found or inactive. Email: {Email}", login.Email);
                    return null;
                }

                var signInResult = await _signInManager.CheckPasswordSignInAsync(user, login.Password, lockoutOnFailure: true);
                if (!signInResult.Succeeded)
                {
                    _logger.LogWarning("LoginWithConfirmCode attempt failed: Invalid password. Email: {Email}", login.Email);
                    return null;
                }

                if (user.IsConfirmCodeActive)
                {
                    var confirmCode = GenerateSecureConfirmCode();
                    var cacheKey = $"{ConfirmCodeCachePrefix}{user.Email}";
                    _cacheService.Set(cacheKey, confirmCode.ToString(), ConfirmCodeExpiry);
                    await _mailService.SendLoginConfirmCodeAsync(user.Email!, confirmCode.ToString());

                    return new AuthResponseDto
                    {
                        Token = "confirm_required",
                        UserId = user.Id,
                        Email = user.Email ?? string.Empty,
                        NameSurname = user.NameSurname
                    };
                }

                var token = await _tokenService.CreateToken(user);
                await _userSessionRepository.AddAsync(new UserSession
                {
                    UserId = user.Id,
                    Username = user.UserName ?? user.Email ?? string.Empty,
                    LoginDate = DateTime.UtcNow,
                    IsOnline = true,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedDate = DateTime.UtcNow
                });

                return new AuthResponseDto
                {
                    Token = token,
                    UserId = user.Id,
                    Email = user.Email ?? string.Empty,
                    NameSurname = user.NameSurname,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(GetJwtExpiryMinutes())
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LoginWithConfirmCodeAsync failed for email: {Email}", login.Email);
                throw;
            }
        }

        public async Task<AuthResponseDto?> VerifyLoginConfirmCodeAsync(LoginConfirmCodeDto model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Email))
                {
                    throw new ArgumentNullException(nameof(model.Email), "Email cannot be null");
                }

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    throw new KeyNotFoundException($"User not found with email: {model.Email}");
                }

                var cacheKey = $"{ConfirmCodeCachePrefix}{model.Email}";
                var storedCode = _cacheService.Get<string>(cacheKey);
                if (storedCode == null)
                {
                    throw new InvalidOperationException("Confirm code not found in Redis cache. Please request a new code.");
                }

                if (storedCode != model.LoginConfirmCode.ToString())
                {
                    throw new InvalidOperationException("Confirm codes do not match.");
                }
                _cacheService.Remove(cacheKey);
                var token = await _tokenService.CreateToken(user);
                await _userSessionRepository.AddAsync(new UserSession
                {
                    UserId = user.Id,
                    Username = user.UserName ?? user.Email ?? string.Empty,
                    LoginDate = DateTime.UtcNow,
                    IsOnline = true,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedDate = DateTime.UtcNow
                });

                return new AuthResponseDto
                {
                    Token = token,
                    UserId = user.Id,
                    Email = user.Email ?? string.Empty,
                    NameSurname = user.NameSurname,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(GetJwtExpiryMinutes())
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "VerifyLoginConfirmCodeAsync failed for email: {Email}", model.Email);
                throw;
            }
        }

        public async Task<AuthResponseDto?> GoogleLoginAsync(GoogleLoginDto dto)
        {
            try
            {
                var existingUser = await _userManager.FindByLoginAsync("Google", dto.ProviderKey);
                if (existingUser == null)
                {
                    existingUser = await _userManager.FindByEmailAsync(dto.Email);
                    if (existingUser == null)
                    {
                        var newUser = new AppUser
                        {
                            UserName = dto.Email,
                            Email = dto.Email,
                            NameSurname = dto.NameSurname ?? dto.Email.Split('@')[0],
                            PhoneNumber = string.IsNullOrEmpty(dto.PhoneNumber) ? "-" : dto.PhoneNumber,
                            Title = dto.Title ?? "Google",
                            Birthdate = DateTime.UtcNow,
                            EmailConfirmed = true,
                            PhoneNumberConfirmed = true,
                            IsActive = true,
                            IsDeleted = false,
                            IsConfirmCodeActive = false,
                            CreatedDate = DateTime.UtcNow
                        };

                        var createResult = await _userManager.CreateAsync(newUser);
                        if (!createResult.Succeeded)
                        {
                            _logger.LogWarning("Google login: Failed to create user. Email: {Email}", dto.Email);
                            return null;
                        }

                        var roleResult = await _userManager.AddToRoleAsync(newUser, "User");
                        if (!roleResult.Succeeded)
                        {
                            _logger.LogWarning("Google login: Failed to assign role to user. Email: {Email}", dto.Email);
                            return null;
                        }

                        existingUser = newUser;
                    }

                    var loginInfo = new UserLoginInfo("Google", dto.ProviderKey, "Google");
                    var addResult = await _userManager.AddLoginAsync(existingUser, loginInfo);

                    if (addResult.Succeeded)
                    {
                        var loginPayload = new
                        {
                            LoginProvider = "Google",
                            ProviderKey = dto.ProviderKey,
                            ProviderDisplayName = "Google",
                            UserId = existingUser.Id
                        };

                        var outboxEvent = new OutboxEvent
                        {
                            EntityType = "AspNetUserLogins",
                            EventType = "Added",
                            Payload = JsonConvert.SerializeObject(loginPayload),
                            IsProcessed = false,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedDate = DateTime.UtcNow
                        };
                        await _outboxEventRepository.AddAsync(outboxEvent);

                        var userRole = await _roleManager.FindByNameAsync("User");
                        if (userRole != null)
                        {
                            var rolePayload = new
                            {
                                UserId = existingUser.Id,
                                RoleId = userRole.Id
                            };

                            var roleOutboxEvent = new OutboxEvent
                            {
                                EntityType = "AspNetUserRoles",
                                EventType = "Added",
                                Payload = JsonConvert.SerializeObject(rolePayload),
                                IsProcessed = false,
                                IsActive = true,
                                IsDeleted = false,
                                CreatedDate = DateTime.UtcNow
                            };
                            await _outboxEventRepository.AddAsync(roleOutboxEvent);
                        }
                    }
                }
                else
                {
                    if (existingUser.IsDeleted || !existingUser.IsActive)
                    {
                        _logger.LogWarning("Google login: User is deleted or inactive. Email: {Email}", existingUser.Email);
                        return null;
                    }
                }
                var token = await _tokenService.CreateToken(existingUser);

                var userSession = new UserSession
                {
                    UserId = existingUser.Id,
                    Username = existingUser.UserName ?? existingUser.Email ?? string.Empty,
                    LoginDate = DateTime.UtcNow,
                    IsOnline = true,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedDate = DateTime.UtcNow
                };
                await _userSessionRepository.AddAsync(userSession);

                return new AuthResponseDto
                {
                    Token = token,
                    UserId = existingUser.Id,
                    Email = existingUser.Email ?? string.Empty,
                    NameSurname = existingUser.NameSurname,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(GetJwtExpiryMinutes())
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GoogleLoginAsync failed for email: {Email}", dto.Email);
                throw;
            }
        }

        public async Task<bool> RegisterAsync(RegisterDto model)
        {
            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model), "Model cannot be null");

                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("Registration attempt with existing email: {Email}", model.Email);
                    return false;
                }

                var nameSurnameParts = model.NameSurname.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (nameSurnameParts.Length < 2)
                {
                    throw new InvalidOperationException("Please provide your full name (e.g., Name Surname).");
                }

                var firstName = nameSurnameParts[0];
                var lastName = nameSurnameParts[nameSurnameParts.Length - 1];
                var baseUsername = $"{firstName}{lastName}";
                var username = baseUsername.ToLowerInvariant();

                int suffix = 1;
                while (await _userManager.FindByNameAsync(username) != null)
                {
                    username = $"{baseUsername.ToLowerInvariant()}{suffix}";
                    suffix++;
                }
                var confirmCode = GenerateSecureConfirmCode();

                var user = new AppUser
                {
                    NameSurname = model.NameSurname,
                    UserName = username,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber ?? "-",
                    Title = model.Title ?? "User",
                    Birthdate = model.Birthdate,
                    IsConfirmCodeActive = true,
                    ConfirmCode = confirmCode,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedDate = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Registration failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    return false;
                }

                await _userManager.AddToRoleAsync(user, "Users");
                await _mailService.SendConfirmCodeAsync(model.Email, confirmCode.ToString());
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RegisterAsync failed for email: {Email}", model.Email);
                throw;
            }
        }

        public async Task<bool> ConfirmMailAsync(ConfirmCodeDto model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Email))
                    throw new ArgumentNullException(nameof(model.Email), "Email cannot be null");

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    throw new KeyNotFoundException($"User not found with email: {model.Email}");

                if (user.ConfirmCode.ToString() != model.ConfirmCode)
                    throw new InvalidOperationException("Confirm codes do not match.");

                user.EmailConfirmed = true;
                user.IsConfirmCodeActive = false;
                user.ConfirmCode = null;
                user.UpdatedDate = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ConfirmMailAsync failed for email: {Email}", model.Email);
                throw;
            }
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto model, string userId)
        {
            try
            {
                var authenticatedUserId = GetUserIdFromClaims();
                if (authenticatedUserId != userId)
                {
                    throw new UnauthorizedAccessException("You can only change your own password.");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    throw new KeyNotFoundException($"User not found with ID: {userId}");

                var hasPassword = await _userManager.HasPasswordAsync(user);
                if (!hasPassword)
                    throw new InvalidOperationException("User does not have a password set.");

                if (model.NewPassword != model.ConfirmNewPassword)
                    throw new InvalidOperationException("Passwords do not match.");

                var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    var errors = string.Join(", ", changePasswordResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("Change password failed for user {UserId}: {Errors}", userId, errors);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChangePasswordAsync failed for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto model, string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                    throw new ArgumentNullException(nameof(code), "Reset code cannot be null");

                if (model == null)
                    throw new ArgumentNullException(nameof(model), "Model cannot be null");

                if (model.NewPassword != model.ConfirmNewPassword)
                {
                    throw new InvalidOperationException("Passwords do not match.");
                }

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    throw new KeyNotFoundException($"User not found with email: {model.Email}");
                }

                var result = await _userManager.ResetPasswordAsync(user, code, model.NewPassword);
                if (result.Succeeded)
                {
                    return true;
                }

                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Reset password failed for user {Email}: {Errors}", model.Email, errors);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ResetPasswordAsync failed for email: {Email}", model.Email);
                throw;
            }
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    throw new ArgumentNullException(nameof(email), "Email cannot be null");

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("Forgot password request for non-existing email: {Email}", email);
                    return true;
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var resetLink = $"https://yourfrontend.com/reset-password?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";
                var subject = "PaymentSystem — Password Reset";
                var body = $@"
                    <h2>Password Reset Request</h2>
                    <p>Click the link below to reset your password:</p>
                    <a href='{resetLink}'>Reset Password</a>
                    <p>This link will expire in 1 hour.</p>
                    <p>If you did not request this, please ignore this email.</p>";

                await _mailService.SendConfirmCodeAsync(email, body);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ForgotPasswordAsync failed for email: {Email}", email);
                throw;
            }
        }

        public async Task<UpdateProfileDto> GetDataUpdateProfileAsync(string userId)
        {
            try
            {
                var authenticatedUserId = GetUserIdFromClaims();
                if (authenticatedUserId != userId)
                {
                    throw new UnauthorizedAccessException("You can only view your own profile.");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    throw new KeyNotFoundException($"User not found with ID: {userId}");

                return new UpdateProfileDto
                {
                    NameSurname = user.NameSurname,
                    Title = user.Title,
                    Birthdate = user.Birthdate,
                    PhoneNumber = user.PhoneNumber
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDataUpdateProfileAsync failed for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> UpdateProfileAsync(UpdateProfileDto model, string userId)
        {
            try
            {
                var authenticatedUserId = GetUserIdFromClaims();
                if (authenticatedUserId != userId)
                {
                    throw new UnauthorizedAccessException("You can only update your own profile.");
                }

                if (model == null)
                    throw new ArgumentNullException(nameof(model), "Model cannot be null");

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    throw new KeyNotFoundException($"User not found with ID: {userId}");

                user.NameSurname = model.NameSurname;
                user.Title = model.Title;
                user.Birthdate = model.Birthdate;
                user.PhoneNumber = model.PhoneNumber;
                user.UpdatedDate = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("Update profile failed for user {UserId}: {Errors}", userId, errors);
                }

                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateProfileAsync failed for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<List<RoleAssignVM>> GetRoleAssignAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException(nameof(id), "User ID cannot be null");

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    throw new KeyNotFoundException($"User not found with ID: {id}");

                var allRoles = await _roleManager.Roles.ToListAsync();
                var userRoles = await _userManager.GetRolesAsync(user);

                var assignRoles = allRoles.Select(role => new RoleAssignVM
                {
                    HasAssign = userRoles.Contains(role.Name!),
                    RoleId = role.Id,
                    RoleName = role.Name!
                }).ToList();

                return assignRoles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetRoleAssignAsync failed for user: {UserId}", id);
                throw;
            }
        }

        public async Task<bool> RoleAssignAsync(List<RoleAssignVM> modelList, string id)
        {
            try
            {
                if (modelList == null || !modelList.Any())
                    throw new ArgumentNullException(nameof(modelList), "Role list cannot be empty");

                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException(nameof(id), "User ID cannot be null");

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    throw new KeyNotFoundException($"User not found with ID: {id}");

                var currentUserRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentUserRoles);

                foreach (var role in modelList.Where(r => r.HasAssign))
                {
                    await _userManager.AddToRoleAsync(user, role.RoleName);
                    var existsInAzure = await _azureDbContext.Set<IdentityUserRole<string>>().AnyAsync(r => r.UserId == id && r.RoleId == role.RoleId);

                    if (!existsInAzure)
                    {
                        await _azureDbContext.Set<IdentityUserRole<string>>()
                            .AddAsync(new IdentityUserRole<string> { UserId = id, RoleId = role.RoleId });
                    }
                }
                await _azureDbContext.SaveChangesAsync();
                await _userManager.UpdateSecurityStampAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoleAssignAsync failed for user: {UserId}", id);
                throw;
            }
        }

        public async Task<bool> LogoutAsync(string userId)
        {
            try
            {
                var authenticatedUserId = GetUserIdFromClaims();
                if (authenticatedUserId != userId)
                {
                    throw new UnauthorizedAccessException("You can only logout yourself.");
                }

                if (string.IsNullOrEmpty(userId))
                    throw new ArgumentNullException(nameof(userId), "User ID cannot be null");

                var activeSession = await _userSessionRepository.GetAsync(s => s.UserId == userId && s.IsActive && !s.IsDeleted && s.IsOnline);

                if (activeSession != null)
                {
                    activeSession.LogoutDate = DateTime.UtcNow;
                    var duration = activeSession.LogoutDate.Value - activeSession.LoginDate;
                    activeSession.OnlineDurationSeconds = (int)Math.Round(duration.TotalSeconds);
                    activeSession.IsOnline = false;
                    activeSession.UpdatedDate = DateTime.UtcNow;

                    await _userSessionRepository.UpdateAsync(activeSession);
                }
                await _signInManager.SignOutAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LogoutAsync failed for user: {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> AdminRegisterAsync(AdminRegisterDto model)
        {
            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model), "Model cannot be null");

                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("Admin registration attempt with existing email: {Email}", model.Email);
                    return false;
                }

                if (!string.IsNullOrEmpty(model.UserName))
                {
                    existingUser = await _userManager.FindByNameAsync(model.UserName);
                    if (existingUser != null)
                    {
                        _logger.LogWarning("Admin registration attempt with existing username: {UserName}", model.UserName);
                        return false;
                    }
                }

                var nameSurnameParts = model.NameSurname.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (nameSurnameParts.Length < 2)
                {
                    throw new InvalidOperationException("Please provide your full name (e.g., Name Surname).");
                }

                var firstName = nameSurnameParts[0];
                var lastName = nameSurnameParts[nameSurnameParts.Length - 1];
                var baseUsername = string.IsNullOrEmpty(model.UserName)
                    ? $"{firstName}{lastName}"
                    : model.UserName;
                var username = baseUsername.ToLowerInvariant();

                int suffix = 1;
                while (await _userManager.FindByNameAsync(username) != null)
                {
                    username = $"{baseUsername.ToLowerInvariant()}{suffix}";
                    suffix++;
                }

                var user = new AppUser
                {
                    NameSurname = model.NameSurname,
                    UserName = username,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber ?? "-",
                    Title = model.Title ?? "Admin",
                    Birthdate = model.Birthdate,
                    IsConfirmCodeActive = false,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedDate = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Admin registration failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    return false;
                }

                if (model.SelectedRoles != null && model.SelectedRoles.Any())
                {
                    foreach (var role in model.SelectedRoles)
                    {
                        var roleExists = await _roleManager.RoleExistsAsync(role);
                        if (roleExists)
                        {
                            await _userManager.AddToRoleAsync(user, role);
                        }
                    }
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, "Admins");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AdminRegisterAsync failed for email: {Email}", model.Email);
                throw;
            }
        }

        public async Task<bool> AdminChangePasswordAsync(AdminChangePasswordDto model)
        {
            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model), "Model cannot be null");

                if (string.IsNullOrEmpty(model.TargetUserId))
                    throw new ArgumentNullException(nameof(model.TargetUserId), "Target user ID cannot be null");

                var user = await _userManager.FindByIdAsync(model.TargetUserId);
                if (user == null)
                    throw new KeyNotFoundException($"User not found with ID: {model.TargetUserId}");

                var hasPassword = await _userManager.HasPasswordAsync(user);
                if (!hasPassword)
                    throw new InvalidOperationException("User does not have a password set.");

                if (model.NewPassword != model.ConfirmNewPassword)
                    throw new InvalidOperationException("Passwords do not match.");

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (!resetResult.Succeeded)
                {
                    var errors = string.Join(", ", resetResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("Admin password reset failed for user {UserId}: {Errors}", model.TargetUserId, errors);
                    return false;
                }

                await _userManager.UpdateSecurityStampAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AdminChangePasswordAsync failed for target user: {TargetUserId}", model.TargetUserId);
                throw;
            }
        }

        public async Task<AdminUpdateProfileDto> AdminGetProfileAsync(string targetUserId)
        {
            try
            {
                if (string.IsNullOrEmpty(targetUserId))
                    throw new ArgumentNullException(nameof(targetUserId), "Target user ID cannot be null");

                var user = await _userManager.FindByIdAsync(targetUserId);
                if (user == null)
                    throw new KeyNotFoundException($"User not found with ID: {targetUserId}");

                return new AdminUpdateProfileDto
                {
                    TargetUserId = user.Id,
                    NameSurname = user.NameSurname,
                    Title = user.Title,
                    Birthdate = user.Birthdate,
                    PhoneNumber = user.PhoneNumber,
                    IsActive = user.IsActive,
                    IsDeleted = user.IsDeleted
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AdminGetProfileAsync failed for target user: {TargetUserId}", targetUserId);
                throw;
            }
        }

        public async Task<bool> AdminUpdateProfileAsync(AdminUpdateProfileDto model)
        {
            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model), "Model cannot be null");

                if (string.IsNullOrEmpty(model.TargetUserId))
                    throw new ArgumentNullException(nameof(model.TargetUserId), "Target user ID cannot be null");

                var user = await _userManager.FindByIdAsync(model.TargetUserId);
                if (user == null)
                    throw new KeyNotFoundException($"User not found with ID: {model.TargetUserId}");

                user.NameSurname = model.NameSurname;
                user.Title = model.Title;
                user.Birthdate = model.Birthdate;
                user.PhoneNumber = model.PhoneNumber;
                user.IsActive = model.IsActive;
                user.IsDeleted = model.IsDeleted;
                user.UpdatedDate = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("Admin update profile failed for user {UserId}: {Errors}", model.TargetUserId, errors);
                }

                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AdminUpdateProfileAsync failed for target user: {TargetUserId}", model.TargetUserId);
                throw;
            }
        }
    }
}
