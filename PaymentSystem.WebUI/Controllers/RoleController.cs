using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace PaymentSystem.WebUI.Controllers
{
    public class RoleController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiEndpoint = "api/Roles";

        public RoleController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all");
                response.EnsureSuccessStatusCode();

                var roles = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View(roles);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View(new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRolesForAdmin()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all-admin");
                response.EnsureSuccessStatusCode();

                var roles = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllRoles", roles);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllRoles", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRoleById(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                var role = await response.Content.ReadFromJsonAsync<dynamic>();
                return View(role);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return RedirectToAction("GetAllRoles");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRoleForEdit(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-for-edit/{id}");
                response.EnsureSuccessStatusCode();

                var role = await response.Content.ReadFromJsonAsync<dynamic>();
                return View("UpdateRole", role);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return RedirectToAction("GetAllRoles");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] object dto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = null });
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{ApiEndpoint}/create", content);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Role created successfully";
                return RedirectToAction("GetAllRoles");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Create failed: {ex.Message}";
                return RedirectToAction("GetAllRoles");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRole([FromBody] object dto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = null });
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{ApiEndpoint}/update", content);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Role updated successfully";
                return RedirectToAction("GetAllRoles");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllRoles");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Role deleted successfully";
                return RedirectToAction("GetAllRoles");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllRoles");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRolesById(List<string> ids)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoint}/delete-multiple", ids);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Selected roles deleted successfully";
                return RedirectToAction("GetAllRoles");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllRoles");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetActive(string id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-active/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Role set as active";
                return RedirectToAction("GetAllRoles");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllRoles");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetInactive(string id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-inactive/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Role set as inactive";
                return RedirectToAction("GetAllRoles");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllRoles");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SoftDelete(string id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/soft-delete/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Role soft deleted";
                return RedirectToAction("GetAllRoles");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllRoles");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Restore(string id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/restore/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Role restored";
                return RedirectToAction("GetAllRoles");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllRoles");
            }
        }
    }
}

