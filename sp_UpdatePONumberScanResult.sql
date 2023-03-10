-- =============================================
-- Author:		Nguyen Xuan Lam
-- Create date: 2022-09-10
-- Description:	Update PONumber Scan Result from Item Style
-- =============================================
CREATE PROCEDURE [dbo].[sp_UpdatePONumberScanResult]
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRAN UpdatePONumberScanResult
    BEGIN TRY
		
		;WITH temp AS
		(
			SELECT DISTINCT r.PONumber
			FROM ScanResult (NOLOCK) r
				LEFT JOIN ItemStyle (NOLOCK) i ON r.PONumber = i.PurchaseOrderNumber
			WHERE i.PurchaseOrderNumber IS NULL
		)

		UPDATE r SET PONumber = i.PurchaseOrderNumber
		FROM ScanResult (NOLOCK) r
				INNER JOIN ItemStyle (NOLOCK) i ON r.PONumber = RIGHT(i.PurchaseOrderNumber,LEN(r.PONumber)) 
		WHERE r.PONumber IN (SELECT PONumber FROM temp)

		COMMIT TRANSACTION UpdatePONumberScanResult

	END TRY
    BEGIN CATCH

        ROLLBACK TRANSACTION UpdatePONumberScanResult

    END CATCH
END
