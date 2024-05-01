using LotusWebApp.Data;
using Microsoft.AspNetCore.Mvc;

namespace LotusWebApp.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController(IDataProviderService dataProviderService) : ControllerBase
{
    [HttpGet("all")]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var result = await dataProviderService.GetAllUsers(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Create user
    /// </summary>
    /// <param name="user">User data</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserDto user, CancellationToken cancellationToken)
    {
        var result = await dataProviderService.CreateUser(user.Name, user.Role, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Update user
    /// </summary>
    /// <param name="userId">Id user</param>
    /// <param name="user">User data</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UserDto user, CancellationToken cancellationToken)
    {
        var result = await dataProviderService.UpdateUser(userId, user.Name, user.Role, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetUser(Guid userId, CancellationToken cancellationToken)
    {
        var result = await dataProviderService.GetUser(userId, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> DeleteUser(Guid userId, CancellationToken cancellationToken)
    {
        var result = await dataProviderService.DeleteUser(userId, cancellationToken);
        return Ok(result);
    }
}

public class UserDto
{
    public string Name { get; set; }
    public string Role { get; set; }
}