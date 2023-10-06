namespace RecipeApp.DapperDataAccess;

public class Ingredients
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public string Ingredient { get; set; }
}