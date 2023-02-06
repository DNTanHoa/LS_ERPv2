
-- =============================================
-- Author:		LamNX
-- Create date: 2022-10-29
-- Description:	Load Scan Result Detail Summary
-- =============================================
CREATE PROCEDURE sp_SelectSummaryScanResultDetail
	@Company NVARCHAR(1000),
	@FromDate DATETIME,
	@ToDate DATETIME
AS
BEGIN
	SELECT *
	FROM ViewSummaryScanResult
	WHERE ISNULL(CompanyID,'') LIKE IIF(ISNULL(@Company,'') = '', '%', @Company) 
		AND StartDate BETWEEN CAST(@FromDate AS DATE) AND  CAST(@ToDate AS DATE)
END
GO
