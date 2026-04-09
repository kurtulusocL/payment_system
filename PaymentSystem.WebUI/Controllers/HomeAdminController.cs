using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PaymentSystem.Shared.ViewModels;

namespace PaymentSystem.WebUI.Controllers
{
    [Authorize]
    public class HomeAdminController : Controller
    {
        private readonly HttpClient _httpClient;

        public HomeAdminController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var usersTask = _httpClient.GetAsync("api/Users/get-all");
                var paymentsTask = _httpClient.GetAsync("api/Payments/get-all");
                var walletsTask = _httpClient.GetAsync("api/Wallets/get-all");
                var merchantsTask = _httpClient.GetAsync("api/Merchants/get-all");
                var exceptionsTask = _httpClient.GetAsync("api/ExceptionLoggers/get-all");

                await Task.WhenAll(usersTask, paymentsTask, walletsTask, merchantsTask, exceptionsTask);

                var dashboardVM = new AdminDashboardViewModel
                {
                    TotalUsers = await CountResponseAsync(usersTask.Result),
                    TotalPayments = await CountResponseAsync(paymentsTask.Result),
                    TotalWallets = await CountResponseAsync(walletsTask.Result),
                    TotalMerchants = await CountResponseAsync(merchantsTask.Result),
                    TotalExceptions = await CountResponseAsync(exceptionsTask.Result)
                };

                return View(dashboardVM);
            }
            catch
            {
                TempData["Error"] = "Failed to load dashboard data.";
                return View(new AdminDashboardViewModel());
            }
        }

        private async Task<int> CountResponseAsync(HttpResponseMessage response)
        {
            try
            {
                if (!response.IsSuccessStatusCode)
                    return 0;

                var data = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return data?.Count ?? 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}

