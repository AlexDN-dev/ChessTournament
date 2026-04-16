DROP DATABASE IF EXISTS ChessTournament
GO

CREATE DATABASE ChessTournament
GO

USE ChessTournament
GO

CREATE TABLE players (
    id UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    username varchar(50) UNIQUE NOT NULL,
    email varchar(200) UNIQUE NOT NULL,
    hashPassword varchar(200) NOT NULL,
    birthday datetime NOT NULL,
    gender varchar(10) NOT NULL,
    role int DEFAULT 0,
    elo int DEFAULT 1200
)
GO

CREATE TABLE tournaments (
    id UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    name varchar(50) NOT NULL,
    location varchar(50) NOT NULL,
    minPlayer int NOT NULL,
    maxPlayer int NOT NULL,
    minElo int,
    maxElo int,
    status varchar(20) NOT NULL DEFAULT 'PENDING',
    actualRound int DEFAULT 0,
    womenOnly bit NOT NULL,
    finalRegisterDate datetime,
    createdAt datetime DEFAULT GETDATE(),
    updatedAt datetime DEFAULT GETDATE()
)
GO

CREATE TABLE categories (
    id UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    name varchar(50) NOT NULL UNIQUE
)
GO

CREATE TABLE tournamentCategories (
    tournamentId UNIQUEIDENTIFIER NOT NULL,
    categoryId UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (tournamentId, categoryId)
)
GO

CREATE TABLE playerTournaments (
    tournamentId UNIQUEIDENTIFIER NOT NULL,
    playerId UNIQUEIDENTIFIER NOT NULL,
    registerDate datetime DEFAULT GETDATE(),
    PRIMARY KEY (tournamentId, playerId)
)
GO

CREATE TABLE encounterTournaments (
    id UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    tournamentId UNIQUEIDENTIFIER NOT NULL,
    player1 UNIQUEIDENTIFIER NOT NULL,
    player2 UNIQUEIDENTIFIER NOT NULL,
    result varchar(10), -- '1-0', '0-1', '1/2-1/2'
    round int NOT NULL,
    encounterDate datetime NOT NULL,
    
    CONSTRAINT CHK_players_different CHECK (player1 <> player2)
)
GO

ALTER TABLE playerTournaments
    ADD FOREIGN KEY (playerId) REFERENCES players(id) ON DELETE CASCADE
GO

ALTER TABLE playerTournaments
    ADD FOREIGN KEY (tournamentId) REFERENCES tournaments(id) ON DELETE CASCADE
GO

ALTER TABLE tournamentCategories
    ADD FOREIGN KEY (tournamentId) REFERENCES tournaments(id) ON DELETE CASCADE
GO

ALTER TABLE tournamentCategories
    ADD FOREIGN KEY (categoryId) REFERENCES categories(id) ON DELETE CASCADE
GO

ALTER TABLE encounterTournaments
    ADD FOREIGN KEY (tournamentId) REFERENCES tournaments(id) ON DELETE CASCADE
GO

ALTER TABLE encounterTournaments
    ADD FOREIGN KEY (player1) REFERENCES players(id)
GO

ALTER TABLE encounterTournaments
    ADD FOREIGN KEY (player2) REFERENCES players(id)
GO

CREATE UNIQUE INDEX IX_match_unique
    ON encounterTournaments (tournamentId, player1, player2, round)
GO