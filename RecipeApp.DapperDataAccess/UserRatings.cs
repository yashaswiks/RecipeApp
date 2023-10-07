namespace RecipeApp.DapperDataAccess;

public class UserRatings
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public string RatingMadeBy { get; set; }
    public int RatingValue { get; set; }
}