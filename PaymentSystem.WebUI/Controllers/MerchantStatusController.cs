using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace PaymentSystem.WebUI.Controllers
{
    public class MerchantStatusController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiEndpoint = "api/MerchantStatuses";

        public MerchantStatusController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMerchantStatuses()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all");
                response.EnsureSuccessStatusCode();

                var statuses = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View(statuses);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View(new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMerchantStatusesByMerchants()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-by-merchant");
                response.EnsureSuccessStatusCode();

                var statuses = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllMerchantStatuses", statuses);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllMerchantStatuses", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMerchantStatusesForAdmin()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all-admin");
                response.EnsureSuccessStatusCode();

                var statuses = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllMerchantStatuses", statuses);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllMerchantStatuses", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMerchantStatusById(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                var status = await response.Content.ReadFromJsonAsync<dynamic>();
                return View(status);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return RedirectToAction("GetAllMerchantStatuses");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMerchantStatusForEdit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-for-edit/{id}");
                response.EnsureSuccessStatusCode();

                var status = await response.Content.ReadFromJsonAsync<dynamic>();
                return View("UpdateMerchantStatus", status);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return RedirectToAction("GetAllMerchantStatuses");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMerchantStatus([FromBody] object dto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = null });
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{ApiEndpoint}/create", content);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Merchant status created successfully";
                return RedirectToAction("GetAllMerchantStatuses");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Create failed: {ex.Message}";
                return RedirectToAction("GetAllMerchantStatuses");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMerchantStatus([FromBody] object dto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = null });
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{ApiEndpoint}/update", content);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Merchant status updated successfully";
                return RedirectToAction("GetAllMerchantStatuses");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllMerchantStatuses");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMerchantStatus(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Merchant status deleted successfully";
                return RedirectToAction("GetAllMerchantStatuses");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllMerchantStatuses");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMerchantStatusesById(List<int> ids)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoint}/delete-multiple", ids);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Selected merchant statuses deleted successfully";
                return RedirectToAction("GetAllMerchantStatuses");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllMerchantStatuses");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetActive(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-active/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Merchant status set as active";
                return RedirectToAction("GetAllMerchantStatuses");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllMerchantStatuses");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetInactive(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-inactive/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Merchant status set as inactive";
                return RedirectToAction("GetAllMerchantStatuses");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllMerchantStatuses");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/soft-delete/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Merchant status soft deleted";
                return RedirectToAction("GetAllMerchantStatuses");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllMerchantStatuses");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/restore/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Merchant status restored";
                return RedirectToAction("GetAllMerchantStatuses");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllMerchantStatuses");
            }
        }
    }
}

