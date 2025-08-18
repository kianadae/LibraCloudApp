BEGIN TRANSACTION;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Author', N'QuantityAvailable', N'Title') AND [object_id] = OBJECT_ID(N'[Books]'))
    SET IDENTITY_INSERT [Books] ON;
INSERT INTO [Books] ([Id], [Author], [QuantityAvailable], [Title])
VALUES (1, N'John Smith', 5, N'C# Programming Basics'),
(2, N'Jane Doe', 3, N'ASP.NET Core MVC Guide'),
(3, N'Alice Johnson', 4, N'Introduction to Databases');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Author', N'QuantityAvailable', N'Title') AND [object_id] = OBJECT_ID(N'[Books]'))
    SET IDENTITY_INSERT [Books] OFF;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250711162500_SeedInitialData', N'8.0.18');
GO

COMMIT;
GO

