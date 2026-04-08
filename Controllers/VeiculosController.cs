using DriveNow.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DriveNow.MVC.Controllers
{
    public class VeiculosController : Controller
    {
        private readonly HttpClient _apiClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public VeiculosController(IHttpClientFactory httpClientFactory)
        {
            _apiClient = httpClientFactory.CreateClient("DriveNowAPI");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }
        public async Task<IActionResult> Index()
        {
            var res = await _apiClient.GetAsync("api/Veiculos");
            if (!res.IsSuccessStatusCode)
            {
                return View(new List<VeiculoView>());
            }
            var json = await res.Content.ReadAsStringAsync();
            var veiculos = JsonSerializer.Deserialize<List<VeiculoView>>(json, _jsonOptions);

            return View(veiculos);

        }

        public async Task<IActionResult> CriarVeiculo()
        {
            var res = await _apiClient.GetAsync("Agencias");
            if (!res.IsSuccessStatusCode)
            {
                return NotFound("Erro ao conectar-se a API");
            }
            var json = await res.Content.ReadAsStringAsync();
            var agencias = JsonSerializer.Deserialize<List<AgenciaView>>(json, _jsonOptions);
            ViewBag.Agencias = agencias;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CriarVeiculo(VeiculoView v)
        {
            if (!ModelState.IsValid)
            {
                return View(v);
            }

            var content = new StringContent(
                JsonSerializer.Serialize(v),
                Encoding.UTF8,
                "application/json"
            );

            var resp = await _apiClient.PostAsync("api/Veiculos", content);

            if (resp.IsSuccessStatusCode) return RedirectToAction("Index");

            ModelState.AddModelError("", "Erro ao cadastrar o veículo");

            return View(v);
        }


        public async Task<IActionResult> DeletarVeiculo(int id)
        {
            var resposta = await _apiClient.GetAsync($"api/veiculos/{id}");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                var veiculo = JsonSerializer.Deserialize<VeiculoView>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(veiculo);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeletarVeiculoConfirmado(int id)
        {
            var resposta = await _apiClient.DeleteAsync($"api/veiculos/{id}");
            if (resposta.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction(nameof(DeletarVeiculo), new { id });
        }

        public async Task<IActionResult> EditarVeiculo(int id)
        {
            var resposta = await _apiClient.GetAsync($"api/veiculos/{id}");

            if (!resposta.IsSuccessStatusCode) return NotFound();

            var json = await resposta.Content.ReadAsStringAsync();
            var veiculo = JsonSerializer.Deserialize<VeiculoView>(json, new JsonSerializerOptions());

            var responseAgencias = await _apiClient.GetAsync("api/Agencias");

            if (responseAgencias.IsSuccessStatusCode)
            {
                var jsonAgencias = await responseAgencias.Content.ReadAsStringAsync();
                var agencias = JsonSerializer.Deserialize<List<AgenciaView>>(jsonAgencias, _jsonOptions);

                ViewBag.Agencias = new SelectList(agencias, "Id", "Nome");
            }
            else
            {
                ViewBag.Agencias = new SelectList(new List<AgenciaView>(), "Id", "Nome");
            }

            return View(veiculo);
        }

        [HttpPost]
        public async Task<IActionResult> EditarVeiculo(VeiculoView v, int id)
        {
            if (id != v.Id) return BadRequest();

            var responseAgencias = await _apiClient.GetAsync("api/Agencias");

            if (responseAgencias.IsSuccessStatusCode)
            {
                var jsonAgencias = await responseAgencias.Content.ReadAsStringAsync();
                var agencias = JsonSerializer.Deserialize<List<AgenciaView>>(jsonAgencias, _jsonOptions);

                ViewBag.Agencias = new SelectList(agencias, "Id", "Nome");
            }
            else
            {
                ViewBag.Agencias = new SelectList(new List<AgenciaView>(), "Id", "Nome");
            }

            if (!ModelState.IsValid) return View(v);

            var content = new StringContent(
                JsonSerializer.Serialize(v),
                Encoding.UTF8,
                "application/json"
            );

            var resp = await _apiClient.PutAsync($"api/veiculos/{id}", content);

            if (resp.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Erro ao atualizar veículo");

            return View(v);
        }
    } 
}
