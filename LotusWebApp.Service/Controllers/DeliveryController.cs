using LotusWebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LotusWebApp.Controllers;

[Route("[controller]")]
[ApiController]
public class DeliveryController(IDeliveryService deliveryService): ControllerBase
{
    [HttpPut("accept")]
    public async Task<ActionResult> Add(Guid idOrder)
    {
        var result = await deliveryService.Accept(idOrder);
        return result switch
        {
            true => Ok(),
            _ => BadRequest($"cant find idOrder: {idOrder}")
        };
    }
}