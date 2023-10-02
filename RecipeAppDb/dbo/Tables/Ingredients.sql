CREATE TABLE [dbo].[Ingredients]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [RecipeId] INT NOT NULL, 
    [Ingredient] NVARCHAR(150) NOT NULL, 
    CONSTRAINT [FK_Ingredients_ToRecipes] FOREIGN KEY ([RecipeId]) REFERENCES [Recipes]([Id])
)
