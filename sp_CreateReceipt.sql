-- ================================================
-- Process for receipt
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Hoa.DNT
-- Create date: 2022.12.15
-- Description:	Create transactions for receipt, update purchase order infor of receipt,... etc
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_CreateReceipt]
	-- Add the parameters for the stored procedure here
	@ReceiptNumber NVARCHAR(1000),
	@UserName NVARCHAR(1000)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;

	BEGIN TRAN CreateReceipt

	--- Handle
	BEGIN TRY
		--- Create Transaction
		INSERT INTO [dbo].[MaterialTransaction]
           ([ItemID]
           ,[ItemName]
           ,[ItemColorCode]
           ,[ItemColorName]
           ,[Position]
           ,[Specify]
           ,[Season]
           ,[UnitID]
           ,[Quantity]
           ,[TransactionDate]
           ,[ReceiptNumber]
           ,[ReceiptGroupLineID]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt]
           ,[GarmentColorCode]
           ,[GarmentColorName]
           ,[GarmentSize]
           ,[CustomerStyle]
           ,[ItemCode]
           ,[LSStyle]
           ,[StorageCode]
           ,[Roll]
           ,[PurchaseOrderNumber]
           ,[StorageBinCode]
           ,[LotNumber]
           ,[InvoiceNumber]
           ,[Remark]
           ,[ItemMasterID]
           ,[OtherName]
           ,[DefaultThreadName])
		SELECT 
			 [RGL].[ItemID]
			,[RGL].[ItemName]
			,[RGL].[ItemColorCode]
			,[RGL].[ItemColorName]
			,[RGL].[Position]
			,[RGL].[Specify]
			,[RGL].[Season]
			,[RGL].[UnitID]
			,[RGL].[ReceiptQuantity]
			,GETDATE() -- Transaction Date
			,[RGL].[ReceiptNumber]
			,[RGL].[ID] -- Receipt groupline ID
			,@UserName -- CreatedBy
			,@UserName -- LastUpdatedBy
			,GETDATE() -- CreatedAt
			,GETDATE() -- LastUpdatedAt
			,[RGL].[GarmentColorCode]
			,[RGL].[GarmentColorName]
			,[RGL].[GarmentSize]
			,[RGL].[CustomerStyle]
			,[RGL].[ItemCode]
			,[RGL].[LSStyle]
			,[R].[StorageCode]
			,[RGL].[Roll]
			,[RGL].[PurchaseOrderNumber]
			,[RGL].[StorageBinCode]
			,[RGL].[LotNumber]
			,[R].[InvoiceNumber]
			,[RGL].[Remark]
			,[RGL].[ItemMasterID]
			,[RGL].[DefaultThreadName]
			,[RGL].[OtherName]
	 FROM
		ReceiptGroupLine [RGL] 
		JOIN Receipt [R] ON [RGL].[ReceiptNumber] = [R].[Number]
			AND [R].[Number] = @ReceiptNumber

		--- Update purchase order information data
		UPDATE [PGL]
			SET [PGL].ReceiptQuantity += sub.ReceiptQuantity
		FROM
			PurchaseOrderGroupLine [PGL]
			JOIN 
			(
				SELECT
					PurchaseOrderGroupLineID,
					SUM(ReceiptQuantity) AS ReceiptQuantity
				FROM
					ReceiptGroupLine [RGL]
				WHERE
					ReceiptNumber = @ReceiptNumber
				GROUP BY
					PurchaseOrderGroupLineID
			) sub ON sub.PurchaseOrderGroupLineID = [PGL].[ID]

		COMMIT TRAN CreateReceipt
	END TRY
	--- Rollback on error
	BEGIN CATCH
		ROLLBACK TRAN CreateReceipt
	END CATCH

END
GO
