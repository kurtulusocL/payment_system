using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace PaymentSystem.WebUI.Controllers
{
    public class CurrencyController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiEndpoint = "api/Currencies";

        public CurrencyController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCurrencies()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all");
                response.EnsureSuccessStatusCode();

                var currencies = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View(currencies);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View(new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCurrenciesByPayment()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-by-payment");
                response.EnsureSuccessStatusCode();

                var currencies = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllCurrencies", currencies);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllCurrencies", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCurrenciesByWallet()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-by-wallet");
                response.EnsureSuccessStatusCode();

                var currencies = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllCurrencies", currencies);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllCurrencies", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCurrenciesByTransaction()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-by-transaction");
                response.EnsureSuccessStatusCode();

                var currencies = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllCurrencies", currencies);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllCurrencies", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCurrenciesForAdmin()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all-admin");
                response.EnsureSuccessStatusCode();

                var currencies = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllCurrencies", currencies);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllCurrencies", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrencyById(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                var currency = await response.Content.ReadFromJsonAsync<dynamic>();
                return View(currency);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return RedirectToAction("GetAllCurrencies");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrencyForEdit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-for-edit/{id}");
                response.EnsureSuccessStatusCode();

                var currency = await response.Content.ReadFromJsonAsync<dynamic>();
                return View("UpdateCurrency", currency);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return RedirectToAction("GetAllCurrencies");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCurrency([FromBody] object dto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = null });
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{ApiEndpoint}/create", content);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Currency created successfully";
                return RedirectToAction("GetAllCurrencies");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Create failed: {ex.Message}";
                return RedirectToAction("GetAllCurrencies");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCurrency([FromBody] object dto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = null });
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{ApiEndpoint}/update", content);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Currency updated successfully";
                return RedirectToAction("GetAllCurrencies");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllCurrencies");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCurrency(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Currency deleted successfully";
                return RedirectToAction("GetAllCurrencies");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllCurrencies");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCurrenciesById(List<int> ids)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoint}/delete-multiple", ids);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Selected currencies deleted successfully";
                return RedirectToAction("GetAllCurrencies");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllCurrencies");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetActive(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-active/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Currency set as active";
                return RedirectToAction("GetAllCurrencies");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllCurrencies");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetInactive(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-inactive/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Currency set as inactive";
                return RedirectToAction("GetAllCurrencies");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllCurrencies");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/soft-delete/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Currency soft deleted";
                return RedirectToAction("GetAllCurrencies");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllCurrencies");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/restore/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Currency restored";
                return RedirectToAction("GetAllCurrencies");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllCurrencies");
            }
        }
    }
}

