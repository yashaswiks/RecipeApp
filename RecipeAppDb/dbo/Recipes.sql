CREATE TABLE [dbo].[Recipes]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Title] NVARCHAR(50) NULL, 
    [Instructions] NVARCHAR(500) NULL, 
    [Category] NVARCHAR(50) NULL, 
    [Owner] NVARCHAR(450) NULL, 
    CONSTRAINT [FK_Recipes_ToAspNetUsers] FOREIGN KEY ([Owner]) REFERENCES [AspNetUsers]([Id])
)
