using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using RecipeApp.Business.Repository.IRepository;
using RecipeApp.Business.Services.IServices;
using RecipeApp.DapperDataAccess;
using System;
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

    public async Task<Recipes> GetByIdAsync(int recipeId)
    {
        var param = new { Id = recipeId };
        using IDbConnection _db = new SqlConnection(_databaseOptions.ConnectionString);

        var sql = @"SELECT * FROM Recipes WHERE Id = @Id; ";

        var recipe = await _db.QueryFirstOrDefaultAsync<Recipes>(sql, param);

        return recipe;
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
}