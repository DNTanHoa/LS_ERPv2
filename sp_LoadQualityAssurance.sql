-- =============================================
-- Author:		Nguyen Xuan Lam
-- Create date: 2022-08-26
-- Description:	Load Quality Assurance
-- =============================================
CREATE PROCEDURE sp_LoadQualityAssurance 
	@CustomerID nvarchar(max),
	@StatusID nvarchar(max),
	@Percent decimal
AS
BEGIN
	----- QA pending -----
	SELECT	ID=0,d.ItemStyleNumber,i.PurchaseOrderNumber,r.CustomerID,r.StorageCode,ApprovedDate=GETDATE(),i.LSStyle,i.CustomerStyle,
		i.Season,GarmentColorCode=i.ColorCode,GarmentColorName=i.ColorName,GarmentSize=d.Size,BinCode=t.StorageBinCode,
		OrderQuantity=d.Quantity,Quantity=SUM(t.Quantity),[Percent]=ROUND(SUM(t.Quantity)/d.Quantity*100,2),QualityStatusID='PE',Remark=N''
	INTO #QA
	FROM MaterialTransaction t
		INNER JOIN Receipt r ON t.ReceiptNumber = r.Number AND r.ReceiptTypeId='RFG' AND r.StorageCode='FG'
		INNER JOIN ItemStyle i ON TRIM(ISNULL(t.PurchaseOrderNumber,'')) = TRIM(ISNULL(i.PurchaseOrderNumber,'')) AND TRIM(ISNULL(t.LSStyle,'')) = TRIM(ISNULL(i.LSStyle,''))
		INNER JOIN OrderDetail d ON i.Number=d.ItemStyleNumber AND TRIM(ISNULL(t.GarmentSize,'')) = TRIM(ISNULL(d.Size,''))
	WHERE ISNULL(t.QualityAssuranceID,'') NOT IN (SELECT ID FROM QualityAssurance) AND r.CustomerID LIKE '%' + @CustomerID +'%' 
	GROUP BY d.ItemStyleNumber,i.PurchaseOrderNumber,r.CustomerID,r.StorageCode,i.LSStyle,i.CustomerStyle,i.Season,i.ColorCode,i.ColorName,d.Size,t.StorageBinCode,d.Quantity
	HAVING (SUM(t.Quantity)/d.Quantity)*100 >= @Percent

	IF (LEN(@StatusID)=0 OR @StatusID='PE')
	BEGIN 
		----- QA passed or failed -----
		SELECT	ID,ItemStyleNumber,PurchaseOrderNumber,CustomerID,StorageCode,ApprovedDate,LSStyle,CustomerStyle,Season,GarmentColorCode,
				GarmentColorName,GarmentSize,BinCode,OrderQuantity,Quantity,[Percent],QualityStatusID,Remark
		FROM QualityAssurance
		WHERE CustomerID LIKE '%' + @CustomerID +'%' AND QualityStatusID LIKE '%' + @StatusID + '%' AND [Percent] >= @Percent

		UNION ALL

		SELECT * FROM #QA
	END
	ELSE
		----- QA passed or failed -----
		SELECT	ID,ItemStyleNumber,PurchaseOrderNumber,CustomerID,StorageCode,ApprovedDate,LSStyle,CustomerStyle,Season,GarmentColorCode,
			GarmentColorName,GarmentSize,BinCode,OrderQuantity,Quantity,[Percent],QualityStatusID,Remark
		FROM QualityAssurance
		WHERE CustomerID LIKE '%' + @CustomerID +'%' AND QualityStatusID LIKE '%' + @StatusID + '%' AND [Percent] >= @Percent

	DROP TABLE #QA

END
GO


