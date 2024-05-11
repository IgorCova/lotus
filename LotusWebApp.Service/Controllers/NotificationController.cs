using LotusWebApp.Data.Models;
using Microsoft.AspNetCore.Identity;
using LotusWebApp.Authorization;
using LotusWebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LotusWebApp.Controllers;

[Route("[controller]")]
[ApiController]
public class NotificationController(UserManager<ApplicationUser> userManager, INotificationService notificationService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<ActionResult> GetNotifications(CancellationToken cancellationToken)
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

        var result = await notificationService.GetUserNotifications(foundUser.Id, cancellationToken);

        return Ok(result);
    }
}