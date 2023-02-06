/****** Object:  Trigger [dbo].[trgIssued_UpdateQuantityMaterialTransaction]  ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		DAT.NGUYEN
-- Create date: 2022.12.21
-- Description:	Trigger update quantity material transaction, storage onhand, roll, lotnumber, dye lotnumber
-- update for fabric purchase order table
-- =============================================
CREATE OR ALTER TRIGGER trgIssued_UpdateQuantityMaterialTransaction ON IssuedLine AFTER UPDATE AS
BEGIN
   ---- UPDATE MATERIAL TRANSACTION
   DECLARE @InsertdQuantity decimal(19,9)
   DECLARE @InsertRoll decimal(19,9)
   DECLARE @DeleteQuantity decimal(19,9)
   DECLARE @DeleteRoll decimal(19,9)
   DECLARE @StorageCode nvarchar(1000)

   SELECT @InsertdQuantity = IssuedQuantity, @InsertRoll = Roll FROM inserted
   SELECT @DeleteQuantity = IssuedQuantity, @DeleteRoll = Roll FROM deleted
   SELECT @StorageCode = StorageCode FROM Issued [ISD] JOIN deleted ON [ISD].Number = deleted.IssuedNumber

   IF(@InsertdQuantity <= @DeleteQuantity AND @StorageCode = 'FB')
   BEGIN TRY
   BEGIN TRAN UPDATE_ISSUED_QUANTITY
	   --- UPDATE MATERIAL TRANSACTION
	   UPDATE MaterialTransaction SET 
	   Quantity = -(SELECT IssuedQuantity FROM inserted WHERE MaterialTransaction.IssuedLineID = inserted.ID),
	   Roll = -(SELECT Roll FROM inserted WHERE MaterialTransaction.IssuedLineID = inserted.ID)
	   FROM MaterialTransaction 
	   JOIN deleted ON MaterialTransaction.IssuedLineID = deleted.ID

	   --- UPDATE ISSUED GROUP LINE
	   UPDATE IssuedGroupLine SET 
	   IssuedQuantity = (SELECT IssuedQuantity FROM inserted WHERE IssuedGroupLine.ID = inserted.IssuedGroupLineID),
	   Roll = (SELECT Roll FROM inserted WHERE IssuedGroupLine.ID = inserted.IssuedGroupLineID)
	   FROM IssuedGroupLine 
	   JOIN deleted ON IssuedGroupLine.ID = deleted.IssuedGroupLineID

	   ------
	   DECLARE @StorageDetailID bigint
	   DECLARE @FabricPurchaseOrderNumber nvarchar(1000)
	   DECLARE @OnHandQuantity decimal(19,9)
	   DECLARE @IssuedQuantity decimal(19,9)
	   DECLARE @FabricRequestDetailID bigint

	   SELECT @StorageDetailID = MaterialTransaction.StorageDetailID,
			  @FabricPurchaseOrderNumber = MaterialTransaction.FabricPurchaseOrderNumber,
			  @FabricRequestDetailID = deleted.FabricRequestDetailID
	   FROM MaterialTransaction JOIN deleted ON MaterialTransaction.IssuedLineID = deleted.ID;

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
								 GROUP BY MaterialTransaction.StorageDetailID)

		WHERE StorageDetail.ID = @StorageDetailID

		----- UPDATE FABRIC PURCHASE ORDER
		IF(@FabricPurchaseOrderNumber IS NOT NULL AND @FabricPurchaseOrderNumber <> '')
		BEGIN
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
			  IssuedQuantity = @IssuedQuantity
		  WHERE Number = @FabricPurchaseOrderNumber
		END

		----- UPDATE FABRIC REQUEST
		IF(@FabricRequestDetailID IS NOT NULL AND @FabricRequestDetailID > 0)
		BEGIN
		 DECLARE @FabricRequestID bigint
		 DECLARE @TotalQuantity decimal(19,9)

		 --- update fabric request detail
		 UPDATE FabricRequestDetail
		 SET IssuedQuantity = (SELECT SUM(IssuedQuantity)
							   FROM IssuedLine
							   WHERE FabricRequestDetailID = @FabricRequestDetailID
							   GROUP BY FabricRequestDetailID)

         WHERE ID = @FabricRequestDetailID

		 --- get fabric request ID
		 SELECT @FabricRequestID = FabricRequestID
		 FROM FabricRequestDetail
		 WHERE ID = @FabricRequestDetailID

		 --- calculator total quantity of fabric request
		 SET @TotalQuantity = (SELECT SUM(RequestQuantity) + SUM(SemiFinishedProductQuantity) + SUM(StreakRequestQuantity)
									  - SUM(IssuedQuantity)
								 FROM FabricRequestDetail
								 WHERE FabricRequestID = @FabricRequestID
								 GROUP BY FabricRequestID)
		  IF(@TotalQuantity < 1)
		  BEGIN			
			  UPDATE FabricRequest
			  SET StatusID = 'I'
			  WHERE ID = @FabricRequestID
		  END
		  ELSE
		  BEGIN		  
			  UPDATE FabricRequest
			  SET StatusID = 'Issuing'
			  WHERE ID = @FabricRequestID
		  END

		END
		COMMIT TRAN UPDATE_ISSUED_QUANTITY
   END TRY
   BEGIN CATCH
		ROLLBACK TRAN UPDATE_ISSUED_QUANTITY
   END CATCH
END
GO