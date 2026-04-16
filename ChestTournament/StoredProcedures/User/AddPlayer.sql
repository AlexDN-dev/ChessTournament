CREATE PROCEDURE [dbo].[AddPlayer]
@Json NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION

        BEGIN TRY
            IF ISJSON(@Json) = 0
                BEGIN
                    RAISERROR('Le JSON fourni est invalide.', 16, 1);
                END

            IF JSON_VALUE(@Json, '$.Username') IS NULL
                OR JSON_VALUE(@Json, '$.Email') IS NULL
                OR JSON_VALUE(@Json, '$.hashPassword') IS NULL
                OR JSON_VALUE(@Json, '$.Birthday') IS NULL
                OR JSON_VALUE(@Json, '$.Gender') IS NULL
                BEGIN
                    RAISERROR('Tous les champs sont obligatoires.', 16, 1);
                END

            IF EXISTS (
                SELECT 1 FROM Players
                WHERE Username = JSON_VALUE(@Json, '$.Username')
            )
                BEGIN
                    RAISERROR('Ce username est déjà utilisé.', 16, 1);
                END

            IF EXISTS (
                SELECT 1 FROM Players
                WHERE Email = JSON_VALUE(@Json, '$.Email')
            )
                BEGIN
                    RAISERROR('Cet email est déjà utilisé.', 16, 1);
                END

            IF CAST(JSON_VALUE(@Json, '$.Birthday') AS DATETIME) >= GETDATE()
                BEGIN
                    RAISERROR('La date de naissance doit être dans le passé.', 16, 1);
                END

            INSERT INTO Players (id, Username, Email, hashPassword, Birthday, Gender)
            SELECT  NEWSEQUENTIALID(), Username, Email, hashPassword, Birthday, Gender
            FROM OPENJSON(@Json)
                          WITH (
                              Username     NVARCHAR(50),
                              Email        NVARCHAR(200),
                              hashPassword NVARCHAR(200),
                              Birthday     DATETIME,
                              Gender       NVARCHAR(10)
                              )

            COMMIT TRANSACTION

        END TRY
        BEGIN CATCH
            ROLLBACK TRANSACTION
            THROW;
        END CATCH
END