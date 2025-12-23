-- ####################################################
-- Création des procédures stockées pour RateMyResto
-- ####################################################


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

-- Permet de récupérer les équipes par son propriétaire
CREATE PROCEDURE sp_GetTeamByOwner
    @Owner NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT eq.Id,
           eq.Nom,
           eq.[Description],
           eq.OwnerTeamId,
           (
               SELECT
                   u.Id AS 'IdUser',
                   u.UserName
               FROM dbo.UserTeams ut
               INNER JOIN dbo.AspNetUsers u
                  ON u.Id = ut.UserId
               WHERE ut.TeamId = eq.Id
               FOR JSON PATH
           ) AS 'Members'
    FROM dbo.Teams eq
    WHERE eq.OwnerTeamId = @Owner
    FOR JSON PATH
END
GO

-- Permet d'ajouter un utilisateur à une équipe
CREATE PROCEDURE sp_AddUserToTeam
    @UserId NVARCHAR(450),
    @TeamId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.UserTeams (UserId, TeamId)
    VALUES (@UserId, @TeamId);
END
GO

-- Permet de récupérer les membres d'une équipe
CREATE PROCEDURE sp_GetUsersByTeam
    @TeamId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ut.Id, ut.UserId, ut.TeamId
    FROM dbo.UserTeams ut
    WHERE ut.TeamId = @TeamId
    FOR JSON PATH;
END
GO