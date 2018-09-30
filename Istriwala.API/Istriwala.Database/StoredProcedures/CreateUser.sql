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
	@UserName int,
	@Password varchar(256),
	@Name varchar(256),
	@Address varchar(250),
	@EmailId varchar(20),
	@MobileNo varchar(20),
	@Gender varchar(50),
	@ProfileUrl varchar(50)=NULL
AS
BEGIN
SET NOCOUNT ON
	
	INSERT INTO Users (Id
	,UserName
	,[Password]
	,[Name]
	,[Address]
	,EmailId
	,MObileNo
	,Gender
	,ProfileUrl)
	VALUES (@Id
	,@UserName
	,@Password
	,@Name
	,@Address
	,@EmailId
	,@MObileNo
	,@Gender
	,@ProfileUrl)

END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CreateUser]') AND type in (N'P', N'PC'))
	PRINT '<<< CREATED PROCEDURE dbo.CreateUser >>>'
ELSE
	PRINT '<<< FAILED CREATING PROCEDURE dbo.CreateUser >>>'
GO