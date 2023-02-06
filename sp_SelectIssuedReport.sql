
/****** Object:  StoredProcedure [dbo].[sp_SelectIssuedReport]    Script Date: 12/14/2022 1:58:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Hoa.DNT
-- Create date: 2022-10-27
-- Description:	Report Issued
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_SelectIssuedReport]
	@CustomerID NVARCHAR(1000),
	@StorageCode NVARCHAR(1000),
	@FromDate DATETIME,
	@ToDate DATETIME
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;

	SELECT
		 [IS].[Number]
	    ,[ISL].[CustomerStyle]
		,[ISL].[LSStyle]
		,[ISL].[Season]
		,[ISL].[ItemID]
		,[ISL].[ItemName]
		,[ISL].[ItemColorCode]
		,[ISL].[ItemColorName]
		,[ISL].[Position]
		,[ISL].[Specify]
		,[ISL].[GarmentColorCode]
		,[ISL].[GarmentColorName]
		,[ISL].[GarmentSize]
		,[IS].[CustomerID]
		,[IS].[IssuedDate]
		,[IS].[IssuedBy]
		,[IS].[Description]
		,[ISL].[IssuedQuantity]
		,[ISL].[StorageBinCode]
		,[ISL].[LotNumber]
		,[IS].[InvoiceNumber]
		,[ISL].[DyeLotNumber]
		,[IS].[InvoiceNumberNoTotal]
		,[ISL].[FabricPurchaseOrderNumber]
		,[ISL].[ItemCode]
		,[ISL].[Season]
		,[ISL].[UnitID]
		,[IS].[StorageCode]
		,[ISL].[Roll]
	FROM
		Issued [IS] (NOLOCK)
		JOIN IssuedLine [ISL] (NOLOCK) ON [IS].[Number] = [ISL].[IssuedNumber]
			AND [IS].[CustomerID] = @CustomerID
			AND [IS].[StorageCode] = @StorageCode
			AND [IS].[IssuedDate] BETWEEN @FromDate AND @ToDate
	
END
