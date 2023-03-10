-- =============================================
-- Author:		Nguyen Xuan Lam
-- Create date: 2022-08-18
-- Description:	Create Item Master Job
-- =============================================
CREATE PROCEDURE [dbo].[sp_CreateItemMaster] 
	@UserName nvarchar(max),
	@CustomerID nvarchar(max)
AS
BEGIN
	DECLARE @MSG INT
	SET NOCOUNT ON;

    BEGIN TRAN CreateItemMaster
    BEGIN TRY
		
		DECLARE @Value INT
		-- Update OtherName & DefaultThreadName before create Item Master
		EXEC sp_UpdateOtherNameByCustomer @CustomerID, @Value OUTPUT

		IF @Value = 1
		BEGIN 
			IF @CustomerID = 'DE'
			BEGIN
			--===== Create Item Master for THREAD =====--
				INSERT INTO ItemMaster(ID,ItemID,ItemName,ItemColorCode,ItemColorName,ItemStyleNumber,Position,Specify,Season,GarmentSize,GarmentColorCode,GarmentColorName,GroupSize,GroupItemColor,OtherName,MaterialTypeCode,CreatedBy,LastUpdatedBy,CreatedAt,LastUpdatedAt,CustomerID,IsDeleted)
				SELECT DISTINCT UPPER(NEWID()),'',UPPER(TRIM(ISNULL(p.DefaultThreadName,''))),REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ',''),'','','','','','','','',
					MAX(CAST(ISNULL(p.GroupSize,0) AS INT)),MAX(CAST(ISNULL(p.GroupItemColor,0) AS INT)),
					MAX(p.OtherName),MAX(p.MaterialTypeCode),@UserName,@UserName,GETDATE(),GETDATE(),@CustomerID,0
				FROM PartMaterial p 
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID 
					LEFT JOIN ItemMaster i ON UPPER(TRIM(ISNULL(p.DefaultThreadName,'')))=i.ItemName AND REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE REPLACE(p.OtherName,' ','') Like N'%THREAD%' AND i.ID IS NULL AND r.CustomerID=@CustomerID AND ISNULL(p.DefaultThreadName,'')<>'' 
				GROUP BY UPPER(TRIM(ISNULL(p.DefaultThreadName,''))),REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ','')

				--- Set ItemMasterID for PartMaterial,ProBOM,....
				UPDATE p SET p.ItemMasterID=i.ID
				FROM PartMaterial p 
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID 
					INNER JOIN ItemMaster i ON UPPER(TRIM(ISNULL(p.DefaultThreadName,'')))=i.ItemName AND REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE REPLACE(p.OtherName,' ','') Like N'%THREAD%' AND p.ItemMasterID IS NULL AND r.CustomerID=@CustomerID AND ISNULL(p.DefaultThreadName,'')<>''

				UPDATE b SET b.ItemMasterID=i.ID
				FROM ProductionBOM b
					INNER JOIN  PartMaterial p ON b.PartMaterialID=p.ID
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					INNER JOIN ItemMaster i ON UPPER(TRIM(ISNULL(b.DefaultThreadName,'')))=i.ItemName AND REPLACE(UPPER(TRIM(ISNULL(b.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE REPLACE(b.OtherName,' ','') Like N'%THREAD%' AND b.ItemMasterID IS NULL AND r.CustomerID=@CustomerID AND ISNULL(b.DefaultThreadName,'')<>''

				UPDATE f SET f.ItemMasterID=i.ID
				FROM ForecastMaterial f
					INNER JOIN  PartMaterial p ON f.PartMaterialID=p.ID
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					INNER JOIN ItemMaster i ON UPPER(TRIM(ISNULL(f.DefaultThreadName,'')))=i.ItemName AND REPLACE(UPPER(TRIM(ISNULL(f.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE REPLACE(f.OtherName,' ','') Like N'%THREAD%' AND f.ItemMasterID IS NULL AND r.CustomerID=@CustomerID AND ISNULL(f.DefaultThreadName,'')<>''

				UPDATE d SET d.ItemMasterID=i.ID
				FROM StorageDetail d
					INNER JOIN ItemMaster i ON UPPER(TRIM(ISNULL(d.DefaultThreadName,'')))=i.ItemName AND REPLACE(UPPER(TRIM(ISNULL(d.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE REPLACE(d.OtherName,' ','') Like N'%THREAD%' AND d.ItemMasterID IS NULL AND d.CustomerID=@CustomerID AND ISNULL(d.DefaultThreadName,'')<>''
			
				UPDATE m SET m.ItemMasterID=i.ID
				FROM MaterialTransaction m
					INNER JOIN StorageDetail d ON m.StorageDetailID = d.ID
					--INNER JOIN PurchaseOrder p ON REPLACE(UPPER(TRIM(ISNULL(d.PurchaseOrderNumber,''))),' ','') = REPLACE(UPPER(TRIM(ISNULL(p.Number,''))),' ','')
					INNER JOIN ItemMaster i ON UPPER(TRIM(ISNULL(m.DefaultThreadName,'')))=i.ItemName AND REPLACE(UPPER(TRIM(ISNULL(m.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE REPLACE(m.OtherName,' ','') Like N'%THREAD%' AND m.ItemMasterID IS NULL AND d.CustomerID=@CustomerID AND ISNULL(m.DefaultThreadName,'')<>''

				UPDATE l SET l.ItemMasterID=i.ID
				FROM PurchaseOrderGroupLine l
					INNER JOIN PurchaseOrder p ON l.PurchaseOrderID=p.ID
					INNER JOIN ItemMaster i ON UPPER(TRIM(ISNULL(l.DefaultThreadName,'')))=i.ItemName AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE REPLACE(l.OtherName,' ','') Like N'%THREAD%' AND l.ItemMasterID IS NULL AND p.CustomerID=@CustomerID AND ISNULL(l.DefaultThreadName,'')<>''

				UPDATE l SET l.ItemMasterID=i.ID
				FROM PurchaseOrderLine l
					INNER JOIN PurchaseOrder p ON l.PurchaseOrderID=p.ID
					INNER JOIN ItemMaster i ON UPPER(TRIM(ISNULL(l.DefaultThreadName,'')))=i.ItemName AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE REPLACE(l.OtherName,' ','') Like N'%THREAD%' AND l.ItemMasterID IS NULL AND p.CustomerID=@CustomerID AND ISNULL(l.DefaultThreadName,'')<>''

				UPDATE l SET l.ItemMasterID=i.ID
				FROM IssuedLine l
					INNER JOIN Issued s on l.IssuedNumber=s.Number
					INNER JOIN ItemMaster i ON UPPER(TRIM(ISNULL(l.DefaultThreadName,'')))=i.ItemName AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE REPLACE(l.OtherName,' ','') Like N'%THREAD%' AND l.ItemMasterID IS NULL AND s.CustomerID=@CustomerID AND ISNULL(l.DefaultThreadName,'')<>''

				UPDATE d SET d.ItemMaterID=i.ID
				FROM MaterialRequestDetails d
					INNER JOIN MaterialRequests r on d.MaterialRequestId=r.Id
					INNER JOIN ItemMaster i ON UPPER(TRIM(ISNULL(d.DefaultThreadName,'')))=i.ItemName AND REPLACE(UPPER(TRIM(ISNULL(d.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE REPLACE(d.OtherName,' ','') Like N'%THREAD%' AND d.ItemMaterID IS NULL AND r.CustomerID=@CustomerID AND ISNULL(d.DefaultThreadName,'')<>''

				UPDATE l SET l.ItemMasterID=i.ID
				FROM ReceiptGroupLine l
					INNER JOIN Receipt r on l.ReceiptNumber=r.Number
					INNER JOIN ItemMaster i ON UPPER(TRIM(ISNULL(l.DefaultThreadName,'')))=i.ItemName AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE REPLACE(l.OtherName,' ','') Like N'%THREAD%' AND l.ItemMasterID IS NULL AND r.CustomerID=@CustomerID AND ISNULL(l.DefaultThreadName,'')<>''
			--===== End Create Item Master for THREAD =====--

			--===== Create Item Master for SIZE + CARE LABEL =====--
				INSERT INTO ItemMaster(ID,ItemID,ItemName,ItemColorCode,ItemColorName,ItemStyleNumber,Position,Specify,Season,GarmentSize,GarmentColorCode,GarmentColorName,GroupSize,GroupItemColor,OtherName,MaterialTypeCode,CreatedBy,LastUpdatedBy,CreatedAt,LastUpdatedAt,CustomerID,IsDeleted)
				SELECT DISTINCT UPPER(NEWID()),REPLACE(UPPER(TRIM(ISNULL(p.DsmItemID,''))),' ',''),'',REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ',''),'','','','','',REPLACE(UPPER(TRIM(ISNULL(p.GarmentSize,''))),' ',''),REPLACE(UPPER(TRIM(ISNULL(p.GarmentColorCode,''))),' ',''),'',
					MAX(CAST(ISNULL(p.GroupSize,0) AS INT)),MAX(CAST(ISNULL(p.GroupItemColor,0) AS INT)),
					MAX(p.OtherName),MAX(p.MaterialTypeCode),@UserName,@UserName,GETDATE(),GETDATE(),@CustomerID,0
				FROM PartMaterial p 
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					LEFT JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(p.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(p.GarmentSize,''))),' ','')=i.GarmentSize AND REPLACE(UPPER(TRIM(ISNULL(p.GarmentColorCode,''))),' ','') = i.GarmentColorCode
				WHERE (REPLACE(p.OtherName,' ','') like '%CARELABEL%' or REPLACE(p.OtherName,' ','') like '%CARE+SIZE%' or REPLACE(p.OtherName,' ','') like '%SIZELABEL%' or REPLACE(p.OtherName,' ','') like '%SIZE+CARE%') AND i.ID IS NULL AND r.CustomerID=@CustomerID AND p.DsmItemID IS NOT NULL
				GROUP BY REPLACE(UPPER(TRIM(ISNULL(p.DsmItemID,''))),' ',''),REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ',''),REPLACE(UPPER(TRIM(ISNULL(p.GarmentSize,''))),' ',''),REPLACE(UPPER(TRIM(ISNULL(p.GarmentColorCode,''))),' ','')

				--- Set ItemMasterID for PartMaterial,ProBOM,....
				UPDATE p SET p.ItemMasterID=i.ID
				FROM PartMaterial p 
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(p.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(p.GarmentSize,''))),' ','')=i.GarmentSize AND REPLACE(UPPER(TRIM(ISNULL(p.GarmentColorCode,''))),' ','') = i.GarmentColorCode
				WHERE (REPLACE(p.OtherName,' ','') like '%CARELABEL%' or REPLACE(p.OtherName,' ','') like '%CARE+SIZE%' or REPLACE(p.OtherName,' ','') like '%SIZELABEL%' or REPLACE(p.OtherName,' ','') like '%SIZE+CARE%') AND p.ItemMasterID IS NULL AND r.CustomerID=@CustomerID

				UPDATE b SET b.ItemMasterID=i.ID
				FROM ProductionBOM b
					INNER JOIN  PartMaterial p ON b.PartMaterialID=p.ID
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(b.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(b.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(b.GarmentSize,''))),' ','')=i.GarmentSize AND REPLACE(UPPER(TRIM(ISNULL(p.GarmentColorCode,''))),' ','') = i.GarmentColorCode
				WHERE (REPLACE(b.OtherName,' ','') like '%CARELABEL%' or REPLACE(b.OtherName,' ','') like '%CARE+SIZE%' or REPLACE(b.OtherName,' ','') like '%SIZELABEL%' or REPLACE(b.OtherName,' ','') like '%SIZE+CARE%') AND b.ItemMasterID IS NULL AND r.CustomerID=@CustomerID

				UPDATE f SET f.ItemMasterID=i.ID
				FROM ForecastMaterial f
					INNER JOIN  PartMaterial p ON f.PartMaterialID=p.ID
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(f.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(f.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(f.GarmentSize,''))),' ','')=i.GarmentSize AND REPLACE(UPPER(TRIM(ISNULL(f.GarmentColorCode,''))),' ','') = i.GarmentColorCode
				WHERE (REPLACE(f.OtherName,' ','') like '%CARELABEL%' or REPLACE(f.OtherName,' ','') like '%CARE+SIZE%' or REPLACE(f.OtherName,' ','') like '%SIZELABEL%' or REPLACE(f.OtherName,' ','') like '%SIZE+CARE%') AND f.ItemMasterID IS NULL AND r.CustomerID=@CustomerID

				UPDATE d SET d.ItemMasterID=i.ID
				FROM StorageDetail d
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(d.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(d.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(d.GarmentSize,''))),' ','')=i.GarmentSize AND REPLACE(UPPER(TRIM(ISNULL(d.GarmentColorCode,''))),' ','') = i.GarmentColorCode
				WHERE (REPLACE(d.OtherName,' ','') like '%CARELABEL%' or REPLACE(d.OtherName,' ','') like '%CARE+SIZE%' or REPLACE(d.OtherName,' ','') like '%SIZELABEL%' or REPLACE(d.OtherName,' ','') like '%SIZE+CARE%') AND d.ItemMasterID IS NULL AND d.CustomerID=@CustomerID
			
				UPDATE m SET m.ItemMasterID=i.ID
				FROM MaterialTransaction m
					INNER JOIN StorageDetail d ON m.StorageDetailID = d.ID
					--INNER JOIN PurchaseOrder p ON REPLACE(UPPER(TRIM(ISNULL(m.PurchaseOrderNumber,''))),' ','') = REPLACE(UPPER(TRIM(ISNULL(p.Number,''))),' ','')
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(m.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(m.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(m.GarmentSize,''))),' ','')=i.GarmentSize AND REPLACE(UPPER(TRIM(ISNULL(m.GarmentColorCode,''))),' ','') = i.GarmentColorCode
				WHERE (REPLACE(m.OtherName,' ','') like '%CARELABEL%' or REPLACE(m.OtherName,' ','') like '%CARE+SIZE%' or REPLACE(m.OtherName,' ','') like '%SIZELABEL%' or REPLACE(m.OtherName,' ','') like '%SIZE+CARE%') AND m.ItemMasterID IS NULL AND d.CustomerID=@CustomerID

				UPDATE l SET l.ItemMasterID=i.ID
				FROM PurchaseOrderGroupLine l
					INNER JOIN PurchaseOrder p ON l.PurchaseOrderID=p.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(l.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(l.GarmentSize,''))),' ','')=i.GarmentSize AND REPLACE(UPPER(TRIM(ISNULL(l.GarmentColorCode,''))),' ','') = i.GarmentColorCode
				WHERE (REPLACE(l.OtherName,' ','') like '%CARELABEL%' or REPLACE(l.OtherName,' ','') like '%CARE+SIZE%' or REPLACE(l.OtherName,' ','') like '%SIZELABEL%' or REPLACE(l.OtherName,' ','') like '%SIZE+CARE%') AND l.ItemMasterID IS NULL AND p.CustomerID=@CustomerID

				UPDATE l SET l.ItemMasterID=i.ID
				FROM PurchaseOrderLine l
					INNER JOIN PurchaseOrder p ON l.PurchaseOrderID=p.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(l.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(l.GarmentSize,''))),' ','')=i.GarmentSize AND REPLACE(UPPER(TRIM(ISNULL(l.GarmentColorCode,''))),' ','') = i.GarmentColorCode
				WHERE (REPLACE(l.OtherName,' ','') like '%CARELABEL%' or REPLACE(l.OtherName,' ','') like '%CARE+SIZE%' or REPLACE(l.OtherName,' ','') like '%SIZELABEL%' or REPLACE(l.OtherName,' ','') like '%SIZE+CARE%') AND l.ItemMasterID IS NULL AND p.CustomerID=@CustomerID

				UPDATE l SET l.ItemMasterID=i.ID
				FROM IssuedLine l
					INNER JOIN Issued s on l.IssuedNumber=s.Number
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(l.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(l.GarmentSize,''))),' ','')=i.GarmentSize AND REPLACE(UPPER(TRIM(ISNULL(l.GarmentColorCode,''))),' ','') = i.GarmentColorCode
				WHERE (REPLACE(l.OtherName,' ','') like '%CARELABEL%' or REPLACE(l.OtherName,' ','') like '%CARE+SIZE%' or REPLACE(l.OtherName,' ','') like '%SIZELABEL%' or REPLACE(l.OtherName,' ','') like '%SIZE+CARE%') AND l.ItemMasterID IS NULL AND s.CustomerID=@CustomerID

				UPDATE d SET d.ItemMaterID=i.ID
				FROM MaterialRequestDetails d
					INNER JOIN MaterialRequests r on d.MaterialRequestId=r.Id
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(d.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(d.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(d.GarmentSize,''))),' ','')=i.GarmentSize AND REPLACE(UPPER(TRIM(ISNULL(d.GarmentColorCode,''))),' ','') = i.GarmentColorCode
				WHERE (REPLACE(d.OtherName,' ','') like '%CARELABEL%' or REPLACE(d.OtherName,' ','') like '%CARE+SIZE%' or REPLACE(d.OtherName,' ','') like '%SIZELABEL%' or REPLACE(d.OtherName,' ','') like '%SIZE+CARE%') AND d.ItemMaterID IS NULL AND r.CustomerID=@CustomerID

				UPDATE l SET l.ItemMasterID=i.ID
				FROM ReceiptGroupLine l
					INNER JOIN Receipt r on l.ReceiptNumber=r.Number
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(l.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(l.GarmentSize,''))),' ','')=i.GarmentSize AND REPLACE(UPPER(TRIM(ISNULL(l.GarmentColorCode,''))),' ','') = i.GarmentColorCode
				WHERE (REPLACE(l.OtherName,' ','') like '%CARELABEL%' or REPLACE(l.OtherName,' ','') like '%CARE+SIZE%' or REPLACE(l.OtherName,' ','') like '%SIZELABEL%' or REPLACE(l.OtherName,' ','') like '%SIZE+CARE%') AND l.ItemMasterID IS NULL AND r.CustomerID=@CustomerID
			--===== End Create Item Master for SIZE + CARE LABEL =====--

			--===== Create Item Master for SIZE HT =====--
				INSERT INTO ItemMaster(ID,ItemID,ItemName,ItemColorCode,ItemColorName,ItemStyleNumber,Position,Specify,Season,GarmentSize,GarmentColorCode,GarmentColorName,GroupSize,GroupItemColor,OtherName,MaterialTypeCode,CreatedBy,LastUpdatedBy,CreatedAt,LastUpdatedAt,CustomerID,IsDeleted)
				SELECT DISTINCT UPPER(NEWID()),REPLACE(UPPER(TRIM(ISNULL(p.DsmItemID,''))),' ',''),'',REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ',''),'','','','','',REPLACE(UPPER(TRIM(ISNULL(p.GarmentSize,''))),' ',''),'','',
					MAX(CAST(ISNULL(p.GroupSize,0) AS INT)),MAX(CAST(ISNULL(p.GroupItemColor,0) AS INT)),
					MAX(p.OtherName),MAX(p.MaterialTypeCode),@UserName,@UserName,GETDATE(),GETDATE(),@CustomerID,0
				FROM PartMaterial p 
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					LEFT JOIN ItemMaster i ON REPLACE( UPPER(TRIM(ISNULL(p.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(p.GarmentSize,''))),' ','')=i.GarmentSize 
				WHERE REPLACE(p.OtherName,' ','') = N'SIZEHT' AND i.ID IS NULL AND r.CustomerID=@CustomerID AND p.DsmItemID IS NOT NULL
				GROUP BY REPLACE(UPPER(TRIM(ISNULL(p.DsmItemID,''))),' ',''),REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ',''),REPLACE(UPPER(TRIM(ISNULL(p.GarmentSize,''))),' ','')

				--- Set ItemMasterID for PartMaterial,ProBOM,....
				UPDATE p SET p.ItemMasterID=i.ID
				FROM PartMaterial p 
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(p.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(p.GarmentSize,''))),' ','')=i.GarmentSize 
				WHERE REPLACE(p.OtherName,' ','') = N'SIZEHT' AND p.ItemMasterID IS NULL AND r.CustomerID=@CustomerID

				UPDATE b SET b.ItemMasterID=i.ID
				FROM ProductionBOM b
					INNER JOIN  PartMaterial p ON b.PartMaterialID=p.ID
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(b.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(b.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(b.GarmentSize,''))),' ','')=i.GarmentSize
				WHERE REPLACE(b.OtherName,' ','') = N'SIZEHT' AND b.ItemMasterID IS NULL AND r.CustomerID=@CustomerID

				UPDATE f SET f.ItemMasterID=i.ID
				FROM ForecastMaterial f
					INNER JOIN  PartMaterial p ON f.PartMaterialID=p.ID
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(f.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(f.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(f.GarmentSize,''))),' ','')=i.GarmentSize 
				WHERE REPLACE(f.OtherName,' ','') = N'SIZEHT' AND f.ItemMasterID IS NULL AND r.CustomerID=@CustomerID

				UPDATE d SET d.ItemMasterID=i.ID
				FROM StorageDetail d
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(d.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(d.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(d.GarmentSize,''))),' ','')=i.GarmentSize 
				WHERE REPLACE(d.OtherName,' ','') = N'SIZEHT' AND d.ItemMasterID IS NULL AND d.CustomerID=@CustomerID
			
				UPDATE m SET m.ItemMasterID=i.ID
				FROM MaterialTransaction m
					INNER JOIN StorageDetail d ON m.StorageDetailID = d.ID
					--INNER JOIN PurchaseOrder p ON REPLACE(UPPER(TRIM(ISNULL(m.PurchaseOrderNumber,''))),' ','')= REPLACE(UPPER(TRIM(ISNULL(p.Number,''))),' ','')
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(m.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(m.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(m.GarmentSize,''))),' ','')=i.GarmentSize 
				WHERE REPLACE(m.OtherName,' ','') = N'SIZEHT' AND m.ItemMasterID IS NULL AND d.CustomerID=@CustomerID

				UPDATE l SET l.ItemMasterID=i.ID
				FROM PurchaseOrderGroupLine l
					INNER JOIN PurchaseOrder p ON l.PurchaseOrderID=p.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(l.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(l.GarmentSize,''))),' ','')=i.GarmentSize
				WHERE REPLACE(l.OtherName,' ','') = N'SIZEHT' AND l.ItemMasterID IS NULL AND p.CustomerID=@CustomerID

				UPDATE l SET l.ItemMasterID=i.ID
				FROM PurchaseOrderLine l
					INNER JOIN PurchaseOrder p ON l.PurchaseOrderID=p.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(l.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(l.GarmentSize,''))),' ','')=i.GarmentSize 
				WHERE REPLACE(l.OtherName,' ','') = N'SIZEHT' AND l.ItemMasterID IS NULL AND p.CustomerID=@CustomerID

				UPDATE l SET l.ItemMasterID=i.ID
				FROM IssuedLine l
					join Issued s on l.IssuedNumber=s.Number
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(l.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(l.GarmentSize,''))),' ','')=i.GarmentSize 
				WHERE REPLACE(l.OtherName,' ','') = N'SIZEHT' AND l.ItemMasterID IS NULL AND s.CustomerID=@CustomerID

				UPDATE d SET d.ItemMaterID=i.ID
				FROM MaterialRequestDetails d
					INNER JOIN MaterialRequests r on d.MaterialRequestId=r.Id
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(d.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(d.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(d.GarmentSize,''))),' ','')=i.GarmentSize 
				WHERE REPLACE(d.OtherName,' ','') = N'SIZEHT' AND d.ItemMaterID IS NULL AND r.CustomerID=@CustomerID

				UPDATE l SET l.ItemMasterID=i.ID
				FROM ReceiptGroupLine l
					INNER JOIN Receipt r on l.ReceiptNumber=r.Number
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(l.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(l.GarmentSize,''))),' ','')=i.GarmentSize 
				WHERE REPLACE(l.OtherName,' ','') = N'SIZEHT' AND l.ItemMasterID IS NULL AND r.CustomerID=@CustomerID
			--===== End Create Item Master for SIZE HT =====--

			--===== Create Item Master for DRAWSTRINGS =====--
				INSERT INTO ItemMaster(ID,ItemID,ItemName,ItemColorCode,ItemColorName,ItemStyleNumber,Position,Specify,Season,GarmentSize,GarmentColorCode,GarmentColorName,GroupSize,GroupItemColor,OtherName,MaterialTypeCode,CreatedBy,LastUpdatedBy,CreatedAt,LastUpdatedAt,CustomerID,IsDeleted)
				SELECT DISTINCT UPPER(NEWID()),REPLACE(UPPER(TRIM(ISNULL(p.DsmItemID,''))),' ',''),'',REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ',''),'','','',REPLACE(UPPER(TRIM(ISNULL(p.Specify,''))),' ',''),'','','','',
					MAX(CAST(ISNULL(p.GroupSize,0) AS INT)),MAX(CAST(ISNULL(p.GroupItemColor,0) AS INT)),
					MAX(p.OtherName),MAX(p.MaterialTypeCode),@UserName,@UserName,GETDATE(),GETDATE(),@CustomerID,0
				FROM PartMaterial p 
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					LEFT JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(p.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(p.Specify,''))),' ','')=i.Specify 
				WHERE REPLACE(p.OtherName,' ','') = N'DRAWSTRINGS' AND i.ID IS NULL AND r.CustomerID=@CustomerID AND p.DsmItemID IS NOT NULL 
				GROUP BY REPLACE(UPPER(TRIM(ISNULL(p.DsmItemID,''))),' ',''),REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ',''),REPLACE(UPPER(TRIM(ISNULL(p.Specify,''))),' ','')

				--- Set ItemMasterID for PartMaterial,ProBOM,....
				UPDATE p SET p.ItemMasterID=i.ID
				FROM PartMaterial p 
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(p.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(p.Specify,''))),' ','')=i.Specify 
				WHERE REPLACE(p.OtherName,' ','') = N'DRAWSTRINGS' AND p.ItemMasterID IS NULL AND r.CustomerID=@CustomerID

				UPDATE b SET b.ItemMasterID=i.ID
				FROM ProductionBOM b
					INNER JOIN  PartMaterial p ON b.PartMaterialID=p.ID
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(b.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(b.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(b.Specify,''))),' ','')=i.Specify
				WHERE REPLACE(b.OtherName,' ','') = N'DRAWSTRINGS' AND b.ItemMasterID IS NULL AND r.CustomerID=@CustomerID

				UPDATE f SET f.ItemMasterID=i.ID
				FROM ForecastMaterial f
					INNER JOIN  PartMaterial p ON f.PartMaterialID=p.ID
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(f.ItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(f.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(f.Specify,''))),' ','')=i.Specify 
				WHERE REPLACE(f.OtherName,' ','') = N'DRAWSTRINGS' AND f.ItemMasterID IS NULL AND r.CustomerID=@CustomerID

				UPDATE d SET d.ItemMasterID=i.ID
				FROM StorageDetail d
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(d.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(d.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(d.Specify,''))),' ','')=i.Specify 
				WHERE REPLACE(d.OtherName,' ','') = N'DRAWSTRINGS' AND d.ItemMasterID IS NULL AND d.CustomerID=@CustomerID
			
				UPDATE m SET m.ItemMasterID=i.ID
				FROM MaterialTransaction m
					INNER JOIN StorageDetail d ON m.StorageDetailID = d.ID
					--INNER JOIN PurchaseOrder p ON REPLACE(UPPER(TRIM(ISNULL(m.PurchaseOrderNumber,''))),' ','') = REPLACE(UPPER(TRIM(ISNULL(p.Number,''))),' ','')
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(m.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(m.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(m.Specify,''))),' ','')=i.Specify 
				WHERE REPLACE(m.OtherName,' ','') = N'DRAWSTRINGS' AND m.ItemMasterID IS NULL AND d.CustomerID=@CustomerID

				UPDATE l SET l.ItemMasterID=i.ID
				FROM PurchaseOrderGroupLine l
					INNER JOIN PurchaseOrder p ON l.PurchaseOrderID=p.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(l.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(l.Specify,''))),' ','')=i.Specify
				WHERE REPLACE(l.OtherName,' ','') = N'DRAWSTRINGS' AND l.ItemMasterID IS NULL AND p.CustomerID=@CustomerID

				UPDATE l SET l.ItemMasterID=i.ID
				FROM PurchaseOrderLine l
					INNER JOIN PurchaseOrder p ON l.PurchaseOrderID=p.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(l.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(l.Specify,''))),' ','')=i.Specify 
				WHERE REPLACE(l.OtherName,' ','') = N'DRAWSTRINGS' AND l.ItemMasterID IS NULL AND p.CustomerID=@CustomerID

				UPDATE l SET l.ItemMasterID=i.ID
				FROM IssuedLine l
					join Issued s on l.IssuedNumber=s.Number
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(l.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(l.Specify,''))),' ','')=i.Specify 
				WHERE REPLACE(l.OtherName,' ','') = N'DRAWSTRINGS' AND l.ItemMasterID IS NULL AND s.CustomerID=@CustomerID

				UPDATE d SET d.ItemMaterID=i.ID
				FROM MaterialRequestDetails d
					INNER JOIN MaterialRequests r on d.MaterialRequestId=r.Id
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(d.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(d.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(d.Specify,''))),' ','')=i.Specify 
				WHERE REPLACE(d.OtherName,' ','') = N'DRAWSTRINGS' AND d.ItemMaterID IS NULL AND r.CustomerID=@CustomerID

				UPDATE l SET l.ItemMasterID=i.ID
				FROM ReceiptGroupLine l
					INNER JOIN Receipt r on l.ReceiptNumber=r.Number
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(l.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(l.Specify,''))),' ','')=i.Specify 
				WHERE REPLACE(l.OtherName,' ','') = N'DRAWSTRINGS' AND l.ItemMasterID IS NULL AND r.CustomerID=@CustomerID
			--===== End Create Item Master for DRAWSTRINGS =====--

			--===== Create Item Master for Others =====--
				INSERT INTO ItemMaster(ID,ItemID,ItemName,ItemColorCode,ItemColorName,ItemStyleNumber,Position,Specify,Season,GarmentSize,GarmentColorCode,GarmentColorName,GroupSize,GroupItemColor,OtherName,MaterialTypeCode,CreatedBy,LastUpdatedBy,CreatedAt,LastUpdatedAt,CustomerID,IsDeleted)
				SELECT DISTINCT UPPER(NEWID()),REPLACE(UPPER(TRIM(ISNULL(p.DsmItemID,''))),' ',''),'',REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ',''),'','','','','','','','',
					MAX(CAST(ISNULL(p.GroupSize,0) AS INT)),MAX(CAST(ISNULL(p.GroupItemColor,0) AS INT)),
					MAX(ISNULL(p.OtherName,'')),MAX(ISNULL(p.MaterialTypeCode,'')),@UserName,@UserName,GETDATE(),GETDATE(),@CustomerID,0
				FROM PartMaterial p 
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					LEFT JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(p.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE ((REPLACE(p.OtherName,' ','') not like N'%THREAD%' AND REPLACE(p.OtherName,' ','') not like '%CARELABEL%' AND REPLACE(p.OtherName,' ','') not like '%CARE+SIZE%' 
					AND REPLACE(p.OtherName,' ','') not like '%SIZELABEL%' AND REPLACE(p.OtherName,' ','') not like '%SIZE+CARE%' AND REPLACE(p.OtherName,' ','') not in (N'SIZEHT')
					AND REPLACE(p.OtherName,' ','') not in (N'DRAWSTRINGS')) OR p.OtherName IS NULL) AND i.ID IS NULL AND r.CustomerID=@CustomerID AND (ISNULL(p.ItemID,'')<>'' OR ISNULL(p.ItemColorCode,'')<>'') AND p.DsmItemID IS NOT NULL
				GROUP BY REPLACE(UPPER(TRIM(ISNULL(p.DsmItemID,''))),' ',''),REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ','') 

				--- Set ItemMasterID for PartMaterial,ProBOM,....
				UPDATE p SET p.ItemMasterID=i.ID
				FROM PartMaterial p 
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(p.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE ((REPLACE(p.OtherName,' ','') not like N'%THREAD%' AND REPLACE(p.OtherName,' ','') not like '%CARELABEL%' AND REPLACE(p.OtherName,' ','') not like '%CARE+SIZE%' 
					AND REPLACE(p.OtherName,' ','') not like '%SIZELABEL%' AND REPLACE(p.OtherName,' ','') not like '%SIZE+CARE%' AND REPLACE(p.OtherName,' ','') not in (N'SIZEHT')
					AND REPLACE(p.OtherName,' ','') not in (N'DRAWSTRINGS')) OR p.OtherName IS NULL) AND p.ItemMasterID IS NULL AND r.CustomerID=@CustomerID AND (ISNULL(p.ItemID,'')<>'' OR ISNULL(p.ItemColorCode,'')<>'')

				UPDATE b SET b.ItemMasterID=i.ID
				FROM ProductionBOM b
					INNER JOIN  PartMaterial p ON b.PartMaterialID=p.ID
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(b.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(b.ItemColorCode,''))),' ','')=i.ItemColorCode 
				WHERE ((REPLACE(b.OtherName,' ','') not like N'%THREAD%' AND REPLACE(b.OtherName,' ','') not like '%CARELABEL%' AND REPLACE(b.OtherName,' ','') not like '%CARE+SIZE%' 
					AND REPLACE(b.OtherName,' ','') not like '%SIZELABEL%' AND REPLACE(b.OtherName,' ','') not like '%SIZE+CARE%' AND REPLACE(b.OtherName,' ','') not in (N'SIZEHT')
					AND REPLACE(b.OtherName,' ','') not in (N'DRAWSTRINGS')) OR b.OtherName IS NULL) AND b.ItemMasterID IS NULL AND r.CustomerID=@CustomerID AND (ISNULL(b.ItemID,'')<>'' OR ISNULL(b.ItemColorCode,'')<>'')

				UPDATE f SET f.ItemMasterID=i.ID
				FROM ForecastMaterial f
					INNER JOIN  PartMaterial p ON f.PartMaterialID=p.ID
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(f.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(f.ItemColorCode,''))),' ','')=i.ItemColorCode 
				WHERE ((REPLACE(f.OtherName,' ','') not like N'%THREAD%' AND REPLACE(f.OtherName,' ','') not like '%CARELABEL%' AND REPLACE(f.OtherName,' ','') not like '%CARE+SIZE%' 
					AND REPLACE(f.OtherName,' ','') not like '%SIZELABEL%' AND REPLACE(f.OtherName,' ','') not like '%SIZE+CARE%' AND REPLACE(f.OtherName,' ','') not in (N'SIZEHT')
					AND REPLACE(f.OtherName,' ','') not in (N'DRAWSTRINGS')) OR f.OtherName IS NULL) AND f.ItemMasterID IS NULL AND r.CustomerID=@CustomerID AND (ISNULL(f.ItemID,'')<>'' OR ISNULL(f.ItemColorCode,'')<>'')

				UPDATE d SET d.ItemMasterID=i.ID
				FROM StorageDetail d
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(d.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(d.ItemColorCode,''))),' ','')=i.ItemColorCode 
				WHERE ((REPLACE(d.OtherName,' ','') not like N'%THREAD%' AND REPLACE(d.OtherName,' ','') not like '%CARELABEL%' AND REPLACE(d.OtherName,' ','') not like '%CARE+SIZE%' 
					AND REPLACE(d.OtherName,' ','') not like '%SIZELABEL%' AND REPLACE(d.OtherName,' ','') not like '%SIZE+CARE%' AND REPLACE(d.OtherName,' ','') not in (N'SIZEHT')
					AND REPLACE(d.OtherName,' ','') not in (N'DRAWSTRINGS')) OR d.OtherName IS NULL) AND d.ItemMasterID IS NULL AND d.CustomerID=@CustomerID AND (ISNULL(d.ItemID,'')<>'' OR ISNULL(d.ItemColorCode,'')<>'')
			
				UPDATE m SET m.ItemMasterID=i.ID
				FROM MaterialTransaction m
					INNER JOIN StorageDetail d ON m.StorageDetailID = d.ID
					--INNER JOIN PurchaseOrder p ON REPLACE(UPPER(TRIM(ISNULL(m.PurchaseOrderNumber,''))),' ','') = REPLACE(UPPER(TRIM(ISNULL(p.Number,''))),' ','')
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(m.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(m.ItemColorCode,''))),' ','')=i.ItemColorCode 
				WHERE ((REPLACE(m.OtherName,' ','') not like N'%THREAD%' AND REPLACE(m.OtherName,' ','') not like '%CARELABEL%' AND REPLACE(m.OtherName,' ','') not like '%CARE+SIZE%' 
					AND REPLACE(m.OtherName,' ','') not like '%SIZELABEL%' AND REPLACE(m.OtherName,' ','') not like '%SIZE+CARE%' AND REPLACE(m.OtherName,' ','') not in (N'SIZEHT')
					AND REPLACE(m.OtherName,' ','') not in (N'DRAWSTRINGS')) OR m.OtherName IS NULL) AND m.ItemMasterID IS NULL AND d.CustomerID=@CustomerID AND (ISNULL(m.ItemID,'')<>'' OR ISNULL(m.ItemColorCode,'')<>'')

				UPDATE l SET l.ItemMasterID=i.ID
				FROM PurchaseOrderGroupLine l
					INNER JOIN PurchaseOrder p ON l.PurchaseOrderID=p.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(l.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE ((REPLACE(l.OtherName,' ','') not like N'%THREAD%' AND REPLACE(l.OtherName,' ','') not like '%CARELABEL%' AND REPLACE(l.OtherName,' ','') not like '%CARE+SIZE%' 
					AND REPLACE(l.OtherName,' ','') not like '%SIZELABEL%' AND REPLACE(l.OtherName,' ','') not like '%SIZE+CARE%' AND REPLACE(l.OtherName,' ','') not in (N'SIZEHT')
					AND REPLACE(l.OtherName,' ','') not in (N'DRAWSTRINGS')) OR l.OtherName IS NULL) AND l.ItemMasterID IS NULL AND p.CustomerID=@CustomerID AND (ISNULL(l.ItemID,'')<>'' OR ISNULL(l.ItemColorCode,'')<>'')

				UPDATE l SET l.ItemMasterID=i.ID
				FROM PurchaseOrderLine l
					INNER JOIN PurchaseOrder p ON l.PurchaseOrderID=p.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(l.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE ((REPLACE(l.OtherName,' ','') not like N'%THREAD%' AND REPLACE(l.OtherName,' ','') not like '%CARELABEL%' AND REPLACE(l.OtherName,' ','') not like '%CARE+SIZE%' 
					AND REPLACE(l.OtherName,' ','') not like '%SIZELABEL%' AND REPLACE(l.OtherName,' ','') not like '%SIZE+CARE%' AND REPLACE(l.OtherName,' ','') not in (N'SIZEHT')
					AND REPLACE(l.OtherName,' ','') not in (N'DRAWSTRINGS')) OR l.OtherName IS NULL) AND l.ItemMasterID IS NULL AND p.CustomerID=@CustomerID AND (ISNULL(l.ItemID,'')<>'' OR ISNULL(l.ItemColorCode,'')<>'')

				UPDATE l SET l.ItemMasterID=i.ID
				FROM IssuedLine l
					join Issued s on l.IssuedNumber=s.Number
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(l.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE ((REPLACE(l.OtherName,' ','') not like N'%THREAD%' AND REPLACE(l.OtherName,' ','') not like '%CARELABEL%' AND REPLACE(l.OtherName,' ','') not like '%CARE+SIZE%' 
					AND REPLACE(l.OtherName,' ','') not like '%SIZELABEL%' AND REPLACE(l.OtherName,' ','') not like '%SIZE+CARE%' AND REPLACE(l.OtherName,' ','') not in (N'SIZEHT')
					AND REPLACE(l.OtherName,' ','') not in (N'DRAWSTRINGS')) OR l.OtherName IS NULL) AND l.ItemMasterID IS NULL AND s.CustomerID=@CustomerID AND (ISNULL(l.ItemID,'')<>'' OR ISNULL(l.ItemColorCode,'')<>'')

				UPDATE d SET d.ItemMaterID=i.ID
				FROM MaterialRequestDetails d
					INNER JOIN MaterialRequests r on d.MaterialRequestId=r.Id
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(d.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(d.ItemColorCode,''))),' ','')=i.ItemColorCode 
				WHERE ((REPLACE(d.OtherName,' ','') not like N'%THREAD%' AND REPLACE(d.OtherName,' ','') not like '%CARELABEL%' AND REPLACE(d.OtherName,' ','') not like '%CARE+SIZE%' 
					AND REPLACE(d.OtherName,' ','') not like '%SIZELABEL%' AND REPLACE(d.OtherName,' ','') not like '%SIZE+CARE%' AND REPLACE(d.OtherName,' ','') not in (N'SIZEHT')
					AND REPLACE(d.OtherName,' ','') not in (N'DRAWSTRINGS')) OR d.OtherName IS NULL) AND d.ItemMaterID IS NULL AND r.CustomerID=@CustomerID AND (ISNULL(d.ItemID,'')<>'' OR ISNULL(d.ItemColorCode,'')<>'')
			
				UPDATE l SET l.ItemMasterID=i.ID
				FROM ReceiptGroupLine l
					INNER JOIN Receipt r on l.ReceiptNumber=r.Number
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(l.DsmItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE ((REPLACE(l.OtherName,' ','') not like N'%THREAD%' AND REPLACE(l.OtherName,' ','') not like '%CARELABEL%' AND REPLACE(l.OtherName,' ','') not like '%CARE+SIZE%' 
					AND REPLACE(l.OtherName,' ','') not like '%SIZELABEL%' AND REPLACE(l.OtherName,' ','') not like '%SIZE+CARE%' AND REPLACE(l.OtherName,' ','') not in (N'SIZEHT')
					AND REPLACE(l.OtherName,' ','') not in (N'DRAWSTRINGS')) OR l.OtherName IS NULL) AND l.ItemMasterID IS NULL AND r.CustomerID=@CustomerID AND (ISNULL(l.ItemID,'')<>'' OR ISNULL(l.ItemColorCode,'')<>'')
			--===== End Create Item Master for Others =====--
			END
			ELSE IF @CustomerID = 'GA' -- OR @CustomerID = 'IFG' OR @CustomerID = 'HM'
			BEGIN
			--====== Create Item Master for THREAD =====--
				INSERT INTO ItemMaster(ID,ItemID,ItemName,ItemColorCode,ItemColorName,ItemStyleNumber,Position,Specify,Season,GarmentSize,GarmentColorCode,GarmentColorName,GroupSize,GroupItemColor,OtherName,MaterialTypeCode,CreatedBy,LastUpdatedBy,CreatedAt,LastUpdatedAt,CustomerID,IsDeleted)
				SELECT DISTINCT UPPER(NEWID()),'',UPPER(TRIM(ISNULL(p.DefaultThreadName,''))),REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ',''),'','','','','','','','',
					MAX(CAST(ISNULL(p.GroupSize,0) AS INT)),MAX(CAST(ISNULL(p.GroupItemColor,0) AS INT)),
					MAX(p.OtherName),MAX(p.MaterialTypeCode),@UserName,@UserName,GETDATE(),GETDATE(),@CustomerID,0
				FROM PartMaterial p 
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID 
					LEFT JOIN ItemMaster i ON UPPER(TRIM(ISNULL(p.DefaultThreadName,'')))=i.ItemName AND REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE p.OtherName Like N'THREAD' AND i.ID IS NULL AND r.CustomerID=@CustomerID AND ISNULL(p.DefaultThreadName,'')<>'' 
				GROUP BY UPPER(TRIM(ISNULL(p.DefaultThreadName,''))),REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ','')

				--- Set ItemMasterID for PartMaterial,ProBOM,....
				UPDATE p SET p.ItemMasterID=i.ID
				FROM PartMaterial p 
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID 
					INNER JOIN ItemMaster i ON UPPER(TRIM(ISNULL(p.DefaultThreadName,'')))=i.ItemName AND REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE p.OtherName Like N'THREAD' AND p.ItemMasterID IS NULL AND r.CustomerID=@CustomerID AND ISNULL(p.DefaultThreadName,'')<>''

				UPDATE b SET b.ItemMasterID=i.ID
				FROM ProductionBOM b
					INNER JOIN  PartMaterial p ON b.PartMaterialID=p.ID
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					INNER JOIN ItemMaster i ON UPPER(TRIM(ISNULL(b.DefaultThreadName,'')))=i.ItemName AND REPLACE(UPPER(TRIM(ISNULL(b.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE b.OtherName Like N'THREAD' AND b.ItemMasterID IS NULL AND r.CustomerID=@CustomerID AND ISNULL(b.DefaultThreadName,'')<>''

				UPDATE f SET f.ItemMasterID=i.ID
				FROM ForecastMaterial f
					INNER JOIN  PartMaterial p ON f.PartMaterialID=p.ID
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					INNER JOIN ItemMaster i ON UPPER(TRIM(ISNULL(f.DefaultThreadName,'')))=i.ItemName AND REPLACE(UPPER(TRIM(ISNULL(f.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE f.OtherName Like N'THREAD' AND f.ItemMasterID IS NULL AND r.CustomerID=@CustomerID AND ISNULL(f.DefaultThreadName,'')<>''

				UPDATE d SET d.ItemMasterID=i.ID
				FROM StorageDetail d
					INNER JOIN ItemMaster i ON UPPER(TRIM(ISNULL(d.DefaultThreadName,'')))=i.ItemName AND REPLACE(UPPER(TRIM(ISNULL(d.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE d.OtherName Like N'THREAD' AND d.ItemMasterID IS NULL AND d.CustomerID=@CustomerID AND ISNULL(d.DefaultThreadName,'')<>''
			
				UPDATE m SET m.ItemMasterID=i.ID
				FROM MaterialTransaction m
					INNER JOIN StorageDetail d ON m.StorageDetailID = d.ID
					--INNER JOIN PurchaseOrder p ON REPLACE(UPPER(TRIM(ISNULL(m.PurchaseOrderNumber,''))),' ','') = REPLACE(UPPER(TRIM(ISNULL(p.Number,''))),' ','')
					INNER JOIN ItemMaster i ON UPPER(TRIM(ISNULL(m.DefaultThreadName,'')))=i.ItemName AND REPLACE(UPPER(TRIM(ISNULL(m.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE m.OtherName Like N'THREAD' AND m.ItemMasterID IS NULL AND d.CustomerID=@CustomerID AND ISNULL(m.DefaultThreadName,'')<>''

				UPDATE l SET l.ItemMasterID=i.ID
				FROM PurchaseOrderGroupLine l
					INNER JOIN PurchaseOrder p ON l.PurchaseOrderID=p.ID
					INNER JOIN ItemMaster i ON UPPER(TRIM(ISNULL(l.DefaultThreadName,'')))=i.ItemName AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE REPLACE(l.OtherName,' ','') Like N'%THREAD%' AND l.ItemMasterID IS NULL AND p.CustomerID=@CustomerID AND ISNULL(l.DefaultThreadName,'')<>''

				UPDATE l SET l.ItemMasterID=i.ID
				FROM PurchaseOrderLine l
					INNER JOIN PurchaseOrder p ON l.PurchaseOrderID=p.ID
					INNER JOIN ItemMaster i ON UPPER(TRIM(ISNULL(l.DefaultThreadName,'')))=i.ItemName AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE l.OtherName Like N'THREAD' AND l.ItemMasterID IS NULL AND p.CustomerID=@CustomerID AND ISNULL(l.DefaultThreadName,'')<>''

				UPDATE l SET l.ItemMasterID=i.ID
				FROM IssuedLine l
					INNER JOIN Issued s on l.IssuedNumber=s.Number
					INNER JOIN ItemMaster i ON UPPER(TRIM(ISNULL(l.DefaultThreadName,'')))=i.ItemName AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE l.OtherName Like N'THREAD' AND l.ItemMasterID IS NULL AND s.CustomerID=@CustomerID AND ISNULL(l.DefaultThreadName,'')<>''

				UPDATE d SET d.ItemMaterID=i.ID
				FROM MaterialRequestDetails d
					INNER JOIN MaterialRequests r on d.MaterialRequestId=r.Id
					INNER JOIN ItemMaster i ON UPPER(TRIM(ISNULL(d.DefaultThreadName,'')))=i.ItemName AND REPLACE(UPPER(TRIM(ISNULL(d.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE d.OtherName Like N'THREAD' AND d.ItemMaterID IS NULL AND r.CustomerID=@CustomerID AND ISNULL(d.DefaultThreadName,'')<>''

				UPDATE l SET l.ItemMasterID=i.ID
				FROM ReceiptGroupLine l
					INNER JOIN Receipt r on l.ReceiptNumber=r.Number
					INNER JOIN ItemMaster i ON UPPER(TRIM(ISNULL(l.DefaultThreadName,'')))=i.ItemName AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode
				WHERE l.OtherName Like N'THREAD' AND l.ItemMasterID IS NULL AND r.CustomerID=@CustomerID AND ISNULL(l.DefaultThreadName,'')<>''
			--====== End Create Item Master for THREAD =====--

			--===== Create Item Master for OTHERS =====--
				INSERT INTO ItemMaster(ID,ItemID,ItemName,ItemColorCode,ItemColorName,ItemStyleNumber,Position,Specify,Season,GarmentSize,GarmentColorCode,GarmentColorName,GroupSize,GroupItemColor,OtherName,MaterialTypeCode,CreatedBy,LastUpdatedBy,CreatedAt,LastUpdatedAt,CustomerID,IsDeleted)
				SELECT DISTINCT UPPER(NEWID()),REPLACE(UPPER(TRIM(ISNULL(p.ItemID,''))),' ',''),'',REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ',''),'','','',REPLACE(UPPER(TRIM(ISNULL(p.Specify,''))),' ',''),'',REPLACE(UPPER(TRIM(ISNULL(p.GarmentSize,''))),' ',''),'','',
					MAX(CAST(ISNULL(p.GroupSize,0) AS INT)),MAX(CAST(ISNULL(p.GroupItemColor,0) AS INT)),
					MAX(p.OtherName),MAX(p.MaterialTypeCode),@UserName,@UserName,GETDATE(),GETDATE(),@CustomerID,0
				FROM PartMaterial p 
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					LEFT JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(p.ItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(p.GarmentSize,''))),' ','')=i.GarmentSize AND REPLACE(UPPER(TRIM(ISNULL(p.Specify,''))),' ','') = i.Specify
				WHERE p.OtherName like 'OTHERS' AND i.ID IS NULL AND r.CustomerID=@CustomerID
				GROUP BY REPLACE(UPPER(TRIM(ISNULL(p.ItemID,''))),' ',''),REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ',''),REPLACE(UPPER(TRIM(ISNULL(p.GarmentSize,''))),' ',''),REPLACE(UPPER(TRIM(ISNULL(p.Specify,''))),' ','')

				--- Set ItemMasterID for PartMaterial,ProBOM,....
				UPDATE p SET p.ItemMasterID=i.ID
				FROM PartMaterial p 
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(p.ItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(p.GarmentSize,''))),' ','')=i.GarmentSize AND REPLACE(UPPER(TRIM(ISNULL(p.Specify,''))),' ','') = i.Specify
				WHERE p.OtherName like 'OTHERS' AND p.ItemMasterID IS NULL AND r.CustomerID=@CustomerID

				UPDATE b SET b.ItemMasterID=i.ID
				FROM ProductionBOM b
					INNER JOIN  PartMaterial p ON b.PartMaterialID=p.ID
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(b.ItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(b.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(b.GarmentSize,''))),' ','')=i.GarmentSize AND REPLACE(UPPER(TRIM(ISNULL(p.Specify,''))),' ','') = i.Specify
				WHERE b.OtherName like 'OTHERS' AND b.ItemMasterID IS NULL AND r.CustomerID=@CustomerID

				UPDATE f SET f.ItemMasterID=i.ID
				FROM ForecastMaterial f
					INNER JOIN  PartMaterial p ON f.PartMaterialID=p.ID
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(f.ItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(f.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(f.GarmentSize,''))),' ','')=i.GarmentSize AND REPLACE(UPPER(TRIM(ISNULL(f.Specify,''))),' ','') = i.Specify
				WHERE f.OtherName like 'OTHERS' AND f.ItemMasterID IS NULL AND r.CustomerID=@CustomerID

				UPDATE d SET d.ItemMasterID=i.ID
				FROM StorageDetail d
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(d.ItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(d.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(d.GarmentSize,''))),' ','')=i.GarmentSize AND REPLACE(UPPER(TRIM(ISNULL(d.Specify,''))),' ','') = i.Specify
				WHERE d.OtherName like 'OTHERS' AND d.ItemMasterID IS NULL AND d.CustomerID=@CustomerID
			
				UPDATE m SET m.ItemMasterID=i.ID
				FROM MaterialTransaction m
					INNER JOIN StorageDetail d ON m.StorageDetailID = d.ID
					--INNER JOIN PurchaseOrder p ON REPLACE(UPPER(TRIM(ISNULL(m.PurchaseOrderNumber,''))),' ','') = REPLACE(UPPER(TRIM(ISNULL(p.Number,''))),' ','')
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(m.ItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(m.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(m.GarmentSize,''))),' ','')=i.GarmentSize AND REPLACE(UPPER(TRIM(ISNULL(m.Specify,''))),' ','') = i.Specify
				WHERE m.OtherName like 'OTHERS' AND m.ItemMasterID IS NULL AND d.CustomerID=@CustomerID

				UPDATE l SET l.ItemMasterID=i.ID
				FROM PurchaseOrderGroupLine l
					INNER JOIN PurchaseOrder p ON l.PurchaseOrderID=p.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(l.ItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(l.GarmentSize,''))),' ','')=i.GarmentSize AND REPLACE(UPPER(TRIM(ISNULL(l.Specify,''))),' ','') = i.Specify
				WHERE l.OtherName like 'OTHERS' AND l.ItemMasterID IS NULL AND p.CustomerID=@CustomerID

				UPDATE l SET l.ItemMasterID=i.ID
				FROM PurchaseOrderLine l
					INNER JOIN PurchaseOrder p ON l.PurchaseOrderID=p.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(l.ItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(l.GarmentSize,''))),' ','')=i.GarmentSize AND REPLACE(UPPER(TRIM(ISNULL(l.Specify,''))),' ','') = i.Specify
				WHERE l.OtherName like 'OTHERS' AND l.ItemMasterID IS NULL AND p.CustomerID=@CustomerID

				UPDATE l SET l.ItemMasterID=i.ID
				FROM IssuedLine l
					INNER JOIN Issued s on l.IssuedNumber=s.Number
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(l.ItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(l.GarmentSize,''))),' ','')=i.GarmentSize AND REPLACE(UPPER(TRIM(ISNULL(l.Specify,''))),' ','') = i.Specify
				WHERE l.OtherName like 'OTHERS' AND l.ItemMasterID IS NULL AND s.CustomerID=@CustomerID

				UPDATE d SET d.ItemMaterID=i.ID
				FROM MaterialRequestDetails d
					INNER JOIN MaterialRequests r on d.MaterialRequestId=r.Id
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(d.ItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(d.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(d.GarmentSize,''))),' ','')=i.GarmentSize AND REPLACE(UPPER(TRIM(ISNULL(d.Specify,''))),' ','') = i.Specify
				WHERE d.OtherName like 'OTHERS' AND d.ItemMaterID IS NULL AND r.CustomerID=@CustomerID

				UPDATE l SET l.ItemMasterID=i.ID
				FROM ReceiptGroupLine l
					INNER JOIN Receipt r on l.ReceiptNumber=r.Number
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(l.ItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(l.GarmentSize,''))),' ','')=i.GarmentSize AND REPLACE(UPPER(TRIM(ISNULL(l.Specify,''))),' ','') = i.Specify
				WHERE l.OtherName like 'OTHERS' AND l.ItemMasterID IS NULL AND r.CustomerID=@CustomerID
			--===== End Create Item Master for OTHERS =====--

			--===== Create Item Master for FABRIC =====--
				INSERT INTO ItemMaster(ID,ItemID,ItemName,ItemColorCode,ItemColorName,ItemStyleNumber,Position,Specify,Season,GarmentSize,GarmentColorCode,GarmentColorName,GroupSize,GroupItemColor,OtherName,MaterialTypeCode,CreatedBy,LastUpdatedBy,CreatedAt,LastUpdatedAt,CustomerID,IsDeleted)
				SELECT DISTINCT UPPER(NEWID()),REPLACE(UPPER(TRIM(ISNULL(p.ItemID,''))),' ',''),UPPER(TRIM(ISNULL(p.ItemName,''))),REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ',''),UPPER(TRIM(ISNULL(p.ItemColorName,''))),'','','','','','','',
					MAX(CAST(ISNULL(p.GroupSize,0) AS INT)),MAX(CAST(ISNULL(p.GroupItemColor,0) AS INT)),
					MAX(p.OtherName),MAX(p.MaterialTypeCode),@UserName,@UserName,GETDATE(),GETDATE(),@CustomerID,0
				FROM PartMaterial p 
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					LEFT JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(p.ItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(p.ItemName,''))),' ','') = REPLACE(i.ItemName,' ','') AND REPLACE(UPPER(TRIM(ISNULL(p.ItemColorName,''))),' ','')= REPLACE(i.ItemColorName,' ','') 
				WHERE p.OtherName like 'FABRIC' AND i.ID IS NULL AND r.CustomerID=@CustomerID
				GROUP BY REPLACE(UPPER(TRIM(ISNULL(p.ItemID,''))),' ',''),UPPER(TRIM(ISNULL(p.ItemName,''))),REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ',''),UPPER(TRIM(ISNULL(p.ItemColorName,'')))

				--- Set ItemMasterID for PartMaterial,ProBOM,....
				UPDATE p SET p.ItemMasterID=i.ID
				FROM PartMaterial p 
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(p.ItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(p.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(p.ItemName,''))),' ','') = REPLACE(i.ItemName,' ','') AND REPLACE(UPPER(TRIM(ISNULL(p.ItemColorName,''))),' ','')= REPLACE(i.ItemColorName,' ','') 
				WHERE p.OtherName like 'FABRIC' AND p.ItemMasterID IS NULL AND r.CustomerID=@CustomerID

				UPDATE b SET b.ItemMasterID=i.ID
				FROM ProductionBOM b
					INNER JOIN  PartMaterial p ON b.PartMaterialID=p.ID
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(b.ItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(b.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(b.ItemName,''))),' ','') = REPLACE(i.ItemName,' ','') AND REPLACE(UPPER(TRIM(ISNULL(b.ItemColorName,''))),' ','')= REPLACE(i.ItemColorName,' ','') 
				WHERE b.OtherName like 'FABRIC' AND b.ItemMasterID IS NULL AND r.CustomerID=@CustomerID

				UPDATE f SET f.ItemMasterID=i.ID
				FROM ForecastMaterial f
					INNER JOIN  PartMaterial p ON f.PartMaterialID=p.ID
					INNER JOIN PartRevision r ON p.PartRevisionID = r.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(f.ItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(f.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(f.ItemName,''))),' ','') = REPLACE(i.ItemName,' ','') AND REPLACE(UPPER(TRIM(ISNULL(f.ItemColorName,''))),' ','')= REPLACE(i.ItemColorName,' ','') 
				WHERE f.OtherName like 'FABRIC' AND f.ItemMasterID IS NULL AND r.CustomerID=@CustomerID

				UPDATE d SET d.ItemMasterID=i.ID
				FROM StorageDetail d
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(d.ItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(d.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(d.ItemName,''))),' ','') = REPLACE(i.ItemName,' ','') AND REPLACE(UPPER(TRIM(ISNULL(d.ItemColorName,''))),' ','')= REPLACE(i.ItemColorName,' ','') 
				WHERE d.OtherName like 'FABRIC' AND d.ItemMasterID IS NULL AND d.CustomerID=@CustomerID
			
				UPDATE m SET m.ItemMasterID=i.ID
				FROM MaterialTransaction m
					INNER JOIN StorageDetail d ON m.StorageDetailID = d.ID
					--INNER JOIN PurchaseOrder p ON REPLACE(UPPER(TRIM(ISNULL(m.PurchaseOrderNumber,''))),' ','') = REPLACE(UPPER(TRIM(ISNULL(p.Number,''))),' ','')
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(m.ItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(m.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(m.ItemName,''))),' ','') = REPLACE(i.ItemName,' ','') AND REPLACE(UPPER(TRIM(ISNULL(m.ItemColorName,''))),' ','')= REPLACE(i.ItemColorName,' ','') 
				WHERE m.OtherName like 'FABRIC' AND m.ItemMasterID IS NULL AND d.CustomerID=@CustomerID

				UPDATE l SET l.ItemMasterID=i.ID
				FROM PurchaseOrderGroupLine l
					INNER JOIN PurchaseOrder p ON l.PurchaseOrderID=p.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(l.ItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(l.ItemName,''))),' ','') = REPLACE(i.ItemName,' ','') AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorName,''))),' ','')= REPLACE(i.ItemColorName,' ','') 
				WHERE l.OtherName like 'FABRIC' AND l.ItemMasterID IS NULL AND p.CustomerID=@CustomerID

				UPDATE l SET l.ItemMasterID=i.ID
				FROM PurchaseOrderLine l
					INNER JOIN PurchaseOrder p ON l.PurchaseOrderID=p.ID
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(l.ItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(l.ItemName,''))),' ','') = REPLACE(i.ItemName,' ','') AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorName,''))),' ','')= REPLACE(i.ItemColorName,' ','')
				WHERE l.OtherName like 'FABRIC' AND l.ItemMasterID IS NULL AND p.CustomerID=@CustomerID

				UPDATE l SET l.ItemMasterID=i.ID
				FROM IssuedLine l
					INNER JOIN Issued s on l.IssuedNumber=s.Number
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(l.ItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(l.ItemName,''))),' ','') = REPLACE(i.ItemName,' ','') AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorName,''))),' ','')= REPLACE(i.ItemColorName,' ','')
				WHERE l.OtherName like 'FABRIC' AND l.ItemMasterID IS NULL AND s.CustomerID=@CustomerID

				UPDATE d SET d.ItemMaterID=i.ID
				FROM MaterialRequestDetails d
					INNER JOIN MaterialRequests r on d.MaterialRequestId=r.Id
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(d.ItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(d.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(d.ItemName,''))),' ','') = REPLACE(i.ItemName,' ','') AND REPLACE(UPPER(TRIM(ISNULL(d.ItemColorName,''))),' ','')= REPLACE(i.ItemColorName,' ','')
				WHERE d.OtherName like 'FABRIC' AND d.ItemMaterID IS NULL AND r.CustomerID=@CustomerID

				UPDATE l SET l.ItemMasterID=i.ID
				FROM ReceiptGroupLine l
					INNER JOIN Receipt r on l.ReceiptNumber=r.Number
					INNER JOIN ItemMaster i ON REPLACE(UPPER(TRIM(ISNULL(l.ItemID,''))),' ','')=i.ItemID AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorCode,''))),' ','')=i.ItemColorCode AND REPLACE(UPPER(TRIM(ISNULL(l.ItemName,''))),' ','') = REPLACE(i.ItemName,' ','') AND REPLACE(UPPER(TRIM(ISNULL(l.ItemColorName,''))),' ','')= REPLACE(i.ItemColorName,' ','')
				WHERE l.OtherName like 'FABRIC' AND l.ItemMasterID IS NULL AND r.CustomerID=@CustomerID
			--===== End Create Item Master for FABRIC =====--
			END

			SET @MSG= 1
			COMMIT TRANSACTION CreateItemMaster
		END
		ELSE
			SET @MSG =0
    END TRY
    BEGIN CATCH
        SET @MSG= 0
        ROLLBACK TRANSACTION CreateItemMaster
    END CATCH

	SELECT @MSG
END
