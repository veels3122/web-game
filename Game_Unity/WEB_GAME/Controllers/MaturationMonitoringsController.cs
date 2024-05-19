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
    public class MaturationMonitoringsController : Controller
    {
        private readonly HttpClient _httpClient;

        public MaturationMonitoringsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://www.guayabagame.somee.com/api/"); // Reemplaza con la URL de tu API publicada
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var response = await _httpClient.GetAsync("MaturationMonitorings");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var maturationMonitorings = JsonConvert.DeserializeObject<IEnumerable<MaturationMonitoringViewModel>>(content);

                // Puedes agregar filtros adicionales aquí si es necesario

                return View(maturationMonitorings);
            }

            return View(new List<MaturationMonitoringViewModel>());
        }

        // Método para mostrar el formulario de creación
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MaturationMonitoringViewModel maturationMonitoring)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(maturationMonitoring), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync("MaturationMonitorings", jsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error al crear el monitoreo de maduración. Inténtalo de nuevo más tarde.";
                        return View(maturationMonitoring);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Por favor, corrige los errores del formulario.";
                    return View(maturationMonitoring);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error interno del servidor: {ex.Message}";
                return View(maturationMonitoring);
            }
        }

        // Método para mostrar el formulario de edición
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"MaturationMonitorings/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var maturationMonitoring = JsonConvert.DeserializeObject<MaturationMonitoringViewModel>(content);
                return View(maturationMonitoring);
            }

            return NotFound();
        }

        // Método para manejar la solicitud de edición del monitoreo de maduración
        [HttpPost]
        public async Task<IActionResult> Edit(MaturationMonitoringViewModel maturationMonitoring)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(maturationMonitoring), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PutAsync($"MaturationMonitorings/{maturationMonitoring.MaturationMonitoringId}", jsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error al guardar los cambios. Inténtalo de nuevo más tarde.";
                        return View(maturationMonitoring);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Por favor, corrige los errores del formulario.";
                    return View(maturationMonitoring);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error interno del servidor: {ex.Message}";
                return View(maturationMonitoring);
            }
        }

        // Método para desactivar el monitoreo de maduración
        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"MaturationMonitorings/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al desactivar el monitoreo de maduración. Inténtalo de nuevo más tarde.";
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
