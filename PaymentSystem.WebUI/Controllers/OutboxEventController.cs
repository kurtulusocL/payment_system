using Microsoft.AspNetCore.Mvc;

namespace PaymentSystem.WebUI.Controllers
{
    public class OutboxEventController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiEndpoint = "api/OutboxEvents";

        public OutboxEventController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOutboxEvents()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all");
                response.EnsureSuccessStatusCode();

                var events = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View(events);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View(new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSuccessfulOutboxEvents()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-successful");
                response.EnsureSuccessStatusCode();

                var events = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllOutboxEvents", events);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllOutboxEvents", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFailedOutboxEvents()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-failed");
                response.EnsureSuccessStatusCode();

                var events = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllOutboxEvents", events);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllOutboxEvents", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOutboxEventsForAdmin()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all-admin");
                response.EnsureSuccessStatusCode();

                var events = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllOutboxEvents", events);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllOutboxEvents", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOutboxEventById(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                var outboxEvent = await response.Content.ReadFromJsonAsync<dynamic>();
                return View(outboxEvent);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return RedirectToAction("GetAllOutboxEvents");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOutboxEvent(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Outbox event deleted successfully";
                return RedirectToAction("GetAllOutboxEvents");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllOutboxEvents");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOutboxEventsById(List<int> ids)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoint}/delete-multiple", ids);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Selected outbox events deleted successfully";
                return RedirectToAction("GetAllOutboxEvents");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllOutboxEvents");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetActive(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-active/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Outbox event set as active";
                return RedirectToAction("GetAllOutboxEvents");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllOutboxEvents");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetInactive(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-inactive/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Outbox event set as inactive";
                return RedirectToAction("GetAllOutboxEvents");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllOutboxEvents");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/soft-delete/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Outbox event soft deleted";
                return RedirectToAction("GetAllOutboxEvents");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllOutboxEvents");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/restore/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Outbox event restored";
                return RedirectToAction("GetAllOutboxEvents");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllOutboxEvents");
            }
        }
    }
}

