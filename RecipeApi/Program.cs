using RecipeApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddAuthServices();
builder.AddStandardServices();
builder.AddDbServices();
builder.AddBusinessServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();