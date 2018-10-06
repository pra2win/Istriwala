USE [Istriwala]
GO

IF (SELECT object_ID('[dbo].[Login]')) IS NULL
	EXEC('CREATE PROCEDURE [dbo].[Login] AS BEGIN SELECT 1 END')
	GRANT EXEC ON [dbo].[Login] TO Analyst
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/**************************************************************************************
REMARKS:

-- PJ 10.6.2018: Intial Creation

**************************************************************************************/

ALTER PROCEDURE [dbo].[Login]
@UserName varchar(256),
@Password varchar(256)
AS
BEGIN
SET NOCOUNT ON
	DECLARE @Id INT;

SELECT @Id = Id
FROM dbo.Users
WHERE (
		EmailId = @UserName
		OR UserName = @UserName
		)
	AND Password = @Password
	AND IsActive = 1

	
SELECT *FROM dbo.Users where id=@Id

SELECT r.RoleId, 
	r.Role
FROM UserRoles u
INNER JOIN Roles r
	ON r.RoleId = u.RoleId
WHERE u.UserId = @Id
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Login]') AND type in (N'P', N'PC'))
	PRINT '<<< CREATED PROCEDURE dbo.Login >>>'
ELSE
	PRINT '<<< FAILED CREATING PROCEDURE dbo.Login >>>'
GO
