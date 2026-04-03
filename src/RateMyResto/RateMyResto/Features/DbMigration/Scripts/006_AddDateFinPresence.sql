-- #################################################
-- Gestion des membres inactifs dans les équipes
--
-- CONTEXTE : Un membre peut quitter l'usage d'une équipe sans être supprimé.
-- Supprimer la ligne UserTeams détruirait l'historique (votes, avis).
-- La colonne DateFinPresence permet de marquer la fin de présence active :
--   - NULL  → membre actif, invité aux futurs événements
--   - Date  → inactif depuis cette date, non invité aux futurs événements
--   La valeur est réversible (réactivation possible).
-- #################################################

-- Ajout de la colonne sur UserTeams
ALTER TABLE dbo.UserTeams
    ADD DateFinPresence DATE NULL;
GO

-- #################################################
-- Nouvelle procédure : définir ou effacer la date de fin de présence
-- Seul le responsable de l'équipe appelle cette procédure (contrôle côté application).
-- #################################################
CREATE PROCEDURE sp_SetMemberEndDate
    @UserId    NVARCHAR(450),
    @TeamId    UNIQUEIDENTIFIER,
    @DateFinPresence DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.UserTeams
    SET DateFinPresence = @DateFinPresence
    WHERE UserId = @UserId
      AND TeamId = @TeamId;
END
GO

-- #################################################
-- Mise à jour de sp_CreateEvent
-- Exclure les membres inactifs (DateFinPresence IS NOT NULL) de l'invitation
-- #################################################
IF OBJECT_ID('dbo.sp_CreateEvent', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_CreateEvent;
GO

CREATE PROCEDURE sp_CreateEvent
    @IdEvent       UNIQUEIDENTIFIER,
    @TeamId        UNIQUEIDENTIFIER,
    @InitiateurId  INT,
    @RestaurantId  INT,
    @DateEvenement DATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Création de l'évènement
    INSERT INTO dbo.EventRepas (Id, TeamId, InitiateurId, RestaurantId, DateEvenement)
    VALUES (@IdEvent, @TeamId, @InitiateurId, @RestaurantId, @DateEvenement);

    -- Ajout uniquement des membres actifs (DateFinPresence IS NULL) en tant que participants
    INSERT INTO dbo.Participants (EventRepasId, UserId, StatusParticipationId)
    SELECT @IdEvent, ut.Id, 1 -- 'Invité'
    FROM dbo.UserTeams ut
    WHERE ut.TeamId = @TeamId
      AND ut.DateFinPresence IS NULL;
END
GO

-- #################################################
-- Mise à jour de sp_GetTeamByOwner
-- Expose DateFinPresence dans la liste des membres
-- #################################################
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
                      u.DisplayName,
                      ut.DateFinPresence
               FROM dbo.UserTeams ut
               INNER JOIN dbo.AspNetUsers u
                  ON u.Id = ut.UserId
               WHERE ut.TeamId = eq.Id
               FOR JSON PATH, INCLUDE_NULL_VALUES
           ) AS 'Members'
    FROM dbo.Teams eq
    INNER JOIN dbo.AspNetUsers usr
       ON usr.Id = eq.OwnerTeamId
    WHERE eq.OwnerTeamId = @Owner
    FOR JSON PATH
END
GO

-- #################################################
-- Mise à jour de sp_GetTeamsByUser
-- Expose DateFinPresence dans la liste des membres
-- #################################################
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
                      u.DisplayName,
                      ut2.DateFinPresence
               FROM dbo.UserTeams ut2
               INNER JOIN dbo.AspNetUsers u
                  ON u.Id = ut2.UserId
               WHERE ut2.TeamId = eq.Id
               FOR JSON PATH, INCLUDE_NULL_VALUES
           ) AS 'Members'
    FROM dbo.Teams eq
    INNER JOIN dbo.UserTeams ut
       ON eq.Id = ut.TeamId
    INNER JOIN dbo.AspNetUsers own
       ON eq.OwnerTeamId = own.Id
    WHERE ut.UserId = @UserId
    FOR JSON PATH;
END
GO
