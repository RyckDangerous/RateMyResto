-- ####################################################
-- Création des procédures stockées pour RateMyResto
-- ####################################################

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
           au.UserName AS 'OwnerName',
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
    INNER JOIN dbo.UserTeams ut
        ON eq.Id = ut.TeamId
    INNER JOIN dbo.AspNetUsers au
        ON au.Id = ut.UserId
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

    SELECT
        eq.Id,
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
-- Permet de supprimer un utilisateur d'une équipe
CREATE PROCEDURE sp_RemoveUserFromTeam
    @UserId NVARCHAR(450),
    @TeamId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.UserTeams
    WHERE UserId = @UserId AND TeamId = @TeamId;
END