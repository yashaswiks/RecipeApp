using Dapper;
using Microsoft.Data.SqlClient;
using RecipeApp.Business.Repository.IRepository;
using RecipeApp.Business.Services.IServices;
using RecipeApp.DapperDataAccess;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace RecipeApp.Business.Repository;

public class CategoriesRepository : ICategoriesRepository
{
    private readonly IDatabaseOptions _databaseOptions;

    public CategoriesRepository(IDatabaseOptions databaseOptions)
    {
        _databaseOptions = databaseOptions;
    }

    public async Task<int?> CreateNewCategoryAsync(string newCategory)
    {
        if (newCategory == null) return null;

        using IDbConnection _db = new SqlConnection(_databaseOptions.ConnectionString);

        var param = new DynamicParameters();
        param.Add("@Id", 0, DbType.Int32, ParameterDirection.Output);
        param.Add("@Category", newCategory);

        var sql = @"INSERT INTO Categories (Category) VALUES (@Category);
                          SELECT @Id = SCOPE_IDENTITY()";

        await _db.ExecuteAsync(sql, param);

        int newIdentity = param.Get<int>("@Id");
        return newIdentity;
    }

    public async Task<List<Categories>> GetAllAsync()
    {
        using IDbConnection _db = new SqlConnection(_databaseOptions.ConnectionString);

        var sql = @"SELECT * FROM Categories;";
        var result = await _db.QueryAsync<Categories>(sql);

        return result?.AsList();
    }

    public async Task<Categories> GetByIdAsync(int id)
    {
        using IDbConnection _db = new SqlConnection(_databaseOptions.ConnectionString);

        var param = new { Id = id };

        var sql = @"SELECT c.* FROM Categories c WHERE c.Id = @Id;";
        var result = await _db.QueryFirstOrDefaultAsync<Categories>(sql, param);

        return result;
    }

    public async Task<Categories> GetRecordDetailsByNameAsync(string categoryName)
    {
        if (categoryName is null) return null;

        using IDbConnection _db = new SqlConnection(_databaseOptions.ConnectionString);

        var param = new { Category = categoryName };

        var sql = @"SELECT * FROM Categories WHERE Category = @Category; ";

        var result = await _db.QueryFirstOrDefaultAsync<Categories>(sql, param);
        return result;
    }
}