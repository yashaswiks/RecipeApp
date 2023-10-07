CREATE TABLE [dbo].[UserRatings]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [RecipeId] INT NOT NULL, 
    [RatingMadeBy] NVARCHAR(450) NOT NULL, 
    [RatingValue] INT NULL, 
    CONSTRAINT [FK_UserRatings_ToRecipes] FOREIGN KEY ([RecipeId]) REFERENCES [Recipes]([Id]), 
    CONSTRAINT [FK_UserRatings_ToAspNetUsers] FOREIGN KEY ([RatingMadeBy]) REFERENCES [AspNetUsers]([Id])
)
