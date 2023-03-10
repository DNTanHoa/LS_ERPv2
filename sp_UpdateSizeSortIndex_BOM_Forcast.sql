
-- =============================================
-- Author: Nguyen Xuan Lam
-- Create date: 2022-07-26
-- Description:	Hangfire update size sort index
-- =============================================
ALTER PROCEDURE [dbo].[sp_UpdateSizeSortIndex_BOM_Forcast]
AS
BEGIN
	DECLARE @MSG INT
	SET NOCOUNT ON;

    BEGIN TRAN UpdateSizeSortIndex
    BEGIN TRY

		UPDATE p SET p.SizeSortIndex = s.SequeneceNumber
		FROM PartMaterial p 
			join PartRevision r ON p.PartRevisionID=r.ID
			join Size s ON REPLACE(p.GarmentSize,CHAR(10),' ')=s.Code AND r.CustomerID=s.CustomerID
		WHERE ISNULL(p.GarmentSize,'') <> '' AND p.SizeSortIndex is null

		UPDATE b SET b.SizeSortIndex = s.SequeneceNumber
		FROM ProductionBOM b 
			join PartMaterial p ON p.ID=b.PartMaterialID
			join PartRevision r ON p.PartRevisionID=r.ID
			join Size s ON REPLACE(b.GarmentSize,char(10),' ')=s.Code AND r.CustomerID=s.CustomerID
		WHERE ISNULL(b.GarmentSize,'') <> '' AND b.SizeSortIndex is null

		UPDATE f SET f.SizeSortIndex = s.SequeneceNumber
		FROM ForecastMaterial f 
			join PartMaterial p ON p.ID=f.PartMaterialID
			join PartRevision r ON p.PartRevisionID=r.ID
			join Size s ON REPLACE(f.GarmentSize,char(10),' ')=s.Code AND r.CustomerID=s.CustomerID
		WHERE ISNULL(f.GarmentSize,'') <> '' AND f.SizeSortIndex is null
            
		SET @MSG= 1
		COMMIT TRANSACTION UpdateSizeSortIndex
    END TRY
    BEGIN CATCH
        SET @MSG= 0
        ROLLBACK TRANSACTION UpdateSizeSortIndex
    END CATCH

	SELECT @MSG
END
