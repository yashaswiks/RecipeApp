using RecipeApp.DapperDataAccess;
using System.Threading.Tasks;

namespace RecipeApp.Business.Repository.IRepository;

public interface IUserRatingsRepository
{
    Task<int?> AddNewRating(AddNewUserRatingModel rating);

    Task<int?> UpdateRating(UpdateUserRatingModel rating);

    Task<UserRatings> GetRatingDetails(int recipeId, string ratingMadeBy);
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