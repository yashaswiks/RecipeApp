﻿using RecipeApp.DapperDataAccess;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecipeApp.Business.Repository.IRepository;

public interface IIngredientsRepository
{
    Task<int?> InsertAsync(InsertIngredientsModel model);

    Task<int?> DeleteIngredientsOfRecipeIdAsync(int recipeId);

    Task<bool?> UpdateIngredients(
        int recipeId,
        List<string> ingredients,
        string OwnerId);

    Task<List<Ingredients>> GetByRecipeIdAsync(int recipeId);
}

public record InsertIngredientsModel(int RecipeId, string Ingredient);