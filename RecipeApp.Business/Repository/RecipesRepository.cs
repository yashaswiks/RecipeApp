using Dapper;
using Microsoft.Data.SqlClient;
using RecipeApp.Business.Repository.IRepository;
using RecipeApp.Business.Services.IServices;
using System.Data;
using System.Threading.Tasks;

namespace RecipeApp.Business.Repository;

public class RecipesRepository : IRecipesRepository
{
    private readonly IDatabaseOptions _databaseOptions;
    private readonly IIngredientsRepository _ingredientsRepository;

    public RecipesRepository(IDatabaseOptions databaseOptions,
        IIngredientsRepository ingredientsRepository)
    {
        _databaseOptions = databaseOptions;
        _ingredientsRepository = ingredientsRepository;
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
}