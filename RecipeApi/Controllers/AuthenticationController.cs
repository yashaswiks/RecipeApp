using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecipeApi.Areas.Identity.Data;
using RecipeApp.Common;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RecipeApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthenticationController(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpPost("CreateUser")]
    [AllowAnonymous]
    public async Task<ActionResult<bool>> CreateUser([FromBody] NewUserRegistation data)
    {
        if (data is null) return BadRequest();

        var createUserData = new ApplicationUser()
        {
            UserName = data.UserName,
            FirstName = data.FirstName,
            Email = data.Email,
            LastName = data.LastName,
            UserCreatedOn = DateTime.Now
        };

        var result = await _userManager.CreateAsync(createUserData, data.Password);

        if (result?.Succeeded is true)
        {
            var newlyCreatedUserDetails = await _userManager
                .FindByNameAsync(createUserData.UserName);

            if (newlyCreatedUserDetails is null) return BadRequest();

            await AddRoles(_roleManager);

            var addRoleToUser = await _userManager
                .AddToRoleAsync(
                newlyCreatedUserDetails, data.isAdmin ? UserRoles.Admin : UserRoles.User);

            if (!addRoleToUser.Succeeded) return BadRequest();

            return Ok();
        }

        return BadRequest();
    }

    private static async Task AddRoles(RoleManager<IdentityRole> _roleManager)
    {
        if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
        {
            var role = new IdentityRole(UserRoles.Admin);
            await _roleManager.CreateAsync(role);
        }

        if (!await _roleManager.RoleExistsAsync(UserRoles.User))
        {
            var role = new IdentityRole(UserRoles.User);
            await _roleManager.CreateAsync(role);
        }
    }
}

public record NewUserRegistation(
    string UserName,
    string Email,
    string Password,
    string FirstName,
    string LastName,
    bool isAdmin);