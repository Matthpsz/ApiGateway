using Microsoft.AspNetCore.Mvc;

namespace Microservico.TesteAPI.Controllers;

[ApiController]
[Route("api/produtos")]
public class ProdutosController : ControllerBase
{
    [HttpGet]
    public IActionResult ListarProdutos()
    {
        var produtos = new[]
        {
            new { Id = 1, Nome = "Teclado Mecânico", Preco = 350.00 },
            new { Id = 2, Nome = "Mouse Gamer", Preco = 220.00 }
        };

        return Ok(produtos);
    }
}
