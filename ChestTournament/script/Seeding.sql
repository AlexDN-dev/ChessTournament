USE ChessTournament
-- ========================
-- CLEAN DATABASE
-- ========================

DELETE FROM encounterTournaments
DELETE FROM playerTournaments
DELETE FROM tournamentCategories
DELETE FROM categories
DELETE FROM tournaments
DELETE FROM players
-- ========================
-- TABLES TEMP
-- ========================
DECLARE @Players TABLE (id UNIQUEIDENTIFIER)
DECLARE @Categories TABLE (id UNIQUEIDENTIFIER)
DECLARE @Tournaments TABLE (id UNIQUEIDENTIFIER)

-- ========================
-- PLAYERS
-- ========================
INSERT INTO players (id, username, email, hashPassword, birthday, gender, role, elo)
OUTPUT inserted.id INTO @Players
SELECT NEWID(), CONCAT('Player', n), CONCAT('p', n, '@mail.com'), 'hash',
       DATEADD(day, -n*100, GETDATE()),
       CASE WHEN n % 2 = 0 THEN 'MALE' ELSE 'FEMALE' END,
       0,
       1200 + n * 10
FROM (SELECT TOP 23 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) n FROM sys.objects) t

-- ========================
-- CATEGORIES
-- ========================
INSERT INTO categories (id, name)
OUTPUT inserted.id INTO @Categories
VALUES
    (NEWID(),'Junior'),
    (NEWID(),'Senior'),
    (NEWID(),'Women'),
    (NEWID(),'Open')

-- ========================
-- TOURNAMENTS
-- ========================
DECLARE @i INT = 1
WHILE @i <= 13
    BEGIN
        INSERT INTO tournaments (id, name, location, minPlayer, maxPlayer, status, womenOnly)
        OUTPUT inserted.id INTO @Tournaments
        VALUES (
                   NEWID(),
                   CONCAT('Tournament ', @i),
                   'Brussels',
                   4,
                   20,
                   CASE
                       WHEN @i <= 4 THEN 'PENDING'
                       WHEN @i <= 9 THEN 'STARTED'
                       ELSE 'FINISHED'
                       END,
                   CASE WHEN @i % 3 = 0 THEN 1 ELSE 0 END
               )

        SET @i += 1
    END

-- ========================
-- TOURNAMENT CATEGORIES
-- ========================
-- 1 catégorie obligatoire
INSERT INTO tournamentCategories (tournamentId, categoryId)
SELECT t.id,
       (SELECT TOP 1 c.id FROM @Categories c ORDER BY NEWID())
FROM @Tournaments t

-- + éventuellement une 2ème
INSERT INTO tournamentCategories (tournamentId, categoryId)
SELECT t.id, c.id
FROM @Tournaments t
         CROSS APPLY (
    SELECT TOP 1 id FROM @Categories ORDER BY NEWID()
) c
WHERE ABS(CHECKSUM(NEWID())) % 2 = 0

-- ========================
-- PLAYER REGISTRATION
-- ========================
-- INSCRIPTIONS GARANTIES
INSERT INTO playerTournaments (tournamentId, playerId)
SELECT t.id, p.id
FROM @Tournaments t
         CROSS APPLY (
    SELECT TOP (4 + ABS(CHECKSUM(NEWID())) % 6) id
    FROM @Players
    ORDER BY NEWID()
) p

-- ========================
-- MATCH GENERATION
-- ========================
-- ========================
-- MATCH GENERATION (BERGER + BYE)
-- ========================

;WITH Players AS (
    SELECT
        tournamentId,
        playerId,
        ROW_NUMBER() OVER (PARTITION BY tournamentId ORDER BY playerId) AS pos,
        COUNT(*) OVER (PARTITION BY tournamentId) AS totalPlayers
    FROM playerTournaments
),

-- Ajout BYE si impair
      PlayersWithBye AS (
          SELECT * FROM Players

          UNION ALL

          SELECT
              tournamentId,
              NULL AS playerId, -- BYE
              totalPlayers + 1,
              totalPlayers + 1
          FROM Players
          WHERE totalPlayers % 2 = 1
      ),

      Adjusted AS (
          SELECT
              tournamentId,
              playerId,
              ROW_NUMBER() OVER (PARTITION BY tournamentId ORDER BY pos) AS pos,
              COUNT(*) OVER (PARTITION BY tournamentId) AS totalPlayers
          FROM PlayersWithBye
      ),

      Rounds AS (
          SELECT DISTINCT
              tournamentId,
              totalPlayers,
              (totalPlayers - 1) AS totalRounds
          FROM Adjusted
      ),

      Numbers AS (
          SELECT TOP 50 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS round
          FROM sys.objects
      ),

      Matches AS (
          SELECT
              a1.tournamentId,
              n.round,
              a1.playerId AS player1,
              a2.playerId AS player2,
              r.totalPlayers,

              ((a1.pos + n.round - 2) % (r.totalPlayers - 1)) + 1 AS newPos1,
              ((a2.pos + n.round - 2) % (r.totalPlayers - 1)) + 1 AS newPos2

          FROM Adjusted a1
                   JOIN Adjusted a2
                        ON a1.tournamentId = a2.tournamentId
                            AND a1.pos < a2.pos
                   JOIN Rounds r ON r.tournamentId = a1.tournamentId
                   JOIN Numbers n ON n.round <= r.totalRounds
      ),

      FilteredMatches AS (
          SELECT *
          FROM Matches
          WHERE newPos1 + newPos2 = totalPlayers
      ),

-- On enlève les matchs avec BYE
      RealMatches AS (
          SELECT *
          FROM FilteredMatches
          WHERE player1 IS NOT NULL AND player2 IS NOT NULL
      )

-- ALLER
 INSERT INTO encounterTournaments (id, tournamentId, player1, player2, round, encounterDate)
 SELECT
     NEWID(),
     tournamentId,
     player1,
     player2,
     round,
     DATEADD(day, round, GETDATE())
 FROM RealMatches

 UNION ALL

-- RETOUR
 SELECT
     NEWID(),
     tournamentId,
     player2,
     player1,
     round + (totalPlayers - 1),
     DATEADD(day, round + (totalPlayers - 1), GETDATE())
 FROM RealMatches

-- ========================
-- RESULTS
-- ========================
UPDATE encounterTournaments
SET result =
        CASE ABS(CHECKSUM(NEWID())) % 3
            WHEN 0 THEN '1-0'
            WHEN 1 THEN '0-1'
            ELSE '1/2-1/2'
            END