-- =============================================================================
-- Migration 010 : Procédure de récupération des destinataires pour la
--                 notification de note finale d'un événement
-- =============================================================================

CREATE PROCEDURE sp_GetFinalScoreEmailData
    @EventRepasId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT usr.Email                               AS Email,
           COALESCE(usr.DisplayName, usr.UserName) AS DisplayName
    FROM dbo.EventRepas AS evt
    INNER JOIN dbo.Participants AS p
       ON p.EventRepasId = evt.Id
    INNER JOIN dbo.UserTeams AS ut
       ON ut.Id = p.UserId
    INNER JOIN dbo.AspNetUsers AS usr
       ON usr.Id = ut.UserId
    WHERE evt.Id = @EventRepasId
      AND p.StatusParticipationId = 2
      AND usr.Email IS NOT NULL
      AND usr.Email <> ''
      AND ut.DateFinPresence IS NULL
    FOR JSON PATH;
END
GO
