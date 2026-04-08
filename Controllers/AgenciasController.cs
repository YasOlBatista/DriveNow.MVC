using DriveNow.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace DriveNow.MVC.Controllers
{
    public class AgenciasController : Controller
    {
        private readonly HttpClient _apiClient;

        public AgenciasController(IHttpClientFactory httpClientFactory)
        {
            _apiClient = httpClientFactory.CreateClient("DriveNowAPI");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _apiClient.GetAsync("Agencias");

            if (!response.IsSuccessStatusCode)
                return View(new List<AgenciaView>());

            var json = await response.Content.ReadAsStringAsync();

            var agencias = JsonSerializer.Deserialize<List<AgenciaView>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(agencias ?? new List<AgenciaView>());
        }

        public IActionResult CriarAgencia()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CriarAgencia(AgenciaView agencia)
        {

            agencia.Logradouro = "a";
            agencia.Localidade = "a";
            agencia.Bairro = "a";
            agencia.Uf = "a";

            ModelState.Remove("Uf");
            ModelState.Remove("Localidade");
            ModelState.Remove("Logradouro");
            ModelState.Remove("Bairro");
            if (!ModelState.IsValid)
                return View(agencia);

            var content = new StringContent(JsonSerializer.Serialize(agencia), System.Text.Encoding.UTF8, "application/json");
            var resp = await _apiClient.PostAsync("Agencias", content);

            if (resp.IsSuccessStatusCode)
                return RedirectToAction("Index");

            return View(agencia);
        }

        public async Task<IActionResult> EditarAgencia(int id)
        {
            var response = await _apiClient.GetAsync($"Agencias/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();

            var agencia = JsonSerializer.Deserialize<AgenciaView>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(agencia);
        }

        [HttpPost]
        public async Task<IActionResult> EditarAgencia(AgenciaView agencia)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(agencia),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _apiClient.PutAsync($"Agencias/{agencia.Id}", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            return View(agencia);
        }

        public async Task<IActionResult> DeletarAgencia(int id)
        {
            var resposta = await _apiClient.GetAsync($"Agencias/{id}");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                var agencia = JsonSerializer.Deserialize<AgenciaView>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(agencia);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeletarAgenciaConfirmado(int id)
        {
            var resposta = await _apiClient.DeleteAsync($"Agencias/{id}");
            if (resposta.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction(nameof(DeletarAgencia), new { id });
        }
    }
}