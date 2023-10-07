using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using RecipeApp.Business.Repository.IRepository;
using RecipeApp.Business.Services.IServices;
using System;
using System.Data;
using System.Threading.Tasks;

namespace RecipeApp.Business.Repository;

public class UserRatingsRepository : IUserRatingsRepository
{
    private readonly ILogger<UserRatingsRepository> _logger;
    private readonly IDatabaseOptions _databaseOptions;

    public UserRatingsRepository(
        ILogger<UserRatingsRepository> logger,
        IDatabaseOptions databaseOptions)
    {
        _logger = logger;
        _databaseOptions = databaseOptions;
    }

    public async Task<int?> AddNewRating(AddNewUserRatingModel rating)
    {
        try
        {
            if (rating is null) return null;

            using IDbConnection _db = new SqlConnection(_databaseOptions.ConnectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@RecipeId", rating.RecipeId);
            parameters.Add("@RatingMadeBy", rating.RatingMadeBy);
            parameters.Add("@RatingValue", rating.RatingValue);
            parameters.Add("@Id", 0, DbType.Int32, ParameterDirection.Output);

            var sql = @"INSERT INTO UserRatings (RecipeId, RatingMadeBy, RatingValue)
                        VALUES (@RecipeId, @RatingMadeBy, @RatingValue);
                        SELECT @Id = SCOPE_IDENTITY();";

            await _db.ExecuteAsync(sql, parameters);

            int newIdentity = parameters.Get<int>("@Id");

            return newIdentity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error Adding New Rating");
            return null;
        }
    }

    public async Task<int?> UpdateRating(UpdateUserRatingModel rating)
    {
        try
        {
            if (rating is null) return null;

            using IDbConnection _db = new SqlConnection(_databaseOptions.ConnectionString);

            var sql = @"UPDATE UserRatings
                            SET RatingValue = @RatingValue
                            WHERE Id = @Id AND RatingMadeBy = @RatingMadeBy; ";

            var param = new
            {
                Id = rating.RatingId,
                RecipeId = rating.RecipeId,
                RatingMadeBy = rating.RatingMadeBy,
                RatingValue = rating.RatingValue,
            };

            var result = await _db.ExecuteAsync(sql, param);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error Updating the Rating");
            return null;
        }
    }
}