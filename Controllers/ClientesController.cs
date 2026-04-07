using DriveNow.MVC.Models;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace DriveNow.MVC.Controllers
{
    public class ClientesController : Controller
    {
        private readonly HttpClient _httpClient;

        public ClientesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("DriveNow.API");
        }


        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("https://localhost:7224/api/Clientes");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var clientes = JsonSerializer.Deserialize<List<ClienteView>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(clientes);
            }

            return View(new List<ClienteView>());
        }


        public IActionResult CriarCliente()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> CriarCliente(ClienteView cliente)
        {
            if (!ModelState.IsValid)
                return View(cliente);

            var json = JsonSerializer.Serialize(cliente);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/clientes", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Erro ao criar cliente");
            return View(cliente);
        }


        public async Task<IActionResult> DeletarCliente(int id)
        {
            var resposta = await _httpClient.GetAsync($"api/clientes/{id}");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                var cliente = JsonSerializer.Deserialize<ClienteView>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(cliente);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Deletar(int id)
        {
            var resposta = await _httpClient.DeleteAsync($"api/clientes/{id}");
            if (resposta.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction(nameof(DeletarCliente), new { id });
        }

        public async Task<IActionResult> EditarCliente(int id)
        {
            var resposta = await _httpClient.GetAsync($"api/clientes/{id}");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                var cliente = JsonSerializer.Deserialize<ClienteView>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(cliente);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditarCliente(ClienteView c, int id)
        {
            if (id != c.Id) return BadRequest();
            if (!ModelState.IsValid) return View(c);

            var content = new StringContent(
                JsonSerializer.Serialize(c),
                Encoding.UTF8,
                "application/json"
            );

            var resp = await _httpClient.PutAsync($"api/clientes/{id}", content);

            if (resp.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Erro ao atualizar cliente");

            return View(c);
        }
    }
}