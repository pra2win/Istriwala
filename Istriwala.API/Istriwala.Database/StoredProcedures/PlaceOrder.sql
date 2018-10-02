USE [Istriwala]
GO

IF (SELECT object_ID('[dbo].[PlaceOrder]')) IS NULL
	EXEC('CREATE PROCEDURE [dbo].[PlaceOrder] AS BEGIN SELECT 1 END')
	GRANT EXEC ON [dbo].[PlaceOrder] TO Analyst
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/**************************************************************************************
REMARKS:

-- PJ 10.2.2018 : Insert Initial data to Following tables
					[dbo].OrderHeader
					[dbo].OrderPickupDtls
					[dbo].OrderDelivery


**************************************************************************************/

ALTER PROCEDURE [dbo].[PlaceOrder]
	@UserId INT,
	@OrderDate DATETIME,
	@OrderUpdateDate DATETIME,
	@TotalAmount DECIMAL,
	@Discount DECIMAL,
	@NetAmount DECIMAL,
	@OrderLatitude VARCHAR(256),
	@OrderLongtitude VARCHAR(256)
	
AS
BEGIN
SET NOCOUNT ON

DECLARE @IsCancled BIT = 0
DECLARE	@IsBilled BIT = 0
DECLARE	@IsPickup BIT = 0
DECLARE	@IsDelivered BIT = 0

/*=============================================================
--Create master entry in OrderHeader table
=============================================================*/
DECLARE @OrderId INT;

INSERT INTO [dbo].OrderHeader (
	UserId
	,OrderDate
	,OrderUpdateDate
	,TotalAmount
	,Discount
	,NetAmount
	,OrderLatitude
	,OrderLongtitude
	,IsCancled
	,IsBilled
	)
VALUES (
	 @UserId
	,@OrderDate
	,@OrderUpdateDate
	,@TotalAmount
	,@Discount
	,@NetAmount
	,@OrderLatitude
	,@OrderLongtitude
	,@IsCancled
	,@IsBilled
	)

SET @OrderId = SCOPE_IDENTITY();

/*=============================================================
--Create OrderPickup ENtry
=============================================================*/

DECLARE @OrderPickupId INT

	INSERT INTO [dbo].OrderPickupDtls (
		OrderId,
		UserId,
		IsPickup,
		IsCancled
		)
	VALUES (
		@OrderId,
		@UserId,
		@IsPickup,
		@IsCancled
		)

SET @OrderPickupId = SCOPE_IDENTITY()

/*=============================================================
--Create OrderDelivery Entry
=============================================================*/
DECLARE @OrderDeliveryId INT

INSERT INTO [dbo].OrderDelivery (
	OrderId,
	OrderPickupId,
	--OrderDeliveryDate,
	UserId,
	IsDelivered,
	IsCancled
	)
VALUES (
	@OrderId,
	@OrderPickupId,
	--@OrderDeliveryDate,
	@UserId,
	@IsDelivered,
	@IsCancled
	)

SET @OrderDeliveryId = SCOPE_IDENTITY();


SELECT * FROM [dbo].OrderHeader WHERE OrderId = @OrderId

END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PlaceOrder]') AND type in (N'P', N'PC'))
	PRINT '<<< CREATED PROCEDURE dbo.PlaceOrder >>>'
ELSE
	PRINT '<<< FAILED CREATING PROCEDURE dbo.PlaceOrder >>>'
GO