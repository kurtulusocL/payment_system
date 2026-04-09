using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace PaymentSystem.WebUI.Controllers
{
    public class UserSessionController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiEndpoint = "api/UserSessions";

        public UserSessionController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUserSessions()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all");
                response.EnsureSuccessStatusCode();

                var sessions = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View(sessions);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View(new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOnlineUserSessions()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-online");
                response.EnsureSuccessStatusCode();

                var sessions = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllUserSessions", sessions);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllUserSessions", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUserSessionsByUserId(string userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-by-user/{userId}");
                response.EnsureSuccessStatusCode();

                var sessions = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                ViewBag.UserId = userId;
                return View("GetAllUserSessions", sessions);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllUserSessions", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUserSessionsByLoginDate()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-by-login-date");
                response.EnsureSuccessStatusCode();

                var sessions = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllUserSessions", sessions);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllUserSessions", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUserSessionsByOnlineDuration()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-by-duration");
                response.EnsureSuccessStatusCode();

                var sessions = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllUserSessions", sessions);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllUserSessions", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUserSessionsForAdmin()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all-admin");
                response.EnsureSuccessStatusCode();

                var sessions = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllUserSessions", sessions);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllUserSessions", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserSessionById(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                var session = await response.Content.ReadFromJsonAsync<dynamic>();
                return View(session);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return RedirectToAction("GetAllUserSessions");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUserSession(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "User session deleted successfully";
                return RedirectToAction("GetAllUserSessions");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllUserSessions");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUserSessionsById(List<int> ids)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoint}/delete-multiple", ids);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Selected user sessions deleted successfully";
                return RedirectToAction("GetAllUserSessions");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllUserSessions");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetActive(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-active/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "User session set as active";
                return RedirectToAction("GetAllUserSessions");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllUserSessions");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetInactive(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-inactive/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "User session set as inactive";
                return RedirectToAction("GetAllUserSessions");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllUserSessions");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/soft-delete/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "User session soft deleted";
                return RedirectToAction("GetAllUserSessions");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllUserSessions");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/restore/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "User session restored";
                return RedirectToAction("GetAllUserSessions");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllUserSessions");
            }
        }
    }
}

