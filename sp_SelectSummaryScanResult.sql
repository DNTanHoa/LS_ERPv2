USE [ERPv2]
GO
-- =============================================
-- Author:		Nguyen Xuan Lam
-- Create date: 2022-09-06
-- Description:	Summary Scan Result Found
-- =============================================
CREATE PROCEDURE [dbo].[sp_SelectSummaryScanResult]
AS
BEGIN
	
	SELECT *
	FROM ViewSummaryScanResult
END
