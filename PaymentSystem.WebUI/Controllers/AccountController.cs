using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentSystem.Shared.Dtos.AuthDtos;

namespace PaymentSystem.WebUI.Controllers
{
    [Authorize(AuthenticationSchemes = "CookieAuth")]
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient GetClient() => _httpClientFactory.CreateClient("PaymentSystemApi");

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginDto { ReturnUrl = returnUrl });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var client = GetClient();
            var response = await client.PostAsJsonAsync("api/accounts/login-with-confirm-code", dto);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(dto);
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            if (result?.ConfirmRequired == true)
            {
                TempData["Email"] = dto.Email;
                return RedirectToAction("VerifyCode");
            }

            await SetAuthCookie(result!.Token!);
            return RedirectToLocal(dto.ReturnUrl);
        }

        [AllowAnonymous]
        public IActionResult GoogleLogin(string? returnUrl = null)
        {
            var redirectUrl = Url.Action("GoogleCallback", "Account", new { returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        public async Task<IActionResult> GoogleCallback(string? returnUrl = null)
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("CookieAuth");
            if (!authenticateResult.Succeeded)
                return RedirectToAction("Login", new { error = "Login with Google failed." });

            var claims = authenticateResult.Principal?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var nameSurname = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var providerKey = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(providerKey))
                return RedirectToAction("Login", new { error = "Information could not be retrieved from Google account." });

            var dto = new GoogleLoginDto
            {
                Email = email,
                NameSurname = nameSurname ?? email,
                ProviderKey = providerKey
            };

            using var client = GetClient();
            var response = await client.PostAsJsonAsync("api/accounts/google-login", dto);
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Login", new { error = "Login with Google failed." });

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var token = json.TryGetProperty("token", out var t) ? t.GetString() : null;
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", new { error = "Token could not be obtained." });

            await SetAuthCookie(token!);
            return RedirectToLocal(returnUrl);
        }

        [AllowAnonymous]
        public IActionResult VerifyCode()
        {
            var email = TempData["Email"]?.ToString();
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");
            ViewBag.Email = email;
            TempData.Keep("Email");
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(LoginConfirmCodeDto dto)
        {
            var client = GetClient();
            var response = await client.PostAsJsonAsync("api/accounts/verify-login-confirm-code", dto);
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Email = dto.Email;
                ModelState.AddModelError("", "Invalid or expired confirmation code.");
                return View(dto);
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            await SetAuthCookie(result!.Token!);
            return RedirectToAction("Index", "UserHome");
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var client = GetClient();
            var response = await client.PostAsJsonAsync("api/accounts/register", dto);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Registration failed. Email may already be in use.");
                return View(dto);
            }
            TempData["Success"] = "Registration successful. Please check your email for confirmation code.";
            return RedirectToAction("ConfirmMail");
        }

        [AllowAnonymous]
        public IActionResult ConfirmMail()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmMail(ConfirmCodeDto dto)
        {
            var client = GetClient();
            var response = await client.PostAsJsonAsync("api/accounts/confirm-mail", dto);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Email verification failed. Invalid code.");
                return View(dto);
            }
            TempData["Success"] = "Email confirmed successfully. You can now login.";
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDto dto)
        {
            var client = GetClient();
            await client.PostAsJsonAsync("api/accounts/forgot-password", dto);
            TempData["Success"] = "If the email exists, a password reset link has been sent.";
            return View(dto);
        }

        [AllowAnonymous]
        public IActionResult ResetPassword(string? code = null)
        {
            ViewBag.Code = code;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto, string? code = null)
        {
            var client = GetClient();
            var response = await client.PostAsJsonAsync($"api/accounts/reset-password?code={code}", dto);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Password reset failed. Token may be invalid or expired.");
                return View(dto);
            }
            TempData["Success"] = "Password reset successful.";
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Profile()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/accounts/get-data-update-profile");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to load profile.";
                return RedirectToAction("Index", "UserHome");
            }
            var dto = await response.Content.ReadFromJsonAsync<UpdateProfileDto>();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            var client = GetClient();
            var response = await client.PutAsJsonAsync("api/accounts/update-profile", dto);
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Profile update failed.";
                return View("Profile", dto);
            }
            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction("Profile");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var client = GetClient();
            var response = await client.PostAsJsonAsync("api/accounts/change-password", dto);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Password change failed. Current password may be incorrect.");
                return View(dto);
            }
            TempData["Success"] = "Password changed successfully.";
            return RedirectToAction("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var client = GetClient();
            await client.PostAsync("api/accounts/logout", null);
            await HttpContext.SignOutAsync("CookieAuth");
            Response.Cookies.Delete("JWToken");
            Response.Cookies.Delete(".AspNetCore.CookieAuth");
            return RedirectToAction("Login", "Account");
        }

        [Authorize(AuthenticationSchemes = "CookieAuth")]
        public async Task<IActionResult> AdminRegister()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/Roles/get-all");
            var allRoles = new List<string>();
            if (response.IsSuccessStatusCode)
            {
                var roles = await response.Content.ReadFromJsonAsync<List<JsonElement>>() ?? new List<JsonElement>();
                allRoles = roles.Select(r => r.TryGetProperty("Name", out var nameProp) ? nameProp.GetString() ?? string.Empty : string.Empty).ToList();
            }
            ViewBag.AllRoles = allRoles;
            return View(new AdminRegisterDto());
        }

        [Authorize(AuthenticationSchemes = "CookieAuth")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminRegister(AdminRegisterDto dto)
        {
            var client = GetClient();
            var response = await client.PostAsJsonAsync("api/accounts/admin-register", dto);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Registration failed. Email may already be in use.");
                var allRolesResponse = await client.GetAsync("api/Roles/get-all");
                var allRoles = new List<string>();
                if (allRolesResponse.IsSuccessStatusCode)
                {
                    var roles = await allRolesResponse.Content.ReadFromJsonAsync<List<JsonElement>>() ?? new List<JsonElement>();
                    allRoles = roles.Select(r => r.TryGetProperty("Name", out var nameProp) ? nameProp.GetString() ?? string.Empty : string.Empty).ToList();
                }
                ViewBag.AllRoles = allRoles;
                return View(dto);
            }
            TempData["Success"] = "Admin user registered successfully.";
            return RedirectToAction("Index", "HomeAdmin");
        }

        [Authorize(AuthenticationSchemes = "CookieAuth")]
        public async Task<IActionResult> AdminChangePassword(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "User ID is required.";
                return RedirectToAction("Index", "HomeAdmin");
            }
            ViewBag.TargetUserId = userId;
            return View(new AdminChangePasswordDto { TargetUserId = userId });
        }

        [Authorize(AuthenticationSchemes = "CookieAuth")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminChangePassword(AdminChangePasswordDto dto)
        {
            var client = GetClient();
            var response = await client.PostAsJsonAsync("api/accounts/admin-change-password", dto);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Password change failed. Please try again.");
                ViewBag.TargetUserId = dto.TargetUserId;
                return View(dto);
            }
            TempData["Success"] = "Password changed successfully.";
            return RedirectToAction("AdminProfile", new { userId = dto.TargetUserId });
        }

        [Authorize(AuthenticationSchemes = "CookieAuth")]
        public async Task<IActionResult> AdminProfile(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "User ID is required.";
                return RedirectToAction("Index", "HomeAdmin");
            }
            var client = GetClient();
            var response = await client.GetAsync($"api/accounts/admin-get-profile/{userId}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to load user profile.";
                return RedirectToAction("Index", "HomeAdmin");
            }
            var dto = await response.Content.ReadFromJsonAsync<AdminUpdateProfileDto>();
            if (dto == null)
            {
                TempData["Error"] = "User profile not found.";
                return RedirectToAction("Index", "HomeAdmin");
            }
            return View(dto);
        }

        [Authorize(AuthenticationSchemes = "CookieAuth")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminUpdateProfile(AdminUpdateProfileDto dto)
        {
            var client = GetClient();
            var response = await client.PutAsJsonAsync("api/accounts/admin-update-profile", dto);
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Profile update failed.";
                return View("AdminProfile", dto);
            }
            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction("AdminProfile", new { userId = dto.TargetUserId });
        }

        private async Task SetAuthCookie(string token)
        {
            Response.Cookies.Append("JWToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var claims = jwtToken.Claims.ToList();
            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal, new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1),
                IsPersistent = true
            });
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "UserHome");
        }
    }
}
