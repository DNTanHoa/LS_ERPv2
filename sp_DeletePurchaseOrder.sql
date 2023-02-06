-- ================================================
-- Delete purchase order base on ID
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Hoa.DNT
-- Create date: 2022.08.23
-- Description:	Delete purchase order base on ID
-- =============================================
CREATE PROCEDURE [dbo].[sp_DeletePurchaseOrder]
	-- Add the parameters for the stored procedure here
	@PurchaseOrderID NVARCHAR(4000)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;
	
	IF OBJECT_ID('tempdb.dbo.#TempPurchaseOrderLine') IS NOT NULL
		DROP TABLE #TempPurchaseOrderLine; 
	IF OBJECT_ID('tempdb.dbo.#TempReservationEntry') IS NOT NULL
		DROP TABLE #TempReservationEntry;
	IF OBJECT_ID('tempdb.dbo.#TempReservationForecastEntry') IS NOT NULL
		DROP TABLE #TempReservationForecastEntry;
	IF OBJECT_ID('tempdb.dbo.#TempPurchaseOrderGroupLines') IS NOT NULL
		DROP TABLE #TempPurchaseOrderGroupLines;

    -- Insert statements for procedure here
	BEGIN TRAN DeletePurchaseOrder
	BEGIN TRY
		
		SELECT 
			[POL].* INTO #TempPurchaseOrderLine
		FROM
			PurchaseOrderLine (NOLOCK) [POL]
		WHERE
			PurchaseOrderID = @PurchaseOrderID 
		
		--- GET Reservation Entries
		SELECT
			[RESER].* INTO #TempReservationEntry
		FROM
			ReservationEntry (NOLOCK) [RESER]
		WHERE
			PurchaseOrderLineID IN (SELECT ID FROM #TempPurchaseOrderLine)

		--- GET Reservation Forecast Entries
		SELECT
			[RFC].* INTO #TempReservationForecastEntry
		FROM
			ReservationForecastEntry [RFC] (NOLOCK)
		WHERE
			PurchaseOrderLineID IN (SELECT ID From #TempPurchaseOrderLine)

		--- UPDATE production BOM
		UPDATE [PROBOM] SET 
			[PROBOM].[ReservedQuantity] = ISNULL([PROBOM].[ReservedQuantity], 0) - [SUMRESER].[ReservedQuantity],
			[PROBOM].[RemainQuantity] = [PROBOM].[RemainQuantity] + [SUMRESER].[ReservedQuantity]
		FROM
			ProductionBOM [PROBOM]
			JOIN 
			(
				SELECT
					ProductionBOMID,
					SUM(ReservedQuantity) AS ReservedQuantity
				FROM
					#TempReservationEntry
				GROUP BY
					ProductionBOMID
			) SUMRESER ON [PROBOM].[ID] = [SUMRESER].[ProductionBOMID]

		--- UPDATE forecast material
		UPDATE [FCM] SET 
			[FCM].[ReservedQuantity] = ISNULL([FCM].[ReservedQuantity], 0) - [SUMRESER].[ReservedQuantity],
			[FCM].[RemainQuantity] = [FCM].[RemainQuantity] + [SUMRESER].[ReservedQuantity]
		FROM
			ForecastMaterial [FCM]
			JOIN 
			(
				SELECT
					ForecastMaterialID,
					SUM(ReservedQuantity) AS ReservedQuantity
				FROM
					#TempReservationForecastEntry
				GROUP BY
					ForecastMaterialID
			) SUMRESER ON [FCM].[ID] = [SUMRESER].[ForecastMaterialID]
		
		--- UPDATE purchase request line
		UPDATE [PRL] SET 
			[PRL].[PurchasedQuantity] = ISNULL([PRL].[PurchasedQuantity], 0) - [SUMRESER].[ReservedQuantity],
			[PRL].[RemainQuantity] = [PRL].[RemainQuantity] + [SUMRESER].[ReservedQuantity]
		FROM
			PurchaseRequestLine [PRL]
			JOIN 
			(
				SELECT
					PurchaseRequestLineID,
					SUM(ReservedQuantity) AS ReservedQuantity
				FROM
					#TempReservationEntry
				GROUP BY
					PurchaseRequestLineID
			) SUMRESER ON [PRL].[ID] = [SUMRESER].[PurchaseRequestLineID]
		
		DELETE FROM ReservationEntry WHERE ID IN (SELECT ID FROM #TempReservationEntry)
		DELETE FROM ReservationForecastEntry WHERE ID IN (SELECT ID FROM #TempReservationForecastEntry)
		--- DELETE lines
		DELETE FROM PurchaseOrderLine WHERE PurchaseOrderID = @PurchaseOrderID
		--- DELETE group lines
		DELETE FROM PurchaseOrderGroupLine WHERE PurchaseOrderID = @PurchaseOrderID
		--- DELETE purchase order
		DELETE FROM PurchaseOrder WHERE ID = @PurchaseOrderID

		COMMIT TRAN DeletePurchaseOrder
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN DeletePurchaseOrder
	END CATCH

	IF OBJECT_ID('tempdb.dbo.#TempPurchaseOrderLine') IS NOT NULL
		DROP TABLE #TempPurchaseOrderLine; 
	IF OBJECT_ID('tempdb.dbo.#TempReservationEntry') IS NOT NULL
		DROP TABLE #TempReservationEntry;
	IF OBJECT_ID('tempdb.dbo.#TempReservationForecastEntry') IS NOT NULL
		DROP TABLE #TempReservationForecastEntry;
	IF OBJECT_ID('tempdb.dbo.#TempPurchaseOrderGroupLines') IS NOT NULL
		DROP TABLE #TempPurchaseOrderGroupLines;

END
GO
