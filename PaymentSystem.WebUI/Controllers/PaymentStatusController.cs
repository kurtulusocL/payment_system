using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;

namespace PaymentSystem.WebUI.Controllers
{
    public class PaymentStatusController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiEndpoint = "api/PaymentStatuses";

        public PaymentStatusController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPaymentStatuses()
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
        public async Task<IActionResult> GetAllPaymentStatusesByPayments()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-by-payment");
                response.EnsureSuccessStatusCode();

                var statuses = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllPaymentStatuses", statuses);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllPaymentStatuses", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPaymentStatusesForAdmin()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all-admin");
                response.EnsureSuccessStatusCode();

                var statuses = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllPaymentStatuses", statuses);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllPaymentStatuses", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentStatusById(int id)
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
                return RedirectToAction("GetAllPaymentStatuses");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentStatusForEdit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-for-edit/{id}");
                response.EnsureSuccessStatusCode();

                var status = await response.Content.ReadFromJsonAsync<dynamic>();
                return View("UpdatePaymentStatus", status);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return RedirectToAction("GetAllPaymentStatuses");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePaymentStatus([FromBody] object dto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = null });
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{ApiEndpoint}/create", content);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Payment status created successfully";
                return RedirectToAction("GetAllPaymentStatuses");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Create failed: {ex.Message}";
                return RedirectToAction("GetAllPaymentStatuses");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePaymentStatus([FromBody] object dto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = null });
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{ApiEndpoint}/update", content);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Payment status updated successfully";
                return RedirectToAction("GetAllPaymentStatuses");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllPaymentStatuses");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePaymentStatus(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Payment status deleted successfully";
                return RedirectToAction("GetAllPaymentStatuses");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllPaymentStatuses");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePaymentStatusesById(List<int> ids)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoint}/delete-multiple", ids);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Selected payment statuses deleted successfully";
                return RedirectToAction("GetAllPaymentStatuses");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllPaymentStatuses");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetActive(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-active/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Payment status set as active";
                return RedirectToAction("GetAllPaymentStatuses");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllPaymentStatuses");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetInactive(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-inactive/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Payment status set as inactive";
                return RedirectToAction("GetAllPaymentStatuses");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllPaymentStatuses");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/soft-delete/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Payment status soft deleted";
                return RedirectToAction("GetAllPaymentStatuses");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllPaymentStatuses");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/restore/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Payment status restored";
                return RedirectToAction("GetAllPaymentStatuses");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllPaymentStatuses");
            }
        }
    }
}

