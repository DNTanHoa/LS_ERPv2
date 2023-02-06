UPDATE MaterialTransaction
SET Supplier = (SELECT VendorName FROM Receipt WHERE MaterialTransaction.ReceiptNumber = Receipt.Number)