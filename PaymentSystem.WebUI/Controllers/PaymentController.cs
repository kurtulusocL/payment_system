using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace PaymentSystem.WebUI.Controllers
{
    public class PaymentController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private const string ApiEndpoint = "api/Payments";

        public PaymentController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayments()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all");
                response.EnsureSuccessStatusCode();

                var payments = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                ViewBag.SignalRHubUrl = _configuration["ApiSettings:BaseUrl"];
                return View(payments);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View(new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPaymentsByUserId(string userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-by-user/{userId}");
                response.EnsureSuccessStatusCode();

                var payments = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                ViewBag.UserId = userId;
                ViewBag.SignalRHubUrl = _configuration["ApiSettings:BaseUrl"];
                return View("GetAllPayments", payments);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllPayments", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPaymentsByStatusId(int statusId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-by-status/{statusId}");
                response.EnsureSuccessStatusCode();

                var payments = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                ViewBag.StatusId = statusId;
                ViewBag.SignalRHubUrl = _configuration["ApiSettings:BaseUrl"];
                return View("GetAllPayments", payments);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllPayments", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPaymentsByCurrencyId(int currencyId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-by-currency/{currencyId}");
                response.EnsureSuccessStatusCode();

                var payments = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                ViewBag.CurrencyId = currencyId;
                ViewBag.SignalRHubUrl = _configuration["ApiSettings:BaseUrl"];
                return View("GetAllPayments", payments);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllPayments", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPaymentsByMerchantId(int merchantId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-by-merchant/{merchantId}");
                response.EnsureSuccessStatusCode();

                var payments = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                ViewBag.MerchantId = merchantId;
                ViewBag.SignalRHubUrl = _configuration["ApiSettings:BaseUrl"];
                return View("GetAllPayments", payments);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllPayments", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPaymentsForAdmin()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all-admin");
                response.EnsureSuccessStatusCode();

                var payments = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                ViewBag.SignalRHubUrl = _configuration["ApiSettings:BaseUrl"];
                return View("GetAllPayments", payments);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllPayments", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                var payment = await response.Content.ReadFromJsonAsync<dynamic>();
                ViewBag.SignalRHubUrl = _configuration["ApiSettings:BaseUrl"];
                return View(payment);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return RedirectToAction("GetAllPayments");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentForEdit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-for-edit/{id}");
                response.EnsureSuccessStatusCode();

                var payment = await response.Content.ReadFromJsonAsync<dynamic>();
                ViewBag.SignalRHubUrl = _configuration["ApiSettings:BaseUrl"];
                return View(payment);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return RedirectToAction("GetAllPayments");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] object dto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = null });
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{ApiEndpoint}/create", content);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Payment created successfully";
                return RedirectToAction("GetAllPayments");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Create failed: {ex.Message}";
                return RedirectToAction("GetAllPayments");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePayment([FromBody] object dto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = null });
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{ApiEndpoint}/update", content);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Payment updated successfully";
                return RedirectToAction("GetAllPayments");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllPayments");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePayment(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Payment deleted successfully";
                return RedirectToAction("GetAllPayments");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllPayments");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePaymentsById(List<int> ids)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoint}/delete-multiple", ids);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Selected payments deleted successfully";
                return RedirectToAction("GetAllPayments");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllPayments");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetActive(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-active/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Payment set as active";
                return RedirectToAction("GetAllPayments");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllPayments");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetInactive(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-inactive/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Payment set as inactive";
                return RedirectToAction("GetAllPayments");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllPayments");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/soft-delete/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Payment soft deleted";
                return RedirectToAction("GetAllPayments");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllPayments");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/restore/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Payment restored";
                return RedirectToAction("GetAllPayments");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllPayments");
            }
        }
    }
}

