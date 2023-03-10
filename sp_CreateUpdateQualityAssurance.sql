-- =============================================
-- Author:		Nguyen Xuan Lam
-- Create date: 2022-08-29
-- Description:	Create / Update Quality Assurance
-- =============================================
CREATE PROCEDURE [dbo].[sp_CreateUpdateQualityAssurance]
	@ID bigint,
	@PurchaseOrderNumber nvarchar(max),
	@ItemStyleNumber nvarchar(max),
	@GarmentSize nvarchar(max),
	@StatusID nvarchar(max),
	@Percent decimal,
	@Remark nvarchar(max),
	@CustomerID nvarchar(max),
	@UserName nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;

    BEGIN TRAN CreateUpdateQualityAssurance
    BEGIN TRY
		IF @ID = 0
		BEGIN
			INSERT INTO QualityAssurance
			SELECT	d.ItemStyleNumber,i.PurchaseOrderNumber,r.CustomerID,r.StorageCode,GETDATE(),i.LSStyle,i.CustomerStyle,i.Season,i.ColorCode,i.ColorName,
					d.Size,t.StorageBinCode,SUM(t.Quantity),@Percent,@UserName,@UserName,GETDATE(),GETDATE(),@StatusID,@Remark,d.Quantity,0
			FROM MaterialTransaction t
				INNER JOIN Receipt r ON t.ReceiptNumber = r.Number AND r.ReceiptTypeId='RFG' AND r.StorageCode='FG'
				INNER JOIN ItemStyle i ON TRIM(ISNULL(t.PurchaseOrderNumber,'')) = TRIM(ISNULL(i.PurchaseOrderNumber,'')) AND TRIM(ISNULL(t.LSStyle,'')) = TRIM(ISNULL(i.LSStyle,''))
				INNER JOIN OrderDetail d ON i.Number=d.ItemStyleNumber AND TRIM(ISNULL(t.GarmentSize,'')) = TRIM(ISNULL(d.Size,''))
			WHERE t.PurchaseOrderNumber = @PurchaseOrderNumber AND i.Number = @ItemStyleNumber AND d.Size = @GarmentSize AND r.CustomerID = @CustomerID
			GROUP BY d.ItemStyleNumber,i.PurchaseOrderNumber,r.CustomerID,r.StorageCode,i.LSStyle,i.CustomerStyle,i.Season,i.ColorCode,i.ColorName,d.Size,t.StorageBinCode,d.Quantity
		
			SELECT @ID = MAX(ID) FROM QualityAssurance 
			
			INSERT INTO QualityAudit(QualityAssuranceID,QualityStatusID,Remark,ApprovedBy,ApprovedDate)
			VALUES(@ID,@StatusID,@Remark,@UserName,GETDATE())

			UPDATE t SET QualityAssuranceID = @ID
			FROM MaterialTransaction t
			WHERE TRIM(ISNULL(PurchaseOrderNumber,''))+TRIM(ISNULL(LSStyle,''))+ TRIM(ISNULL(GarmentSize,''))
				IN (SELECT TRIM(ISNULL(PurchaseOrderNumber,''))+TRIM(ISNULL(LSStyle,''))+ TRIM(ISNULL(GarmentSize,''))
					FROM QualityAssurance WHERE ID = @ID)
		END
		ELSE
		BEGIN
			UPDATE q SET ApprovedDate=GETDATE(), QualityStatusID=@StatusID, Remark=@Remark,LastUpdatedBy = @UserName, LastUpdatedAt=GETDATE()
			FROM QualityAssurance q
			WHERE ID = @ID

			INSERT INTO QualityAudit(QualityAssuranceID,QualityStatusID,Remark,ApprovedBy,ApprovedDate)
			VALUES(@ID,@StatusID,@Remark,@UserName,GETDATE())
		END
		
		COMMIT TRANSACTION CreateUpdateQualityAssurance
	END TRY
    BEGIN CATCH

        ROLLBACK TRANSACTION CreateUpdateQualityAssurance

    END CATCH

	SELECT * FROM QualityAssurance WHERE ID = @ID
END
