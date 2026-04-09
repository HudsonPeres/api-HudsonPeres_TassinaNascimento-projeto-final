using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace api_HudsonPeres_TassinaNascimento_projeto_final.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IHttpClientFactory httpClientFactory, ILogger<PaymentsController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
    {
        var client = _httpClientFactory.CreateClient("ResilientClient");
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("http://localhost:8000/payments", content);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<object>(json);
            return Ok(data);
        }

        _logger.LogWarning("Falha ao processar pagamento. Status: {StatusCode}", response.StatusCode);
        return StatusCode((int)response.StatusCode, "Erro ao processar pagamento.");
    }
}

public class PaymentRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EUR";
    public string CardNumber { get; set; } = string.Empty;
}