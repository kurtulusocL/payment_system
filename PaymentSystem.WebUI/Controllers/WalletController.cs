using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;

namespace PaymentSystem.WebUI.Controllers
{
    public class WalletController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private const string ApiEndpoint = "api/Wallets";

        public WalletController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWallets()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all");
                response.EnsureSuccessStatusCode();

                var wallets = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                ViewBag.SignalRHubUrl = _configuration["ApiSettings:BaseUrl"];
                return View(wallets);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View(new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWalletsByUserId(string userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-by-user/{userId}");
                response.EnsureSuccessStatusCode();

                var wallets = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                ViewBag.UserId = userId;
                ViewBag.SignalRHubUrl = _configuration["ApiSettings:BaseUrl"];
                return View("GetAllWallets", wallets);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllWallets", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWalletsByCurrencyId(int currencyId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-by-currency/{currencyId}");
                response.EnsureSuccessStatusCode();

                var wallets = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                ViewBag.CurrencyId = currencyId;
                ViewBag.SignalRHubUrl = _configuration["ApiSettings:BaseUrl"];
                return View("GetAllWallets", wallets);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllWallets", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWalletsForAdmin()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all-admin");
                response.EnsureSuccessStatusCode();

                var wallets = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                ViewBag.SignalRHubUrl = _configuration["ApiSettings:BaseUrl"];
                return View("GetAllWallets", wallets);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllWallets", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetWalletById(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                var wallet = await response.Content.ReadFromJsonAsync<dynamic>();
                ViewBag.SignalRHubUrl = _configuration["ApiSettings:BaseUrl"];
                return View(wallet);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return RedirectToAction("GetAllWallets");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetWalletForEdit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-for-edit/{id}");
                response.EnsureSuccessStatusCode();

                var wallet = await response.Content.ReadFromJsonAsync<dynamic>();
                ViewBag.SignalRHubUrl = _configuration["ApiSettings:BaseUrl"];
                return View(wallet);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return RedirectToAction("GetAllWallets");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateWallet([FromBody] object dto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = null });
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{ApiEndpoint}/create", content);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Wallet created successfully";
                return RedirectToAction("GetAllWallets");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Create failed: {ex.Message}";
                return RedirectToAction("GetAllWallets");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateWallet([FromBody] object dto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = null });
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{ApiEndpoint}/update", content);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Wallet updated successfully";
                return RedirectToAction("GetAllWallets");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllWallets");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteWallet(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Wallet deleted successfully";
                return RedirectToAction("GetAllWallets");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllWallets");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteWalletsById(List<int> ids)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoint}/delete-multiple", ids);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Selected wallets deleted successfully";
                return RedirectToAction("GetAllWallets");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllWallets");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetActive(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-active/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Wallet set as active";
                return RedirectToAction("GetAllWallets");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllWallets");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetInactive(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-inactive/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Wallet set as inactive";
                return RedirectToAction("GetAllWallets");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllWallets");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/soft-delete/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Wallet soft deleted";
                return RedirectToAction("GetAllWallets");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllWallets");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/restore/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Wallet restored";
                return RedirectToAction("GetAllWallets");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllWallets");
            }
        }
    }
}

