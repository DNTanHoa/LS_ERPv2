
/****** Object:  Trigger [dbo].[trg_UpdateQuantityMaterialTransaction]    Script Date: 12/26/2022 1:22:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		DAT.NGUYEN
-- Create date: 2022.12.21
-- Description:	Trigger update quantity material transaction, storage onhand, roll
-- =============================================
ALTER   TRIGGER [dbo].[trg_UpdateQuantityMaterialTransaction] ON [dbo].[ReceiptGroupLine] AFTER UPDATE AS
BEGIN
	DECLARE @StorageCode nvarchar(500)
	SELECT @StorageCode = StorageCode FROM Receipt JOIN inserted ON Receipt.Number = inserted.ReceiptNumber

	IF(@StorageCode = 'FB')
	BEGIN
		   ---- UPDATE MATERIAL TRANSACTION
		   UPDATE MaterialTransaction SET 
		   Quantity = (SELECT ReceiptQuantity FROM inserted WHERE MaterialTransaction.ReceiptGroupLineID = inserted.ID),
		   Roll = (SELECT Roll FROM inserted WHERE MaterialTransaction.ReceiptGroupLineID = inserted.ID),
		   LotNumber = (SELECT LotNumber FROM inserted WHERE MaterialTransaction.ReceiptGroupLineID = inserted.ID),
		   DyeLotNumber = (SELECT DyeLotNumber FROM inserted WHERE MaterialTransaction.ReceiptGroupLineID = inserted.ID)
		   FROM MaterialTransaction 
		   JOIN deleted ON MaterialTransaction.ReceiptGroupLineID = deleted.ID

		   ------
		   DECLARE @StorageDetailID bigint
		   DECLARE @LotNumber nvarchar(500)
		   DECLARE @DyeLotNumber nvarchar(500)
		   DECLARE @FabricPurchaseOrderNumber nvarchar(1000)
		   DECLARE @OnHandQuantity decimal(19,9)
		   DECLARE @ReceivedQuantity decimal(19,9)
		   DECLARE @IssuedQuantity decimal(19,9)

		   SELECT @StorageDetailID = MaterialTransaction.StorageDetailID,
				  @LotNumber = MaterialTransaction.LotNumber,
				  @DyeLotNumber = MaterialTransaction.DyeLotNumber,
				  @FabricPurchaseOrderNumber = MaterialTransaction.FabricPurchaseOrderNumber

		   FROM MaterialTransaction JOIN deleted ON MaterialTransaction.ReceiptGroupLineID = deleted.ID;

		   --- SET ONHAND FOR STORAGE DETAIL, GROUP STORAGE DETAIL ID
		   SET @OnHandQuantity = (SELECT SUM(MaterialTransaction.Quantity) AS OnHandQuantity
									 FROM MaterialTransaction
									 WHERE MaterialTransaction.StorageDetailID = @StorageDetailID							
									 GROUP BY MaterialTransaction.StorageDetailID)

		   --- UPDATE STORAGE DETAIL
		   UPDATE StorageDetail
			SET OnHandQuantity = @OnHandQuantity,
				CanUseQuantity = @OnHandQuantity,
				Roll = (SELECT SUM(MaterialTransaction.Roll) AS Roll
									 FROM MaterialTransaction
									 WHERE MaterialTransaction.StorageDetailID = @StorageDetailID							
									 GROUP BY MaterialTransaction.StorageDetailID),
				LotNumber = @LotNumber,
				DyeLotNumber = @DyeLotNumber

			WHERE StorageDetail.ID = @StorageDetailID

			----- UPDATE FABRIC PURCHASE ORDER
			IF(@FabricPurchaseOrderNumber IS NOT NULL AND @FabricPurchaseOrderNumber <> '')
			BEGIN
			 SET @ReceivedQuantity = (SELECT SUM(MaterialTransaction.Quantity) AS OnHandQuantity
									 FROM MaterialTransaction
									 WHERE MaterialTransaction.FabricPurchaseOrderNumber = @FabricPurchaseOrderNumber
									 AND Quantity > 0
									 GROUP BY MaterialTransaction.FabricPurchaseOrderNumber)

			SET @OnHandQuantity = (SELECT SUM(MaterialTransaction.Quantity) AS OnHandQuantity
									 FROM MaterialTransaction
									 WHERE MaterialTransaction.FabricPurchaseOrderNumber = @FabricPurchaseOrderNumber
									 GROUP BY MaterialTransaction.FabricPurchaseOrderNumber)

			SET @IssuedQuantity = Abs((SELECT SUM(MaterialTransaction.Quantity) AS OnHandQuantity
									 FROM MaterialTransaction
									 WHERE MaterialTransaction.FabricPurchaseOrderNumber = @FabricPurchaseOrderNumber
									 AND Quantity < 0
									 GROUP BY MaterialTransaction.FabricPurchaseOrderNumber))
			  UPDATE FabricPurchaseOrder
			  SET OnHandQuantity = @OnHandQuantity,
				  ReceivedQuantity = @ReceivedQuantity,
				  IssuedQuantity = @IssuedQuantity
			  WHERE Number = @FabricPurchaseOrderNumber
			END
	END
END
