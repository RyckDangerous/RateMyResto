-- #################################################
-- Mise à jour de sp_GetEventById
--
-- AJOUT : expose par participant l'AspNetUserId (string Identity)
--         expose à la racine InitiateurAspNetUserId et OwnerAspNetUserId
--         nécessaire pour que le service puisse vérifier si l'utilisateur courant
--         est l'initiateur ou le responsable d'équipe, et appeler
--         sp_UpdateParticipationStatus avec le bon identifiant.
-- #################################################

IF OBJECT_ID('dbo.sp_GetEventById', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_GetEventById;
GO

CREATE PROCEDURE sp_GetEventById
    @IdEvent UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    -- Informations sur les participants
    WITH Participants_CTE (IdUser, AspNetUserId, UserName, DisplayName, Note, Commentaire, DateReview, StatusId)
    AS
    (
        SELECT p.UserId,
               ut.UserId,
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
    InfoInitiateur_CTE (InitiateurName, InitiateurAspNetUserId) AS
    (
        SELECT COALESCE(usr.DisplayName, usr.UserName),
               ut.UserId
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
           eq.OwnerTeamId AS 'OwnerAspNetUserId',
           (
               SELECT IdUser, AspNetUserId, UserName, DisplayName, Note, Commentaire, DateReview, StatusId
               FROM Participants_CTE
               FOR JSON PATH, INCLUDE_NULL_VALUES
           ) AS 'InfoParticipants',
           (SELECT TOP 1 InitiateurName
            FROM InfoInitiateur_CTE) AS 'Initiateur',
           (SELECT TOP 1 InitiateurAspNetUserId
            FROM InfoInitiateur_CTE) AS 'InitiateurAspNetUserId'
    FROM dbo.EventRepas evt
    INNER JOIN dbo.Restaurants rt
       ON rt.Id = evt.RestaurantId
    INNER JOIN dbo.Teams eq
       ON eq.Id = evt.TeamId
    WHERE evt.Id = @IdEvent
    FOR JSON PATH, WITHOUT_ARRAY_WRAPPER, INCLUDE_NULL_VALUES;
END
GO
