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
    public class ScoreController : Controller
    {
        private readonly HttpClient _httpClient;

        public ScoreController(IHttpClientFactory httpClientFactory)
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

                            // Realizar una solicitud a la API para obtener todos los puntajes
                            var response = await _httpClient.GetAsync("Score");

                            if (response.IsSuccessStatusCode)
                            {
                                var content = await response.Content.ReadAsStringAsync();
                                var scores = JsonConvert.DeserializeObject<List<ScoreViewModel>>(content);

                                // Obtener nombres de usuario para los puntajes
                                foreach (var score in scores)
                                {
                                    var userResp = await _httpClient.GetAsync($"Users/{score.UserId}");

                                    if (userResp.IsSuccessStatusCode)
                                    {
                                        var userCont = await userResp.Content.ReadAsStringAsync();
                                        var user = JsonConvert.DeserializeObject<UserViewModel>(userCont);
                                        score.TraslateName = user?.NameUser ?? "Usuario Desconocido";
                                    }
                                    else
                                    {
                                        score.TraslateName = "Usuario Desconocido";
                                    }
                                }

                                // Filtrar los puntajes por UserId si no es admin
                                if (userId != 17)
                                {
                                    scores = scores.Where(s => s.UserId == userId).ToList();
                                }

                                // Filtrar los puntajes por término de búsqueda si se proporciona
                                if (!string.IsNullOrEmpty(searchString))
                                {
                                    scores = scores.Where(s =>
                                        s.TraslateName.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                                    ).ToList();
                                }

                                return View(scores);
                            }
                            else
                            {
                                ViewBag.ErrorMessage = "Error al consultar los puntajes en la API.";
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

            return View(new List<ScoreViewModel>());
        }

        // Método para desactivar una puntuación
        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Score/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al desactivar la puntuación. Inténtalo de nuevo más tarde.";
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
