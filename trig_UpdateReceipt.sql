
/****** Object:  Trigger [dbo].[trg_UpdateReceipt]    Script Date: 12/28/2022 10:22:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		DAT.NGUYEN
-- Create date: 2022.12.21
-- Description:	Trigger update invoice number, document, invoice number no total
-- =============================================
CREATE OR ALTER   TRIGGER [dbo].[trg_UpdateReceipt] ON [dbo].[Receipt] AFTER UPDATE AS
BEGIN
   DECLARE @InvoiceNumber nvarchar(1000)
   DECLARE @StorageCode nvarchar(1000)
   DECLARE @InvoiceNumberNoTotal nvarchar(1000)
   DECLARE @DocumentReferenceNumber nvarchar(1000)
   DECLARE @StorageDetailID bigint

   SELECT @InvoiceNumber = inserted.InvoiceNumber,
		  @InvoiceNumberNoTotal = inserted.InvoiceNumberNoTotal,
		  @DocumentReferenceNumber = inserted.DocumentReferenceNumber,
		  @StorageDetailID = MaterialTransaction.StorageDetailID,
		  @StorageCode = inserted.StorageCode
   FROM inserted JOIN MaterialTransaction ON MaterialTransaction.ReceiptNumber = inserted.Number

   IF(@StorageCode = 'FB')
   BEGIN
   ---- UPDATE MATERIAL TRANSACTION
   UPDATE MaterialTransaction SET 
   MaterialTransaction.InvoiceNumber = @InvoiceNumber,
   MaterialTransaction.InvoiceNumberNoTotal = @InvoiceNumberNoTotal,
   MaterialTransaction.DocumentNumber = @DocumentReferenceNumber
   FROM MaterialTransaction 
   JOIN inserted ON MaterialTransaction.ReceiptNumber = inserted.Number

   ---- UPDATE STORAGE DETAIL
   UPDATE StorageDetail SET 
   StorageDetail.InvoiceNumber = @InvoiceNumber,
   StorageDetail.InvoiceNumberNoTotal = @InvoiceNumberNoTotal,
   StorageDetail.DocumentNumber = @DocumentReferenceNumber
   FROM StorageDetail
   JOIN MaterialTransaction  ON MaterialTransaction.StorageDetailID = StorageDetail.ID
   JOIN inserted ON MaterialTransaction.ReceiptNumber = inserted.Number
   --WHERE StorageDetail.ID = @StorageDetailID

   END
END
