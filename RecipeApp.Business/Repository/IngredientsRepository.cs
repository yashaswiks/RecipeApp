using Dapper;
using Microsoft.Data.SqlClient;
using RecipeApp.Business.Repository.IRepository;
using RecipeApp.Business.Services.IServices;
using System;
using System.Data;
using System.Threading.Tasks;

namespace RecipeApp.Business.Repository;

public class IngredientsRepository : IIngredientsRepository
{
    private readonly IDatabaseOptions _databaseOptions;

    public IngredientsRepository(IDatabaseOptions databaseOptions)
    {
        _databaseOptions = databaseOptions;
    }

    public Task<int?> DeleteIngredientsOfRecipeIdAsync(int recipeId)
    {
        throw new NotImplementedException();
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
}