DECLARE @Username NVARCHAR(4000) = 'admin'

--- Init Order Type
INSERT INTO [dbo].[SalesOrderType] ([Code],[Name],[CreatedBy],[LastUpdatedBy],[CreatedAt],[LastUpdatedAt])
     VALUES ('0', '0', @Username,NULL,GETDATE(),NULL)
INSERT INTO [dbo].[SalesOrderType] ([Code],[Name],[CreatedBy],[LastUpdatedBy],[CreatedAt],[LastUpdatedAt])
     VALUES ('1', '1', @Username,NULL,GETDATE(),NULL)
INSERT INTO [dbo].[SalesOrderType] ([Code],[Name],[CreatedBy],[LastUpdatedBy],[CreatedAt],[LastUpdatedAt])
     VALUES ('2', '2', @Username,NULL,GETDATE(),NULL)
INSERT INTO [dbo].[SalesOrderType] ([Code],[Name],[CreatedBy],[LastUpdatedBy],[CreatedAt],[LastUpdatedAt])
     VALUES ('3', '3', @Username,NULL,GETDATE(),NULL)

--- Init Order Status
INSERT INTO [dbo].[SalesOrderStatus] ([Code],[Name],[CreatedBy],[LastUpdatedBy],[CreatedAt],[LastUpdatedAt])
     VALUES ('N/A', NULL, @Username,NULL,GETDATE(),NULL)
INSERT INTO [dbo].[SalesOrderStatus] ([Code],[Name],[CreatedBy],[LastUpdatedBy],[CreatedAt],[LastUpdatedAt])
     VALUES ('Order', 'Order', @Username,NULL,GETDATE(),NULL)
INSERT INTO [dbo].[SalesOrderStatus] ([Code],[Name],[CreatedBy],[LastUpdatedBy],[CreatedAt],[LastUpdatedAt])
     VALUES ('Confirm', 'Confirm', @Username,NULL,GETDATE(),NULL)

--- Migrate Payment Term
INSERT INTO [dbo].[PaymentTerm]
           ([Code]
           ,[Description]
           ,[DueDays]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt])
	SELECT 
		 ROW_NUMBER() OVER (ORDER BY [PAY].[Description])
		,[PAY].[Description]
		,[PAY].[DueDays]
		,@Username
		,NULL
		,GETDATE()
		,NULL
	FROM
		[LS_ERPv1.5].[dbo].[PaymentTerm] (NOLOCK) [PAY]

--- Migrate price term
INSERT INTO [dbo].[PriceTerm]
           ([Code]
           ,[Description]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt])
	SELECT 
		 [PRICE].[Description]
		,[PRICE].[Description]
		,@Username
		,NULL
		,GETDATE()
		,NULL
	FROM
		[LS_ERPv1.5].[dbo].[PriceTerms] (NOLOCK) [PRICE]

--- Migrate currency
INSERT INTO [dbo].[Currency]
           ([ID]
           ,[Description]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt])
  SELECT [CurrencyID]
        ,[Description]
		,@Username
		,NULL
		,GETDATE()
		,NULL
  FROM [LS_ERPv1.5].[dbo].[CurrencyTable]

--- Migrate division
INSERT INTO [dbo].[Division]
           ([ID]
           ,[Name]
           ,[Description]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt])
	SELECT 
		 [Description]
		,[Description]
		,[Description]
		,@Username
		,NULL
		,GETDATE()
		,NULL
	FROM [LS_ERPv1.5].[dbo].[DivisionTable]

--- Migrate customer
INSERT INTO [dbo].[Customer]
           ([ID]
           ,[Name]
           ,[Description]
           ,[PaymentTermCode]
           ,[PriceTermCode]
           ,[DivisionID]
           ,[CurrencyID]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt])
	SELECT 
		 [CUST].[CustID]
		,[CUST].[CustName]
		,[CUST].[Description]
		,[CONVERTPAY].[Code]
		,[CUST].[PriceTerm]
		,[CUST].[Division]
		,[CUST].[Currency]
		,@Username
		,NULL
		,GETDATE()
		,NULL
	FROM
		[LS_ERPv1.5].[dbo].[Customer] (NOLOCK) [CUST]
		LEFT JOIN [LS_ERPv1.5].[dbo].[PaymentTerm] (NOLOCK) [PAY] ON [PAY].[Description] = [CUST].[PayTerm]
		LEFT JOIN [dbo].[PaymentTerm] [CONVERTPAY] (NOLOCK) ON [PAY].[Description] = [CONVERTPAY].[Description]

--- Migrate size
INSERT INTO [dbo].[Size]
           ([CustomerID]
           ,[SequeneceNumber]
           ,[Code]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt])
     SELECT
		   [CustID]
		  ,[Sequence]
		  ,[Code]
		  ,@UserName
		  ,NULL
		  ,GETDATE()
		  ,NULL
	  FROM [LS_ERPv1.5].[dbo].[ItemSize]

--- Migrate ItemModel
INSERT INTO [dbo].[ItemModel]
           ([Style]
           ,[GarmentColorCode]
           ,[GarmentColorName]
           ,[GarmentSize]
           ,[UPC]
           ,[CustomerID]
           ,[FileName]
           ,[SaveFilePath]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt]
           ,[ContractNo]
           ,[DeptSubFineline]
           ,[FixtureCode]
           ,[MSRP]
           ,[Mfg]
           ,[ModelName]
           ,[ReplCode]
           ,[Season]
           ,[SuppPlt]
           ,[TagSticker])
	SELECT 
		   [Style]
		  ,[Color]
		  ,[ColorName]
		  ,[Size]
		  ,[UPC]
		  ,[Customer]
		  ,[FileName]
		  ,NULL
		  ,@UserName
		  ,NULL
		  ,GETDATE()
		  ,NULL
		  ,[ContractNo]
		  ,[DeptSubFineline]
		  ,[FixtureCode]
		  ,[MSRP]
		  ,[Mfg]
		  ,[ModelName]
		  ,[ReplCode]
		  ,[SeasonCode]
		  ,[SupPlt]
		  ,[TagSticker]
	  FROM 
		[LS_ERPv1.5].[dbo].[ItemModel] [IM]

--- Migrate brand
INSERT INTO [dbo].[Brand]
           ([Code]
           ,[CustomerID]
           ,[Name]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt])
	SELECT 
		 [BRAND].[Code]
		,[BRAND].[CustID]
		,[BRAND].[BranchName]
		,@Username
		,NULL
		,GETDATE()
		,NULL
	FROM
		[LS_ERPv1.5].[dbo].[Branch] (NOLOCK) [BRAND]

--- Migrate sale order
INSERT INTO [dbo].[SalesOrders]
           ([ID]
           ,[CustomerID]
           ,[CustomerName]
           ,[BrandCode]
           ,[OrderDate]
           ,[ConfirmDate]
           ,[DivisionID]
           ,[PaymentTermCode]
           ,[PaymentTermDescription]
           ,[PriceTermCode]
           ,[PriceTermDescription]
           ,[CurrencyID]
           ,[Year]
           ,[SalesOrderStatusCode]
           ,[SaveFilePath]
           ,[FileName]
           ,[SalesOrderTypeCode]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt])
     SELECT
		 [SO].[OrderID]
		,[SO].[CustID]
		,[CUST].[CustName]
		,[SO].[Branch]
		,[SO].[OrderDate]
		,[SO].[OConfirmedDate]
		,[SO].[Division]
		,[CONVERTPAY].[Code]
		,[CONVERTPAY].[Description]
		,[SO].[PriceTerm]
		,[PRICE].[Description]
		,[SO].[Currency]
		,CASE WHEN [SO].[Year] = 0 OR [SO].[Year] IS NULL THEN YEAR([SO].[OrderDate])
			ELSE [SO].[Year] END
		,[ORDERSTATUS].[Code]
		,NULL
		,[SO].[FileName]
		,[ORDERTYPE].[Code]
		,[CREATEUSER].[UserName]
		,[UPDATEUSER].[UserName]
		,[SO].[CreatedDate]
		,[SO].[UpdatedDate]
	 FROM
		[LS_ERPv1.5].[dbo].[SalesOrder] (NOLOCK) [SO]
		LEFT JOIN [LS_ERPv1.5].[dbo].[Customer] (NOLOCK) [CUST] ON [SO].[CustID] = [CUST].[CustID]
		LEFT JOIN [LS_ERPv1.5].[dbo].[Branch] (NOLOCK) [BRAND] ON [BRAND].[Code] = [SO].[Branch]
		LEFT JOIN [LS_ERPv1.5].[dbo].[PaymentTerm] (NOLOCK) [PAY] ON [PAY].[Description] = [SO].[PayTerm]
		LEFT JOIN [dbo].[PaymentTerm] [CONVERTPAY] (NOLOCK) ON [PAY].[Description] = [CONVERTPAY].[Description]
		LEFT JOIN [LS_ERPv1.5].[dbo].[PriceTerms] (NOLOCK) [PRICE] ON [PRICE].[Description] = [SO].[PriceTerm]
		LEFT JOIN [LS_ERPv1.5].[dbo].[Person] (NOLOCK) [CREATE] ON [CREATE].[ID] = [SO].[Person]
		LEFT JOIN [LS_ERPv1.5].[dbo].[PermissionPolicyUser] (NOLOCK) [CREATEUSER] ON [CREATE].[User] = [CREATEUSER].[Oid]
		LEFT JOIN [LS_ERPv1.5].[dbo].[Person] (NOLOCK) [UPDATE] ON [UPDATE].[ID] = [SO].[PersonUpdated]
		LEFT JOIN [LS_ERPv1.5].[dbo].[PermissionPolicyUser] (NOLOCK) [UPDATEUSER] ON [UPDATE].[User] = [UPDATEUSER].[Oid]
			 JOIN [dbo].[SalesOrderStatus] (NOLOCK) [ORDERSTATUS] ON [ORDERSTATUS].[Name] = [SO].[OrderStatus]
			 JOIN [dbo].[SalesOrderType] (NOLOCK) [ORDERTYPE] ON [ORDERTYPE].[Name] = CAST([SO].[OrderType] AS NVARCHAR(100))

    --- Update order detail
	UPDATE [OD]
		SET SizeSortIndex = [SIZE].[SequeneceNumber]
	FROM 
		OrderDetail [OD]
		LEFT JOIN ItemStyle [STYLE] ON [STYLE].[Number] = [OD].[ItemStyleNumber]
		LEFT JOIN SalesOrders [SO] on [SO].[ID] = [STYLE].[SalesOrderID]
		LEFT JOIN Size [SIZE] ON [OD].[Size] = [Size].[Code]
				AND [SIZE].[CustomerID] <> 'PU'
				AND [SIZE].[CustomerID] = [SO].[CustomerID]
