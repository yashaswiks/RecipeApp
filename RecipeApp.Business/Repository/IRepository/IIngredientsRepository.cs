using System.Threading.Tasks;

namespace RecipeApp.Business.Repository.IRepository;

public interface IIngredientsRepository
{
    Task<int?> InsertAsync(InsertIngredientsModel model);

    Task<int?> DeleteIngredientsOfRecipeIdAsync(int recipeId);
}

public record InsertIngredientsModel(int RecipeId, string Ingredient);