using DriveNow.MVC.Models;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace DriveNow.MVC.Controllers
{
    public class VeiculosController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _webHost;

        public VeiculosController(
            IHttpClientFactory httpClientFactory,
            IWebHostEnvironment webHost)
        {
            _httpClient = httpClientFactory.CreateClient("DriveNowAPI");
            _webHost = webHost;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/veiculos");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                var veiculos = JsonSerializer.Deserialize<List<VeiculoView>>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                return View(veiculos);
            }

            return View(new List<VeiculoView>());
        }

        public IActionResult CriarVeiculo()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CriarVeiculo(VeiculoView veiculo)
        {
            if (!ModelState.IsValid)
                return View(veiculo);

            if (veiculo.FotoUpload != null)
            {
                string pastaImagens = Path.Combine(
                    _webHost.WebRootPath,
                    "imagens",
                    "veiculos"
                );

                if (!Directory.Exists(pastaImagens))
                    Directory.CreateDirectory(pastaImagens);

                string extensao = Path.GetExtension(
                    veiculo.FotoUpload.FileName
                );

                string nomeArquivo =
                    Guid.NewGuid().ToString() + extensao;

                string caminhoCompleto = Path.Combine(
                    pastaImagens,
                    nomeArquivo
                );

                using (var stream = new FileStream(
                    caminhoCompleto,
                    FileMode.Create))
                {
                    await veiculo.FotoUpload.CopyToAsync(stream);
                }

                veiculo.FotoUrl =
                    "/imagens/veiculos/" + nomeArquivo;
            }

            var json = JsonSerializer.Serialize(veiculo);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                "api/veiculos",
                content
            );

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Erro ao criar veículo");

            return View(veiculo);
        }

        public async Task<IActionResult> DeletarVeiculo(int id)
        {
            var resposta = await _httpClient.GetAsync(
                $"api/veiculos/{id}"
            );

            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();

                var veiculo = JsonSerializer.Deserialize<VeiculoView>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                return View(veiculo);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Deletar(int id)
        {
            var resposta = await _httpClient.DeleteAsync(
                $"api/veiculos/{id}"
            );

            if (resposta.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction(
                nameof(DeletarVeiculo),
                new { id });
        }

        public async Task<IActionResult> EditarVeiculo(int id)
        {
            var resposta = await _httpClient.GetAsync(
                $"api/veiculos/{id}"
            );

            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();

                var veiculo = JsonSerializer.Deserialize<VeiculoView>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                return View(veiculo);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditarVeiculo(
            VeiculoView v,
            int id)
        {
            if (id != v.Id) return BadRequest();
            if (!ModelState.IsValid) return View(v);

            if (v.FotoUpload != null)
            {
                string pastaImagens = Path.Combine(
                    _webHost.WebRootPath,
                    "imagens",
                    "veiculos"
                );

                if (!Directory.Exists(pastaImagens))
                    Directory.CreateDirectory(pastaImagens);

                string extensao = Path.GetExtension(
                    v.FotoUpload.FileName
                );

                string nomeArquivo =
                    Guid.NewGuid().ToString() + extensao;

                string caminhoCompleto = Path.Combine(
                    pastaImagens,
                    nomeArquivo
                );

                using (var stream = new FileStream(
                    caminhoCompleto,
                    FileMode.Create))
                {
                    await v.FotoUpload.CopyToAsync(stream);
                }

                v.FotoUrl =
                    "/imagens/veiculos/" + nomeArquivo;
            }

            var content = new StringContent(
                JsonSerializer.Serialize(v),
                Encoding.UTF8,
                "application/json"
            );

            var resp = await _httpClient.PutAsync(
                $"api/veiculos/{id}",
                content
            );

            if (resp.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Erro ao atualizar veículo");

            return View(v);
        }
    }
}