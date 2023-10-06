using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeApp.Business.Repository.IRepository;
using RecipeApp.Common;
using RecipeApp.DapperDataAccess;

namespace RecipeApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(
        ICategoriesRepository categoriesRepository,
        ILogger<CategoriesController> logger)
    {
        _categoriesRepository = categoriesRepository;
        _logger = logger;
    }

    // GET: api/<CategoriesController>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<Categories>>> Get()
    {
        try
        {
            var categoryList = await _categoriesRepository.GetAllAsync();
            if (categoryList is null) return NotFound();
            return Ok(categoryList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "THE GET call to api/Categories failed");
            return BadRequest();
        }
    }

    // GET api/<CategoriesController>/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Categories>> Get(int id)
    {
        try
        {
            var categoryDetails = await _categoriesRepository.GetByIdAsync(id);
            if (categoryDetails is null) return NotFound();
            return Ok(categoryDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "THE GET call to api/Categories/id failed");
            return BadRequest();
        }
    }

    // POST api/<CategoriesController>
    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<int>> Post([FromBody] string newCategory)
    {
        if (newCategory is null) return BadRequest();

        try
        {
            var newIdentity = await _categoriesRepository.CreateNewCategoryAsync(newCategory);

            if (newIdentity is null) return BadRequest();

            return Ok(newIdentity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "THE POST call to api/Categories failed");
            return BadRequest();
        }
    }
}