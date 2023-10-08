CREATE TABLE [dbo].[UserFavourites]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [RecipeId] INT NOT NULL, 
    [UserId] NVARCHAR(450) NOT NULL, 
    CONSTRAINT [FK_UserFavourites_ToRecipes] FOREIGN KEY ([RecipeId]) REFERENCES [Recipes]([Id]), 
    CONSTRAINT [FK_UserFavourites_ToAspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
)
