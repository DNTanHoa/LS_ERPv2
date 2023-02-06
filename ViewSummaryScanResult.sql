-- =============================================
-- Author:		Nguyen Xuan Lam
-- Create date: 2022-09-06
-- Description:	View Summary Scan Result
-- =============================================

CREATE VIEW ViewSummaryScanResult
AS
	SELECT r.PONumber, i.LSStyle, r.Barcode, 
			TotalFound = SUM(d.Found), OrderQuantity = i.TotalQuantity,
			[Percent] = ROUND(CONVERT(DECIMAL,SUM(d.Found))/i.TotalQuantity * 100,2)
	FROM ScanResult (NOLOCK) r 
		INNER JOIN ScanResultDetail (NOLOCK) d ON r.ID = d.ScanResultID
		INNER JOIN ItemStyle  (NOLOCK) i ON r.PONumber = i.PurchaseOrderNumber
	WHERE ISNULL(r.Status,'') = 'G'
	GROUP BY r.PONumber, i.LSStyle, r.Barcode, i.TotalQuantity
