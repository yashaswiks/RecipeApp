using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeApp.Business.Models;
using RecipeApp.Business.Repository.IRepository;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RecipeApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RecipesController : ControllerBase
{
    private readonly ILogger<RecipesController> _logger;
    private readonly IRecipesRepository _recipesRepository;

    public RecipesController(ILogger<RecipesController> logger,
        IRecipesRepository recipesRepository)
    {
        _logger = logger;
        _recipesRepository = recipesRepository;
    }

    // GET: api/<RecipesController>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<RecipeModel>>> Get()
    {
        var data = await _recipesRepository.GetAll();

        if (data is null) return NotFound();

        return Ok(data);
    }

    // GET api/<RecipesController>/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<RecipeDetailsModel>> Get(int id)
    {
        var data = await _recipesRepository.GetByIdAsync(id);

        if (data is null) return NotFound();

        return Ok(data);
    }

    // POST api/<RecipesController>
    [HttpPost]
    public async Task<ActionResult<int?>> Post([FromBody] AddNewRecipeDto newRecipe)
    {
        try
        {
            if (newRecipe is null) return BadRequest();

            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var insertRecipeModel = new AddNewRecipeModel(
                newRecipe.Title,
                newRecipe.Instructions,
                newRecipe.Ingredients,
                newRecipe.CategoryId,
                userId);
            var newIdentity = await _recipesRepository.CreateNewRecipeAsync(insertRecipeModel);

            return Ok(newIdentity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The POST Call to api/Recipes failed");
            return BadRequest();
        }
    }

    // PUT api/<RecipesController>/5
    [HttpPut("{recipeId}")]
    public async Task<IActionResult> Put(
        int recipeId,
        [FromBody] UpdateExistingRecipeDto updatedRecipeDetails)
    {
        if (updatedRecipeDetails is null) return BadRequest();

        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var updateRecipeModel = new UpdateExistingRecipeModel(
            recipeId,
            updatedRecipeDetails.NewRecipeTitle,
            updatedRecipeDetails.NewRecipeInstructions,
            updatedRecipeDetails.NewIngredients,
            updatedRecipeDetails.NewCategoryId,
            userId);
        var isUpdateSuccesful = await _recipesRepository
            .UpdateRecipeAsync(updateRecipeModel);

        if (isUpdateSuccesful is null || !isUpdateSuccesful.Value) return BadRequest();

        return Ok(isUpdateSuccesful);
    }

    // DELETE api/<RecipesController>/5
    [HttpDelete("{recipeId}")]
    public async Task<ActionResult<bool?>> Delete(int recipeId)
    {
        var deleteResult = await _recipesRepository.DeleteByIdAsync(recipeId, GetUserId());
        return Ok(deleteResult);
    }

    private string GetUserId()
    {
        return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }

    public record AddNewRecipeDto(
        string Title,
        string Instructions,
        List<string> Ingredients,
        int CategoryId);

    public record UpdateExistingRecipeDto(
        string NewRecipeTitle,
        string NewRecipeInstructions,
        List<string> NewIngredients,
        int NewCategoryId);
}