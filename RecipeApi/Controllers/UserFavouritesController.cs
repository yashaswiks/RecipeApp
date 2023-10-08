using Microsoft.AspNetCore.Mvc;
using RecipeApp.Business.Repository.IRepository;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RecipeApi.Controllers;

[Route("api")]
[ApiController]
public class UserFavouritesController : ControllerBase
{
    private readonly IUserFavouritesRepository _userFavouritesRepository;

    public UserFavouritesController(IUserFavouritesRepository userFavouritesRepository)
    {
        _userFavouritesRepository = userFavouritesRepository;
    }

    // GET api/users/{userId}/favourites
    [HttpGet("users/favourites")]
    public async Task<ActionResult<List<UserFavouritesModel>>> Get()
    {
        var userId = GetUserId();
        var data = await _userFavouritesRepository
            .GetByUserAsync(userId);

        if (data is null) return NotFound();

        return Ok(data);
    }

    // POST api/recipes/{recipeId}/favourite
    [HttpPost("recipes/{recipeId}/favourite")]
    public async Task<ActionResult<bool?>> Post(int recipeId)
    {
        var userId = GetUserId();

        var isFavouriteMarked = await _userFavouritesRepository
            .AddNewUserFavouriteAsync(recipeId, userId);
        if (isFavouriteMarked is null) return BadRequest();
        return Ok(isFavouriteMarked);
    }

    // DELETE api/<RecipesController>/5
    [HttpDelete("recipes/{recipeId}/favourite")]
    public async Task<ActionResult<bool?>> Delete(int recipeId)
    {
        var userId = GetUserId();

        var isUserFavouriteDeleted = await _userFavouritesRepository
            .DeleteAsync(recipeId, userId);

        if (isUserFavouriteDeleted is null) return BadRequest();
        return Ok(isUserFavouriteDeleted);
    }

    private string GetUserId()
    {
        return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}