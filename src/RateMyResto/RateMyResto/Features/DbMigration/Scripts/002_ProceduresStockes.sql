-- ####################################################
-- Création des procédures stockées pour RateMyResto
-- ####################################################

-- #####################################################################################

-- #################################################
-- ## Gestion des équipes
-- #################################################

-- #################################################
-- Permet de créer une nouvelle équipe
CREATE PROCEDURE sp_CreateTeam
    @IdTeam UNIQUEIDENTIFIER,
    @Nom NVARCHAR(100),
    @Description NVARCHAR(255) = NULL,
    @Owner NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Teams (Id, Nom, [Description], OwnerTeamId)
    VALUES (@IdTeam, @Nom, @Description, @Owner);
END
GO

-- #################################################
-- Permet de récupérer les équipes par son propriétaire
-- et leurs membres
CREATE PROCEDURE sp_GetTeamByOwner
    @Owner NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT eq.Id,
           eq.Nom,
           eq.[Description],
           eq.OwnerTeamId AS 'OwnerId',
           usr.UserName AS 'OwnerName',
           (
               SELECT u.Id AS 'IdUser',
                      u.UserName
               FROM dbo.UserTeams ut
               INNER JOIN dbo.AspNetUsers u
                  ON u.Id = ut.UserId
               WHERE ut.TeamId = eq.Id
               FOR JSON PATH
           ) AS 'Members'
    FROM dbo.Teams eq
    -- JOINTURE SEULEMENT AVEC LE OWNER
    INNER JOIN dbo.AspNetUsers usr
       ON usr.Id = eq.OwnerTeamId
    WHERE eq.OwnerTeamId = @Owner
    FOR JSON PATH
END
GO

-- #################################################
-- Permet d'ajouter un utilisateur à une équipe
CREATE PROCEDURE sp_AddMemberToTeam
    @UserId NVARCHAR(450),
    @TeamId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.UserTeams (UserId, TeamId)
    VALUES (@UserId, @TeamId);
END
GO

-- #################################################
-- Permet de récupérer les équipes d'un utilisateur
-- en incluant les membres de chaque équipe
CREATE PROCEDURE sp_GetTeamsByUser
    @UserId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT eq.Id,
           eq.Nom,
           eq.[Description],
           own.Id AS 'OwnerId',
           own.UserName AS 'OwnerName',
           (
               SELECT u.Id AS 'IdUser',
                      u.UserName
               FROM dbo.UserTeams ut2
               INNER JOIN dbo.AspNetUsers u
                  ON u.Id = ut2.UserId
               WHERE ut2.TeamId = eq.Id
               FOR JSON PATH
           ) AS 'Members'
    FROM dbo.Teams eq
    -- On cherche les équipes où l'utilisateur est MEMBRE
    INNER JOIN dbo.UserTeams ut
       ON eq.Id = ut.TeamId
    -- pour trouver le propriétaire
    INNER JOIN dbo.AspNetUsers own
       ON eq.OwnerTeamId = own.Id
    WHERE ut.UserId = @UserId
    FOR JSON PATH;
END
GO

-- #################################################
-- Permet de récupérer les équipes d'un utilisateur
CREATE PROCEDURE sp_GetTeamsByUser_Light
    @UserId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT eq.Id AS 'IdEquipe',
           eq.Nom AS 'NomEquipe'
    FROM dbo.Teams eq
    INNER JOIN dbo.UserTeams ut
        ON eq.Id = ut.TeamId
    WHERE ut.UserId = @UserId
    FOR JSON PATH;
END
GO

-- #################################################
-- Permet de supprimer un utilisateur d'une équipe
CREATE PROCEDURE sp_RemoveUserFromTeam
    @UserId NVARCHAR(450),
    @TeamId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.UserTeams
    WHERE UserId = @UserId
      AND TeamId = @TeamId;
END
GO

-- #################################################
-- Permet de récupérer l'ID du UserTeam
CREATE PROCEDURE sp_GetUserTeamId
    @UserId NVARCHAR(450),
    @TeamId UNIQUEIDENTIFIER
AS
BEGIN
SET NOCOUNT ON;

SELECT Id
FROM dbo.UserTeams
WHERE UserId = @UserId
  AND TeamId = @TeamId;
END
GO

-- #####################################################################################

-- #################################################
-- ## Gestion des évènements de repas
-- #################################################

-- #################################################
-- Permet de créer un nouvel évènement de repas
CREATE PROCEDURE sp_CreateEvent
    @IdEvent UNIQUEIDENTIFIER,
    @TeamId UNIQUEIDENTIFIER,
    @InitiateurId INT,
    @RestaurantId INT,
    @DateEvenement DATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Création de l'évènement
    INSERT INTO dbo.EventRepas (Id, TeamId, InitiateurId, RestaurantId, DateEvenement)
    VALUES (@IdEvent, @TeamId, @InitiateurId, @RestaurantId, @DateEvenement);

    -- Ajout de toute l'équipe en tant que participants en invité
    INSERT INTO dbo.Participants (EventRepasId, UserId, StatusParticipationId)
    SELECT @IdEvent, ut.Id, 1 -- 'Invité'
    FROM dbo.UserTeams ut
    WHERE ut.TeamId = @TeamId;
END
GO

-- #################################################
-- Permet de récupérer la liste des évènements
-- par rapport à l'id d'un utilisateur
CREATE PROCEDURE sp_GetEventsByUser
    @UserId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT evt.Id AS 'IdEvent',
           evt.DateEvenement AS 'DateEvent',
           resto.Id AS 'IdRestaurant',
           resto.Nom AS 'RestaurantName',
           part.StatusParticipationId AS 'ParticipationStatus',
           eq.Id AS 'IdEquipe',
           eq.Nom AS 'EquipeName'
    FROM dbo.EventRepas evt
    INNER JOIN dbo.Restaurants resto
       ON evt.RestaurantId = resto.Id
    INNER JOIN dbo.Participants part
       ON part.EventRepasId = evt.Id
    INNER JOIN dbo.UserTeams usrEq
       ON usrEq.Id = part.UserId
    INNER JOIN dbo.AspNetUsers usr
       ON usr.Id = usrEq.UserId
    INNER JOIN dbo.Teams eq
       ON eq.Id = usrEq.TeamId
    WHERE usr.Id = @UserId
    FOR JSON PATH;
END
GO

-- #################################################
-- Permet de mettre à jour le statut de participation d'un utilisateur
CREATE PROCEDURE sp_UpdateParticipationStatus
    @UserId VARCHAR(450),
    @EventId UNIQUEIDENTIFIER,
    @StatusParticipationId TINYINT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE p
    SET p.StatusParticipationId = @StatusParticipationId
    FROM dbo.Participants p
    INNER JOIN dbo.UserTeams ut
       ON ut.Id = p.UserId
    INNER JOIN dbo.EventRepas evt
       ON evt.Id = @EventId
      AND ut.TeamId = evt.TeamId
    WHERE ut.UserId = @UserId
      AND p.EventRepasId = @EventId;
END
GO

-- #################################################
-- Permet de récupérer le détail d'un évènement par son Id
CREATE PROCEDURE sp_GetEventById
    @IdEvent UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    -- Informations sur les participants
    WITH Participants_CTE (IdUser, UserName, Note, Commentaire, DateReview, StatusId)
    AS
    (
        SELECT p.UserId,
               usr.UserName,
               p.Note,
               p.Commentaire,
               p.DateReview,
               p.StatusParticipationId
        FROM dbo.Participants p
        INNER JOIN dbo.UserTeams ut
           ON ut.Id = p.UserId
        INNER JOIN dbo.AspNetUsers usr
           ON usr.Id = ut.UserId
        WHERE p.EventRepasId = @IdEvent
    ),
    -- Informations sur l'initiateur de l'évènement
    InfoInitiateur_CTE (InitiateurName) AS
    (
        SELECT usr.UserName
        FROM dbo.EventRepas evt
        INNER JOIN dbo.UserTeams ut 
	       ON ut.Id = evt.InitiateurId
        INNER JOIN dbo.AspNetUsers usr 
	       ON usr.Id = ut.UserId
        WHERE evt.Id = @IdEvent
    )
    -- Récupération des informations de l'évènement
    SELECT evt.Id,
           evt.DateEvenement,
           rt.Nom AS 'NomRestaurant',
           rt.Adresse AS 'Adresse',
           rt.LienGoogleMaps AS 'LienGoogleMaps',
           eq.Nom AS 'NomEquipe',
           (
               SELECT IdUser, UserName, Note, Commentaire, DateReview, StatusId
               FROM Participants_CTE
               FOR JSON PATH, INCLUDE_NULL_VALUES
           ) AS 'InfoParticipants',
           (SELECT TOP 1 InitiateurName 
	          FROM InfoInitiateur_CTE) AS 'Initiateur'
    FROM dbo.EventRepas evt
    INNER JOIN dbo.Restaurants rt 
       ON rt.Id = evt.RestaurantId
    INNER JOIN dbo.Teams eq 
       ON eq.Id = evt.TeamId
    WHERE evt.Id = @IdEvent
    FOR JSON PATH, WITHOUT_ARRAY_WRAPPER, INCLUDE_NULL_VALUES;
END
GO

-- #################################################
-- Permet de sauvegarder la revue
-- d'un participant pour un évènement
CREATE PROCEDURE sp_SaveParticipantReview
    @EventId UNIQUEIDENTIFIER,
    @UserId VARCHAR(450),
    @Note DECIMAL(2,1),
    @DateReview DATE,
    @Commentaire NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE p
    SET p.Note = @Note,
        p.Commentaire = @Commentaire,
        p.DateReview = @DateReview
    FROM dbo.Participants p
    INNER JOIN dbo.UserTeams ut
       ON ut.Id = p.UserId
    WHERE ut.UserId = @UserId
      AND p.EventRepasId = @EventId;
END
GO

-- #####################################################################################
-- #################################################
-- ## Gestion des restaurants
-- #################################################

-- #################################################
-- Permet de créer un nouveau restaurant
CREATE PROCEDURE sp_CreateRestaurant
    @Nom NVARCHAR(100),
    @Adresse NVARCHAR(255),
    @LienGoogleMaps NVARCHAR(2048) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Restaurants (Nom, Adresse, LienGoogleMaps)
    VALUES (@Nom, @Adresse, @LienGoogleMaps);

    -- Retourner l'ID inséré
    -- SCOPE_IDENTITY() retourne un type de données NUMERIC
    -- https://learn.microsoft.com/fr-fr/sql/t-sql/functions/scope-identity-transact-sql?view=sql-server-ver17
    -- Conversion en INT pour correspondre au type de données de la colonne IdRestaurant
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS 'IdRestaurant';
END
GO

-- #################################################
-- Permet de récupérer la liste des restaurants
CREATE PROCEDURE sp_GetAllRestaurants
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id AS 'IdRestaurant',
           Nom AS 'RestaurantName',
           Adresse AS 'RestaurantAddress',
           LienGoogleMaps AS 'RestaurantGoogleMapsLink'
    FROM dbo.Restaurants
    FOR JSON PATH;
END
GO

-- #################################################
-- Permet de récupérer un restaurant par son Id
CREATE PROCEDURE sp_GetRestaurantById
    @RestaurantId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id AS 'IdRestaurant',
           Nom AS 'RestaurantName',
           Adresse AS 'RestaurantAddress',
           LienGoogleMaps AS 'RestaurantGoogleMapsLink'
    FROM dbo.Restaurants
    WHERE Id = @RestaurantId
    FOR JSON PATH, WITHOUT_ARRAY_WRAPPER;
END
GO

-- #################################################
-- Permet de vérifer l'existence d'un restaurant par son nom
CREATE PROCEDURE sp_CheckRestaurantExistByName
    @Nom NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1) AS 'Exists'
    FROM dbo.Restaurants
    WHERE Nom = @Nom;
END
GO