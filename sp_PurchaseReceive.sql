/****** Object:  StoredProcedure [dbo].[sp_SelectPurchaseReceived]    Script Date: 11/25/2022 2:09:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		NGUYEN PHUC DAT
-- Create date: 10-17-2022
-- Description:	Get item purchase order line
-- =============================================
CREATE OR ALTER   PROCEDURE [dbo].[sp_SelectPurchaseReceived] 
	-- Add the parameters for the stored procedure here
	@CustomerID NVARCHAR(4000) = '',
	@StorageCode NVARCHAR(4000) = '',
	@FromDate DATETIME,
	@ToDate DATETIME
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;
	SELECT 
	    [SUB].CustomerPurchaseOrderNumber, 
		[SUB].ContractNo, 
		[SUB].ItemID,
		[SUB].ItemColorName,
		[SUB].MaterialTypeClass,
		[SUB].MaterialType,
		[SUB].CustomerStyle,
		[SUB].ItemNo,
		[SUB].Quantity,
		[SUB].TrxDate,
		[SUB].DeliveryNo,
		[SUB].ShipmentID,
		[SUB].InvoiceNumber,
		[SUB].Number,
		[SUB].ArrivedDate,
		[SUB].CustomerID,
		[SUB].StorageCode
	FROM 
	(
		SELECT  DISTINCT
			PO.CustomerPurchaseOrderNumber, 
			PO.ContractNo, 
			PO.ItemID,
			PO.ItemColorName,
			PO.MaterialTypeClass,
			PO.MaterialType,
			PO.CustomerStyle,
			PO.ItemNo,
			PO.Quantity,
			SD.TrxDate,
			SD.DeliveryNo,
			SD.ShipmentID,
			VRD.InvoiceNumber,
			VRD.Number,
			VRD.ArrivedDate,
			VRD.CustomerID,
			VRD.StorageCode

		FROM [dbo].[PurchaseOrderLine] PO
			 JOIN [ShipmentDetail] SD ON PO.CustomerPurchaseOrderNumber = SD.CustomerPurchaseOrderNumber 
									 AND PO.ContractNo = SD.ContractNo
									 AND PO.ItemID = SD.MaterialCode
									 AND PO.ItemColorName = SD.Color
			 LEFT JOIN [dbo].[ViewReceiptDetails]  VRD ON SD.CustomerPurchaseOrderNumber = VRD.PurchaseOrderNumber
															AND SD.MaterialCode = VRD.ItemID
															AND SD.Color = VRD.ItemColorName
	) [SUB]
  
  WHERE 
	--[SUB].[CustomerID] = @CustomerID AND 
	--[SUB].[StorageCode] = @StorageCode AND
	[SUB].[MaterialType] = @StorageCode AND 
	--([SUB].[ArrivedDate] >= @FromDate OR @FromDate IS NULL) AND 
	--([SUB].[ArrivedDate] <= @ToDate OR @ToDate IS NULL)
	([SUB].TrxDate >= @FromDate OR @FromDate IS NULL) AND 
	([SUB].TrxDate <= @ToDate OR @ToDate IS NULL)

END