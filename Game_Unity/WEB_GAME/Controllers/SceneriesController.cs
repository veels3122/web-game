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
    public class SceneriesController : Controller
    {
        private readonly HttpClient _httpClient;

        public SceneriesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://www.guayabagame.somee.com/api/"); // Reemplaza con la URL de tu API publicada
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var response = await _httpClient.GetAsync("Sceneries");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var sceneries = JsonConvert.DeserializeObject<IEnumerable<ScoreViewModel>>(content);

                if (!string.IsNullOrEmpty(searchString))
                {
                    // Filtrar por algún campo relevante
                    sceneries = sceneries.Where(s => s.UserId.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase) || s.ScoreId.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase));
                }

                return View(sceneries);
            }

            return View(new List<ScoreViewModel>());
        }

        // Método para mostrar el formulario de creación
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ScoreViewModel scenery)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(scenery), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync("Sceneries", jsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error al crear el escenario. Inténtalo de nuevo más tarde.";
                        return View(scenery);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Por favor, corrige los errores del formulario.";
                    return View(scenery);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error interno del servidor: {ex.Message}";
                return View(scenery);
            }
        }

        // Método para mostrar el formulario de edición
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"Sceneries/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var scenery = JsonConvert.DeserializeObject<ScoreViewModel>(content);
                return View(scenery);
            }

            return NotFound();
        }

        // Método para manejar la solicitud de edición del escenario
        [HttpPost]
        public async Task<IActionResult> Edit(ScoreViewModel scenery)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(scenery), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PutAsync($"Sceneries/{scenery.ScoreId}", jsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error al guardar los cambios. Inténtalo de nuevo más tarde.";
                        return View(scenery);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Por favor, corrige los errores del formulario.";
                    return View(scenery);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error interno del servidor: {ex.Message}";
                return View(scenery);
            }
        }

        // Método para desactivar el escenario
        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Sceneries/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al desactivar el escenario. Inténtalo de nuevo más tarde.";
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
