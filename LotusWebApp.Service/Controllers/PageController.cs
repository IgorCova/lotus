using LotusWebApp.Data;
using LotusWebApp.Data.Models;
using Microsoft.AspNetCore.Identity;
using LotusWebApp.Authorization;
using LotusWebApp.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LotusWebApp.Controllers;

[Route("[controller]")]
[ApiController]
public class PageController(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext) : ControllerBase
{
    [Authorize]
    [HttpPost("new")]
    public async Task<ActionResult<Page>> CreatePage(NewPageDto pageDto)
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

        var page = new Page
        {
            Title = pageDto.Title,
            Author = pageDto.Author,
            Body = pageDto.Body,
            Userid = foundUser.Id
        };

        dbContext.Pages.Add(page);
        await dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPage), new { id = page.Id }, page);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PageDto>> GetPage(int id)
    {
        var page = await dbContext.Pages.FindAsync(id);

        if (page is null)
        {
            return NotFound();
        }

        var pageDto = new PageDto
        {
            Id = page.Id,
            Author = page.Author,
            Body = page.Body,
            Title = page.Title
        };

        return pageDto;
    }

    [HttpGet]
    public async Task<PagesDto> ListPages()
    {
        var pagesFromDb = await dbContext.Pages.ToListAsync();

        var pagesDto = new PagesDto();

        foreach (var page in pagesFromDb)
        {
            var pageDto = new PageDto
            {
                Id = page.Id,
                Author = page.Author,
                Body = page.Body,
                Title = page.Title
            };

            pagesDto.Pages.Add(pageDto);
        }

        return pagesDto;
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<bool>> DeletePage(int id)
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

        var page = await dbContext.Pages.FindAsync(id);

        if (page is null)
        {
            return NotFound();
        }

        if (page.Userid != foundUser.Id)
        {
            return Forbid("It's not your page, only author can delete this page");
        }


        dbContext.Pages.Remove(page);
        await dbContext.SaveChangesAsync();

        return Ok(true);
    }
}