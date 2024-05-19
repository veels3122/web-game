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
    public class WeatherConditionsController : Controller
    {
        private readonly HttpClient _httpClient;

        public WeatherConditionsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://www.guayabagame.somee.com/api/"); // Reemplaza con la URL de tu API publicada
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var response = await _httpClient.GetAsync("WeatherConditions");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var weatherConditions = JsonConvert.DeserializeObject<IEnumerable<WeatherConditionViewModel>>(content);

                if (!string.IsNullOrEmpty(searchString))
                {
                    weatherConditions = weatherConditions.Where(w => w.TypeWeather.Contains(searchString, StringComparison.OrdinalIgnoreCase) || w.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase));
                }

                return View(weatherConditions);
            }

            return View(new List<WeatherConditionViewModel>());
        }

        // Método para mostrar el formulario de creación
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(WeatherConditionViewModel weatherConditions)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(weatherConditions), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync("WeatherConditions", jsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error al crear las condiciones climáticas. Inténtalo de nuevo más tarde.";
                        return View(weatherConditions);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Por favor, corrige los errores del formulario.";
                    return View(weatherConditions);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error interno del servidor: {ex.Message}";
                return View(weatherConditions);
            }
        }

        // Método para mostrar el formulario de edición
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"WeatherConditions/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var weatherConditions = JsonConvert.DeserializeObject<WeatherConditionViewModel>(content);
                return View(weatherConditions);
            }

            return NotFound();
        }

        // Método para manejar la solicitud de edición de condiciones climáticas
        [HttpPost]
        public async Task<IActionResult> Edit(WeatherConditionViewModel weatherConditions)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(weatherConditions), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PutAsync($"WeatherConditions/{weatherConditions.WeatherConditionId}", jsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error al guardar los cambios. Inténtalo de nuevo más tarde.";
                        return View(weatherConditions);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Por favor, corrige los errores del formulario.";
                    return View(weatherConditions);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error interno del servidor: {ex.Message}";
                return View(weatherConditions);
            }
        }

        // Método para desactivar las condiciones climáticas
        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"WeatherConditions/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al desactivar las condiciones climáticas. Inténtalo de nuevo más tarde.";
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
