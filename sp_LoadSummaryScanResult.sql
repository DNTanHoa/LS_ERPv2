
-- =============================================
-- Author:		LamNX
-- Create date: 2022-10-24
-- Description:	Load Summary Scan Result
-- =============================================
CREATE PROCEDURE [dbo].[sp_LoadSummaryScanResult]
	@CompanyID NVARCHAR(100),
	@FromDate DATETIME,
	@ToDate DATETIME,
	@Search NVARCHAR(1000)
AS
BEGIN
	
	SELECT PONumber,LSStyle,OrderQuantity=ISNULL(OrderQuantity,0),EntryQuantity=SUM(ISNULL(TotalFound,0)), 
		[Status] = IIF(ISNULL(OrderQuantity,0) = SUM(ISNULL(TotalFound,0)), 'OK','')
	FROM ViewSummaryScanResult 
	WHERE ISNULL(CompanyID,'') LIKE IIF(ISNULL(@CompanyID,'') = '', '%', @CompanyID)
		AND StartDate BETWEEN CAST(@FromDate AS DATE) AND CAST(@ToDate AS DATE)
		AND (ISNULL(PONumber,'') LIKE N'%' +  @Search + '%' 
		OR ISNULL(LSStyle,'') LIKE N'%' +  @Search + '%' 
		OR ISNULL(Season,'') LIKE N'%' +  @Search + '%')
	GROUP BY PONumber,LSStyle,ISNULL(OrderQuantity,0)
END
