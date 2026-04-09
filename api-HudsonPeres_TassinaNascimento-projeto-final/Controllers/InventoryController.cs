using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace api_HudsonPeres_TassinaNascimento_projeto_final.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(IHttpClientFactory httpClientFactory, ILogger<InventoryController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [HttpGet("{sku}")]
    public async Task<IActionResult> GetInventory(string sku)
    {
        // Cria um HttpClient com as políticas de resiliência (retry + circuit breaker)
        var client = _httpClientFactory.CreateClient("ResilientClient");
        var response = await client.GetAsync($"http://localhost:8000/inventory/{sku}");

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<object>(json);
            return Ok(data);
        }

        _logger.LogWarning("Falha ao consultar inventário para SKU {Sku}. Status: {StatusCode}", sku, response.StatusCode);
        return StatusCode((int)response.StatusCode, "Erro ao consultar inventário.");
    }
}