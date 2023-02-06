
/****** Object:  StoredProcedure [dbo].[sp_UpdateFabricPurchaseOrderFromTransactions]    Script Date: 12/21/2022 8:47:14 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		DAT.NGUYEN
-- Create date: 2022.12.20
-- Description:	Update fabric purchase onhand, issued quantity
-- =============================================
CREATE OR ALTER   PROCEDURE [dbo].[sp_UpdateFabricPurchaseOrderFromTransactions]

AS
BEGIN
	
	SET NOCOUNT ON;

	IF OBJECT_ID('tempdb.dbo.#TempTotalOnHandFBTransaction') IS NOT NULL
		DROP TABLE #TempTotalOnHandFBTransaction;
	IF OBJECT_ID('tempdb.dbo.#TempTotalIssuedFBTransaction') IS NOT NULL
		DROP TABLE #TempTotalIssuedFBTransaction;
	IF OBJECT_ID('tempdb.dbo.#TempIssuedFBTransaction') IS NOT NULL
		DROP TABLE #TempIssuedFBTransaction;

	BEGIN TRAN UPDATEFABRICSTORAGEDETAIL;

	BEGIN TRY
		---CALCULATE TOTAL ON HAND
		SELECT
			SUM([TRAN].Quantity) AS OnHandQuantity,
			[TRAN].FabricPurchaseOrderNumber 
			INTO #TempTotalOnHandFBTransaction
		FROM
			MaterialTransaction [TRAN]
		WHERE
			--[TRAN].[IsReversed] <> 1 AND
			--AND [TRAN].[IsProcessed] <> 1
			 [TRAN].[StorageCode] = 'FB'
		GROUP BY
			[TRAN].FabricPurchaseOrderNumber

		---CALCUALATE ISSUED
		SELECT
			SUM(Quantity) AS IssuedQuantity,
			[TRAN].ItemMasterID,
			[TRAN].FabricPurchaseOrderNumber,
			[TRAN].StorageDetailID INTO #TempIssuedFBTransaction
		FROM
			MaterialTransaction [TRAN]
		WHERE
			--[TRAN].[IsReversed] <> 1
			--AND [TRAN].[IsProcessed] <> 1
			 [TRAN].[Quantity] < 0
			AND [TRAN].[StorageCode] = 'FB'
		GROUP BY
			[TRAN].[ItemMasterID],
			[TRAN].FabricPurchaseOrderNumber,
			[TRAN].[StorageDetailID]

		---CALCUALATE TOTAL ISSUED
		SELECT
			SUM([TRAN].Quantity) AS IssuedQuantity,
			[TRAN].FabricPurchaseOrderNumber INTO #TempTotalIssuedFBTransaction
		FROM
			MaterialTransaction [TRAN]
		WHERE
			--[TRAN].[IsReversed] <> 1
			--AND [TRAN].[IsProcessed] <> 1
			[TRAN].[Quantity] < 0
			AND [TRAN].[StorageCode] = 'FB'
		GROUP BY
			[TRAN].FabricPurchaseOrderNumber
			
		---UPDATE STORAGE
		UPDATE StorageDetail
		SET 
			IssuedQuantity = Abs([ISSUED].[IssuedQuantity])
		FROM
			StorageDetail [SD]
			JOIN #TempIssuedFBTransaction [ISSUED] ON [SD].[ID] = [ISSUED].[StorageDetailID]
				AND [SD].[ItemMasterID] = [ISSUED].[ItemMasterID]

		---UPDATE FABRIC PURCHASE ORDER 
		UPDATE FabricPurchaseOrder
		SET OnHandQuantity = 0, IssuedQuantity = 0


		---UPDATE FABRIC PURCHASE ORDER ONHAND QUANTITY
		UPDATE FabricPurchaseOrder
		SET 
			OnHandQuantity = [ONHAND].[OnHandQuantity]
		FROM
			FabricPurchaseOrder [FB]
			JOIN StorageDetail [SD] ON SD.FabricPurchaseOrderNumber = FB.Number
			JOIN #TempTotalOnHandFBTransaction [ONHAND] ON [SD].[FabricPurchaseOrderNumber] = [ONHAND].[FabricPurchaseOrderNumber]
		WHERE Number = [SD].[FabricPurchaseOrderNumber]

		---UPDATE FABRIC PURCHASE ORDER ISSUED QUANTITY
		UPDATE FabricPurchaseOrder
		SET 
			IssuedQuantity = Abs([ISSUED].[IssuedQuantity])
		FROM
			FabricPurchaseOrder [FB]
			JOIN StorageDetail [SD] ON SD.FabricPurchaseOrderNumber = FB.Number
			JOIN #TempTotalIssuedFBTransaction [ISSUED] ON [ISSUED].[FaBricPurchaseOrderNumber] = SD.[FabricPurchaseOrderNumber]	
		WHERE Number = [SD].[FabricPurchaseOrderNumber]

		COMMIT TRAN UPDATEFABRICSTORAGEDETAIL
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN UPDATEFABRICSTORAGEDETAIL
	END CATCH

	IF OBJECT_ID('tempdb.dbo.#TempIssuedFBTransaction') IS NOT NULL
		DROP TABLE #TempIssuedFBTransaction;
	IF OBJECT_ID('tempdb.dbo.#TempTotalOnHandFBTransaction') IS NOT NULL
		DROP TABLE #TempTotalOnHandFBTransaction;
	IF OBJECT_ID('tempdb.dbo.#TempTotalIssuedFBTransaction') IS NOT NULL
		DROP TABLE #TempTotalIssuedFBTransaction;

END
