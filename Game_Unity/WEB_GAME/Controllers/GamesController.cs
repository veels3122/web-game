using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WEB_GAME.Models;
using Microsoft.AspNetCore.Http;

namespace WEB_GAME.Controllers
{
    public class GameController : Controller
    {
        private readonly HttpClient _httpClient;

        public GameController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://www.guayabagame.somee.com/api/");
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
                    var userResponse = await _httpClient.GetAsync("Users");

                    if (userResponse.IsSuccessStatusCode)
                    {
                        var userContent = await userResponse.Content.ReadAsStringAsync();
                        var users = JsonConvert.DeserializeObject<List<UserViewModel>>(userContent);

                        // Filtrar los usuarios por el nombre de usuario proporcionado
                        var filteredUsers = users.Where(u => u.NameUser.Equals(username, StringComparison.OrdinalIgnoreCase));

                        // Verificar si se encontraron usuarios
                        if (filteredUsers.Any())
                        {
                            // Obtener el userId del primer usuario encontrado
                            int userId = filteredUsers.First().UserId;

                            // Ahora puedes utilizar userId como desees
                            ViewBag.UserId = userId;

                            // Realizar una solicitud a la API para obtener todos los juegos
                            var response = await _httpClient.GetAsync("Game");

                            if (response.IsSuccessStatusCode)
                            {
                                var content = await response.Content.ReadAsStringAsync();
                                var games = JsonConvert.DeserializeObject<List<GameViewModel>>(content);

                                // Filtrar los juegos por UserId si no es admin
                                if (userId == 17)
                                {
                                    ViewBag.ShowUserId = true;

                                    // Obtener nombres de usuario y de escenario para los juegos
                                    foreach (var game in games)
                                    {
                                        // Obtener nombre de usuario
                                        var userResp = await _httpClient.GetAsync($"Users/{game.UserId}");
                                        if (userResp.IsSuccessStatusCode)
                                        {
                                            var userCont = await userResp.Content.ReadAsStringAsync();
                                            var user = JsonConvert.DeserializeObject<UserViewModel>(userCont);
                                            game.TransleteNameUser = user?.NameUser ?? "Usuario Desconocido";
                                        }
                                        else
                                        {
                                            game.TransleteNameUser = "Usuario Desconocido";
                                        }

                                        // Obtener nombre de escenario
                                        if (game.ScenarioId.HasValue)
                                        {
                                            var scenarioResp = await _httpClient.GetAsync($"SceneryService/{game.ScenarioId}");
                                            if (scenarioResp.IsSuccessStatusCode)
                                            {
                                                var scenarioCont = await scenarioResp.Content.ReadAsStringAsync();
                                                var scenario = JsonConvert.DeserializeObject<SceneryViewModel>(scenarioCont);
                                                game.TrasleteNameScenarie = scenario?.NameScenery ?? "Escenario Desconocido";
                                            }
                                            else
                                            {
                                                game.TrasleteNameScenarie = "Escenario Desconocido";
                                            }
                                        }
                                    }

                                    // Filtrar los juegos por búsqueda si se proporciona
                                    if (!string.IsNullOrEmpty(searchString))
                                    {
                                        games = games.Where(g =>
                                            (g.TransleteNameUser != null && g.TransleteNameUser.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                                            (g.TrasleteNameScenarie != null && g.TrasleteNameScenarie.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                                        ).ToList();
                                    }

                                    return View(games);
                                }
                                else // Si el userId no es 17, mostrar solo los detalles del usuario
                                {
                                    ViewBag.ShowUserId = false;
                                    var filteredGames = games.Where(g => g.UserId == userId).ToList();

                                    // Obtener nombres de usuario y de escenario para los juegos
                                    foreach (var game in filteredGames)
                                    {
                                        // Obtener nombre de usuario
                                        var userResp = await _httpClient.GetAsync($"Users/{game.UserId}");
                                        if (userResp.IsSuccessStatusCode)
                                        {
                                            var userCont = await userResp.Content.ReadAsStringAsync();
                                            var user = JsonConvert.DeserializeObject<UserViewModel>(userCont);
                                            game.TransleteNameUser = user?.NameUser ?? "Usuario Desconocido";
                                        }
                                        else
                                        {
                                            game.TransleteNameUser = "Usuario Desconocido";
                                        }

                                        // Obtener nombre de escenario
                                        if (game.ScenarioId.HasValue)
                                        {
                                            var scenarioResp = await _httpClient.GetAsync($"SceneryService/{game.ScenarioId}");
                                            if (scenarioResp.IsSuccessStatusCode)
                                            {
                                                var scenarioCont = await scenarioResp.Content.ReadAsStringAsync();
                                                var scenario = JsonConvert.DeserializeObject<SceneryViewModel>(scenarioCont);
                                                game.TrasleteNameScenarie = scenario?.NameScenery ?? "Escenario Desconocido";
                                            }
                                            else
                                            {
                                                game.TrasleteNameScenarie = "Escenario Desconocido";
                                            }
                                        }
                                    }

                                    // Filtrar los juegos por búsqueda si se proporciona
                                    if (!string.IsNullOrEmpty(searchString))
                                    {
                                        filteredGames = filteredGames.Where(g =>
                                            (g.TransleteNameUser != null && g.TransleteNameUser.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                                            (g.TrasleteNameScenarie != null && g.TrasleteNameScenarie.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                                        ).ToList();
                                    }

                                    return View(filteredGames);
                                }
                            }
                            else
                            {
                                ViewBag.ErrorMessage = "Error al consultar los juegos en la API.";
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
                    ViewBag.ErrorMessage = $"Error al obtener datos: {ex.Message}";
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Nombre de usuario no válido.";
            }

            return View(new List<GameViewModel>());
        }

        // Método para desactivar una puntuación
        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Game/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al desactivar la Partida. Inténtalo de nuevo más tarde.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error interno del servidor: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

    }
}
