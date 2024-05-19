using API_Cliente.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API_Cliente.Controllers
{
    public class UserWebController : Controller
    {
        private readonly HttpClient _httpClient;
        public UserWebController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5212/api");
        }
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("/api/Users");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<IEnumerable<UsersViewMod>>(content);
                return View("Index", users);
            }

            // Manejar el caso en que la solicitud HTTP no fue exitosa.
            return View(new List<UsersViewMod>()); // Puedes mostrar una vista vacía o un mensaje de error.
        }
    }
}
