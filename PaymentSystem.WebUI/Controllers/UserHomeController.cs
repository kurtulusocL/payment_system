using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.Json;
using PaymentSystem.Shared.ViewModels;

namespace PaymentSystem.WebUI.Controllers
{
    [Authorize]
    public class UserHomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private const string ApiBaseUrl = "api";

        public UserHomeController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User is not authenticated.");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = GetCurrentUserId();
                var walletsTask = _httpClient.GetAsync($"{ApiBaseUrl}/Wallets/get-by-user/{userId}");
                var paymentsTask = _httpClient.GetAsync($"{ApiBaseUrl}/Payments/get-by-user/{userId}");
                await Task.WhenAll(walletsTask, paymentsTask);

                var dashboardVM = new UserDashboardViewModel
                {
                    UserId = userId,
                    Wallets = walletsTask.Result.IsSuccessStatusCode
                ? await walletsTask.Result.Content.ReadFromJsonAsync<List<dynamic>>()
                : new List<dynamic>(),
                    Payments = paymentsTask.Result.IsSuccessStatusCode
                ? await paymentsTask.Result.Content.ReadFromJsonAsync<List<dynamic>>()
                : new List<dynamic>(),
                    SignalRHubUrl = _configuration["ApiSettings:BaseUrl"]
                };

                return View(dashboardVM);
            }
            catch
            {
                TempData["Error"] = "Failed to load dashboard data.";
                return View(new UserDashboardViewModel { UserId = GetCurrentUserId() });
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyWallets()
        {
            try
            {
                var userId = GetCurrentUserId();
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/Wallets/get-by-user/{userId}");
                response.EnsureSuccessStatusCode();
                var wallets = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                ViewBag.SignalRHubUrl = _configuration["ApiSettings:BaseUrl"];
                return View(wallets);
            }
            catch
            {
                TempData["Error"] = "API connection error.";
                return View(new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyPayments()
        {
            try
            {
                var userId = GetCurrentUserId();
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/Payments/get-by-user/{userId}");
                response.EnsureSuccessStatusCode();
                var payments = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                ViewBag.SignalRHubUrl = _configuration["ApiSettings:BaseUrl"];
                return View(payments);
            }
            catch
            {
                TempData["Error"] = "API connection error.";
                return View(new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyTransactions()
        {
            try
            {
                var userId = GetCurrentUserId();
                var walletsResponse = await _httpClient.GetAsync($"{ApiBaseUrl}/Wallets/get-by-user/{userId}");
                walletsResponse.EnsureSuccessStatusCode();
                var wallets = await walletsResponse.Content.ReadFromJsonAsync<List<dynamic>>();

                var allTransactions = new List<dynamic>();
                if (wallets != null && wallets.Count > 0)
                {
                    foreach (var wallet in wallets)
                    {
                        var walletId = (int)wallet.Id;
                        var txResponse = await _httpClient.GetAsync($"{ApiBaseUrl}/Transactions/get-by-wallet/{walletId}");
                        if (txResponse.IsSuccessStatusCode)
                        {
                            var txs = await txResponse.Content.ReadFromJsonAsync<List<dynamic>>();
                            if (txs != null) allTransactions.AddRange(txs);
                        }
                    }
                }

                ViewBag.SignalRHubUrl = _configuration["ApiSettings:BaseUrl"];
                return View(allTransactions);
            }
            catch
            {
                TempData["Error"] = "API connection error.";
                return View(new List<dynamic>());
            }
        }

        [HttpGet] public IActionResult CreateWallet() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWallet([FromBody] dynamic dto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{ApiBaseUrl}/Wallets/create", content);
                response.EnsureSuccessStatusCode();
                TempData["Success"] = "Wallet created successfully";
                return RedirectToAction("MyWallets");
            }
            catch
            {
                TempData["Error"] = "Create failed.";
                return RedirectToAction("CreateWallet");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditWallet(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/Wallets/get-for-edit/{id}");
                response.EnsureSuccessStatusCode();
                var wallet = await response.Content.ReadFromJsonAsync<dynamic>();
                return View(wallet);
            }
            catch
            {
                TempData["Error"] = "API connection error.";
                return RedirectToAction("MyWallets");
            }
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateWallet([FromBody] dynamic dto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{ApiBaseUrl}/Wallets/update", content);
                response.EnsureSuccessStatusCode();
                TempData["Success"] = "Wallet updated successfully";
                return RedirectToAction("MyWallets");
            }
            catch
            {
                TempData["Error"] = "Update failed.";
                return RedirectToAction("MyWallets");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SoftDeleteWallet(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiBaseUrl}/Wallets/soft-delete/{id}", null);
                response.EnsureSuccessStatusCode();
                TempData["Success"] = "Wallet soft deleted";
                return RedirectToAction("MyWallets");
            }
            catch
            {
                TempData["Error"] = "Delete failed.";
                return RedirectToAction("MyWallets");
            }
        }

        [HttpPost]
        public async Task<IActionResult> BulkDeleteWallets(List<int> ids)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/Wallets/delete-multiple", ids);
                response.EnsureSuccessStatusCode();
                TempData["Success"] = "Selected wallets deleted successfully";
                return RedirectToAction("MyWallets");
            }
            catch
            {
                TempData["Error"] = "Delete failed.";
                return RedirectToAction("MyWallets");
            }
        }

        [HttpGet] public IActionResult CreatePayment() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePayment([FromBody] dynamic dto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{ApiBaseUrl}/Payments/create", content);
                response.EnsureSuccessStatusCode();
                TempData["Success"] = "Payment created successfully";
                return RedirectToAction("MyPayments");
            }
            catch
            {
                TempData["Error"] = "Create failed.";
                return RedirectToAction("CreatePayment");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditPayment(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/Payments/get-for-edit/{id}");
                response.EnsureSuccessStatusCode();
                var payment = await response.Content.ReadFromJsonAsync<dynamic>();
                return View(payment);
            }
            catch
            {
                TempData["Error"] = "API connection error.";
                return RedirectToAction("MyPayments");
            }
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePayment([FromBody] dynamic dto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{ApiBaseUrl}/Payments/update", content);
                response.EnsureSuccessStatusCode();
                TempData["Success"] = "Payment updated successfully";
                return RedirectToAction("MyPayments");
            }
            catch
            {
                TempData["Error"] = "Update failed.";
                return RedirectToAction("MyPayments");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SoftDeletePayment(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiBaseUrl}/Payments/soft-delete/{id}", null);
                response.EnsureSuccessStatusCode();
                TempData["Success"] = "Payment soft deleted";
                return RedirectToAction("MyPayments");
            }
            catch
            {
                TempData["Error"] = "Delete failed.";
                return RedirectToAction("MyPayments");
            }
        }

        [HttpPost]
        public async Task<IActionResult> BulkDeletePayments(List<int> ids)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/Payments/delete-multiple", ids);
                response.EnsureSuccessStatusCode();
                TempData["Success"] = "Selected payments deleted successfully";
                return RedirectToAction("MyPayments");
            }
            catch
            {
                TempData["Error"] = "Delete failed.";
                return RedirectToAction("MyPayments");
            }
        }

        [HttpGet] public IActionResult CreateTransaction() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTransaction([FromBody] dynamic dto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{ApiBaseUrl}/Transactions/create", content);
                response.EnsureSuccessStatusCode();
                TempData["Success"] = "Transaction created successfully";
                return RedirectToAction("MyTransactions");
            }
            catch
            {
                TempData["Error"] = "Create failed.";
                return RedirectToAction("CreateTransaction");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditTransaction(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/Transactions/get-for-edit/{id}");
                response.EnsureSuccessStatusCode();
                var transaction = await response.Content.ReadFromJsonAsync<dynamic>();
                return View(transaction);
            }
            catch
            {
                TempData["Error"] = "API connection error.";
                return RedirectToAction("MyTransactions");
            }
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTransaction([FromBody] dynamic dto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{ApiBaseUrl}/Transactions/update", content);
                response.EnsureSuccessStatusCode();
                TempData["Success"] = "Transaction updated successfully";
                return RedirectToAction("MyTransactions");
            }
            catch
            {
                TempData["Error"] = "Update failed.";
                return RedirectToAction("MyTransactions");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SoftDeleteTransaction(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiBaseUrl}/Transactions/soft-delete/{id}", null);
                response.EnsureSuccessStatusCode();
                TempData["Success"] = "Transaction soft deleted";
                return RedirectToAction("MyTransactions");
            }
            catch
            {
                TempData["Error"] = "Delete failed.";
                return RedirectToAction("MyTransactions");
            }
        }

        [HttpPost]
        public async Task<IActionResult> BulkDeleteTransactions(List<int> ids)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/Transactions/delete-multiple", ids);
                response.EnsureSuccessStatusCode();
                TempData["Success"] = "Selected transactions deleted successfully";
                return RedirectToAction("MyTransactions");
            }
            catch
            {
                TempData["Error"] = "Delete failed.";
                return RedirectToAction("MyTransactions");
            }
        }
    }
}

