
-- ####################################################
--Création du schéma initial de la base de données pour RateMyResto
-- ####################################################

-- Table Restaurants
CREATE TABLE dbo.Restaurants
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nom NVARCHAR(100) NOT NULL,
    Adresse NVARCHAR(255) NOT NULL,
    LienGoogleMaps NVARCHAR(2048) NULL,

    -- On s'assure qu'il n'y a pas deux restaurants avec le même nom
    CONSTRAINT AK_Restaurant UNIQUE (Nom)
);

-- Table "Teams"
CREATE TABLE dbo.Teams
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nom NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(255) NULL,

    -- On s'assure qu'il n'y a pas deux équipes avec le même nom
    CONSTRAINT AK_Team UNIQUE (Nom)
);

-- Table "UserTeams"
CREATE TABLE dbo.UserTeams
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId NVARCHAR(450) NOT NULL,
    TeamId INT NOT NULL,

    FOREIGN KEY (TeamId) REFERENCES dbo.Teams(Id),
    FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id),

    -- On s'assure qu'un utilisateur ne peut être
    -- dans une même équipe qu'une seule fois
    CONSTRAINT AK_UserTeam UNIQUE (UserId, TeamId)
);

-- Table "EventRepas"
CREATE TABLE dbo.EventRepas
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    TeamId INT NOT NULL,
    InitiateurId INT NOT NULL, -- Référence à UserTeams.Id
    RestaurantId INT NOT NULL,
    DateEvenement DATE NOT NULL,

    FOREIGN KEY (TeamId) REFERENCES dbo.Teams(Id),
    FOREIGN KEY (InitiateurId) REFERENCES dbo.UserTeams(Id),
    FOREIGN KEY (RestaurantId) REFERENCES dbo.Restaurants(Id)
);

-- Table "Participants"
CREATE TABLE dbo.Participants
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    EventRepasId INT NOT NULL,
    UserId INT NOT NULL,
    Note DECIMAL(2,1) NULL,
    Commentaire NVARCHAR(1000) NULL,
    DateReview DATE NOT NULL,

    FOREIGN KEY (EventRepasId) REFERENCES dbo.EventRepas(Id),
    FOREIGN KEY (UserId) REFERENCES dbo.UserTeams(Id),

    -- On s'assure qu'un utilisateur ne peut participer
    -- qu'une seule fois à une sortie donnée
    CONSTRAINT AK_Participant UNIQUE (EventRepasId, UserId)
);

