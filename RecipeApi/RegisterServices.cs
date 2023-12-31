﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme.
                    Enter 'Bearer' [space] and then your token in the text input below.
                    Example: 'Bearer 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                    },
                    new string[] { }
                },
            });
        });
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
        builder.Services.AddScoped<IUserFavouritesRepository, UserFavouritesRepository>();
    }
}