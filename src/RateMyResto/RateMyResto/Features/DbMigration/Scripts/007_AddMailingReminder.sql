-- #################################################
-- Mise en place du système de rappels de vote par email.
--
-- - Création de la table dbo.ReminderSent pour tracer les
--   rappels déjà envoyés (idempotence du mode CLI --reminder).
-- - Création de sp_GetPendingVoteReminders qui retourne, au
--   format JSON, les participants confirmés d'événements
--   terminés qui n'ont pas encore voté et qui n'ont pas encore
--   reçu de rappel.
-- - Création de sp_MarkReminderSent qui enregistre un envoi
--   de rappel pour un couple (évènement, participant).
-- #################################################

-- #################################################
-- Table des rappels envoyés
CREATE TABLE dbo.ReminderSent
(
    Id           INT IDENTITY(1,1) NOT NULL,
    EventRepasId UNIQUEIDENTIFIER  NOT NULL,
    UserTeamsId  INT               NOT NULL,
    SentAt       DATETIME2         NOT NULL,

    CONSTRAINT PK_ReminderSent
        PRIMARY KEY (Id),

    CONSTRAINT FK_ReminderSent_EventRepas
        FOREIGN KEY (EventRepasId) REFERENCES dbo.EventRepas(Id),

    CONSTRAINT FK_ReminderSent_UserTeams
        FOREIGN KEY (UserTeamsId) REFERENCES dbo.UserTeams(Id),

    CONSTRAINT UK_ReminderSent_Event_User
        UNIQUE (EventRepasId, UserTeamsId)
);
GO

CREATE INDEX IX_ReminderSent_EventRepasId
    ON dbo.ReminderSent (EventRepasId);
GO


-- #################################################
-- Récupère la liste des rappels de vote à envoyer.
-- Critères :
--   - Événement dont la date est passée (< aujourd'hui)
--   - Participant au statut "Confirmé" (StatusParticipationId = 2)
--   - Pas de note déposée (Note IS NULL)
--   - Utilisateur actif (UserTeams.DateFinPresence IS NULL)
--   - Adresse email renseignée
--   - Aucun rappel déjà envoyé pour ce couple (EventRepasId, UserTeamsId)
-- #################################################
CREATE PROCEDURE sp_GetPendingVoteReminders
AS
BEGIN
    SET NOCOUNT ON;

    SELECT evt.Id                                  AS EventId,
           evt.DateEvenement                       AS DateEvenement,
           r.Nom                                   AS NomRestaurant,
           ut.Id                                   AS UserTeamsId,
           usr.Id                                  AS AspNetUserId,
           usr.Email                               AS Email,
           COALESCE(usr.DisplayName, usr.UserName) AS DisplayName
    FROM dbo.EventRepas     AS evt
    INNER JOIN dbo.Participants AS p
        ON p.EventRepasId = evt.Id
    INNER JOIN dbo.UserTeams    AS ut
        ON ut.Id = p.UserId
    INNER JOIN dbo.AspNetUsers  AS usr
        ON usr.Id = ut.UserId
    INNER JOIN dbo.Restaurants  AS r
        ON r.Id = evt.RestaurantId
    LEFT JOIN dbo.ReminderSent  AS rs
        ON rs.EventRepasId = evt.Id
       AND rs.UserTeamsId  = ut.Id
    WHERE evt.DateEvenement < CAST(GETDATE() AS DATE)
      AND p.StatusParticipationId = 2
      AND p.Note IS NULL
      AND ut.DateFinPresence IS NULL
      AND usr.Email IS NOT NULL
      AND usr.Email <> ''
      AND rs.Id IS NULL
    ORDER BY evt.DateEvenement ASC, usr.Email ASC
    FOR JSON PATH, INCLUDE_NULL_VALUES;
END
GO


-- #################################################
-- Enregistre un rappel envoyé pour un couple (évènement, participant)
-- L'unique UK_ReminderSent_Event_User garantit l'idempotence en base.
-- #################################################
CREATE PROCEDURE sp_MarkReminderSent
    @EventRepasId UNIQUEIDENTIFIER,
    @UserTeamsId  INT,
    @SentAt       DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.ReminderSent (EventRepasId, UserTeamsId, SentAt)
    VALUES (@EventRepasId, @UserTeamsId, @SentAt);
END
GO
