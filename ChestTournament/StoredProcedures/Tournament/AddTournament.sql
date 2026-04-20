CREATE PROCEDURE [dbo].[AddTournament]
@Json NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        IF ISJSON(@Json) = 0
            THROW 50000, 'JSON invalide.', 1;

        IF JSON_VALUE(@Json, '$.Name') IS NULL
            OR JSON_VALUE(@Json, '$.Location') IS NULL
            OR JSON_VALUE(@Json, '$.MinPlayer') IS NULL
            OR JSON_VALUE(@Json, '$.MaxPlayer') IS NULL
            OR JSON_VALUE(@Json, '$.FinalRegisterDate') IS NULL
            OR JSON_VALUE(@Json, '$.WomenOnly') IS NULL
            BEGIN
                THROW 50001, 'Champs obligatoires manquants.', 1;
            END

        DECLARE @Id UNIQUEIDENTIFIER = NEWID();
        DECLARE @Name VARCHAR(50) = JSON_VALUE(@Json, '$.Name');
        DECLARE @Location VARCHAR(50) = JSON_VALUE(@Json, '$.Location');
        DECLARE @MinPlayer INT = JSON_VALUE(@Json, '$.MinPlayer');
        DECLARE @MaxPlayer INT = JSON_VALUE(@Json, '$.MaxPlayer');
        DECLARE @MinElo INT = JSON_VALUE(@Json, '$.MinElo');
        DECLARE @MaxElo INT = JSON_VALUE(@Json, '$.MaxElo');
        DECLARE @WomenOnly BIT = CAST(JSON_VALUE(@Json, '$.WomenOnly') AS BIT);
        DECLARE @FinalRegisterDate DATETIME = JSON_VALUE(@Json, '$.FinalRegisterDate');

        IF @MinPlayer < 2 OR @MaxPlayer < @MinPlayer
            THROW 50002, 'Nombre de joueurs invalide.', 1;

        IF @MinElo IS NOT NULL AND @MaxElo IS NOT NULL AND @MinElo > @MaxElo
            THROW 50003, 'Elo invalide.', 1;

        IF @FinalRegisterDate <= GETDATE()
            THROW 50004, 'Date d’inscription invalide.', 1;

        IF EXISTS (
            SELECT 1 FROM Tournaments
            WHERE Name = @Name
              AND Status NOT IN ('CANCELLED', 'FINISHED')
        )
            THROW 50005, 'Un tournoi actif avec ce nom existe déjà.', 1;

        DECLARE @CategoryCount INT;

        SELECT @CategoryCount = COUNT(*)
        FROM OPENJSON(@Json, '$.CategoryIds');

        IF @CategoryCount < 1 OR @CategoryCount > 2
            THROW 50006, 'Le tournoi doit avoir 1 ou 2 catégories.', 1;

        IF EXISTS (
            SELECT value
            FROM OPENJSON(@Json, '$.CategoryIds')
            WHERE value NOT IN (SELECT Id FROM Categories)
        )
            THROW 50007, 'Une catégorie est invalide.', 1;

        INSERT INTO Tournaments (
            Id, Name, Location, MinPlayer, MaxPlayer,
            MinElo, MaxElo, Status, ActualRound,
            WomenOnly, FinalRegisterDate, CreatedAt, UpdatedAt
        )
        VALUES (
                   @Id, @Name, @Location, @MinPlayer, @MaxPlayer,
                   @MinElo, @MaxElo, 'PENDING', 0,
                   @WomenOnly, @FinalRegisterDate, GETDATE(), GETDATE()
               );

        INSERT INTO TournamentCategories (TournamentId, CategoryId)
        SELECT @Id, value
        FROM OPENJSON(@Json, '$.CategoryIds');

        COMMIT TRANSACTION;

        SELECT @Id AS Id;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END