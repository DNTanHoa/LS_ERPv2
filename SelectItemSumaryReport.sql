-- ================================================
-- Get summary item report
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Duong Nguyen Tan Hoa
-- Create date: 2022-02-25
-- Description:	Get summary item report
-- =============================================
CREATE PROCEDURE [dbo].[sp_SelectSummaryItemReport]
	-- Add the parameters for the stored procedure here
	@CustomerID NVARCHAR(4000) = '',
	@FromDate DATETIME,
	@ToDate DATETIME
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;
	
	SELECT 
		 [SUB].[CustomerID]
		,[SUB].[ItemCode]
		,[SUB].[ItemID]
		,[SUB].[ItemName]
		,[SUB].[ItemColorCode]
		,[SUB].[ItemColorName]
		,[SUB].[Position]
		,[SUB].[Specify]
		,[SUB].[PerUnitID]
		,[SUB].[VendorID]
		,[SUB].[GarmentColorCode]
		,[SUB].[GarmentColorName]
		,[SUB].[CustomerStyle] AS [CustomerStyle]
		,[SUB].[Season] AS [Season]
		,SUM([SUB].[RequiredQuantity]) AS [RequiredQuantity]
		,SUM([SUB].[ForecastQuantity]) aS [ForecastQuantity] 
	FROM
	(
		----- Production BOM
		SELECT
			 [SO].[CustomerID]
			,[BOM].[ItemCode]
			,[BOM].[ItemID]
			,[BOM].[ItemName]
			,[BOM].[ItemColorCode]
			,[BOM].[ItemColorName]
			,[BOM].[Position]
			,[BOM].[Specify]
			,[BOM].[PerUnitID]
			,[BOM].[VendorID]
			,[STYLE].[ColorCode] AS [GarmentColorCode]
			,[STYLE].[ColorName] AS [GarmentColorName]
			,[STYLE].[CustomerStyle] AS [CustomerStyle]
			,[STYLE].[Season] AS [Season]
			,[BOM].[RequiredQuantity] AS [RequiredQuantity]
			,0 AS [ForecastQuantity]
			,[BOM].[ReservedQuantity] AS [ReservedQuantity]
			,[BOM].[RequiredQuantity] - ISNULL([BOM].[ReservedQuantity], 0) AS RemainQuantity
		FROM
			[ProductionBOM] [BOM] (NOLOCK)
			JOIN [ItemStyle] [STYLE] (NOLOCK) ON [STYLE].[Number] = [BOM].[ItemStyleNumber]
			JOIN [SalesOrders] [SO] (NOLOCK) ON [SO].[ID] = [STYLE].[SalesOrderID]
				AND ([SO].[CustomerID] = @CustomerID OR @CustomerID = '' OR @CustomerID IS NULL)
				AND [SO].[OrderDate] >= @FromDate
				AND [SO].[OrderDate] <= @ToDate

		UNION ALL

		---- Forecast Material
		SELECT
			 [FG].[CustomerID]
			,[FM].[ItemCode]
			,[FM].[ItemID]
			,[FM].[ItemName]
			,[FM].[ItemColorCode]
			,[FM].[ItemColorName]
			,[FM].[Position]
			,[FM].[Specify]
			,[FM].[PerUnitID]
			,[FM].[VendorID]
			,[FO].[GarmentColorCode] AS [GarmentColorCode]
			,[FO].[GarmentColorName] AS [GarmentColorName]
			,[FO].[CustomerStyle] AS [CustomerStyle]
			,[FO].[Season] AS [Season]
			,0 AS [RequiredQuantity]
			,[FM].[RequiredQuantity] AS [ForecastQuantity]
			,[FM].[ReservedQuantity] AS [ReservedQuantity]
			,[FM].[RequiredQuantity] - ISNULL([FM].[ReservedQuantity], 0) AS RemainQuantity
		FROM
			[ForecastMaterial] [FM] (NOLOCK)
			JOIN [ForecastOverall] [FO] ON [FM].[ForecastOverallID] = [FO].[ID]
			JOIN [ForecastEntry] [FE] ON [FE].[ID] = [FO].[ForecastEntryID]
				AND [FE].[IsDeactive] <> 1
			JOIN [ForecastGroup] [FG] ON [FG].[ID] = [FE].[ForecastGroupID]
				AND ([FG].[CustomerID] = @CustomerID OR @CustomerID = '' OR @CustomerID IS NULL)
	) [SUB]

	GROUP BY
		 [SUB].[CustomerID]
		,[SUB].[ItemCode]
		,[SUB].[ItemID]
		,[SUB].[ItemName]
		,[SUB].[ItemColorCode]
		,[SUB].[ItemColorName]
		,[SUB].[Position]
		,[SUB].[Specify]
		,[SUB].[PerUnitID]
		,[SUB].[VendorID]
		,[SUB].[GarmentColorCode]
		,[SUB].[GarmentColorName]
		,[SUB].[CustomerStyle]
		,[SUB].[Season]
END
GO
