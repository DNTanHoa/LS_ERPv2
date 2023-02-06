-- =============================================
-- Author:		Dương Nguyễn Tấn Hòa
-- Create date: 2023-01-04
-- Description:	Get information from packing list for print shu
-- EXEC sp_SelectPackingListForPrintShu 15814
-- =============================================
CREATE OR ALTER   PROCEDURE [dbo].[sp_SelectPackingListForPrintShu]
	-- Add the parameters for the stored procedure here
	@PackingListID INT 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;

	SELECT
	    REPLACE(NEWID(),'-','') AS BarCode,
		PL.Size,
		STYLE.LSStyle,
		STYLE.PurchaseOrderNumber AS OrderNumber,
		STYLE.CustomerStyle,
		CAST(PL.TotalCarton AS INT) AS TotalCarton,
		CAST(PL.QuantityPerCarton AS INT) AS PCB,
		CAST(PL.QuantitySize AS INT) AS QuantitySize,
		CAST(PL.PackagesPerBox AS INT) AS UE,
		IB.BarCode AS ItemCode
	FROM
		PackingLine PL (NOLOCK)
		JOIN PackingList P (NOLOCK) ON PL.PackingListID = P.ID
			AND PackingListID = @PackingListID
			AND PL.TotalCarton > 0
		JOIN ItemStylePackingList (NOLOCK) ISP ON P.ID = ISP.PackingListsID
		JOIN ItemStyle STYLE (NOLOCK) ON ISP.ItemStylesNumber = STYLE.Number
		JOIN ItemStyleBarCode IB (NOLOCK) ON IB.ItemStyleNumber = STyLE.Number
			AND PL.Size = IB.Size
END
