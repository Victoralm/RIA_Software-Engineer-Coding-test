using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class TestExceptionController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        throw new Exception("Exceção de teste!");
    }
}
