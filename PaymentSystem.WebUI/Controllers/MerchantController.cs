using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace PaymentSystem.WebUI.Controllers
{
    public class MerchantController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiEndpoint = "api/Merchants";

        public MerchantController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMerchants()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all");
                response.EnsureSuccessStatusCode();

                var merchants = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View(merchants);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View(new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMerchantsByStatusId(int statusId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-by-status/{statusId}");
                response.EnsureSuccessStatusCode();

                var merchants = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                ViewBag.StatusId = statusId;
                return View("GetAllMerchants", merchants);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllMerchants", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMerchantsByNullTaskNumber()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-null-task-numbers");
                response.EnsureSuccessStatusCode();

                var merchants = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllMerchants", merchants);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllMerchants", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMerchantsByPayment()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-by-payment");
                response.EnsureSuccessStatusCode();

                var merchants = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllMerchants", merchants);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllMerchants", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMerchantsForAdmin()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all-admin");
                response.EnsureSuccessStatusCode();

                var merchants = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllMerchants", merchants);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllMerchants", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMerchantById(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                var merchant = await response.Content.ReadFromJsonAsync<dynamic>();
                return View(merchant);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return RedirectToAction("GetAllMerchants");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMerchantForEdit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-for-edit/{id}");
                response.EnsureSuccessStatusCode();

                var merchant = await response.Content.ReadFromJsonAsync<dynamic>();
                return View(merchant);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return RedirectToAction("GetAllMerchants");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMerchant([FromBody] object dto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = null });
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{ApiEndpoint}/create", content);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Merchant created successfully";
                return RedirectToAction("GetAllMerchants");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Create failed: {ex.Message}";
                return RedirectToAction("GetAllMerchants");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMerchant([FromBody] object dto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = null });
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{ApiEndpoint}/update", content);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Merchant updated successfully";
                return RedirectToAction("GetAllMerchants");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllMerchants");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMerchant(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Merchant deleted successfully";
                return RedirectToAction("GetAllMerchants");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllMerchants");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMerchantsById(List<int> ids)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoint}/delete-multiple", ids);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Selected merchants deleted successfully";
                return RedirectToAction("GetAllMerchants");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllMerchants");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetActive(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-active/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Merchant set as active";
                return RedirectToAction("GetAllMerchants");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllMerchants");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetInactive(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-inactive/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Merchant set as inactive";
                return RedirectToAction("GetAllMerchants");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllMerchants");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/soft-delete/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Merchant soft deleted";
                return RedirectToAction("GetAllMerchants");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllMerchants");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/restore/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Merchant restored";
                return RedirectToAction("GetAllMerchants");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllMerchants");
            }
        }
    }
}

