using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecipeApp.Business.Repository.IRepository;

public interface IRecipesRepository
{
    Task<int?> CreateNewRecipeAsync(AddNewRecipeModel recipe);
}

public record AddNewRecipeModel(
    string Title,
    string Instructions,
    List<string> Ingredients,
    int CategoryId,
    string OwnerId);