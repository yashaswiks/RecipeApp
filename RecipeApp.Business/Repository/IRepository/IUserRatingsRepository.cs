using RecipeApp.DapperDataAccess;
using System.Threading.Tasks;

namespace RecipeApp.Business.Repository.IRepository;

public interface IUserRatingsRepository
{
    Task<int?> AddNewRatingAsync(AddNewUserRatingModel rating);

    Task<int?> UpdateRatingAsync(UpdateUserRatingModel rating);

    Task<UserRatings> GetRatingDetailsAsync(int recipeId, string ratingMadeBy);
}

public record AddNewUserRatingModel(
    int RecipeId,
    string RatingMadeBy,
    int RatingValue);

public record UpdateUserRatingModel(
    int RatingId,
    int RecipeId,
    string RatingMadeBy,
    int RatingValue);