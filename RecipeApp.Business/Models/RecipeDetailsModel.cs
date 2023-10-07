using System.Collections.Generic;

namespace RecipeApp.Business.Models;

public class RecipeDetailsModel : RecipeModel
{
    // public RecipeModel RecipeDetails { get; set; }
    public List<string> Ingredients { get; set; } = new();
}

public class RecipeModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Instructions { get; set; }
    public string Category { get; set; }
    public string Owner { get; set; }
}