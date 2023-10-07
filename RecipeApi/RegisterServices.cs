using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecipeApi.Areas.Identity.Data;
using RecipeApi.Data;
using RecipeApp.Business.Repository;
using RecipeApp.Business.Repository.IRepository;
using RecipeApp.Business.Services;
using RecipeApp.Business.Services.IServices;
using System.Text;

namespace RecipeApi;

public static class RegisterServices
{
    public static void AddAuthServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<RecipeApiContext>();

        builder.Services.AddAuthorization(opts =>
        {
            opts.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration.GetValue<string>("Authentication:Issuer"),
                    ValidAudience = builder.Configuration.GetValue<string>("Authentication:Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(
                            builder.Configuration.GetValue<string>("Authentication:SecretKey")))
                };
            });
    }

    public static void AddStandardServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }

    public static void AddDbServices(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'Default' not found.");

        builder.Services.AddDbContext<RecipeApiContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddScoped<IDatabaseOptions, DatabaseOptions>();
    }

    public static void AddBusinessServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ICategoriesRepository, CategoriesRepository>();
        builder.Services.AddScoped<IIngredientsRepository, IngredientsRepository>();
        builder.Services.AddScoped<IRecipesRepository, RecipesRepository>();
        builder.Services.AddScoped<IUserRatingsRepository, UserRatingsRepository>();
    }
}