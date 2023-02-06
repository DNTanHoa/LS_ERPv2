USE [ERPv2]
GO

DECLARE @UserName NVARCHAR(400) = 'admin'

--- Migrate supplier CNUF
INSERT INTO [dbo].[SupplierCNUF]
           ([Code]
           ,[Name]
           ,[SiteName]
           ,[DPPOfice]
           ,[SiteCountry]
           ,[ProcessLeader]
           ,[CustomerID]
           ,[FileName]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt])
     SELECT 
	   [CNUF]
      ,[Name]
      ,[SiteName]
      ,[DPPOffice]
      ,[SiteCountry]
      ,[ProcessLeader]
      ,[Customer]
      ,[FileName]
	  ,@UserName
	  ,NULL
	  ,GETDATE()
	  ,NULL
  FROM 
    [LS_ERPv1.5].[dbo].[SupplierCNUF]
  WHERE  
    [LS_ERPv1.5].[dbo].[SupplierCNUF].[CNUF] IS NOT NULL

--- Migrate shipping term
INSERT INTO [dbo].[ShippingTerm]
           ([Code]
           ,[Description]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt])
	SELECT 
		 [Description]
		,[Description]
		,@UserName
		,NULL
		,GETDATE()
		,NULL
	FROM [LS_ERPv1.5].[dbo].[ShippingTerms]

--- Migrate Tax
INSERT INTO [dbo].[Tax]
           ([Code]
           ,[Description]
           ,[Value]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt])
	SELECT [Code]
		  ,[Description]
		  ,[Value]
		  ,@UserName
		  ,NULL
		  ,GETDATE()
		  ,NULL
	FROM [LS_ERPv1.5].[dbo].[TaxCode]

--- Migrate Shipping Method
INSERT INTO [dbo].[ShippingMethod]
           ([Code]
           ,[Name]
           ,[LeadTime]
           ,[Price]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt])
	SELECT 
		 [Description]
		,[Description]
		,NULL
		,NULL
		,@UserName
		,NULL
		,GETDATE()
		,NULL
	FROM [LS_ERPv1.5].[dbo].[ShippingMethod]

--- Migrate company
INSERT INTO [dbo].[Company]
           ([Code]
           ,[Name]
           ,[DisplayName]
           ,[Phone]
           ,[DisplayPhone]
           ,[Email]
           ,[DisplayEmail]
           ,[FaxNumber]
           ,[DisplayFaxNumber]
           ,[Address]
           ,[DisplayAddress])
	SELECT [Code]
		  ,[Name]
		  ,NULL
		  ,[Phone]
		  ,[PhoneEN]
		  ,[Email]
		  ,NULL
		  ,[FaxNumber]
		  ,[FaxNumberEN]
		  ,[Address]
		  ,[AddressEN]
	  FROM [LS_ERPv1.5].[dbo].[Company]

--- Migrate incoterm
INSERT INTO [dbo].[IncoTerm]
           ([Code]
           ,[Name])
     SELECT
		 [ID]
		,[Name]
	 FROM
		[LS_ERPv1.5].[dbo].[Incoterm]

--- Init Currency Exchange Type
INSERT INTO [dbo].[CurrencyExchangeType]
           ([ID], [Name], [CurrencyID], [DestinationCurrencyID], [CreatedBy], [LastUpdatedBy], [CreatedAt], [LastUpdatedAt])
     VALUES
           ('USD/VND', 'USD/VND', 'USD' ,'VND',@UserName,NULL,GETDATE(),NULL)

INSERT INTO [dbo].[CurrencyExchangeType]
           ([ID], [Name], [CurrencyID], [DestinationCurrencyID], [CreatedBy], [LastUpdatedBy], [CreatedAt], [LastUpdatedAt])
     VALUES
           ('VND/USD', 'VND/USD', 'VND' ,'USD',@UserName,NULL,GETDATE(),NULL)

--- Migrate purchase order
INSERT INTO [dbo].[PurchaseOrder]
           ([ID]
           ,[Number]
           ,[OrderDate]
           ,[EstimateShipDate]
           ,[VendorConfirmedDate]
           ,[ShipDate]
           ,[CustomerID]
           ,[InvoiceNo]
           ,[Reason]
           ,[PaymentTermCode]
           ,[PurchaseOrderStatusCode]
           ,[Description]
           ,[Remark]
           ,[CompanyCode]
           ,[VendorID]
           ,[SupplierCNUFCode]
           ,[CurrencyExchangeTypeID]
           ,[CurrencyID]
           ,[CurrencyExchangeValue]
           ,[Discount]
           ,[IsIncludedTax]
           ,[TaxCode]
           ,[Percentage]
           ,[ShipTo]
		   ,[IncoTermCode]
           ,[ShippingMethodCode]
           ,[ShippingTermCode]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt])
	SELECT [PO].[OrderID]
		  ,[PO].[NewOrderID]
		  ,[PO].[OrderDate]
		  ,NULL
		  ,[PO].[VendorConfirmDate]
		  ,[PO].[ShipmentDate]
		  ,[PO].[CustID]
		  ,[PO].[InvoiceNo]
		  ,[PO].[Reason]
		  ,[PAY].[Code]
		  ,[POSTATUS].[Code]
		  ,[PO].[Description]
		  ,[PO].[Remark]
		  ,[PO].[Company]
		  ,[PO].[VendID]
		  ,[NEWCNUF].[Code]
		  ,CASE WHEN [PO].[USDVND] = 1 THEN 'USD/VND'
				WHEN [PO].[VNDUSD] = 1 THEN 'VND/USD'
				ELSE NULL END
		  ,[PO].[CurrencyID]
		  ,[PO].[ExchangeRate]
		  ,[PO].[Discount]
		  ,[PO].[IncludedTax]
		  ,[PO].[TaxID]
		  ,[PO].[Percentage]
		  ,[PO].[ShipTo]
		  ,[PO].[Incoterm]
		  ,[SHIP].[Code]
		  ,[PO].[ShippingTerm]
		  ,[CREATEUSER].[UserName]
		  ,[CREATEUSER].[UserName]
		  ,[PO].[OrderDate]
		  ,[PO].[OrderDate]
	  FROM 
		[LS_ERPv1.5].[dbo].[PurchOrder] [PO]
		LEFT JOIN [PaymentTerm] [PAY] ON [PAY].[Description] = [PO].[PaymentTerm]
		LEFT JOIN [Company] [CPNY] ON [CPNY].[Code] = [PO].[Company]
		LEFT JOIN [LS_ERPv1.5].[dbo].[SupplierCNUF] [OLDCNUF] ON [OLDCNUF].[ID] = [PO].[SupplierCnufs]
		LEFT JOIN [SupplierCNUF] [NEWCNUF] ON [NEWCNUF].[Code] = [OLDCNUF].[CNUF]
		LEFT JOIN [LS_ERPv1.5].[dbo].[Person] (NOLOCK) [CREATE] ON [CREATE].[ID] = [PO].[Person]
		LEFT JOIN [LS_ERPv1.5].[dbo].[PermissionPolicyUser] (NOLOCK) [CREATEUSER] ON [CREATE].[User] = [CREATEUSER].[Oid]
		LEFT JOIN [PurchaseOrderStatus] [POSTATUS] (NOLOCK) ON [POSTATUS].[Code] = CAST([PO].[Status] AS NVARCHAR(400))
		LEFT JOIN [ShippingMethod] [SHIP] (NOLOCK) ON [SHIP].[Code] = [PO].[Shipment]

--- Delete test data
DELETE FROM PurchaseOrder WHERE ID IN 
(
	'qqq_test',
	'qqq_test_w32',
	'qqq_test2',
	'qqq_test4',
	'test2'
)

--- Migrate purchase order line

SET IDENTITY_INSERT [dbo].[PurchaseOrderLine] ON

INSERT INTO [dbo].[PurchaseOrderLine]
           ([ID]
		   ,[PurchaseOrderID]
           ,[Remarks]
           ,[Price]
           ,[UnitID]
           ,[SecondUnitID]
           ,[ReservedQuantity]
           ,[ReservedForecastQuantity]
           ,[Quantity]
           ,[QuantityPerUnit]
           ,[FreePercent]
           ,[FreePercentQuantity]
           ,[WareHouseQuantity]
           ,[MSRP]
           ,[CanReusedQuantity]
           ,[UPC]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt]
           ,[CustomerStyle]
           ,[Division]
           ,[GarmentColorCode]
           ,[GarmentColorName]
           ,[GarmentSize]
           ,[ItemColorCode]
           ,[ItemColorName]
           ,[ItemID]
           ,[ItemName]
           ,[Label]
           ,[Mfg]
           ,[Position]
           ,[Season]
           ,[Specify]
		   ,[LSStyle]
		   ,[SalesOrderID]
           ,[ContractNo]
           ,[DeptSubFineline]
           ,[FixtureCode]
           ,[ModelName]
           ,[ReplCode]
           ,[SuppPlt]
           ,[TagSticker])
	SELECT 
	       [POLine].[ID]
		  ,[POLine].[OrderID]
		  ,[POLine].[Remarks]
		  ,[POLine].[Price]
		  ,[POLine].[UnitID]
		  ,[POLine].[SecondUnit]
		  ,[POLine].[ReservedQty]
		  ,[POLine].[ReservedFCQty]
		  ,[POLine].[Qty]
		  ,[POLine].[QtyPerUnit]
		  ,[POLine].[FreePercent]
		  ,[POLine].[FreePercentQty]
		  ,[POLine].[WareHouseQty]
		  ,[POLine].[MSRP]
		  ,[POLine].[QtyV]
		  ,[POLine].[UPC]
		  ,@UserName
		  ,NULL
		  ,GETDATE()
		  ,NULL
		  ,CASE WHEN [POLine].[CustomerStyle] IS NOT NULL THEN [POLine].[CustomerStyle]
			ELSE NULL END
		  ,[POLine].[Division]
		  ,[POLine].[ColorGarment]
		  ,[POLine].[ColorNameGarment]
		  ,CASE WHEN [POLine].[ProBom] IS NOT NULL THEN [ProBom].[GarmentSize]
				WHEN [POLine].[ForecastMtl] IS NOT NULL THEN [FCMTL].[GarmentSize]
				ELSE NULL END
		  ,[POLine].[Color]
		  ,[POLine].[GridValue]
		  ,[POLine].[ItemID]
		  ,[POLine].[ItemName]
		  ,[POLine].[Label]
		  ,[POLine].[Mfg]
		  ,[POLine].[Position]
		  ,[POLine].[Season]
		  ,[POLine].[Specify]
		  ,[POLine].[LSStyle]
		  ,[POLine].[SalesOrderID]
          ,[POLine].[ContractNo]
          ,[POLine].[DeptSubFineline]
          ,[POLine].[FixtureCode]
          ,[POLine].[ModelName]
          ,[POLine].[ReplCode]
          ,[POLine].[SuppPlt]
          ,[POLine].[TagSticker]
	  FROM [LS_ERPv1.5].[dbo].[PurchOrderLine] [POLine]
		   LEFT JOIN [LS_ERPv1.5].[dbo].[ProBom] [ProBom] ON [ProBom].[ID] = [POLine].[ProBom]
		   LEFT JOIn [LS_ERPv1.5].[dbo].[ForecastMtl] [FCMTL] ON [FCMTL].[ID] = [POLine].[ForecastMtl]
	  WHERE
		[POLine].[OrderID] NOT IN 
		(
			'qqq_test',
			'qqq_test_w32',
			'qqq_test2',
			'qqq_test4',
			'test2'
		)

SET IDENTITY_INSERT [dbo].[PurchaseOrderLine] OFF

--- Migrate purchase order group line
IF OBJECT_ID('tempdb.dbo.#TempPurchaseOrderGroupLine') IS NOT NULL
		DROP TABLE #TempPurchaseOrderGroupLine;

--- Get group line data
SELECT
	 [POLine].[PurchaseOrderID]
    ,[POLine].[ItemID]
    ,[POLine].[ItemName]
    ,[POLine].[ItemColorCode]
    ,[POLine].[ItemColorName]
    ,[POLine].[Position]
    ,[POLine].[Mfg]
    ,[POLine].[UPC]
    ,[POLine].[Division]
    ,[POLine].[Label]
    ,[POLine].[Specify]
    ,[POLine].[Remarks]
    ,[POLine].[Price]
    ,[POLine].[UnitID]
    ,[POLine].[CustomerStyle]
    ,[POLine].[GarmentColorCode]
    ,[POLine].[GarmentColorName]
    ,[POLine].[GarmentSize]
    ,[POLine].[Season]
    ,[POLine].[CreatedBy]
    ,[POLine].[LastUpdatedBy]
    ,[POLine].[CreatedAt]
    ,[POLine].[LastUpdatedAt]
    ,[POLine].[ContractNo]
	,SUM([POLine].[Quantity]) AS Quantity
	,ROW_NUMBER() OVER (ORDER BY [POLine].[PurchaseOrderID]) AS RowKey INTO #TempPurchaseOrderGroupLine
FROM
	[dbo].[PurchaseOrderLine] [POLine] (NOLOCK)
GROUP BY
	 [POLine].[PurchaseOrderID]
    ,[POLine].[ItemID]
    ,[POLine].[ItemName]
    ,[POLine].[ItemColorCode]
    ,[POLine].[ItemColorName]
    ,[POLine].[Position]
    ,[POLine].[Mfg]
    ,[POLine].[UPC]
    ,[POLine].[Division]
    ,[POLine].[Label]
    ,[POLine].[Specify]
    ,[POLine].[Remarks]
    ,[POLine].[Price]
    ,[POLine].[UnitID]
    ,[POLine].[CustomerStyle]
    ,[POLine].[GarmentColorCode]
    ,[POLine].[GarmentColorName]
    ,[POLine].[GarmentSize]
    ,[POLine].[Season]
	,[POLine].[CreatedBy]
    ,[POLine].[LastUpdatedBy]
    ,[POLine].[CreatedAt]
    ,[POLine].[LastUpdatedAt]
    ,[POLine].[ContractNo]

SET IDENTITY_INSERT [dbo].[PurchaseOrderGroupLine] ON

--- Insert group line
INSERT INTO [dbo].[PurchaseOrderGroupLine]
           ([ID]
		   ,[PurchaseOrderID]
           ,[ItemID]
           ,[ItemName]
           ,[ItemColorCode]
           ,[ItemColorName]
           ,[Position]
           ,[Mfg]
           ,[UPC]
           ,[Division]
           ,[Label]
           ,[Specify]
           ,[Remarks]
           ,[Price]
           ,[UnitID]
           ,[CustomerStyle]
           ,[GarmentColorCode]
           ,[GarmentColorName]
           ,[GarmentSize]
           ,[ContractNo]
           ,[Season]
		   ,[Quantity]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt])
     SELECT
			[POGroupLine].[RowKey]
		   ,[POGroupLine].[PurchaseOrderID]
           ,[POGroupLine].[ItemID]
           ,[POGroupLine].[ItemName]
           ,[POGroupLine].[ItemColorCode]
           ,[POGroupLine].[ItemColorName]
           ,[POGroupLine].[Position]
           ,[POGroupLine].[Mfg]
           ,[POGroupLine].[UPC]
           ,[POGroupLine].[Division]
           ,[POGroupLine].[Label]
           ,[POGroupLine].[Specify]
           ,[POGroupLine].[Remarks]
           ,[POGroupLine].[Price]
           ,[POGroupLine].[UnitID]
           ,[POGroupLine].[CustomerStyle]
           ,[POGroupLine].[GarmentColorCode]
           ,[POGroupLine].[GarmentColorName]
           ,[POGroupLine].[GarmentSize]
           ,[POGroupLine].[ContractNo]
           ,[POGroupLine].[Season]
		   ,[POGroupLine].[Quantity]
           ,[POGroupLine].[CreatedBy]
           ,[POGroupLine].[LastUpdatedBy]
           ,[POGroupLine].[CreatedAt]
           ,[POGroupLine].[LastUpdatedAt]
	 FROM
		#TempPurchaseOrderGroupLine [POGroupLine]

SET IDENTITY_INSERT [dbo].[PurchaseOrderGroupLine] OFF

UPDATE [POLine]
	SET [POLine].[PurchaseOrderGroupLineID] = [POGroupLine].[RowKey]
FROM
	[dbo].[PurchaseOrderLine] [POLine]
	JOIN #TempPurchaseOrderGroupLine [POGroupLine] ON [POLine].[PurchaseOrderID] = [POGroupLine].[PurchaseOrderID]
		AND ([POLine].[ItemID] = [POGroupLine].[ItemID] OR [POLine].[ItemID] IS NULL) 
		AND ([POLine].[ItemName] = [POGroupLine].[ItemName] OR [POLine].[ItemName] IS NULL) 
		AND ([POLine].[ItemColorCode] = [POGroupLine].[ItemColorCode] OR [POLine].[ItemColorCode] IS NULL) 
		AND ([POLine].[ItemColorName] = [POGroupLine].[ItemColorName] OR [POLine].[ItemColorName] IS NULL) 
		AND ([POLine].[Position] = [POGroupLine].[Position] OR [POLine].[Position] IS NULL) 
		AND ([POLine].[Mfg] = [POGroupLine].[Mfg] OR [POLine].[Mfg] IS NULL) 
		AND ([POLine].[UPC] = [POGroupLine].[UPC] OR [POLine].[UPC] IS NULL) 
		AND ([POLine].[Division] = [POGroupLine].[Division] OR [POLine].[Division] IS NULL) 
		AND ([POLine].[Label] = [POGroupLine].[Label] OR [POLine].[Label] IS NULL) 
		AND ([POLine].[Specify] = [POGroupLine].[Specify] OR [POLine].[Specify] IS NULL) 
		AND ([POLine].[Remarks] = [POGroupLine].[Remarks] OR [POLine].[Remarks] IS NULL) 
		AND ([POLine].[Price] = [POGroupLine].[Price] OR [POLine].[Price] IS NULL) 
		AND ([POLine].[UnitID] = [POGroupLine].[UnitID] OR [POLine].[UnitID] IS NULL) 
		AND ([POLine].[CustomerStyle] = [POGroupLine].[CustomerStyle] OR [POLine].[CustomerStyle] IS NULL) 
		AND ([POLine].[GarmentColorCode] = [POGroupLine].[GarmentColorCode] OR [POLine].[GarmentColorCode] IS NULL) 
		AND ([POLine].[GarmentColorName] = [POGroupLine].[GarmentColorName] OR [POLine].[GarmentColorName] IS NULL) 
		AND ([POLine].[GarmentSize] = [POGroupLine].[GarmentSize] OR [POLine].[GarmentSize] IS NULL) 
		AND ([POLine].[Season] = [POGroupLine].[Season] OR [POLine].[Season] IS NULL) 
		AND ([POLine].[CreatedBy] = [POGroupLine].[CreatedBy] OR [POLine].[CreatedBy] IS NULL) 
		AND ([POLine].[LastUpdatedBy] = [POGroupLine].[LastUpdatedBy] OR [POLine].[LastUpdatedBy] IS NULL) 
		AND ([POLine].[CreatedAt] = [POGroupLine].[CreatedAt] OR [POLine].[CreatedAt] IS NULL) 
		AND ([POLine].[LastUpdatedAt] = [POGroupLine].[LastUpdatedAt] OR [POLine].[LastUpdatedAt] IS NULL)
        AND ([POLine].[ContractNo] = [POGroupLine].[ContractNo] OR [POLine].[ContractNo] IS NULL)
	

IF OBJECT_ID('tempdb.dbo.#TempPurchaseOrderGroupLine') IS NOT NULL
		DROP TABLE #TempPurchaseOrderGroupLine;

UPDATE [POLine]
SET [POLine].SalesOrderID = [IS].[SalesOrderID]
FROM PurchaseOrderLine [POLine]
	 JOIN ItemStyle [IS] ON [IS].[LSStyle] = [POLine].[LSStyle]

GO


