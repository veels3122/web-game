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
    public class SoilQualitysController : Controller
    {
        private readonly HttpClient _httpClient;

        public SoilQualitysController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://www.guayabagame.somee.com/api/"); // Reemplaza con la URL de tu API publicada
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var response = await _httpClient.GetAsync("SoilQualitys");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var soilQualitys = JsonConvert.DeserializeObject<IEnumerable<SoilQualityViewModel>>(content);

                if (!string.IsNullOrEmpty(searchString))
                {
                    // Filtrar por algún campo relevante
                    soilQualitys = soilQualitys.Where(s => s.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) || s.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase));
                }

                return View(soilQualitys);
            }

            return View(new List<SoilQualityViewModel>());
        }

        // Método para mostrar el formulario de creación
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(SoilQualityViewModel soilQualitys)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(soilQualitys), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync("SoilQualitys", jsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error al crear la calidad del suelo. Inténtalo de nuevo más tarde.";
                        return View(soilQualitys);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Por favor, corrige los errores del formulario.";
                    return View(soilQualitys);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error interno del servidor: {ex.Message}";
                return View(soilQualitys);
            }
        }

        // Método para mostrar el formulario de edición
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"SoilQualitys/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var soilQualitys = JsonConvert.DeserializeObject<SoilQualityViewModel>(content);
                return View(soilQualitys);
            }

            return NotFound();
        }

        // Método para manejar la solicitud de edición de la calidad del suelo
        [HttpPost]
        public async Task<IActionResult> Edit(SoilQualityViewModel soilQualitys)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(soilQualitys), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PutAsync($"SoilQualitys/{soilQualitys.SoilQualityId}", jsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error al guardar los cambios. Inténtalo de nuevo más tarde.";
                        return View(soilQualitys);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Por favor, corrige los errores del formulario.";
                    return View(soilQualitys);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error interno del servidor: {ex.Message}";
                return View(soilQualitys);
            }
        }

        // Método para desactivar la calidad del suelo
        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"SoilQualitys/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al desactivar la calidad del suelo. Inténtalo de nuevo más tarde.";
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
