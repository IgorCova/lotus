using LotusWebApp.Data;
using LotusWebApp.Data.Enums;
using LotusWebApp.Data.Models;
using LotusWebApp.Services;
using Microsoft.AspNetCore.Identity;
using LotusWebApp.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LotusWebApp.Controllers;

[Route("[controller]")]
[ApiController]
public class ProfileController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, TokenService tokenService) : ControllerBase
{
    /// <summary>
    /// Register user
    /// </summary>
    /// <param name="request">User data with login and password</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns></returns>
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(RegistrationRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await userManager.CreateAsync(
            new ApplicationUser { UserName = request.Username, Email = request.Email, Role = Role.User },
            request.Password!
        );

        if (result.Succeeded)
        {
            request.Password = "";
            return CreatedAtAction(nameof(Register), new { email = request.Email, role = request.Role }, request);
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(error.Code, error.Description);
        }

        return BadRequest(ModelState);
    }

    /// <summary>
    /// Login user
    /// </summary>
    /// <param name="request">login and password</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns></returns>
    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<AuthResponse>> Authenticate([FromBody] AuthRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var managedUser = await userManager.FindByEmailAsync(request.Email!);
        if (managedUser == null)
        {
            return BadRequest("Bad credentials");
        }

        var isPasswordValid = await userManager.CheckPasswordAsync(managedUser, request.Password!);
        if (!isPasswordValid)
        {
            return BadRequest("Bad credentials");
        }

        var userInDb = context.Users.FirstOrDefault(u => u.Email == request.Email);

        if (userInDb is null)
        {
            return Unauthorized();
        }

        var accessToken = tokenService.CreateToken(userInDb);
        await context.SaveChangesAsync(cancellationToken);

        return Ok(new AuthResponse
        {
            Email = userInDb.Email ?? "no email",
            Token = accessToken,
        });
    }

    /// <summary>
    /// Get profile
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize]
    [HttpGet]
    public Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var user = HttpContext.Items["User"];
        return Task.FromResult<IActionResult>(Ok(user));
    }

    /// <summary>
    /// Update profile
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        if (HttpContext.Items["User"] is not ApplicationUser user)
        {
            return NotFound("User not found");
        }

        var foundUser = await userManager.FindByIdAsync(user.Id);

        if (foundUser == null)
        {
            return NotFound("User not found");
        }

        foundUser.UserName = request.Username;

        var result = await userManager.UpdateAsync(foundUser);

        if (result.Succeeded)
        {
            return BadRequest("User not updated");
        }

        return Ok();
    }
}