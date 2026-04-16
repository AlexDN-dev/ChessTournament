DROP DATABASE IF EXISTS ChessTournament
CREATE DATABASE ChessTournament
GO

USE ChessTournament
CREATE TABLE [players] (
    [id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    [username] varchar(50) UNIQUE NOT NULL,
    [email] varchar(200) UNIQUE NOT NULL,
    [hashPassword] varchar(200) NOT NULL,
    [birthday] datetime NOT NULL,
    [gender] varchar(10) NOT NULL,
    [role] integer DEFAULT (0),
    [elo] integer DEFAULT (1200)
)
GO

CREATE TABLE [tournaments] (
    [id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    [name] varchar(50) NOT NULL,
    [location] varchar(50) NOT NULL,
    [minPlayer] integer NOT NULL,
    [maxPlayer] integer NOT NULL,
    [minElo] integer,
    [maxElo] integer,
    [status] varchar(50) NOT NULL DEFAULT 'Pending',
    [actualRound] integer DEFAULT (0),
    [womenOnly] bit NOT NULL,
    [finalRegisterDate] datetime,
    [createdAt] datetime DEFAULT (GETDATE()),
    [updatedAt] datetime DEFAULT (GETDATE())
)
GO

CREATE TABLE [categories] (
    [id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    [name] varchar(50) NOT NULL
)
GO

CREATE TABLE [tournamentCategories] (
    [tournamentId] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID(),
    [categoryId] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID(),
    PRIMARY KEY ([tournamentId], [categoryId])
)
GO

CREATE TABLE [playerTournaments] (
    [tournamentId] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID(),
    [playerId] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID(),
    [registerDate] datetime DEFAULT (GETDATE()),
    PRIMARY KEY ([tournamentId], [playerId])
)
GO

CREATE TABLE [encounterTournaments] (
    [id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    [tournamentId] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() NOT NULL,
    [player1] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() NOT NULL,
    [player2] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() NOT NULL,
    [result] nvarchar(255) DEFAULT '0-0',
    [winner] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID(),
    [round] integer NOT NULL,
    [encounterDate] datetime NOT NULL
)
GO

ALTER TABLE [playerTournaments] ADD FOREIGN KEY ([playerId]) REFERENCES [players] ([id]) ON DELETE CASCADE
GO

ALTER TABLE [playerTournaments] ADD FOREIGN KEY ([tournamentId]) REFERENCES [tournaments] ([id]) ON DELETE CASCADE
GO

ALTER TABLE [encounterTournaments] ADD FOREIGN KEY ([tournamentId]) REFERENCES [tournaments] ([id]) ON DELETE CASCADE
GO

ALTER TABLE [encounterTournaments] ADD FOREIGN KEY ([player1]) REFERENCES [players] ([id])
GO

ALTER TABLE [encounterTournaments] ADD FOREIGN KEY ([player2]) REFERENCES [players] ([id])
GO

ALTER TABLE [tournamentCategories] ADD FOREIGN KEY ([tournamentId]) REFERENCES [tournaments] ([id]) ON DELETE CASCADE
GO

ALTER TABLE [tournamentCategories] ADD FOREIGN KEY ([categoryId]) REFERENCES [categories] ([id]) ON DELETE CASCADE
GO

ALTER TABLE [encounterTournaments] ADD FOREIGN KEY ([tournamentId], [player1]) REFERENCES [playerTournaments] ([tournamentId], [playerId])
GO

ALTER TABLE [encounterTournaments] ADD FOREIGN KEY ([tournamentId], [player2]) REFERENCES [playerTournaments] ([tournamentId], [playerId])
GO
