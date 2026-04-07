using DriveNow.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Consultorio.Mvc.Controllers
{
    public class VeiculosController : Controller
    {
        private readonly HttpClient _apiClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public VeiculosController(IHttpClientFactory httpClientFactory)
        {
            _apiClient = httpClientFactory.CreateClient("DriveNow.API"); 
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }
        public async Task<IActionResult> Index()
        {
            var res = await _apiClient.GetAsync("https://localhost:7224/api/Veiculos/{id}");
            if (res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync();
                var veiculos = System.Text.Json.JsonSerializer.Deserialize<List<VeiculoView>>(json, _jsonOptions);
                return View(veiculos);
            }
            return View(new List<VeiculoView>());
        }

        public async Task<IActionResult> CriarVeiculo()
        {
            var response = await _apiClient.GetAsync("https://localhost:7224/api/Agencias");
            if (!response.IsSuccessStatusCode)
            {
                return View();
            }

            var json = await response.Content.ReadAsStringAsync();

            var agencias = System.Text.Json.JsonSerializer.Deserialize<List<AgenciaView>>(json, _jsonOptions);

            ViewBag.Agencias = agencias;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CriarVeiculo(VeiculoView v)
        {
            if (!ModelState.IsValid) return View(v);

            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(v), System.Text.Encoding.UTF8, "application/json");
            var resp = await _apiClient.PostAsync("api/Veiculos", content);

            if (resp.IsSuccessStatusCode) return RedirectToAction("Index");
            ModelState.AddModelError("", "Erro ao cadastrar o veículo");

            var response = await _apiClient.GetAsync("api/Agencias");

            if (!response.IsSuccessStatusCode)
            {
                return View();
            }

            var json = await response.Content.ReadAsStringAsync();

            var agencias = System.Text.Json.JsonSerializer.Deserialize<List<AgenciaView>>(json, _jsonOptions);

            ViewBag.Agencias = agencias;
            return View(v);
        }


        public async Task<IActionResult> DeletarVeiculo(int id)
        {
            var resposta = await _apiClient.GetAsync($"api/Veiculos/{id}");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                var veiculo = System.Text.Json.JsonSerializer.Deserialize<VeiculoView>(json, _jsonOptions);
                return View(veiculo);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Deletar(int id)
        {
            var resposta = await _apiClient.DeleteAsync($"api/Veiculos/{id}");
            if (resposta.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction(nameof(DeletarVeiculo), new { id });

        }
        public async Task<ActionResult> EditarVeiculo(int id)
        {
            var resposta = await _apiClient.GetAsync($"api/Veiculos/{id}");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                var veiculo = System.Text.Json.JsonSerializer.Deserialize<VeiculoView>(json, _jsonOptions);
                return View(veiculo);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditarVeiculo(VeiculoView v, int id)
        {
            if (id != v.Id) return BadRequest();
            if (!ModelState.IsValid) return View(v);

            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(v), System.Text.Encoding.UTF8, "application/json");

            var resp = await _apiClient.PutAsync($"api/Medicos/{id}", content);

            if (resp.IsSuccessStatusCode) return RedirectToAction("Index");

            ModelState.AddModelError("", "Erro no cadastro");

            return View(v);

        }
    }
}
