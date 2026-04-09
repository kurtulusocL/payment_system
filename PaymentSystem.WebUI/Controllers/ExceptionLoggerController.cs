using Microsoft.AspNetCore.Mvc;

namespace PaymentSystem.WebUI.Controllers
{
    public class ExceptionLoggerController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiEndpoint = "api/ExceptionLoggers";

        public ExceptionLoggerController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllExceptions()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all");
                response.EnsureSuccessStatusCode();

                var exceptions = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View(exceptions);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View(new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllExceptionsForAdmin()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/get-all-admin");
                response.EnsureSuccessStatusCode();

                var exceptions = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("GetAllExceptions", exceptions);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return View("GetAllExceptions", new List<dynamic>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetExceptionById(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                var exception = await response.Content.ReadFromJsonAsync<dynamic>();
                return View(exception);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API connection error: {ex.Message}";
                return RedirectToAction("GetAllExceptions");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteException(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Exception log deleted successfully";
                return RedirectToAction("GetAllExceptions");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllExceptions");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteExceptionsById(List<int> ids)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoint}/delete-multiple", ids);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Selected exception logs deleted successfully";
                return RedirectToAction("GetAllExceptions");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("GetAllExceptions");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetActive(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-active/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Exception log set as active";
                return RedirectToAction("GetAllExceptions");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllExceptions");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetInactive(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/set-inactive/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Exception log set as inactive";
                return RedirectToAction("GetAllExceptions");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllExceptions");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/soft-delete/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Exception log soft deleted";
                return RedirectToAction("GetAllExceptions");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllExceptions");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{ApiEndpoint}/restore/{id}", null);
                response.EnsureSuccessStatusCode();

                TempData["Success"] = "Exception log restored";
                return RedirectToAction("GetAllExceptions");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return RedirectToAction("GetAllExceptions");
            }
        }
    }
}

