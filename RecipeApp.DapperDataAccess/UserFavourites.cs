namespace RecipeApp.DapperDataAccess;

public class UserFavourites
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public string UserId { get; set; }
}