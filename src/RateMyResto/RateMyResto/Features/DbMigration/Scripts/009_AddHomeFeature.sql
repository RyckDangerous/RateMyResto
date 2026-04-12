-- ============================================================
-- Migration 009 : Procédures stockées pour la vitrine publique
-- ============================================================

-- ------------------------------------------------------------
-- 1. sp_GetAllTeamsPublic
--    Retourne toutes les équipes avec nb membres et nb sorties.
-- ------------------------------------------------------------
CREATE PROCEDURE sp_GetAllTeamsPublic
AS
BEGIN
    SET NOCOUNT ON;

    SELECT t.Id,
           t.Nom,
           t.[Description],
           COUNT(DISTINCT ut.Id) AS NombreMembres,
           COUNT(DISTINCT evt.Id) AS NombreEvenements
    FROM dbo.Teams AS t
    LEFT JOIN dbo.UserTeams AS ut
        ON ut.TeamId = t.Id
       AND ut.DateFinPresence IS NULL
    LEFT JOIN dbo.EventRepas AS evt
        ON evt.TeamId = t.Id
       AND evt.DateEvenement < CAST(GETDATE() AS DATE)
    GROUP BY t.Id, t.Nom, t.[Description]
    ORDER BY COUNT(DISTINCT evt.Id) DESC, t.Nom ASC
    FOR JSON PATH;
END
GO

-- ------------------------------------------------------------
-- 2. sp_GetTeamEventsPublic
--    Retourne l'équipe + ses événements passés (JSON imbriqué).
-- ------------------------------------------------------------
CREATE PROCEDURE sp_GetTeamEventsPublic
    @TeamId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT t.Id                AS TeamId,
           t.Nom               AS NomEquipe,
           t.[Description]     AS DescriptionEquipe,
           (
               SELECT evt.Id   AS EventId,
                      r.Nom    AS NomRestaurant,
                      r.Adresse AS Adresse,
                      evt.DateEvenement,
                      evt.Note AS NoteGlobale
               FROM dbo.EventRepas AS evt
               INNER JOIN dbo.Restaurants AS r
                   ON r.Id = evt.RestaurantId
               WHERE evt.TeamId = t.Id
                 AND evt.DateEvenement < CAST(GETDATE() AS DATE)
               ORDER BY evt.DateEvenement DESC
               FOR JSON PATH
           ) AS Evenements
    FROM dbo.Teams AS t
    WHERE t.Id = @TeamId
    FOR JSON PATH, WITHOUT_ARRAY_WRAPPER, INCLUDE_NULL_VALUES;
END
GO

-- ------------------------------------------------------------
-- 3. sp_GetEventDetailPublic
--    Retourne le détail d'un événement avec avis anonymisés.
-- ------------------------------------------------------------
CREATE PROCEDURE sp_GetEventDetailPublic
    @EventId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT evt.Id,
           evt.DateEvenement,
           evt.Note            AS NoteGlobale,
           r.Nom               AS NomRestaurant,
           r.Adresse           AS Adresse,
           r.LienGoogleMaps,
           t.Nom               AS NomEquipe,
           t.Id                AS TeamId,
           (
               SELECT p.Note,
                      p.Commentaire,
                      p.DateReview
               FROM dbo.Participants AS p
               WHERE p.EventRepasId = evt.Id
                 AND p.StatusParticipationId = 2
                 AND p.Note IS NOT NULL
               FOR JSON PATH
           ) AS Avis
    FROM dbo.EventRepas AS evt
    INNER JOIN dbo.Restaurants AS r
        ON r.Id = evt.RestaurantId
    INNER JOIN dbo.Teams AS t
        ON t.Id = evt.TeamId
    WHERE evt.Id = @EventId
    FOR JSON PATH, WITHOUT_ARRAY_WRAPPER, INCLUDE_NULL_VALUES;
END
GO
