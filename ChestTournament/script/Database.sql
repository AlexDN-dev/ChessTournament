DROP DATABASE IF EXISTS ChessTournament
GO

CREATE DATABASE ChessTournament
GO

USE ChessTournament
GO

CREATE TABLE Players (
    Id UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    Username varchar(50) UNIQUE NOT NULL,
    Email varchar(200) UNIQUE NOT NULL,
    HashPassword varchar(200) NOT NULL,
    Birthday datetime NOT NULL,
    Gender varchar(10) NOT NULL,
    Role int DEFAULT 0,
    Elo int DEFAULT 1200
)
GO

CREATE TABLE Tournaments (
    Id UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    Name varchar(50) NOT NULL,
    Location varchar(50) NOT NULL,
    MinPlayer int NOT NULL,
    MaxPlayer int NOT NULL,
    MinElo int,
    MaxElo int,
    Status varchar(20) NOT NULL DEFAULT 'PENDING',
    ActualRound int DEFAULT 0,
    WomenOnly bit NOT NULL,
    FinalRegisterDate datetime,
    CreatedAt datetime DEFAULT GETDATE(),
    UpdatedAt datetime DEFAULT GETDATE()
)
GO

CREATE TABLE Categories (
    Id UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    Name varchar(50) NOT NULL UNIQUE
)
GO

CREATE TABLE TournamentCategories (
    TournamentId UNIQUEIDENTIFIER NOT NULL,
    CategoryId UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (tournamentId, categoryId)
)
GO

CREATE TABLE PlayerTournaments (
    TournamentId UNIQUEIDENTIFIER NOT NULL,
    PlayerId UNIQUEIDENTIFIER NOT NULL,
    RegisterDate datetime DEFAULT GETDATE(),
    PRIMARY KEY (TournamentId, PlayerId)
)
GO

CREATE TABLE EncounterTournaments (
    Id UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    TournamentId UNIQUEIDENTIFIER NOT NULL,
    Player1 UNIQUEIDENTIFIER NOT NULL,
    Player2 UNIQUEIDENTIFIER NOT NULL,
    Result varchar(10), -- '1-0', '0-1', '1/2-1/2'
    Round int NOT NULL,
    EncounterDate datetime NOT NULL,
    
    CONSTRAINT CHK_players_different CHECK (Player1 <> Player2)
)
GO

ALTER TABLE PlayerTournaments
    ADD FOREIGN KEY (PlayerId) REFERENCES Players(Id) ON DELETE CASCADE
GO

ALTER TABLE PlayerTournaments
    ADD FOREIGN KEY (TournamentId) REFERENCES Tournaments(Id) ON DELETE CASCADE
GO

ALTER TABLE TournamentCategories
    ADD FOREIGN KEY (TournamentId) REFERENCES Tournaments(Id) ON DELETE CASCADE
GO

ALTER TABLE TournamentCategories
    ADD FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE CASCADE
GO

ALTER TABLE EncounterTournaments
    ADD FOREIGN KEY (TournamentId) REFERENCES Tournaments(Id) ON DELETE CASCADE
GO

ALTER TABLE EncounterTournaments
    ADD FOREIGN KEY (Player1) REFERENCES Players(Id)
GO

ALTER TABLE EncounterTournaments
    ADD FOREIGN KEY (Player2) REFERENCES Players(Id)
GO

CREATE UNIQUE INDEX IX_match_unique
    ON EncounterTournaments (TournamentId, Player1, Player2, Round)
GO