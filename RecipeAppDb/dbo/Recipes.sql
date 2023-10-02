CREATE TABLE [dbo].[Recipes]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Title] NVARCHAR(50) NULL, 
    [Instructions] NVARCHAR(500) NULL, 
    [Category] NVARCHAR(50) NULL
)
