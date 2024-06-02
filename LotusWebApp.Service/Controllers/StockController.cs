using LotusWebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LotusWebApp.Controllers;

[Route("[controller]")]
[ApiController]
public class StockController(IStockService stockService): ControllerBase
{
    [HttpPut("add")]
    public async Task<ActionResult> Add(int idSubscription, int count)
    {
        var result = await stockService.Add(idSubscription, count);
        return result switch
        {
            true => Ok(),
            _ => BadRequest($"cant find idSubscription: {idSubscription}")
        };
    }
}