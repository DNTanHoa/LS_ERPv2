SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		NGUYEN PHUC DAT
-- Create date: 11-25-2022
-- Description:	View get item receipt details
-- =============================================
CREATE VIEW  ViewReceiptDetails
AS
    SELECT R.PurchaseOrderNumber, R.ArrivedDate, R.InvoiceNumber, R.InvoiceNumberNoTotal,R.CustomerID, R.Number, R.StorageCode,
		   RGL.ItemID, RGL.ItemColorName, RGL.ReceiptQuantity
	FROM Receipt R
		 JOIN ReceiptGroupLine RGL ON R.Number = RGL.ReceiptNumber
	GROUP BY R.PurchaseOrderNumber, R.ArrivedDate, R.InvoiceNumber, R.InvoiceNumberNoTotal, R.CustomerID, R.Number, R.StorageCode,
		   RGL.ItemID, RGL.ItemColorName, RGL.ReceiptQuantity

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		NGUYEN PHUC DAT
-- Create date: 11-25-2022
-- Description:	View get item receipt details
-- =============================================
 
 CREATE VIEW ViewPurchaseReceived	AS
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