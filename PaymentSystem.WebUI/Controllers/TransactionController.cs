using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace PaymentSystem.WebUI.Controllers
{
    public class TransactionController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private const string ApiEndpoint = "api/Transactions";

        public TransactionController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactions()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all");
                response.EnsureSuccessStatusCode();
                var transactions = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                ViewBag.SignalRHubUrl = _configuration["ApiSettings:BaseUrl"];
                return View(transactions);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View(new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactionsByWalletId(int walletId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-by-wallet/{walletId}");
                response.EnsureSuccessStatusCode();
                var transactions = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                ViewBag.WalletId = walletId;
                ViewBag.SignalRHubUrl = _configuration["ApiSettings:BaseUrl"];
                return View("GetAllTransactions", transactions);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllTransactions", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactionsByPaymentId(int? paymentId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-by-payment/{paymentId}");
                response.EnsureSuccessStatusCode();
                var transactions = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                ViewBag.PaymentId = paymentId;
                ViewBag.SignalRHubUrl = _configuration["ApiSettings:BaseUrl"];
                return View("GetAllTransactions", transactions);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllTransactions", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactionsByCurrencyId(int currencyId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-by-currency/{currencyId}");
                response.EnsureSuccessStatusCode();
                var transactions = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                ViewBag.CurrencyId = currencyId;
                ViewBag.SignalRHubUrl = _configuration["ApiSettings:BaseUrl"];
                return View("GetAllTransactions", transactions);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllTransactions", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactionsByTransactionTypeId(int transactionTypeId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-by-type/{transactionTypeId}");
                response.EnsureSuccessStatusCode();
                var transactions = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                ViewBag.TransactionTypeId = transactionTypeId;
                ViewBag.SignalRHubUrl = _configuration["ApiSettings:BaseUrl"];
                return View("GetAllTransactions", transactions);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllTransactions", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactionsForAdmin()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all-admin");
                response.EnsureSuccessStatusCode();
                var transactions = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                ViewBag.SignalRHubUrl = _configuration["ApiSettings:BaseUrl"];
                return View("GetAllTransactions", transactions);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllTransactions", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();
                var transaction = await response.Content.ReadFromJsonAsync<dynamic>();
                ViewBag.SignalRHubUrl = _configuration["ApiSettings:BaseUrl"];
                return View(transaction);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return RedirectToAction("GetAllTransactions");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactionForEdit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-for-edit/{id}");
                response.EnsureSuccessStatusCode();
                var transaction = await response.Content.ReadFromJsonAsync<dynamic>();
                return View("UpdateTransaction", transaction);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return RedirectToAction("GetAllTransactions");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] object dto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = null });
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{ApiEndpoint}/create", content);
                response.EnsureSuccessStatusCode();
                TempData["Success"] = "Transaction created successfully";
                return RedirectToAction("GetAllTransactions");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Create failed: {ex.Message}";
                return RedirectToAction("GetAllTransactions");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTransaction([FromBody] object dto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = null });
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{ApiEndpoint}/update", content);
                response.EnsureSuccessStatusCode();
                TempData["Success"] = "Transaction updated successfully";
                return RedirectToAction("GetAllTransactions");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllTransactions");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();
                TempData["Success"] = "Transaction deleted successfully";
                return RedirectToAction("GetAllTransactions");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllTransactions");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTransactionsById(List<int> ids)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoint}/delete-multiple", ids);
                response.EnsureSuccessStatusCode();
                TempData["Success"] = "Selected transactions deleted successfully";
                return RedirectToAction("GetAllTransactions");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllTransactions");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetActive(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-active/{id}", null);
                response.EnsureSuccessStatusCode();
                TempData["Success"] = "Transaction set as active";
                return RedirectToAction("GetAllTransactions");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllTransactions");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetInactive(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-inactive/{id}", null);
                response.EnsureSuccessStatusCode();
                TempData["Success"] = "Transaction set as inactive";
                return RedirectToAction("GetAllTransactions");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllTransactions");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/soft-delete/{id}", null);
                response.EnsureSuccessStatusCode();
                TempData["Success"] = "Transaction soft deleted";
                return RedirectToAction("GetAllTransactions");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllTransactions");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/restore/{id}", null);
                response.EnsureSuccessStatusCode();
                TempData["Success"] = "Transaction restored";
                return RedirectToAction("GetAllTransactions");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllTransactions");
            }
        }
    }
}

