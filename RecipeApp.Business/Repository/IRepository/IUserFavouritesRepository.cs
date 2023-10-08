using RecipeApp.DapperDataAccess;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecipeApp.Business.Repository.IRepository;

public interface IUserFavouritesRepository
{
    Task<bool?> AddNewUserFavouriteAsync(int recipeId, string userId);

    Task<UserFavourites> GetAsync(int recipeId, string userId);

    Task<List<UserFavouritesModel>> GetByUserAsync(string userId);

    Task<bool?> DeleteAsync(int recipeId, string userId);
}

public record UserFavouritesModel(
    int RecipeId,
    string RecipeName);