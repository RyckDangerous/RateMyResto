-- =============================================================================
-- Migration 008 : Procédure de récupération des destinataires pour la
--                 notification de création d'un nouvel événement
-- =============================================================================

CREATE PROCEDURE sp_GetNewEventEmailData
    @EventRepasId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT usr.Email                                        AS Email,
           COALESCE(usr.DisplayName, usr.UserName)          AS DisplayName,
           COALESCE(ini_usr.DisplayName, ini_usr.UserName)  AS NomOrganisateur,
           t.Nom                                            AS NomEquipe,
           r.Nom                                            AS NomRestaurant,
           r.Adresse                                        AS AdresseRestaurant,
           evt.DateEvenement                                AS DateEvenement
    FROM dbo.EventRepas AS evt
    INNER JOIN dbo.Participants AS p
        ON p.EventRepasId = evt.Id
    INNER JOIN dbo.UserTeams AS ut
        ON ut.Id = p.UserId
    INNER JOIN dbo.AspNetUsers AS usr
        ON usr.Id = ut.UserId
    INNER JOIN dbo.Teams AS t
        ON t.Id = evt.TeamId
    INNER JOIN dbo.Restaurants AS r
        ON r.Id = evt.RestaurantId
    INNER JOIN dbo.UserTeams AS ini_ut
        ON ini_ut.Id = evt.InitiateurId
    INNER JOIN dbo.AspNetUsers AS ini_usr
        ON ini_usr.Id = ini_ut.UserId
    WHERE evt.Id = @EventRepasId
      AND usr.Email IS NOT NULL
      AND usr.Email <> ''
      AND ut.DateFinPresence IS NULL
    FOR JSON PATH;
END
GO
