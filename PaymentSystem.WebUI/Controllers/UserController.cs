using Microsoft.AspNetCore.Mvc;

namespace PaymentSystem.WebUI.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiEndpoint = "api/Users";

        public UserController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all");
                response.EnsureSuccessStatusCode();

                var users = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View(users);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View(new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInactiveUsers()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-inactive");
                response.EnsureSuccessStatusCode();

                var users = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllUsers", users);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllUsers", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDeletedUsers()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-deleted");
                response.EnsureSuccessStatusCode();

                var users = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllUsers", users);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllUsers", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveUsers()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-active");
                response.EnsureSuccessStatusCode();

                var users = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllUsers", users);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllUsers", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsersForAdmin()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all-admin");
                response.EnsureSuccessStatusCode();

                var users = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllUsers", users);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllUsers", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                var user = await response.Content.ReadFromJsonAsync<dynamic>();
                return View(user);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return RedirectToAction("GetAllUsers");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "User deleted successfully";
                return RedirectToAction("GetAllUsers");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllUsers");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUsersById(List<string> ids)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoint}/delete-multiple", ids);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Selected users deleted successfully";
                return RedirectToAction("GetAllUsers");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllUsers");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetActive(string id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-active/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "User set as active";
                return RedirectToAction("GetAllUsers");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllUsers");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetInactive(string id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-inactive/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "User set as inactive";
                return RedirectToAction("GetAllUsers");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllUsers");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SoftDelete(string id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/soft-delete/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "User soft deleted";
                return RedirectToAction("GetAllUsers");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllUsers");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Restore(string id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/restore/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "User restored";
                return RedirectToAction("GetAllUsers");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllUsers");
            }
        }
    }
}

