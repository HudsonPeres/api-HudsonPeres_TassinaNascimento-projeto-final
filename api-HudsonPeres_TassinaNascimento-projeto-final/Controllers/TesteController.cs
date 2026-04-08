using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TesteController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new {mensagem = "API funcionando"});
    }
}