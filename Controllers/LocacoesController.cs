using DriveNow.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DriveNow.MVC.Controllers
{
    public class LocacoesController : Controller
    {
        private readonly HttpClient _apiClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public LocacoesController(IHttpClientFactory httpClientFactory)
        {
            _apiClient = httpClientFactory.CreateClient("DriveNow");
            _apiClient.BaseAddress = new Uri("https://localhost:7262/");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }
        public async Task<IActionResult> Index()
        {
            var res = await _apiClient.GetAsync("/api/Locacoes");
            if (res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync();
                var locacoes = JsonSerializer.Deserialize<List<LocacaoView>>(json, _jsonOptions);
                return View(locacoes);
            }
            return View(new List<LocacaoView>());
        }

        public async Task<IActionResult> CriarLocacao()
        {
            var clienteResponse = await _apiClient.GetAsync("api/Clientes");
            var veiculoResponse = await _apiClient.GetAsync("api/Veiculos");

            List<ClienteView> clientes = new();
            List<VeiculoView> veiculos = new();

            if (clienteResponse.IsSuccessStatusCode)
            {
                var content = await clienteResponse.Content.ReadAsStringAsync();
                clientes = JsonSerializer.Deserialize<List<ClienteView>>(content, _jsonOptions) ?? new();
            }

            if (veiculoResponse.IsSuccessStatusCode)
            {
                var content = await veiculoResponse.Content.ReadAsStringAsync();
                veiculos = JsonSerializer.Deserialize<List<VeiculoView>>(content, _jsonOptions) ?? new();
            }

            ViewBag.Clientes = clientes;
            ViewBag.Veiculos = veiculos;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CriarLocacao(LocacaoView p)
        {
            if (!ModelState.IsValid) return View(p);
            var content = new StringContent(JsonSerializer.Serialize(p), System.Text.Encoding.UTF8, "application/json");
            var resp = await _apiClient.PostAsync("api/Locacoes", content);
            if (resp.IsSuccessStatusCode) return RedirectToAction("Index");
            ModelState.AddModelError("", "Erro ao cadastrar a locação");
            return View(p);
        }

        public async Task<IActionResult> DeletarLocacao(int id)
        {
            var resposta = await _apiClient.GetAsync($"api/Locacoes/{id}");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                var locacao = JsonSerializer.Deserialize<LocacaoView>(json, _jsonOptions);
                return View(locacao);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Deletar(int id)
        {
            var resposta = await _apiClient.DeleteAsync($"api/Locacoes/{id}");
            if (resposta.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction(nameof(DeletarLocacao), new { id });

        }
        public async Task<IActionResult> EditarLocacao(int id)
        {
            var resposta = await _apiClient.GetAsync($"api/Locacoes/{id}");
            if (!resposta.IsSuccessStatusCode)
                return NotFound();

            var json = await resposta.Content.ReadAsStringAsync();
            var locacao = JsonSerializer.Deserialize<LocacaoView>(json, _jsonOptions);

            var clienteResponse = await _apiClient.GetAsync("api/Clientes");
            var veiculoResponse = await _apiClient.GetAsync("api/Veiculos");

            ViewBag.Clientes = new List<ClienteView>();
            ViewBag.Veiculos = new List<VeiculoView>();

            if (clienteResponse.IsSuccessStatusCode)
            {
                var content = await clienteResponse.Content.ReadAsStringAsync();
                ViewBag.Clientes = JsonSerializer.Deserialize<List<ClienteView>>(content, _jsonOptions) ?? new();
            }

            if (veiculoResponse.IsSuccessStatusCode)
            {
                var content = await veiculoResponse.Content.ReadAsStringAsync();
                ViewBag.Veiculos = JsonSerializer.Deserialize<List<VeiculoView>>(content, _jsonOptions) ?? new();
            }

            return View(locacao);
        }

        [HttpPost]
        public async Task<IActionResult> EditarLocacao(int id, LocacaoView locacao)
        {
            if (id != locacao.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await CriarLocacao();
                return View(locacao);
            }

            var content = new StringContent(
                JsonSerializer.Serialize(locacao),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var resp = await _apiClient.PutAsync($"api/Locacoes/{id}", content);

            if (resp.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Erro ao editar");

            await CriarLocacao();
            return View(locacao);
        }
    }
}
