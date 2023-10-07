using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using RecipeApp.Business.Models;
using RecipeApp.Business.Repository.IRepository;
using RecipeApp.Business.Services.IServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace RecipeApp.Business.Repository;

public class RecipesRepository : IRecipesRepository
{
    private readonly IDatabaseOptions _databaseOptions;
    private readonly IIngredientsRepository _ingredientsRepository;
    private readonly ILogger<RecipesRepository> _logger;

    public RecipesRepository(IDatabaseOptions databaseOptions,
        IIngredientsRepository ingredientsRepository,
        ILogger<RecipesRepository> logger)
    {
        _databaseOptions = databaseOptions;
        _ingredientsRepository = ingredientsRepository;
        _logger = logger;
    }

    public async Task<int?> CreateNewRecipeAsync(AddNewRecipeModel recipe)
    {
        if (recipe is null) return null;

        var param = new DynamicParameters();

        var sql = @"INSERT INTO Recipes (Title, Instructions, CategoryId, Owner)
                        VALUES (@Title, @Instructions, @CategoryId, @Owner);
                        SELECT @Id = SCOPE_IDENTITY();";

        param.Add("@Title", recipe.Title);
        param.Add("@Instructions", recipe.Instructions);
        param.Add("@CategoryId", recipe.CategoryId);
        param.Add("@Owner", recipe.OwnerId);
        param.Add("@Id", 0, DbType.Int32, ParameterDirection.Output);

        using IDbConnection _db = new SqlConnection(_databaseOptions.ConnectionString);

        await _db.ExecuteAsync(sql, param);

        int newRecipeIdentity = param.Get<int>("@Id");

        if (recipe.Ingredients?.Count > 0)
        {
            foreach (var ingredient in recipe.Ingredients)
            {
                var insertIngredientModel = new InsertIngredientsModel(
                    newRecipeIdentity,
                    ingredient);

                await _ingredientsRepository.InsertAsync(insertIngredientModel);
            }
        }

        return newRecipeIdentity;
    }

    public async Task<List<RecipeDetailsModel>> GetAll()
    {
        // TODO: This is wrong, fix it
        using IDbConnection _db = new SqlConnection(_databaseOptions.ConnectionString);

        var sql = @"SELECT
                        r.Id, r.Title, r.Instructions, c.Category, CONCAT(u.FirstName, ' ', u.LastName) 'Owner', i.Ingredient
                        FROM Recipes r
                        LEFT JOIN Ingredients i ON r.Id = i.RecipeId
                        LEFT JOIN Categories c ON r.CategoryId = c.Id
                        LEFT JOIN AspNetUsers u ON r.Owner = u.Id;";

        var data = await _db
            .QueryAsync<RecipeDetailsModel, string, RecipeDetailsModel>(sql,
            (recipe, ingredient) =>
            {
                recipe.Ingredients.Add(ingredient);
                return recipe;
            },
            splitOn: "Ingredient");

        return data?.AsList();
    }

    public async Task<bool?> UpdateRecipeAsync(UpdateExistingRecipeModel recipe)
    {
        if (recipe is null) return null;

        using IDbConnection _db = new SqlConnection(_databaseOptions.ConnectionString);

        var updateRecipeParams = new
        {
            Id = recipe.RecipeId,
            Title = recipe.NewRecipeTitle,
            Instructions = recipe.NewRecipeInstructions,
            CategoryId = recipe.NewCategoryId,
            Owner = recipe.OwnerId
        };

        var sql = @"UPDATE
                        Recipes SET Title = @Title,
                        Instructions = @Instructions,
                        CategoryId = @CategoryId
                        WHERE Id = @Id AND Owner = @Owner;";

        var deleteIngredientsParams = new
        {
            recipe.RecipeId,
        };
        var deleteIngredientsSql = @"DELETE FROM Ingredients WHERE RecipeId = @RecipeId;";

        var insertIngredientsSql = @"INSERT INTO Ingredients (RecipeId, Ingredient)
                        VALUES (@RecipeId, @Ingredient);
                        SELECT @Id = SCOPE_IDENTITY();";

        _db.Open();
        using var trans = _db.BeginTransaction();

        try
        {
            // Update recipe
            var affectedRows = await _db.ExecuteAsync(sql, updateRecipeParams, trans);

            // If affectedRows is 0,
            // this means the recipe does not exist or the user is not the owner
            if (affectedRows == 0)
            {
                trans.Rollback();
                return false;
            }

            // Delete Existing Ingredients
            await _db.ExecuteAsync(deleteIngredientsSql, deleteIngredientsParams, trans);

            // Insert New Ingredients
            foreach (var newIngredient in recipe.NewIngredients)
            {
                var param = new DynamicParameters();
                param.Add("@RecipeId", recipe.RecipeId);
                param.Add("@Ingredient", newIngredient);
                param.Add("@Id", 0, DbType.Int32, ParameterDirection.Output);

                await _db.ExecuteAsync(insertIngredientsSql, param, trans);
            }

            trans.Commit();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating recipe");
            trans.Rollback();
            return false;
        }
    }

    public async Task<RecipeDetailsModel> GetByIdAsync(int recipeId)
    {
        using IDbConnection _db = new SqlConnection(_databaseOptions.ConnectionString);
        var param = new { Id = recipeId };

        var sql = @"SELECT
                        r.Id, r.Title, r.Instructions, c.Category, CONCAT(u.FirstName, ' ', u.LastName) 'Owner'
                        FROM Recipes r
                        LEFT JOIN Categories c ON r.CategoryId = c.Id
                        LEFT JOIN AspNetUsers u ON r.Owner = u.Id
                        WHERE r.Id = @Id;";

        var recipe = await _db
            .QueryFirstOrDefaultAsync<RecipeModel>(sql, param);

        var ingredients = new List<string>();

        var ingredientsData = await _ingredientsRepository
            .GetByRecipeIdAsync(recipeId);

        if (ingredientsData?.Count > 0)
        {
            foreach (var ingredient in ingredientsData)
            {
                ingredients.Add(ingredient?.Ingredient);
            }
        }

        var output = new RecipeDetailsModel
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Instructions = recipe.Instructions,
            Category = recipe.Category,
            Owner = recipe.Owner,
            Ingredients = ingredients
        };

        return output;
    }

    public async Task<bool?> DeleteByIdAsync(int recipeId, string ownerId)
    {
        using IDbConnection _db = new SqlConnection(_databaseOptions.ConnectionString);

        var deleteRecipeSql = @"DELETE FROM Recipes WHERE Id = @Id AND Owner = @Owner;";
        var deleteIngredientsSql = @"DELETE FROM Ingredients WHERE RecipeId = @RecipeId;";

        var deleteRecipeParams = new
        {
            Id = recipeId,
            Owner = ownerId
        };

        var deleteIngredientsParams = new
        {
            RecipeId = recipeId
        };

        _db.Open();
        using var trans = _db.BeginTransaction();

        try
        {
            await _db.ExecuteAsync(deleteIngredientsSql, deleteIngredientsParams, trans);

            int deleteRecipeResult = await _db.ExecuteAsync(deleteRecipeSql, deleteRecipeParams, trans);

            if (deleteRecipeResult == 0)
            {
                trans.Rollback();
                return false;
            }

            trans.Commit();
            return true;
        }
        catch (Exception ex)
        {
            trans.Rollback();
            _logger.LogError(ex, "Error deleting recipe");
            return false;
        }
    }
}