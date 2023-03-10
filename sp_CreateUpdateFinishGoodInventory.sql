
--==================================
-- Author:		LamNX
-- Create date: 2022-09-26
-- Description:	Create/Update On Hand Quantity Finish Good Inventory
-- =============================================
CREATE PROCEDURE [dbo].[sp_CreateUpdateFinishGoodInventory] 
	@UserName NVARCHAR(1000),
	@PeriodID BIGINT
AS
BEGIN
	
	SET NOCOUNT ON;

	BEGIN TRAN CreateUpdateFinishGoodInventory
    BEGIN TRY

		DECLARE @FromDate DATE
		DECLARE @ToDate DATE
		
		----- GET PEROID OPENNING TO GET DATA -----
		SELECT 
			@FromDate = CAST(FromDate AS DATE), @ToDate= CAST(ToDate AS DATE)
		FROM 
			InventoryPeriod 
		WHERE 
			ID = @PeriodID
		
		------ GET DATA FROM SCAN RESULT & SHIPPING PLAN ------
		SELECT 
			* INTO #InventoryDetailTemp
		FROM 
		(
			----- RECEIPT FROM SCAN RESULT
			SELECT 
				PeriodID = @PeriodID, 
				PurchaseOrderNumber = r.PONumber,
				LSStyle='', 
				i.CustomerStyle, 
				i.GarmentColorCode, 
				i.GarmentColorName,
				i.GarmentSize,
				d.[Description], 
				Quantity = SUM(ISNULL(d.Found,0) + ISNULL(d.Missing,0) - ISNULL(d.Unexpected,0)), 
				ScanResultDetailID=d.ID, 
				ShippingPlanDetailID =0, 
				CompanyCode=r.CompanyID, 
				d.ItemCode, 
				PackingListID = 0
			FROM ScanResult (NOLOCK) r
				INNER JOIN ScanResultDetail (NOLOCK) d ON r.ID=d.ScanResultID
				INNER JOIN 
					(
						SELECT DISTINCT 
							i1.CustomerStyle, 
							GarmentColorCode = b.Color, 
							GarmentColorName = i1.ColorName,
							GarmentSize=b.Size,
							b.BarCode
						FROM 
							ItemStyleBarCode (NOLOCK) b 
							INNER JOIN ItemStyle (NOLOCK) i1 ON i1.Number=b.ItemStyleNumber
					) i ON d.ItemCode = i.BarCode AND d.GarmentColorCode= i.GarmentColorCode
			WHERE 
				CAST(r.StartDate AS DATE) >= @FromDate 
				AND CAST(r.EndDate AS DATE) <= @ToDate 
				AND ISNULL(r.IsConfirm,0) = 1 
				AND ISNULL(r.IsDeleted,0) <> 1 
				AND ISNULL(r.IsDeActive,0) <> 1 
				AND ISNULL(r.PONumber,'') <> ''
			GROUP BY 
				r.PONumber,
				i.CustomerStyle, 
				i.GarmentColorCode, 
				i.GarmentColorName,
				i.GarmentSize, 
				d.[Description],
				d.ID, 
				r.CompanyID,
				d.ItemCode

			--UNION ALL

			------- ISSUE FROM SHIPPING PLAN
			--SELECT 
			--	PeriodID=@PeriodID, 
			--	PONumner=d.PurchaseOrderNumber,
			--	d.LSStyle, 
			--	d.CustomerStyle, 
			--	d.GarmentColorCode, 
			--	GarmentColorName=l.ColorName,
			--	GarmentSize=l.Size, 
			--	l.Description, 
			--	Quantity=-1*SUM(l.TotalQuantity),
			--	ScanResultDetailID = 0, 
			--	ShippingPlanDetailID=d.ID, 
			--	CompanyCode,
			--	ItemCode,
			--	PackingListID
			--FROM ShippingPlanDetails (NOLOCK) d 
			--	INNER JOIN 
			--	(
			--		SELECT 
			--			l.LSStyle,
			--			l.Size,
			--			l.TotalQuantity,
			--			p.CompanyCode,
			--			i.ColorName,
			--			i.Description,
			--			ItemCode=b.BarCode,
			--			PackingListID=p.ID
			--		FROM PackingList (NOLOCK) p
			--			 INNER JOIN PackingLine (NOLOCK) l ON p.ID=l.PackingListID
			--			 INNER JOIN ItemStyle (NOLOCK) i ON  l.LSStyle=i.LSStyle	
			--			 INNER JOIN ItemStyleBarCode (NOLOCK) b ON i.Number=b.ItemStyleNumber AND l.Size=b.Size
			--			 LEFT JOIN PackingSheetName (NOLOCK) sh ON p.SheetNameID=sh.ID 
			--		WHERE 
			--			(sh.SheetName = 'Total' OR sh.ID IS NULL) 
			--			AND ISNULL(p.Confirm,0)=1 
			--			AND ISNULL(IsShipped,0) <> 1

			--		UNION ALL

			--		SELECT 
			--			LSStyle = l.LSStyle + '-' + sh.SheetName,
			--			l.Size,l.TotalQuantity,
			--			p.CompanyCode,
			--			i.ColorName,
			--			i.Description,
			--			ItemCode=b.BarCode,
			--			PackingListID=p.ID
			--		FROM PackingList (NOLOCK) p
			--			INNER JOIN PackingLine (NOLOCK) l ON p.ID=l.PackingListID
			--			INNER JOIN ItemStyle (NOLOCK) i ON  l.LSStyle=i.LSStyle	
			--			INNER JOIN ItemStyleBarCode (NOLOCK) b ON i.Number=b.ItemStyleNumber AND l.Size=b.Size
			--			INNER JOIN PackingSheetName (NOLOCK) sh ON p.SheetNameID=sh.ID 
			--		WHERE 
			--			sh.SheetName NOT IN ('Total','Fail') 
			--			AND ISNULL(p.Confirm,0)=1 
			--			AND ISNULL(IsShipped,0) <> 1
			--	) l ON d.LSStyle = l.LSStyle
			--WHERE 
			--	CAST(d.ShipDate AS DATE) BETWEEN @FromDate AND @ToDate 
			--	AND ISNULL(d.IsConfirm,0) = 1 
			--	AND ISNULL(d.IsDeActive,0) <> 1
			--GROUP BY 
			--	d.PurchaseOrderNumber,
			--	d.LSStyle, 
			--	d.CustomerStyle,
			--	d.GarmentColorCode,
			--	l.ColorName,
			--	l.Size,
			--	l.Description,
			--	d.ID,
			--	CompanyCode,
			--	ItemCode,
			--	PackingListID 
		) t
			
		----- CALCULATOR SUMMARY QUANTITY -----
		SELECT 
			t.CustomerStyle,
			t.GarmentColorCode,
			t.GarmentColorName,
			t.GarmentSize,
			t.[Description],
			t.CompanyCode,
			t.ItemCode,
			OnHandQty=SUM(t.Quantity) INTO #InventoryTemp
		FROM 
			#InventoryDetailTemp t
			INNER JOIN ItemStyle (NOLOCK) i ON t.PurchaseOrderNumber = i.PurchaseOrderNumber
		GROUP BY 
			t.CustomerStyle,
			t.GarmentColorCode,
			t.GarmentColorName,
			t.GarmentSize,
			t.[Description],
			t.CompanyCode,
			t.ItemCode

		------ INSERT/UPDATE DATA RELATE TO INVENTORY FG -----
		MERGE 
			InventoryFG ivt1
		USING 
			(	
				SELECT 
					i.ID,
					t.CustomerStyle,
					t.GarmentColorCode,
					t.GarmentColorName,
					t.GarmentSize,
					t.Description,
					t.CompanyCode, 
					t.ItemCode,
					t.OnHandQty
				FROM 
					InventoryFG (NOLOCK) i 
					RIGHT JOIN #InventoryTemp t ON i.ItemCode = t.ItemCode 
						AND i.CompanyCode = t.CompanyCode
			) ivt2 ON ivt1.ID = ivt2.ID
		WHEN MATCHED 
			THEN
				UPDATE SET 
					ivt1.OnHandQuantity += ivt2.OnHandQty, 
					LastUpdatedBy=@UserName,
					LastUpdatedAt=GETDATE()
		WHEN NOT MATCHED 
			THEN
				INSERT VALUES
				(
					ivt2.CustomerStyle,
					ivt2.GarmentColorCode,
					ivt2.GarmentColorName,
					ivt2.GarmentSize,
					N'',
					ivt2.Description,
					ivt2.OnHandQty,
					@UserName,
					@UserName,
					GETDATE(),
					GETDATE(),
					0,
					0,
					ivt2.CompanyCode, 
					ivt2.ItemCode,
					ivt2.Description + ' ' + ivt2.GarmentSize,
					N'PIECE',
					0
				);
		
			INSERT INTO InventoryDetailFG
			(
				InventoryFGID,
				PurchaseOrderNumber,
				LSStyle,CustomerStyle,GarmentColorCode,GarmentColorName,GarmentSize,Season,Description,TransactionDate,Quantity,ScanResultDetailID,ShippingPlanDetailID,CreatedBy,LastUpdatedBy,CreatedAt,LastUpdatedAt,IsDeleted,InventoryPeriodID,PackingListID)
		SELECT i.ID,t.PurchaseOrderNumber,s.LSStyle,t.CustomerStyle,t.GarmentColorCode,t.GarmentColorName,t.GarmentSize,s.Season,t.Description,GETDATE(),t.Quantity,t.ScanResultDetailID,t.ShippingPlanDetailID,@UserName,@UserName,GETDATE(),GETDATE(),0,t.PeriodID,t.PackingListID
		FROM #InventoryDetailTemp t 
			INNER JOIN InventoryFG (NOLOCK) i ON i.ItemCode = t.ItemCode AND i.CompanyCode = t.CompanyCode
			LEFT JOIN ItemStyle (NOLOCK) s ON t.PurchaseOrderNumber = s.PurchaseOrderNumber
		
		INSERT INTO FinishGoodTransaction(InventoryFGID, TransactionDate,PurchaseOrderNumber,LSStyle,CustomerStyle,GarmentColorCode,GarmentColorName,GarmentSize,Season,Description,Quantity,IsReversed,ScanResultDetailID,ShippingPlanDetailID,InventoryPeriodID,PackingListID)
		SELECT i.ID,GETDATE(),t.PurchaseOrderNumber,s.LSStyle,t.CustomerStyle,t.GarmentColorCode,t.GarmentColorName,t.GarmentSize,s.Season,t.Description,t.Quantity,NULL,t.ScanResultDetailID,t.ShippingPlanDetailID,t.PeriodID,t.PackingListID
		FROM #InventoryDetailTemp t 
			INNER JOIN InventoryFG (NOLOCK) i ON i.ItemCode = t.ItemCode AND i.CompanyCode = t.CompanyCode
			INNER JOIN ItemStyle (NOLOCK) s ON t.PurchaseOrderNumber = s.PurchaseOrderNumber
	
		----- DEACTIVE SCAN RESULT/SHIPPING PLAN WHEN UPDATED QUANTITY
		UPDATE r SET r.IsDeActive = 1
		FROM ScanResult (NOLOCK) r 
			INNER JOIN ScanResultDetail (NOLOCK) d ON r.ID = d.ScanResultID
		WHERE d.ID IN (SELECT ScanResultDetailID FROM #InventoryDetailTemp WHERE ISNULL(ScanResultDetailID,0) <> 0)

		--UPDATE d SET d.IsDeActive = 1
		--FROM ShippingPlanDetails (NOLOCK) d 
		--WHERE d.ID IN (SELECT ShippingPlanDetailID FROM #InventoryDetailTemp WHERE ISNULL(ShippingPlanDetailID,0) <> 0)

		UPDATE p SET p.IsShipped = 1
		FROM PackingList (NOLOCK) p 
		WHERE p.ID IN (SELECT PackingListID FROM #InventoryDetailTemp WHERE ISNULL(PackingListID,0) <> 0)

		DROP TABLE #InventoryTemp
		DROP TABLE #InventoryDetailTemp

		COMMIT TRANSACTION CreateUpdateFinishGoodInventory
	END TRY
    BEGIN CATCH

        ROLLBACK TRANSACTION CreateUpdateFinishGoodInventory

    END CATCH
END
