namespace RecipeApp.DapperDataAccess;

public class Recipes
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Instructions { get; set; }
    public int CategoryId { get; set; }
    public string Owner { get; set; }
}