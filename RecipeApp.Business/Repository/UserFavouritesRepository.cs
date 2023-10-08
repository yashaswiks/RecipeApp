using Dapper;
using Microsoft.Data.SqlClient;
using RecipeApp.Business.Repository.IRepository;
using RecipeApp.Business.Services.IServices;
using RecipeApp.DapperDataAccess;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace RecipeApp.Business.Repository;

public class UserFavouritesRepository : IUserFavouritesRepository
{
    private readonly IDatabaseOptions _databaseOptions;

    public UserFavouritesRepository(IDatabaseOptions databaseOptions)
    {
        _databaseOptions = databaseOptions;
    }

    public async Task<bool?> AddNewUserFavouriteAsync(int recipeId, string userId)
    {
        var userFavourite = await GetAsync(recipeId, userId);
        if (userFavourite != null) return null;

        using IDbConnection _db = new SqlConnection(_databaseOptions.ConnectionString);

        var param = new DynamicParameters();
        param.Add("@RecipeId", recipeId);
        param.Add("@UserId", userId);
        param.Add("@Id", 0, DbType.Int32, ParameterDirection.Output);

        var sql = @"INSERT INTO UserFavourites (RecipeId, UserId)
                        VALUES (@RecipeId, @UserId);
                        SELECT @Id = SCOPE_IDENTITY();";

        await _db.ExecuteAsync(sql, param);

        int newIdentity = param.Get<int>("@Id");

        if (newIdentity > 0) return true;

        return false;
    }

    public async Task<bool?> DeleteAsync(int recipeId, string userId)
    {
        using IDbConnection _db = new SqlConnection(_databaseOptions.ConnectionString);

        var param = new { RecipeId = recipeId, UserId = userId };

        var sql = @"DELETE FROM UserFavourites
                        WHERE RecipeId = @RecipeId AND UserId = @UserId;";

        var result = await _db.ExecuteAsync(sql, param);

        if (result > 0) return true;

        return false;
    }

    public async Task<UserFavourites> GetAsync(int recipeId, string userId)
    {
        using IDbConnection _db = new SqlConnection(_databaseOptions.ConnectionString);

        var sql = @"SELECT
                        u.* FROM UserRatings u
                        WHERE u.RecipeId = @RecipeId AND u.RatingMadeBy = @UserId";

        var param = new { RecipeId = recipeId, UserId = userId };

        var result = await _db
            .QueryFirstOrDefaultAsync<UserFavourites>(sql, param);

        return result;
    }

    public async Task<List<UserFavouritesModel>> GetByUserAsync(string userId)
    {
        if (userId is null) return null;

        using IDbConnection _db = new SqlConnection(_databaseOptions.ConnectionString);

        var sql = @"SELECT
                        f.RecipeId, r.Title 'RecipeName'
                        FROM UserFavourites f
                        LEFT JOIN Recipes r ON r.Id = f.RecipeId
                        WHERE f.UserId = @UserId;";

        var param = new { UserId = userId };

        var result = await _db
            .QueryAsync<UserFavouritesModel>(sql, param);

        return result?.AsList();
    }
}