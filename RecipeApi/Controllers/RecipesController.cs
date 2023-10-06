using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RecipeApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RecipesController : ControllerBase
{
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
    public void Post([FromBody] string value)
    {
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
}