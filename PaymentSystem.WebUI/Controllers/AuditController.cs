using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace PaymentSystem.WebUI.Controllers
{
    public class AuditController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiEndpoint = "api/Audits";

        public AuditController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAudits()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all");
                response.EnsureSuccessStatusCode();

                var audits = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View(audits);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View(new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAuditsByUserId(string userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-by-user/{userId}");
                response.EnsureSuccessStatusCode();

                var audits = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                ViewBag.UserId = userId;
                return View("GetAllAudits", audits);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllAudits", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAuditsForAdmin()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all-admin");
                response.EnsureSuccessStatusCode();

                var audits = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllAudits", audits);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllAudits", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAuditById(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                var audit = await response.Content.ReadFromJsonAsync<dynamic>();
                return View(audit);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return RedirectToAction("GetAllAudits");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiEndpoint}/hard-delete/{id}");
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Audit deleted successfully";
                return RedirectToAction("GetAllAudits");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllAudits");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAuditsById(List<int> ids)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoint}/delete-multiple", ids);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Selected audits deleted successfully";
                return RedirectToAction("GetAllAudits");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllAudits");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetActive(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-active/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Audit set as active";
                return RedirectToAction("GetAllAudits");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllAudits");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetInactive(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-inactive/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Audit set as inactive";
                return RedirectToAction("GetAllAudits");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllAudits");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/soft-delete/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Audit soft deleted";
                return RedirectToAction("GetAllAudits");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllAudits");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/restore/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Audit restored";
                return RedirectToAction("GetAllAudits");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllAudits");
            }
        }
    }
}

