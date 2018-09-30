USE [Istriwala]
GO

IF (SELECT object_ID('[dbo].[GetUser]')) IS NULL
	EXEC('CREATE PROCEDURE [dbo].[GetUser] AS BEGIN SELECT 1 END')
	GRANT EXEC ON [dbo].[GetUser] TO Analyst
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/**************************************************************************************
REMARKS:

-- PJ 9.29.2018: Initial Creation
-- PJ 9.30.2018: Added Roles
**************************************************************************************/

ALTER PROCEDURE [dbo].[GetUser]
	@Id INT
AS
BEGIN
SET NOCOUNT ON
	
SELECT [Id]
	,[UserName]
	,[Password]
	,[Name]
	,[Address]
	,[EmailId]
	,[MobileNo]
	,[Gender]
	,[ProfileUrl]
	,[Created]
	,[Updated]
	,[IsActive]
	,[IsDelete]
FROM [dbo].[Users] WHERE Id = @id;

SELECT RoleId FROM UserRoles WHERE UserId = @id
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetUser]') AND type in (N'P', N'PC'))
	PRINT '<<< CREATED PROCEDURE dbo.GetUser >>>'
ELSE
	PRINT '<<< FAILED CREATING PROCEDURE dbo.GetUser >>>'
GO