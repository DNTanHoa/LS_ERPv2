-- =============================================
-- Author:		Nguyen Xuan Lam
-- Create date: 2022-09-01
-- Description:	Update Item Master ID Storage Detail Job
-- =============================================
CREATE PROCEDURE sp_UpdateItemMasterIDStorageDetailFromPartMaster
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRAN UpdateItemMasterIDStorageDetail
    BEGIN TRY
		
		UPDATE d SET d.ItemMasterID = m.ID
		FROM StorageDetail d INNER JOIN PartMaster m ON	
			TRIM(ISNULL(d.GarmentColorCode,'')) = TRIM(ISNULL(m.GarmentColorCode,'')) AND
			TRIM(ISNULL(d.GarmentColorName,'')) = TRIM(ISNULL(m.GarmentColorName,'')) AND
			TRIM(ISNULL(d.GarmentSize,'')) = TRIM(ISNULL(m.GarmentSize,'')) AND
			TRIM(ISNULL(d.Season,'')) = TRIM(ISNULL(m.Season,'')) AND
			TRIM(ISNULL(d.CustomerStyle,'')) = TRIM(ISNULL(m.CustomerStyle,'')) AND
			d.CustomerID=m.CustomerID 
		WHERE d.StorageCode='FG' AND ISNULL(ItemMasterID,'')=''

		COMMIT TRANSACTION UpdateItemMasterIDStorageDetail

	END TRY
    BEGIN CATCH

        ROLLBACK TRANSACTION UpdateItemMasterIDStorageDetail

    END CATCH
END
GO
