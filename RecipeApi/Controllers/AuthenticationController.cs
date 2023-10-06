using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RecipeApi.Areas.Identity.Data;
using RecipeApp.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RecipeApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _config;

    public AuthenticationController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration _config
        )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        this._config = _config;
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

    [HttpPost("token")]
    [AllowAnonymous]
    public async Task<ActionResult<string>> Authenticate([FromBody] AuthenticationData data)
    {
        if (data?.UserName is null || data.Password is null) return BadRequest();

        var user = await _userManager.FindByNameAsync(data.UserName);
        if (user is null) return BadRequest();

        var passwordHasher = new PasswordHasher<ApplicationUser>();
        var passwordVerificationResult = passwordHasher
            .VerifyHashedPassword(user, user.PasswordHash, data.Password);

        var userData = new UserData(
            user.Id,
            user.UserName,
            user.Email,
            user.FirstName,
            user.LastName
            );

        var token = "";

        switch (passwordVerificationResult)
        {
            case PasswordVerificationResult.Success:

                token = GenerateToken(userData);
                return Ok(token);

            case PasswordVerificationResult.Failed:
                return BadRequest();

            case PasswordVerificationResult.SuccessRehashNeeded:
                string newPasswordHash = passwordHasher.HashPassword(user, data.Password);
                user.PasswordHash = newPasswordHash;
                await _userManager.UpdateAsync(user);
                token = GenerateToken(userData);
                return Ok(token);

            default:
                return BadRequest();
        }
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

    private string GenerateToken(UserData user)
    {
        var secretKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(_config.GetValue<string>("Authentication:SecretKey")));

        var signingCredentials = new SigningCredentials(secretKey,
            SecurityAlgorithms.HmacSha256);

        List<Claim> claims = new();
        claims.Add(new(JwtRegisteredClaimNames.Sub, user.UserId));
        claims.Add(new(JwtRegisteredClaimNames.UniqueName, user.UserName));
        claims.Add(new(JwtRegisteredClaimNames.GivenName, user.FirstName));
        claims.Add(new(JwtRegisteredClaimNames.FamilyName, user.LastName));
        claims.Add(new(JwtRegisteredClaimNames.Email, user.Email));

        var jwtExpiryInMinutes = _config.GetValue<int>("Authentication:JwtExpiryInMinutes");

        var token = new JwtSecurityToken(
            _config.GetValue<string>("Authentication:Issuer"),
            _config.GetValue<string>("Authentication:Audience"),
            claims,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(jwtExpiryInMinutes),
            signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record NewUserRegistation(
    string UserName,
    string Email,
    string Password,
    string FirstName,
    string LastName,
    bool isAdmin);

public record AuthenticationData(string UserName, string Password);

public record UserData(
    string UserId,
    string UserName,
    string Email,
    string FirstName,
    string LastName);