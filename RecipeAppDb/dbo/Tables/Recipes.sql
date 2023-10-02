CREATE TABLE [dbo].[Recipes]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Title] NVARCHAR(50) NOT NULL, 
    [Instructions] NVARCHAR(500) NOT NULL, 
    [Category] NVARCHAR(50) NOT NULL, 
    [Owner] NVARCHAR(450) NOT NULL, 
    CONSTRAINT [FK_Recipes_ToAspNetUsers] FOREIGN KEY ([Owner]) REFERENCES [AspNetUsers]([Id])
)
