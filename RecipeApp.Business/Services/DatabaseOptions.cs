using Microsoft.Extensions.Configuration;
using RecipeApp.Business.Services.IServices;

namespace RecipeApp.Business.Services;

public class DatabaseOptions : IDatabaseOptions
{
    public string ConnectionString { get; set; }

    public DatabaseOptions(
        IConfiguration config,
        string connectionId = "Default")
    {
        ConnectionString = config.GetConnectionString(connectionId);
    }
}