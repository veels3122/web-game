using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WEB_GAME.Models;

namespace WEB_GAME.Controllers
{
    public class TimeCollectionTracksController : Controller
    {
        private readonly HttpClient _httpClient;

        public TimeCollectionTracksController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://www.guayabagame.somee.com/api/"); // Reemplaza con la URL de tu API publicada
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var response = await _httpClient.GetAsync("TimeCollectionTracks");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var timeCollectionTracks = JsonConvert.DeserializeObject<IEnumerable<TimeCollectionTracksViewModel>>(content);

                if (!string.IsNullOrEmpty(searchString))
                {
                    // Filtrar por el nombre del detective o cualquier otro campo relevante
                    timeCollectionTracks = timeCollectionTracks.Where(t => t.DetectiveId.ToString().Contains(searchString) || t.GameId.ToString().Contains(searchString) || t.DetectiveCluesId.ToString().Contains(searchString));
                }

                return View(timeCollectionTracks);
            }

            return View(new List<TimeCollectionTracksViewModel>());
        }

        // Método para mostrar el formulario de creación
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TimeCollectionTracksViewModel timeCollectionTracks)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(timeCollectionTracks), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync("TimeCollectionTracks", jsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error al crear el registro de colección de tiempo. Inténtalo de nuevo más tarde.";
                        return View(timeCollectionTracks);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Por favor, corrige los errores del formulario.";
                    return View(timeCollectionTracks);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error interno del servidor: {ex.Message}";
                return View(timeCollectionTracks);
            }
        }

        // Método para mostrar el formulario de edición
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"TimeCollectionTracks/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var timeCollectionTracks = JsonConvert.DeserializeObject<TimeCollectionTracksViewModel>(content);
                return View(timeCollectionTracks);
            }

            return NotFound();
        }

        // Método para manejar la solicitud de edición de registro de colección de tiempo
        [HttpPost]
        public async Task<IActionResult> Edit(TimeCollectionTracksViewModel timeCollectionTracks)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(timeCollectionTracks), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PutAsync($"TimeCollectionTracks/{timeCollectionTracks.TimeCollectionTracksId}", jsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error al guardar los cambios. Inténtalo de nuevo más tarde.";
                        return View(timeCollectionTracks);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Por favor, corrige los errores del formulario.";
                    return View(timeCollectionTracks);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error interno del servidor: {ex.Message}";
                return View(timeCollectionTracks);
            }
        }

        // Método para desactivar el registro de colección de tiempo
        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"TimeCollectionTracks/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al desactivar el registro de colección de tiempo. Inténtalo de nuevo más tarde.";
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
