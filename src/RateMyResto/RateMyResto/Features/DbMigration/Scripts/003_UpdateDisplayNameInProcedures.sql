-- #################################################
-- Met à jour les procédures stockées pour utiliser DisplayName
-- au lieu de UserName pour l'affichage

-- #################################################
-- Mise à jour de sp_GetTeamByOwner
IF OBJECT_ID('dbo.sp_GetTeamByOwner', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_GetTeamByOwner;
GO

CREATE PROCEDURE sp_GetTeamByOwner
    @Owner NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT eq.Id,
           eq.Nom,
           eq.[Description],
           eq.OwnerTeamId AS 'OwnerId',
           COALESCE(usr.DisplayName, usr.UserName) AS 'OwnerName',
           (
               SELECT u.Id AS 'IdUser',
                      u.UserName,
                      u.DisplayName
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
-- Mise à jour de sp_GetTeamsByUser
IF OBJECT_ID('dbo.sp_GetTeamsByUser', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_GetTeamsByUser;
GO

CREATE PROCEDURE sp_GetTeamsByUser
    @UserId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT eq.Id,
           eq.Nom,
           eq.[Description],
           own.Id AS 'OwnerId',
           COALESCE(own.DisplayName, own.UserName) AS 'OwnerName',
           (
               SELECT u.Id AS 'IdUser',
                      u.UserName,
                      u.DisplayName
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
-- Mise à jour de sp_GetEventById
IF OBJECT_ID('dbo.sp_GetEventById', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_GetEventById;
GO

CREATE PROCEDURE sp_GetEventById
    @IdEvent UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    -- Informations sur les participants
    WITH Participants_CTE (IdUser, UserName, DisplayName, Note, Commentaire, DateReview, StatusId)
    AS
    (
        SELECT p.UserId,
               usr.UserName,
               usr.DisplayName,
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
        SELECT COALESCE(usr.DisplayName, usr.UserName)
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
           evt.Note AS 'NoteGlobale',
           rt.Nom AS 'NomRestaurant',
           rt.Adresse AS 'Adresse',
           rt.LienGoogleMaps AS 'LienGoogleMaps',
           eq.Nom AS 'NomEquipe',
           (
               SELECT IdUser, UserName, DisplayName, Note, Commentaire, DateReview, StatusId
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
