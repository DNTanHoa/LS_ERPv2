USE [ERPv2]
GO
/****** Object:  StoredProcedure [dbo].[sp_DailyTargetDetailSummary]    Script Date: 06/25/2022 3:16:50 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_DailyTargetDetailSummary](
  @CustomerID NVARCHAR(4000)
 ,@Style NVARCHAR(4000)
 ,@FROMDATE DATETIME
 ,@TODATE DATETIME
)
AS

BEGIN

IF(@FROMDATE IS NULL)
	BEGIN
		SET @FROMDATE = '1990-01-01 00:00:00'
	END

IF(@TODATE IS NULL)
	BEGIN
		SET @TODATE = GETDATE()
	END

IF OBJECT_ID('tempdb.dbo.#Orders') IS NOT NULL
    DROP TABLE #Orders

IF OBJECT_ID('tempdb.dbo.#Output') IS NOT NULL
    DROP TABLE #Output

--- Lấy thông tin dữ liệu đơn đặt hàng
SELECT 
	 [ItemStyle].[CustomerStyle]
	,[ItemStyle].[LSStyle]
	,[SalesOrders].[CustomerID]
	,[ItemStyle].[ColorCode] AS GarmentColorCode
	,[ItemStyle].[ColorName] AS GarmentColorName
	,[ItemStyle].[Season] AS Season
	,[ItemStyle].[Description]
	,[OrderDetail].[Size] as GarmentSize
	,CAST(([OrderDetail].[Quantity]) AS INT) AS OrderQuantity INTO #Orders
FROM 
	OrderDetail (NOLOCK) [OrderDetail]
	JOIN ItemStyle (NOLOCK) [ItemStyle] ON [OrderDetail].[ItemStyleNumber] = [ItemStyle].[Number]
		AND ([ItemStyle].[LSStyle] LIKE '%' + @Style + '%' OR @Style IS NULL OR TRIM(@Style) = '')
	JOIN SalesOrders (NOLOCK) [SalesOrders] ON [SalesOrders].[ID] = [ItemStyle].[SalesOrderID]
		AND ([SalesOrders].[CustomerID] = @CustomerID OR TRIM(@CustomerID) = '' OR @CustomerID IS NULL)
GROUP BY
	 [ItemStyle].[CustomerStyle]
	,[ItemStyle].[LSStyle]
	,[SalesOrders].[CustomerID]
	,[ItemStyle].[ColorCode]
	,[ItemStyle].[ColorName]
	,[ItemStyle].[Season]
	,[ItemStyle].[Description]
	,[OrderDetail].[Size]
	,[OrderDetail].[Quantity]
--- Lấy thông tin sản lượng theo công đoạn
SELECT
	 [DailyTarget].[LSStyle]
	,[DailyTarget].[CustomerID]
	,[DailyTarget].[Size]
	,[DailyTarget].[Operation]
	,CAST(SUM([DailyTarget].[Quantity]) AS INT) AS OutputQuantity INTO #Output
FROM
	DailyTargetDetail (NOLOCK) [DailyTarget]
	LEFT JOIN AllocDailyOutput (NOLOCK) [AllocDailyOutput] ON [DailyTarget].[LSStyle] = [AllocDailyOutput].[LSStyle]
	AND [DailyTarget].[Size] = [AllocDailyOutput].[Size]
WHERE
	[DailyTarget].[LSStyle] LIKE '%' + @Style + '%' OR @Style IS NULL OR TRIM(@Style) = ''
	AND ([DailyTarget].[CustomerID] = @CustomerID OR TRIM(@CustomerID) = '' OR @CustomerID IS NULL)
	AND ([DailyTarget].[LSStyle] IS NOT NULL)
GROUP BY
	 [DailyTarget].[LSStyle]
	 ,[DailyTarget].[CustomerID]
	 ,[DailyTarget].[Size]
	 ,[DailyTarget].[Operation]

SELECT 
	[Order].*
    ,[Output].[Operation]
    ,[Output].[OutputQuantity]
FROM
	#Orders [Order]
LEFT JOIN #Output [Output] ON [Order].[LSStyle] = [Output].[LSStyle]
		--AND [Order].[CustomerID] = [Output].[CustomerID]
		AND [Order].[GarmentSize] = [Output].[Size]

IF OBJECT_ID('tempdb.dbo.#Orders') IS NOT NULL
    DROP TABLE #Orders

IF OBJECT_ID('tempdb.dbo.#Output') IS NOT NULL
    DROP TABLE #Output

END

