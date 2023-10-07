using RecipeApp.DapperDataAccess;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecipeApp.Business.Repository.IRepository;

public interface IRecipesRepository
{
    Task<Recipes> GetByIdAsync(int recipeId);

    Task<int?> CreateNewRecipeAsync(AddNewRecipeModel recipe);

    Task<bool?> UpdateRecipeAsync(UpdateExistingRecipeModel recipe);
}

public record AddNewRecipeModel(
    string Title,
    string Instructions,
    List<string> Ingredients,
    int CategoryId,
    string OwnerId);

public record UpdateExistingRecipeModel(
        int RecipeId,
        string NewRecipeTitle,
        string NewRecipeInstructions,
        List<string> NewIngredients,
        int NewCategoryId,
        string OwnerId);