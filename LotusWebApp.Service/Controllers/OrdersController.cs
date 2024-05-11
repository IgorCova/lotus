using LotusWebApp.Data.Models;
using Microsoft.AspNetCore.Identity;
using LotusWebApp.Authorization;
using LotusWebApp.Data.Entities;
using LotusWebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LotusWebApp.Controllers;

public class Subscription
{
    public int Id { get; init; }
    public string Name { get; set; } = "Subscription";
    public decimal Cost { get; init; }
}

[Route("[controller]")]
[ApiController]
public class OrderController(UserManager<ApplicationUser> userManager, IOrderService orderService) : ControllerBase
{
    private readonly List<Subscription> _subscriptions =
    [
        new Subscription
        {
            Id = 1,
            Name = "daily",
            Cost = 10
        },
        new Subscription
        {
            Id = 2,
            Name = "monthly",
            Cost = 95
        },
        new Subscription
        {
            Id = 3,
            Name = "yearly",
            Cost = 1200
        }
    ];

    [Authorize]
    [HttpPost("pay")]
    public async Task<ActionResult<Page>> Pay(int idSubscription, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (HttpContext.Items["User"] is not ApplicationUser user)
        {
            return NotFound("User not found");
        }

        var foundUser = await userManager.FindByIdAsync(user.Id);

        if (foundUser == null)
        {
            return NotFound("User not found");
        }

        var subscription = _subscriptions.FirstOrDefault(x => x.Id == idSubscription);

        if (subscription == null)
        {
            return NotFound($"Subscription {idSubscription} not found");
        }

        var orderResult = await orderService.MakeAnOrder(foundUser.Id, subscription.Cost, cancellationToken);

        if (orderResult == false)
        {
            return BadRequest("Not enough money");
        }

        return Ok();
    }

    [HttpGet("subscriptions")]
    public ActionResult GetSubscriptions()
    {
        return Ok(_subscriptions);
    }
}