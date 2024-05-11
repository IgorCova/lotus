using LotusWebApp.Data.Models;
using Microsoft.AspNetCore.Identity;
using LotusWebApp.Authorization;
using LotusWebApp.Data.Entities;
using LotusWebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LotusWebApp.Controllers;

[Route("[controller]")]
[ApiController]
public class BillingController(UserManager<ApplicationUser> userManager, IBillingService billingService) : ControllerBase
{
    [Authorize]
    [HttpPost("add-money")]
    public async Task<ActionResult<Page>> AddMoney(decimal money, CancellationToken cancellationToken)
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

        await billingService.UserAddMoney(foundUser.Id, money, cancellationToken);
        return Ok();
    }

    [Authorize]
    [HttpPost("withdraw-money")]
    public async Task<ActionResult> WithdrawMoney(decimal money, CancellationToken cancellationToken)
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

        var result = await billingService.UserWithdrawsMoney(foundUser.Id, money, cancellationToken);
        if (result == false)
        {
            return BadRequest("Not enough money");
        }
        return Ok();
    }

    [Authorize]
    [HttpGet("balance")]
    public async Task<ActionResult> GetBalance(CancellationToken cancellationToken)
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

        var result = await billingService.UserBalance(foundUser.Id, cancellationToken);

        return Ok(result);
    }
}