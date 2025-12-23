-- ####################################################
-- Création du schéma initial de la base de données pour RateMyResto
-- ####################################################

-- Table Restaurants
CREATE TABLE dbo.Restaurants
(
    Id INT CONSTRAINT PK_Restaurants PRIMARY KEY IDENTITY(1,1),
    Nom NVARCHAR(100) NOT NULL,
    Adresse NVARCHAR(255) NOT NULL,
    LienGoogleMaps NVARCHAR(2048) NULL,

    -- On s'assure qu'il n'y a pas deux restaurants avec le même nom
    CONSTRAINT AK_Restaurant UNIQUE (Nom)
);

-- Table "Teams"
CREATE TABLE dbo.Teams
(
    Id UNIQUEIDENTIFIER CONSTRAINT PK_Teams PRIMARY KEY,
    Nom NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(255) NULL,
    OwnerTeamId NVARCHAR(450) NOT NULL,

    CONSTRAINT FK_Teams_To_AspNetUsers
        FOREIGN KEY (OwnerTeamId) REFERENCES dbo.AspNetUsers(Id),

    -- On s'assure qu'il n'y a pas deux équipes avec le même nom
    CONSTRAINT AK_Team UNIQUE (Nom)
);

-- Table "UserTeams"
CREATE TABLE dbo.UserTeams
(
    Id INT CONSTRAINT PK_UserTeams PRIMARY KEY IDENTITY(1,1),
    UserId NVARCHAR(450) NOT NULL,
    TeamId UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_UserTeams_To_Teams
        FOREIGN KEY (TeamId) REFERENCES dbo.Teams(Id),

    CONSTRAINT FK_UserTeams_To_AspNetUsers
        FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id),

    -- On s'assure qu'un utilisateur ne peut être
    -- dans une même équipe qu'une seule fois
    CONSTRAINT AK_UserTeam UNIQUE (UserId, TeamId)
);

-- Table "EventRepas"
CREATE TABLE dbo.EventRepas
(
    Id INT CONSTRAINT PK_EventRepas PRIMARY KEY IDENTITY(1,1),
    TeamId UNIQUEIDENTIFIER NOT NULL,
    InitiateurId INT NOT NULL, -- Référence à UserTeams.Id
    RestaurantId INT NOT NULL,
    DateEvenement DATE NOT NULL,

    CONSTRAINT FK_EventRepas_To_Teams
        FOREIGN KEY (TeamId) REFERENCES dbo.Teams(Id),

    CONSTRAINT FK_EventRepas_To_UserTeams
        FOREIGN KEY (InitiateurId) REFERENCES dbo.UserTeams(Id),

    CONSTRAINT FK_EventRepas_To_Restaurants
        FOREIGN KEY (RestaurantId) REFERENCES dbo.Restaurants(Id)
);

-- Table "Participants"
CREATE TABLE dbo.Participants
(
    Id INT CONSTRAINT PK_Participants PRIMARY KEY IDENTITY(1,1),
    EventRepasId INT NOT NULL,
    UserId INT NOT NULL,
    Note DECIMAL(2,1) NULL,
    Commentaire NVARCHAR(1000) NULL,
    DateReview DATE NOT NULL,

    CONSTRAINT FK_Participants_To_EventRepasId
        FOREIGN KEY (EventRepasId) REFERENCES dbo.EventRepas(Id),

    CONSTRAINT FK_Participants_To_UserId_UserTeams
        FOREIGN KEY (UserId) REFERENCES dbo.UserTeams(Id),

    -- On s'assure qu'un utilisateur ne peut participer
    -- qu'une seule fois à une sortie donnée
    CONSTRAINT AK_Participant UNIQUE (EventRepasId, UserId)
);