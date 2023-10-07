using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace RecipeApp.Business.Repository.IRepository;

public interface IIngredientsRepository
{
    Task<int?> InsertAsync(InsertIngredientsModel model);

    Task<int?> DeleteIngredientsOfRecipeIdAsync(int recipeId, 
        string ownerId);

    Task<bool?> UpdateIngredients(
        int recipeId,
        List<string> ingredients,
        string OwnerId);
}

public record InsertIngredientsModel(int RecipeId, string Ingredient);