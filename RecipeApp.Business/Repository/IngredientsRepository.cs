using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using RecipeApp.Business.Repository.IRepository;
using RecipeApp.Business.Services.IServices;
using RecipeApp.DapperDataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace RecipeApp.Business.Repository;

public class IngredientsRepository : IIngredientsRepository
{
    private readonly IDatabaseOptions _databaseOptions;
    private readonly ILogger<IngredientsRepository> _logger;

    public IngredientsRepository(
        IDatabaseOptions databaseOptions,
        ILogger<IngredientsRepository> logger
        )
    {
        _databaseOptions = databaseOptions;
        _logger = logger;
    }

    public Task<int?> DeleteIngredientsOfRecipeIdAsync(int recipeId,
        string ownerId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Ingredients>> GetByRecipeIdAsync(int recipeId)
    {
        using IDbConnection _db = new SqlConnection(_databaseOptions.ConnectionString);

        var sql = @"SELECT Id, RecipeId, Ingredient
                        FROM Ingredients
                        WHERE RecipeId = @RecipeId;";

        var details = await _db
            .QueryAsync<Ingredients>(sql, new { RecipeId = recipeId });

        return details?.AsList();
    }

    public async Task<int?> InsertAsync(InsertIngredientsModel model)
    {
        if (model is null) return null;

        var param = new DynamicParameters();

        param.Add("@Id", 0, DbType.Int32, ParameterDirection.Output);
        param.Add("@RecipeId", model.RecipeId);
        param.Add("@Ingredient", model.Ingredient);

        using IDbConnection _db = new SqlConnection(_databaseOptions.ConnectionString);

        var sql = @"INSERT INTO Ingredients (RecipeId, Ingredient)
                        VALUES (@RecipeId, @Ingredient);
                        SELECT @Id = SCOPE_IDENTITY();";

        await _db.ExecuteAsync(sql, param);

        int newIdentity = param.Get<int>("@Id");
        return newIdentity;
    }

    public async Task<bool?> UpdateIngredients(
        int recipeId,
        List<string> ingredients,
        string OwnerId)
    {
        throw new NotImplementedException();
        //if (ingredients is null || ingredients.Count == 0) return null;

        //using IDbConnection _db = new SqlConnection(_databaseOptions.ConnectionString);

        //var recipeDetails = await _recipesRepository.GetByIdAsync(recipeId);
        //if (recipeDetails is null) return null;

        //_db.Open();

        //var sql = @"INSERT INTO Ingredients (RecipeId, Ingredient)
        //                VALUES (@RecipeId, @Ingredient);
        //                SELECT @Id = SCOPE_IDENTITY();";

        //using var transaction = _db.BeginTransaction();

        //try
        //{
        //    await DeleteIngredientsOfRecipeIdAsync(recipeId, OwnerId);
        //    foreach (var newIngredient in ingredients)
        //    {
        //        var param = new DynamicParameters();
        //        param.Add("@Id", 0, DbType.Int32, ParameterDirection.Output);
        //        param.Add("@RecipeId", recipeId);
        //        param.Add("@Ingredient", newIngredient);

        //        await _db.ExecuteAsync(sql, param, transaction);
        //    }

        //    transaction.Commit();
        //    return true;
        //}
        //catch (Exception ex)
        //{
        //    _logger.LogError(ex, "Update Ingredients of RecipeId failed");
        //    transaction.Rollback();
        //    throw;
        //}
    }
}