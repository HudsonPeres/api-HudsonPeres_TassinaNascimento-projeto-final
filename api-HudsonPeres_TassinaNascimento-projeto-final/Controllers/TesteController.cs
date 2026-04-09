using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[Authorize]
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