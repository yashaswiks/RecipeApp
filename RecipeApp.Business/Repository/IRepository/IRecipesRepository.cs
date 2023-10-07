using RecipeApp.Business.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecipeApp.Business.Repository.IRepository;

public interface IRecipesRepository
{
    Task<RecipeDetailsModel> GetByIdAsync(int recipeId);

    Task<int?> CreateNewRecipeAsync(AddNewRecipeModel recipe);

    Task<bool?> UpdateRecipeAsync(UpdateExistingRecipeModel recipe);

    Task<List<RecipeDetailsModel>> GetAll();

    Task<bool?> DeleteByIdAsync(int recipeId, string ownerId);
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