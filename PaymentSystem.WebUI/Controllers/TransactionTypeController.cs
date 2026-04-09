using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace PaymentSystem.WebUI.Controllers
{
    public class TransactionTypeController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiEndpoint = "api/TransactionTypes";

        public TransactionTypeController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactionTypes()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all");
                response.EnsureSuccessStatusCode();

                var types = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View(types);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View(new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactionTypesByTransactions()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-by-transaction");
                response.EnsureSuccessStatusCode();

                var types = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllTransactionTypes", types);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllTransactionTypes", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactionTypesForAdmin()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all-admin");
                response.EnsureSuccessStatusCode();

                var types = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllTransactionTypes", types);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllTransactionTypes", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactionTypeById(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                var type = await response.Content.ReadFromJsonAsync<dynamic>();
                return View(type);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return RedirectToAction("GetAllTransactionTypes");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactionTypeForEdit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-for-edit/{id}");
                response.EnsureSuccessStatusCode();

                var type = await response.Content.ReadFromJsonAsync<dynamic>();
                return View("UpdateTransactionType", type);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return RedirectToAction("GetAllTransactionTypes");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransactionType([FromBody] object dto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = null });
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{ApiEndpoint}/create", content);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Transaction type created successfully";
                return RedirectToAction("GetAllTransactionTypes");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Create failed: {ex.Message}";
                return RedirectToAction("GetAllTransactionTypes");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTransactionType([FromBody] object dto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = null });
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{ApiEndpoint}/update", content);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Transaction type updated successfully";
                return RedirectToAction("GetAllTransactionTypes");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllTransactionTypes");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTransactionType(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Transaction type deleted successfully";
                return RedirectToAction("GetAllTransactionTypes");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllTransactionTypes");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTransactionTypesById(List<int> ids)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoint}/delete-multiple", ids);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Selected transaction types deleted successfully";
                return RedirectToAction("GetAllTransactionTypes");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllTransactionTypes");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetActive(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-active/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Transaction type set as active";
                return RedirectToAction("GetAllTransactionTypes");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllTransactionTypes");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetInactive(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-inactive/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Transaction type set as inactive";
                return RedirectToAction("GetAllTransactionTypes");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllTransactionTypes");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/soft-delete/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Transaction type soft deleted";
                return RedirectToAction("GetAllTransactionTypes");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllTransactionTypes");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/restore/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Transaction type restored";
                return RedirectToAction("GetAllTransactionTypes");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllTransactionTypes");
            }
        }
    }
}

