using LotusWebApp.Data.Enums;
using Microsoft.AspNetCore.Identity;

namespace LotusWebApp.Data.Models;

public class ApplicationUser: IdentityUser
{
    public Role Role { get; set; }
}