using Microsoft.AspNetCore.Mvc;
using RecipeApp.Business.Repository.IRepository;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RecipeApi.Controllers;

[Route("api/recipes")]
[ApiController]
public class UserRatingsController : ControllerBase
{
    private readonly IUserRatingsRepository _userRatingsRepository;
    private readonly ILogger<UserRatingsController> _logger;

    public UserRatingsController(IUserRatingsRepository userRatingsRepository,
        ILogger<UserRatingsController> logger)
    {
        _userRatingsRepository = userRatingsRepository;
        _logger = logger;
    }

    // POST api/recipes/{recipeId}/rate
    [HttpPost]
    [Route("{recipeId}/rate")]
    public async Task<ActionResult<int?>> Post(int recipeId, [FromBody] int rating)
    {
        try
        {
            var addNewRating = new AddNewUserRatingModel(
                recipeId,
                GetUserId(),
                rating);

            var newRatingId = await _userRatingsRepository.AddNewRating(addNewRating);
            if (newRatingId is null) return BadRequest();
            return Ok(newRatingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Rating of Recipe failed");
            return BadRequest();
        }
    }

    // PUT api/recipes/{recipeId}/rate
    [HttpPut("{recipeId}")]
    public void Put(int recipeId, [FromBody] string value)
    {
    }

    private string GetUserId()
    {
        return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}