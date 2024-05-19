using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using WEB_GAME.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;


namespace WEB_GAME.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;

        public UserController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://www.guayabagame.somee.com/api/"); // Reemplaza con la URL de tu API publicada
        }



        public async Task<IActionResult> Index(string searchString)
        {
            // Obtener el nombre de usuario de la sesión
            string username = HttpContext.Session.GetString("Username");

            if (!string.IsNullOrEmpty(username))
            {
                try
                {
                    // Realizar una solicitud a la API para obtener todos los usuarios
                    var response = await _httpClient.GetAsync("Users");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var users = JsonConvert.DeserializeObject<List<UserViewModel>>(content);

                        // Filtrar los usuarios por el nombre de usuario proporcionado
                        var filteredUsers = users.Where(u => u.NameUser.Equals(username, StringComparison.OrdinalIgnoreCase));

                        // Verificar si se encontraron usuarios
                        if (filteredUsers.Any())
                        {
                            // Obtener el userId del primer usuario encontrado
                            int userId = filteredUsers.First().UserId;

                            // Ahora puedes utilizar userId como desees
                            ViewBag.UserId = userId;

                            // Si el userId es 17, mostrar todos los usuarios
                            if (userId == 17)
                            {
                                ViewBag.ShowUserId = true;
                                // Filtrar los usuarios por búsqueda si se proporciona
                                if (!string.IsNullOrEmpty(searchString))
                                {
                                    users = users.Where(u => u.NameUser.Contains(searchString, StringComparison.OrdinalIgnoreCase) || u.EMail.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                                }

                                return View(users);
                            }
                            else // Si el userId no es 17, mostrar solo los detalles del usuario
                            {
                                ViewBag.ShowUserId = false;
                                return View(filteredUsers);
                            }
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "No se encontraron usuarios con el nombre de usuario proporcionado.";
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Error al consultar los usuarios en la API.";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = $"Error al obtener usuarios: {ex.Message}";
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Nombre de usuario no válido.";
            }

            return View(new List<UserViewModel>());
        }











        // Método para mostrar el formulario de creación
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    DateTime fechaHoraActual = DateTime.Now;

                    string fechaHoraFormateada = fechaHoraActual.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    user.RegistrationDate = DateTime.Parse(fechaHoraFormateada);

                    var jsonContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync("Users", jsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error al crear el usuario. Inténtalo de nuevo más tarde.";
                        return View(user);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Por favor, corrige los errores del formulario.";
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error interno del servidor: {ex.Message}";
                return View(user);
            }
        }


        // Método para mostrar el formulario de edición
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"Users/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<UserViewModel>(content);
                return View(user);
            }

            return NotFound();
        }




        // Método para manejar la solicitud de edición de usuario
        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Encriptar la nueva contraseña antes de enviarla al servidor
                    user.Password = EncryptPassword(user.Password);

                    var jsonContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PutAsync($"Users/{user.UserId}", jsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error al guardar los cambios. Inténtalo de nuevo más tarde.";
                        return View(user);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Por favor, corrige los errores del formulario.";
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error interno del servidor: {ex.Message}";
                return View(user);
            }
        }






        // Método para desactivar un usuario
        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Users/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al desactivar el usuario. Inténtalo de nuevo más tarde.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error interno del servidor: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private string EncryptPassword(string password)
        {
            // Esta es una implementación básica de encriptación, se recomienda mejorarla para un entorno de producción
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }


    }
}
