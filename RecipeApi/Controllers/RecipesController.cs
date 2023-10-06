using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

    // GET api/<RecipesController>/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public string Get(int id)
    {
        return "value";
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
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<RecipesController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
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
}