USE [Istriwala]
GO

IF (SELECT object_ID('[dbo].[<ProcName>]')) IS NULL
	EXEC('CREATE PROCEDURE [dbo].[<ProcName>] AS BEGIN SELECT 1 END')
	GRANT EXEC ON [dbo].[<ProcName>] TO Analyst
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/**************************************************************************************
REMARKS:

-- <initials date>: <comment>

**************************************************************************************/

ALTER PROCEDURE [dbo].[<ProcName>]
	
AS
BEGIN
SET NOCOUNT ON
	
	

END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[<ProcName>]') AND type in (N'P', N'PC'))
	PRINT '<<< CREATED PROCEDURE dbo.<ProcName> >>>'
ELSE
	PRINT '<<< FAILED CREATING PROCEDURE dbo.<ProcName> >>>'
GO