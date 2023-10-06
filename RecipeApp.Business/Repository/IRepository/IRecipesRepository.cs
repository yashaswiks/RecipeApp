using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecipeApp.Business.Repository.IRepository;

public interface IRecipesRepository
{
    Task<int?> CreateNewRecipe(AddNewRecipeDto recipe);
}

public record AddNewRecipeDto(
    string Title,
    string Instructions,
    List<string> Ingredients,
    string Category,
    string OwnerId);