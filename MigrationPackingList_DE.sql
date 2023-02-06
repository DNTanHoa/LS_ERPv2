/****** Script for SelectTopNRows command from SSMS  ******/

--- Update order detail
	UPDATE [OD]
		SET [OD].SizeSortIndex = [SIZE].[SequeneceNumber]
	FROM 
		OrderDetail [OD]
		JOIN ItemStyle [STYLE] ON [STYLE].[Number] = [OD].[ItemStyleNumber]
		JOIN SalesOrders [SO] on [SO].[ID] = [STYLE].[SalesOrderID]
		JOIN Size [SIZE] ON [OD].[Size] = [Size].[Code]
				AND [SIZE].[CustomerID] = 'DE'
				AND [SIZE].[CustomerID] = [SO].[CustomerID]
GO
	UPDATE StyleNetWeight
	SET CustomerID = 'PU'
GO
	UPDATE StyleNetWeight
	SET CustomerID = 'DE'
	WHERE CustomerStyle IN ('125474','131921','300614','303903','304627','304635','304853'
							,'305038','305056','306450','309088','310009','311074','311508'
							,'311518','311815')
GO
	UPDATE ItemStyleBarCode
	SET Size = REPLACE(Size, 'years/',' YEARS / ')
GO
	UPDATE ItemStyleBarCode
	SET Size = REPLACE(Size, '  ',' ')
GO
--- UPDATE QUANTITY BARCODE
	UPDATE [BC]
		SET [BC].Quantity = [OD].Quantity
	FROM 
		ItemStyleBarCode [BC]
		LEFT JOIN ItemStyle [STYLE] ON [STYLE].[Number] = [BC].[ItemStyleNumber]
		LEFT JOIN SalesOrders [SO] on [SO].[ID] = [STYLE].[SalesOrderID]
		JOIN OrderDetail [OD] ON [BC].[Size] = [OD].[Size]
				AND [SO].[CustomerID] = 'DE'


