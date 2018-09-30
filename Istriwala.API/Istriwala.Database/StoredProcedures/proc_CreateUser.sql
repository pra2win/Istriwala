USE [Istriwala]
GO

IF (SELECT object_ID('[dbo].[CreateUser]')) IS NULL
	EXEC('CREATE PROCEDURE [dbo].[CreateUser] AS BEGIN SELECT 1 END')
	GRANT EXEC ON [dbo].[CreateUser] TO Analyst
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/**************************************************************************************
REMARKS:

-- PJ 9.29.2018: Initial Creation

**************************************************************************************/

ALTER PROCEDURE [dbo].[CreateUser]
	@Id	int,
	@UserName varchar(256),
	@Password varchar(256),
	@Name varchar(256),
	@Address varchar(250),
	@EmailId varchar(20),
	@MobileNo varchar(20),
	@Gender varchar(50),
	@Roles varchar(max),
	@ProfileUrl varchar(50)=NULL

AS
BEGIN
SET NOCOUNT ON
	
	DECLARE @RolesDataTable TABLE (ITEM VARCHAR(MAX))
	INSERT INTO @RolesDataTable SELECT item FROM dbo.fnSplit(@Roles, ',')

	INSERT INTO Users (UserName
	,[Password]
	,[Name]
	,[Address]
	,EmailId
	,MObileNo
	,Gender
	,ProfileUrl)
	VALUES (@UserName
	,@Password
	,@Name
	,@Address
	,@EmailId
	,@MObileNo
	,@Gender
	,@ProfileUrl)

	SET @Id = SCOPE_IDENTITY()

	INSERT INTO UserRoles(UserId, RoleId)
	SELECT @Id, cast(r.item as int)
	FROM @RolesDataTable r

	RETURN @Id
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CreateUser]') AND type in (N'P', N'PC'))
	PRINT '<<< CREATED PROCEDURE dbo.CreateUser >>>'
ELSE
	PRINT '<<< FAILED CREATING PROCEDURE dbo.CreateUser >>>'
GO