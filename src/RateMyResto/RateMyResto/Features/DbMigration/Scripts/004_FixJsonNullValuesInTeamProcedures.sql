-- #################################################
-- Correction des procédures stockées sp_GetTeamByOwner et sp_GetTeamsByUser
-- 
-- PROBLÈME : Les sous-requêtes FOR JSON PATH qui récupèrent les membres des équipes
-- n'incluaient pas la clause INCLUDE_NULL_VALUES. Lorsque DisplayName est NULL,
-- le champ n'apparaissait pas dans le JSON retourné, ce qui causait une erreur
-- de désérialisation côté C# car la propriété TeamMemberDb.DisplayName est marquée
-- comme "required string?" et attend donc explicitement la présence du champ même si null.
-- 
-- SOLUTION : Ajout de INCLUDE_NULL_VALUES dans les sous-requêtes FOR JSON PATH
-- pour garantir que tous les champs soient présents dans le JSON, même s'ils sont NULL.
-- #################################################

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
               FOR JSON PATH, INCLUDE_NULL_VALUES
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
               FOR JSON PATH, INCLUDE_NULL_VALUES
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
