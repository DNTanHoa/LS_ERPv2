USE [ERPv2]
GO
/****** Object:  StoredProcedure [dbo].[sp_SelectSummaryStorageDetailReport]    Script Date: 12/29/2022 8:10:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		NGUYEN PHUC DAT
-- Create date: 10-17-2022
-- Description:	Get item storage detail report
-- =============================================
ALTER   PROCEDURE [dbo].[sp_SelectSummaryStorageDetailReport] 
	-- Add the parameters for the stored procedure here
	@CustomerID NVARCHAR(4000) = '',
	@StorageCode NVARCHAR(4000) = '',
	@ProductionMethodCode NVARCHAR(4000) = '',
	@FromDate DATETIME,
	@ToDate DATETIME,
	@OnHandQuantity Decimal(19,9)
AS
BEGIN
	SET NOCOUNT ON;

	-- edit call from view -- edit by Truongnv 20221205
	select * from ViewStorageDetailReport v
	WHERE v.[CustomerID] = @CustomerID AND v.[StorageCode] = @StorageCode
	AND (v.TransactionDate >= @FromDate OR @FromDate IS NULL) 
	AND (v.TransactionDate <= @ToDate OR @ToDate IS NULL)
	AND (v.ProductionMethodCode = @ProductionMethodCode OR @ProductionMethodCode IS NULL OR @ProductionMethodCode = '')
	AND v.OnHandQuantity > @OnHandQuantity
END
