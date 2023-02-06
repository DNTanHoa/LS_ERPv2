-- ================================================
-- Update purchase order to relate table
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ================================================
-- Author:		HoaDNT
-- Create date: 2022.08.22
-- Description:	Update purchase order to relate table
-- ================================================
CREATE PROCEDURE [dbo].[sp_UpdatePurchaseOrderToRelatedTable]
	@PurchaseOrderNumber NVARCHAR(4000),
	@UserName NVARCHAR(4000)
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

    -- Insert statements for procedure here
	BEGIN TRAN UpdatePOToRelatedTable

	BEGIN TRY
		
		--- GET Purchase Order line
		SELECT 
			[POL].* INTO #TempPurchaseOrderLine
		FROM 
			PurchaseOrderLine (NOLOCK) [POL]
			JOIN PurchaseOrder (NOLOCK) [PO] ON [POL].[PurchaseOrderID] = [PO].[ID]
				AND [PO].[Number] = @PurchaseOrderNumber

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

		--- UPDATE production bom
		UPDATE [PROBOM] SET 
			[PROBOM].[ReservedQuantity] = [SUMRESER].[ReservedQuantity],
			[PROBOM].[RemainQuantity] = [PROBOM].[RequiredQuantity] - [SUMRESER].[ReservedQuantity]
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
			[FCM].[ReservedQuantity] = [SUMRESER].[ReservedQuantity],
			[FCM].[RemainQuantity] = [FCM].[RequiredQuantity] - [SUMRESER].[ReservedQuantity]
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
			[PRL].[PurchasedQuantity] = [SUMRESER].[ReservedQuantity],
			[PRL].[RemainQuantity] = [PRL].[Quantity] - [SUMRESER].[ReservedQuantity]
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

		COMMIT TRAN UpdatePOToRelatedTable
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN UpdatePOToRelatedTable
	END CATCH

	IF OBJECT_ID('tempdb.dbo.#TempPurchaseOrderLine') IS NOT NULL
		DROP TABLE #TempPurchaseOrderLine;
	IF OBJECT_ID('tempdb.dbo.#TempReservationEntry') IS NOT NULL
		DROP TABLE #TempReservationEntry;
	IF OBJECT_ID('tempdb.dbo.#TempReservationForecastEntry') IS NOT NULL
		DROP TABLE #TempReservationForecastEntry;
END
GO
