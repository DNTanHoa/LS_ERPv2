-- ================================================
-- Update production bom quantity
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Hoa DNT
-- Create date: 2022-12-22
-- Description:	Cập nhật thông tin reserved quantity
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_UpdateProdutionBomQuantityBySalesOrder]
	-- Add the parameters for the stored procedure here
	@SaleOrderID NVARCHAR(1000)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE Bom
		Set Bom.RemainQuantity = Bom.RequiredQuantity - sub.ReservedQuantity,
			Bom.ReservedQuantity = sub.ReservedQuantity
	FROM 
		ProductionBOM [Bom]
		JOIN
		(
			select
				ProductionBOMID,
				SUM(ReservedQuantity)  AS ReservedQuantity
			from
				ReservationEntry
			where 
				ProductionBOMID IN (select ID from ProductionBOM (nolock) where ItemStyleNumber IN (select Number from ItemStyle (nolock) where SalesOrderID = @SaleOrderID))
			group by
				ProductionBOMID
		) sub ON sub.ProductionBOMID = [Bom].[ID]
				AND [Bom].[ItemStyleNumber] IN (select Number from ItemStyle (nolock) where SalesOrderID = @SaleOrderID)

END
GO