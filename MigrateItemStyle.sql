DECLARE @Username NVARCHAR(4000) = 'admin'

--- Init Purchase Order Type
INSERT INTO [dbo].[PurchaseOrderType]([Code],[Name],[CreatedBy],[LastUpdatedBy],[CreatedAt],[LastUpdatedAt])
     SELECT 
		 ROW_NUMBER() OVER (ORDER BY [POType] DESC)
		,[sub].[POType]
		,@Username
		,NULL
		,GETDATE()
		,NULL
	 FROM 
	 (
		SELECT DISTINCT
			[ItemStyle].[POType]
		FROM
			[LS_ERPv1.5].[dbo].[ItemStyle] [ItemStyle] (NOLOCK)
	 ) sub

--- Init Order Status
INSERT INTO [dbo].[PurchaseOrderStatus]([Code],[Name],[CreatedBy],[LastUpdatedBy],[CreatedAt],[LastUpdatedAt])
     SELECT 
		 ROW_NUMBER() OVER (ORDER BY [POStatus] DESC)
		,[sub].[POStatus]
		,@Username
		,NULL
		,GETDATE()
		,NULL
	 FROM 
	 (
		SELECT DISTINCT
			[ItemStyle].[POStatus]
		FROM
			[LS_ERPv1.5].[dbo].[ItemStyle] [ItemStyle] (NOLOCK)
	 ) sub

--- Init ItemStyle Status
INSERT INTO [dbo].[ItemStyleStatus] ([Code],[Name],[CreatedBy],[LastUpdatedBy],[CreatedAt],[LastUpdatedAt])
     VALUES ('1', 'New', @Username,NULL,GETDATE(),NULL)
INSERT INTO [dbo].[ItemStyleStatus] ([Code],[Name],[CreatedBy],[LastUpdatedBy],[CreatedAt],[LastUpdatedAt])
     VALUES ('2', 'Update', @Username,NULL,GETDATE(),NULL)
INSERT INTO [dbo].[ItemStyleStatus] ([Code],[Name],[CreatedBy],[LastUpdatedBy],[CreatedAt],[LastUpdatedAt])
     VALUES ('3', 'Cancel', @Username,NULL,GETDATE(),NULL)
INSERT INTO [dbo].[ItemStyleStatus] ([Code],[Name],[CreatedBy],[LastUpdatedBy],[CreatedAt],[LastUpdatedAt])
     VALUES ('4', 'Submitted', @Username,NULL,GETDATE(),NULL)

--- Migrate ItemStyle
INSERT INTO [dbo].[ItemStyle]
           ([Number]
           ,[UE]
           ,[CustomerStyle]
           ,[LSStyle]
           ,[ColorName]
           ,[Description]
           ,[ShipTo]
           ,[PurchaseOrderNumber]
           ,[PurchaseOrderDate]
           ,[PurchaseOrderTypeCode]
           ,[PurchaseOrderStatusCode]
           ,[SalesOrderID]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt]
           ,[Brand]
           ,[ColorCode]
           ,[ContractDate]
           ,[ContractNo]
           ,[DeliveryDate]
           ,[DeliveryPlace]
           ,[Division]
           ,[EstimatedSupplierHandOver]
           ,[FabricContent]
           ,[Gender]
           ,[Image]
           ,[IsBomPulled]
           ,[IsCalculateRequiredQuantity]
           ,[IsConfirmed]
           ,[IsEnoughQuantity]
           ,[IsNew]
           ,[IsQuantityBalanced]
           ,[ItemStyleStatusCode]
           ,[LabelCode]
           ,[LabelName]
           ,[MSRP]
           ,[OldShipDate]
           ,[OldTotalQuantity]
           ,[PCB]
           ,[PIC]
           ,[Packing]
           ,[ProductionDescription]
           ,[Season]
           ,[ShipDate]
           ,[ShipMode]
           ,[TotalQuantity])
     SELECT
		 [ITEMSTYLE].[StyleNumber]
		,[ITEMSTYLE].[UE]
		,[ITEMSTYLE].[CustomerStyle]
		,[ITEMSTYLE].[LSStyle]
		,[ITEMSTYLE].[ItemColorName]
		,[ITEMSTYLE].[Description]
		,[ITEMSTYLE].[ShipTo]
		,[ITEMSTYLE].[OrderPO]
		,[ITEMSTYLE].[PODate]
		,[CONVERTPOTYPE].[Code]
		,[CONVERTPOSTATUS].[Code]
		,[ITEMSTYLE].[OrderID]
		,[CREATEUSER].[UserName]
		,[UPDATEUSER].[UserName]
		,[ITEMSTYLE].[CreatedDate]
		,[ITEMSTYLE].[UpdatedDate]
		,[ITEMSTYLE].[Branch]
		,[ITEMSTYLE].[ItemColor]
		,[ITEMSTYLE].[ContractDate]
        ,[ITEMSTYLE].[ContractNo]
        ,[ITEMSTYLE].[DeliveryDate]
        ,[ITEMSTYLE].[DeliveryPlace]
        ,[ITEMSTYLE].[Division]
		,[ITEMSTYLE].[EstimatedSupplierHandOver]
		,[ITEMSTYLE].[FabricContent]
        ,[ITEMSTYLE].[Gender]
        ,NULL 
        ,[ITEMSTYLE].[BomPulled]
        ,[ITEMSTYLE].[QtyCalculated]
        ,[ITEMSTYLE].[Confirm]
        ,ISNULL([ITEMSTYLE].[EnoughQty],0)
        ,[ITEMSTYLE].[IsNew]
        ,[ITEMSTYLE].[QtyBalanced]
		,[CONVERTITEMSTYLESTATUS].[Code]
		,[ITEMSTYLE].[Label]
        ,[ITEMSTYLE].[LabelName]
        ,[ITEMSTYLE].[MSRP]
        ,[ITEMSTYLE].[OldShipDate]
        ,CAST([ITEMSTYLE].[OldTotal] AS DECIMAL(19,4))
        ,[ITEMSTYLE].[PCB]
        ,[ITEMSTYLE].[PIC]
        ,[ITEMSTYLE].[Packaging2]
        ,[ITEMSTYLE].[FullDescription]
        ,[ITEMSTYLE].[Season]
        ,[ITEMSTYLE].[ShipDate]
        ,[ITEMSTYLE].[ShipMode]
        ,CAST([ITEMSTYLE].[OldTotal] AS DECIMAL(19,4))
	 FROM
		[LS_ERPv1.5].[dbo].[ItemStyle] [ITEMSTYLE] (NOLOCK)
		LEFT JOIN [LS_ERPv1.5].[dbo].[SalesOrder] (NOLOCK) [SO] ON [SO].[OrderID] = [ITEMSTYLE].[OrderID]
		LEFT JOIN [LS_ERPv1.5].[dbo].[Person] (NOLOCK) [CREATE] ON [CREATE].[ID] = [SO].[Person]
		LEFT JOIN [LS_ERPv1.5].[dbo].[PermissionPolicyUser] (NOLOCK) [CREATEUSER] ON [CREATE].[User] = [CREATEUSER].[Oid]
		LEFT JOIN [LS_ERPv1.5].[dbo].[Person] (NOLOCK) [UPDATE] ON [UPDATE].[ID] = [SO].[PersonUpdated]
		LEFT JOIN [LS_ERPv1.5].[dbo].[PermissionPolicyUser] (NOLOCK) [UPDATEUSER] ON [UPDATE].[User] = [UPDATEUSER].[Oid]
		LEFT JOIN [PurchaseOrderStatus] [CONVERTPOSTATUS] (NOLOCK) ON [CONVERTPOSTATUS].[Name] = [ITEMSTYLE].[POStatus]
		LEFT JOIN [PurchaseOrderType] [CONVERTPOTYPE] (NOLOCK) ON [CONVERTPOTYPE].[Name] = [ITEMSTYLE].[POType]
		LEFT JOIN [ItemStyleStatus] [CONVERTITEMSTYLESTATUS] (NOLOCK) ON [CONVERTITEMSTYLESTATUS].[Code] = CAST([ITEMSTYLE].[Status] AS NVARCHAR(100))
	WHERE 
		[ITEMSTYLE].[OrderID] IN (SELECT ID FROM SalesOrders)
           
--- Migrate OrderDtetail
INSERT INTO [dbo].[OrderDetail]
           ([ItemStyleNumber]
           ,[Size]
           ,[Quantity]
           ,[ReservedQuantity]
           ,[OldQuantity]
		   ,[ConsumedQuantity])
	SELECT
		 [ORDERDTL].[ItemStyle]
		,[ORDERDTL].[Size]
		,[ORDERDTL].[Qty]
		,[ORDERDTL].[ReservedQty]
		,[ORDERDTL].[OldQty]
		,[ORDERDTL].[ConsumeQty]
	FROM
		[LS_ERPv1.5].[dbo].[OrderDtl] [ORDERDTL] (NOLOCK)
	WHERE ([ORDERDTL].[ItemStyle] IN (SELECT Number FROM ItemStyle))

--- Migrate ItemStyleBarcode
INSERT INTO [dbo].[ItemStyleBarCode]
           ([ItemStyleNumber]
           ,[BarCode]
           ,[Color]
           ,[Size]
           ,[UE]
           ,[PCB]
           ,[Packing]
           ,[Quantity]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt])
	SELECT
		    [BARCODE].[StyleNumber]
           ,[BARCODE].[BarCode]
           ,[BARCODE].[Color]
           ,[BARCODE].[Size]
           ,[BARCODE].[UE]
           ,[BARCODE].[PCB]
           ,[BARCODE].[Packaging]
           ,[BARCODE].[Qty]
           ,@Username
           ,NULL
           ,GETDATE()
           ,NULL
	FROM
		[LS_ERPv1.5].[dbo].[StyleBarcode] [BARCODE] (NOLOCK)


