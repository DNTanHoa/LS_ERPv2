USE [ERPv2_Production]
GO
    -- Drop temp table here
	-- Xóa các bảng tạm liên quan
	IF OBJECT_ID('tempdb.dbo.#TempItemStyle') IS NOT NULL
		DROP TABLE #TempItemStyle;

	IF OBJECT_ID('tempdb.dbo.#TempItemStyleDim') IS NOT NULL
		DROP TABLE #TempItemStyleDim;

	IF OBJECT_ID('tempdb.dbo.#TempItemSizeName') IS NOT NULL
		DROP TABLE #TempItemSizeName;

	IF OBJECT_ID('tempdb.dbo.#TempItemSizeCode') IS NOT NULL
		DROP TABLE #TempItemSizeCode;

	IF OBJECT_ID('tempdb.dbo.#TempItemSizeQuantity') IS NOT NULL
		DROP TABLE #TempItemSizeQuantity;

	IF OBJECT_ID('dbo.[ItemStyleConvertv1]') IS NOT NULL
		DROP TABLE [ItemStyleConvertv1];

--- create table convert itemstyle
CREATE TABLE [dbo].[ItemStyleConvertv1](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[StyleNumber] [nvarchar](30) NULL,
	[LSStyle] [nvarchar](100) NULL,
	[CustomerStyle] [nvarchar](50) NULL,
	[SizeGroup] [nvarchar](100) NULL,
	[GamentType] [nvarchar](20) NULL,
	[Country] [nvarchar](20) NULL,
	[TeamCode] [nvarchar](50) NULL,
	[Description] [nvarchar](100) NULL,
	[FullDescription] [nvarchar](100) NULL,
	[DataAreaID] [nvarchar](20) NULL,
	[QtyUnit] [nvarchar](20) NULL,
	[OrderID] [nvarchar](25) NULL,
	[Price] [float] NULL,
	[Quantity] [float] NULL,
	[SizeColumns] [nvarchar](255) NULL,
	[Image] [image] NULL,
	[Season] [nvarchar](20) NULL,
	[OrderPO] [nvarchar](50) NULL,
	[DeliveryPlace] [nvarchar](250) NULL,
	[DeliveryDate] [datetime] NULL,
	[ContractNo] [nvarchar](50) NULL,
	[ContractDate] [datetime] NULL,
	[DeActive] [bit] NULL,
	[Branch] [nvarchar](50) NULL,
	[PODate] [datetime] NULL,
	[POType] [nvarchar](50) NULL,
	[PIC] [nvarchar](50) NULL,
	[PCB] [nvarchar](200) NULL,
	[UE] [nvarchar](200) NULL,
	[Packaging] [nvarchar](200) NULL,
	[POStatus] [nvarchar](50) NULL,
	[ShipMode] [nvarchar](50) NULL,
	[CustCode] [nvarchar](50) NULL,
	[UCustCode] [nvarchar](50) NULL,
	[CustCoNo] [nvarchar](50) NULL,
	[UCustCoNo] [nvarchar](50) NULL,
	[Label] [nvarchar](50) NULL,
	[ProductionDate] [datetime] NULL,
	[Planned] [bit] NULL,
	[Division] [nvarchar](50) NULL,
	[RefCutting] [nvarchar](50) NULL,
	[HangFlat] [nvarchar](50) NULL,
	[FreightType] [nvarchar](50) NULL,
	[MSRP] [float] NULL,
	[ShipDate] [datetime] NULL,
	[CreateDate] [datetime] NULL,
	[ProfitPercent] [float] NULL,
	[ConfirmedPrice] [float] NULL,
	[UpCharge] [float] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

----- INSERT DATA FROM V1 TO V2 OF TABLE [ItemStyleConvertv1]
INSERT INTO [dbo].[ItemStyleConvertv1]
           ([StyleNumber]
           ,[LSStyle]
           ,[CustomerStyle]
           ,[SizeGroup]
           ,[GamentType]
           ,[Country]
           ,[TeamCode]
           ,[Description]
           ,[FullDescription]
           ,[DataAreaID]
           ,[QtyUnit]
           ,[OrderID]
           ,[Price]
           ,[Quantity]
           ,[SizeColumns]
           ,[Image]
           ,[Season]
           ,[OrderPO]
           ,[DeliveryPlace]
           ,[DeliveryDate]
           ,[ContractNo]
           ,[ContractDate]
           ,[DeActive]
           ,[Branch]
           ,[PODate]
           ,[POType]
           ,[PIC]
           ,[PCB]
           ,[UE]
           ,[Packaging]
           ,[POStatus]
           ,[ShipMode]
           ,[CustCode]
           ,[UCustCode]
           ,[CustCoNo]
           ,[UCustCoNo]
           ,[Label]
           ,[ProductionDate]
           ,[Planned]
           ,[Division]
           ,[RefCutting]
           ,[HangFlat]
           ,[FreightType]
           ,[MSRP]
           ,[ShipDate]
           ,[CreateDate]
           ,[ProfitPercent]
           ,[ConfirmedPrice]
           ,[UpCharge])
     SELECT
           [StyleNumber]
           ,[LSStyle]
           ,[CustomerStyle]
           ,[SizeGroup]
           ,[GamentType]
           ,[Country]
           ,[TeamCode]
           ,[Description]
           ,[FullDescription]
           ,[DataAreaID]
           ,[QtyUnit]
           ,[OrderID]
           ,[Price]
           ,[Quantity]
           ,[SizeColumns]
           ,[Image]
           ,[Season]
           ,[OrderPO]
           ,[DeliveryPlace]
           ,[DeliveryDate]
           ,[ContractNo]
           ,[ContractDate]
           ,[DeActive]
           ,[Branch]
           ,[PODate]
           ,[POType]
           ,[PIC]
           ,[PCB]
           ,[UE]
           ,[Packaging]
           ,[POStatus]
           ,[ShipMode]
           ,[CustCode]
           ,[UCustCode]
           ,[CustCoNo]
           ,[UCustCoNo]
           ,[Label]
           ,[ProductionDate]
           ,[Planned]
           ,[Division]
           ,[RefCutting]
           ,[HangFlat]
           ,[FreightType]
           ,[MSRP]
           ,[ShipDate]
           ,[CreateDate]
           ,[ProfitPercent]
           ,[ConfirmedPrice]
           ,[UpCharge]
	FROM [LSERP].[dbo].[ItemStyle]
	WHERE OrderID IN (SELECT ORDERID FROM [LSERP].[dbo].SalesOrder WHERE CustID = 'PU')
GO
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
		 CONCAT([SO].[OrderID],'_PU')
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
		,[SO].UserBlock
		,[SO].UserBlock
		,GETDATE()
		,GETDATE()
	 FROM
		[LSERP].[dbo].[SalesOrder] (NOLOCK) [SO]
		LEFT JOIN [LS_ERPv1.5].[dbo].[Customer] (NOLOCK) [CUST] ON [SO].[CustID] = [CUST].[CustID]
		LEFT JOIN [LS_ERPv1.5].[dbo].[Branch] (NOLOCK) [BRAND] ON [BRAND].[Code] = [SO].[Branch]
		LEFT JOIN [LSERP].[dbo].[PaymentTerm] (NOLOCK) [PAY] ON [PAY].[PayID] = [SO].[PayTerm]
		LEFT JOIN [dbo].[PaymentTerm] [CONVERTPAY] (NOLOCK) ON [PAY].[Description] = [CONVERTPAY].[Description]
		LEFT JOIN [LS_ERPv1.5].[dbo].[PriceTerms] (NOLOCK) [PRICE] ON [PRICE].[Description] = [SO].[PriceTerm]
			 JOIN [dbo].[SalesOrderStatus] (NOLOCK) [ORDERSTATUS] ON [ORDERSTATUS].[Name] = [SO].[OrderStatus]
			 JOIN [dbo].[SalesOrderType] (NOLOCK) [ORDERTYPE] ON [ORDERTYPE].[Name] = CAST([SO].[OrderType] AS NVARCHAR(100))
	WHERE SO.CustID = 'PU'
GO
	--- update Payment term Sales order PU
	UPDATE [dbo].[SalesOrders]
	SET PaymentTermCode = (SELECT Code FROM [dbo].PaymentTerm WHERE Description = 'LC at sight')
	WHERE PaymentTermCode is null AND CustomerID = 'PU'
GO
	UPDATE [dbo].[SalesOrders]
	SET [PaymentTermDescription] = (SELECT Description FROM [dbo].PaymentTerm WHERE Description = 'LC at sight')
	WHERE [PaymentTermDescription] is null AND CustomerID = 'PU'
GO
	--- Migrate ItemStyle
INSERT INTO [dbo].[ItemStyle]
           ([Number]
           ,[UE]
           ,[CustomerStyle]
           ,[LSStyle]
           ,[ColorName]
           ,[Description]
           ,[PurchaseOrderNumber]
           ,[PurchaseOrderDate]
           ,[SalesOrderID]
           ,[CreatedAt]
           ,[LastUpdatedAt]
           ,[Brand]
           ,[ColorCode]
           ,[ContractDate]
           ,[ContractNo]
           ,[DeliveryDate]
           ,[DeliveryPlace]
           ,[Division]
           ,[Image]
           ,[MSRP]
           ,[PCB]
           ,[PIC]
           ,[ProductionDescription]
           ,[Season]
           ,[ShipDate]
           ,[ShipMode]
           ,[TotalQuantity]
		   ,CustomerCode
		   ,CustomerCodeNo
		   ,UCustomerCode
		   ,UCustomerCodeNo)
     SELECT
		 [ITEMSTYLE].[StyleNumber]
		,[ITEMSTYLE].[UE]
		,[ITEMSTYLE].[CustomerStyle]
		,[ITEMSTYLE].[LSStyle]
		,[ItemStyleDim].ColorName
		,[ITEMSTYLE].[Description]
		,[ITEMSTYLE].[OrderPO]
		,[ITEMSTYLE].[PODate]
		,CONCAT([ITEMSTYLE].[OrderID], '_PU')
		,GETDATE()
		,GETDATE()
		,[ITEMSTYLE].[Branch]
		,[ItemStyleDim].Color
		,[ITEMSTYLE].[ContractDate]
        ,[ITEMSTYLE].[ContractNo]
        ,[ITEMSTYLE].[DeliveryDate]
        ,[ITEMSTYLE].[DeliveryPlace]
        ,[ITEMSTYLE].[Division]
        ,NULL 
        ,[ITEMSTYLE].[MSRP]
        ,[ITEMSTYLE].[PCB]
        ,[ITEMSTYLE].[PIC]
        ,[ITEMSTYLE].[FullDescription]
        ,[ITEMSTYLE].[Season]
        ,[ITEMSTYLE].[ShipDate]
        ,[ITEMSTYLE].[ShipMode]
		,[ITEMSTYLE].Quantity
		,[ITEMSTYLE].CustCode
		,[ITEMSTYLE].CustCoNo
		,[ITEMSTYLE].UCustCode
		,[ITEMSTYLE].UCustCoNo
		
	 FROM
		[LSERP].[dbo].[ItemStyle] [ITEMSTYLE] (NOLOCK)
		JOIN [LSERP].[dbo].[ItemStyleDim] [ItemStyleDim] ON [ItemStyleDim].StyleID = [ITEMSTYLE].StyleNumber
		JOIN [dbo].[SalesOrders] (NOLOCK) [SO] ON [SO].[ID] = CONCAT([ITEMSTYLE].[OrderID], '_PU')
			 
	WHERE 
		[ITEMSTYLE].[OrderID] IN (SELECT SUBSTRING(ID,1, LEN(ID) - 3) AS ID FROM [dbo].SalesOrders WHERE CustomerID = 'PU')
GO
	-- lấy hết tất cả các item style
	SELECT [ID]
      ,[StyleNumber]
      ,[LSStyle]
      ,[CustomerStyle]
      ,[SizeGroup]
      ,[GamentType]
      ,[Country]
      ,[TeamCode]
      ,[Description]
      ,[FullDescription]
      ,[DataAreaID]
      ,[QtyUnit]
      ,[OrderID]
      ,[Price]
      ,[Quantity]
      ,[SizeColumns]
      ,[Image]
      ,[Season]
      ,[OrderPO]
      ,[DeliveryPlace]
      ,[DeliveryDate]
      ,[ContractNo]
      ,[ContractDate]
      ,[DeActive]
      ,[Branch]
      ,[PODate]
      ,[POType]
      ,[PIC]
      ,[PCB]
      ,[UE]
      ,[Packaging]
      ,[POStatus]
      ,[ShipMode]
      ,[CustCode]
      ,[UCustCode]
      ,[CustCoNo]
      ,[UCustCoNo]
      ,[Label]
      ,[ProductionDate]
      ,[Planned]
      ,[Division]
      ,[RefCutting]
      ,[HangFlat]
      ,[FreightType]
      ,[MSRP]
      ,[ShipDate]
      ,[CreateDate]
      ,[ProfitPercent]
      ,[ConfirmedPrice]
      ,[UpCharge]
	  INTO #TempItemStyle
    FROM [dbo].[ItemStyleConvertv1]

	---- GET ITEM STYLE DIM
	SELECT
		*
	INTO #TempItemStyleDim
	FROM
	(
		SELECT
			itemStyleDim.ID
		  ,[StyleID]
		  ,[Color]
		  ,[ColorName]
		  ,itemStyleDim.[CMPrice]
		  ,itemStyleDim.[Price]
		  ,[S1]
		  ,[S2]
		  ,[S3]
		  ,[S4]
		  ,[S5]
		  ,[S6]
		  ,[S7]
		  ,[S8]
		  ,[S9]
		  ,[S10]
		  ,[S11]
		  ,[S12]
		  ,[S13]
		  ,[S14]
		  ,[P1]
		  ,[P2]
		  ,[P3]
		  ,[P4]
		  ,[P5]
		  ,[P6]
		  ,[P7]
		  ,[P8]
		  ,[P9]
		  ,[P10]
		  ,[P11]
		  ,[P12]
		  ,[P13]
		  ,[P14]
		  ,S1 + S2 + S3 + S4 + S5 + S6 + S7 + S8 + S9 + S10 + S11 + S12 + S13 + S14 AS Total --INTO #TempItemStyleDim
	   FROM 
		   [LSERP].[dbo].[ItemStyleDim] itemStyleDim (NOLOCK)
		   LEFT JOIN [LSERP].[dbo].ItemStyle itemStyle (NOLOCK) ON itemStyleDim.StyleID = itemStyle.StyleNumber
	   WHERE 
			itemStyle.OrderID IN (SELECT OrderID FROM [LSERP].[dbo].SalesOrder  WHERE CustID = 'PU')
	) sub
	WHERE sub.ToTal > 0
----------------------------------
GO
	-- Get ItemSizeName
	SELECT
	sub1.StyleNumber AS ItemStyle,
	sub1.SizeName,
	sub1.SizeCode 
	INTO #TempItemSizeName
		FROM
		(
			SELECT
				sub.StyleNumber,
				SUBSTRING(SizeColumns, 0, CHARINDEX('$', SizeColumns)) AS SizeName,
				REVERSE(SUBSTRING(REVERSE(SizeColumns), 0, CHARINDEX('$', REVERSE(SizeColumns)))) AS SizeCode
			FROM
				(
					SELECT
						StyleNumber,
						Value AS SizeColumns
					FROM
						[dbo].ItemStyleConvertv1
						CROSS APPLY STRING_SPLIT(SizeColumns, ',')
					WHERE 
						OrderID IN (SELECT OrderID FROM [LSERP].[dbo].SalesOrder WHERE CustID = 'PU')
				) sub
		) sub1

	-------------
	-- Get ItemSizeQuantity
		SELECT  DISTINCT
		sub.StyleID AS ItemStyle,
		sub.Size AS Size,
		sub.Quantity AS Quantity ,
		sub.CMPrice AS CMPrice,
		sub.FOBPrice AS FOBPrice,
		sub.PricePerSize AS Price
		INTO #TempItemSizeQuantity
	FROM
	(
		SELECT
			StyleID,
			Size,
			Quantity,
			CMPrice,
			Price AS FOBPrice,
			PricePerSize
		FROM 
			[LSERP].[dbo].ItemStyleDim dim
			UNPIVOT
			(
				Quantity
				FOR Size in (S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14)

			) u
			UNPIVOT
			(
				PricePerSize
				FOR PriceQty in (P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14)
			) p

		--where RIGHT(Size, 1) = RIGHT(PriceQty, 1) and PricePerSize != 0 and Quantity != 0
	) sub
	WHERE
		StyleID IN 
		(
			SELECT
				StyleNumber
			FROM
				ItemStyleConvertv1
			WHERE
				OrderID IN (SELECT OrderID FROM [LSERP].[dbo].SalesOrder  WHERE CustID = 'PU')
		)
		AND Quantity > 0
GO
	-- Create OrderDtl 
	-- Tạo OrderDtl
	INSERT INTO [dbo].OrderDetail
           (ItemStyleNumber
           ,[Size]
           ,[Quantity]
		   ,[Price])
	SELECT
		sizeName.ItemStyle,
		sizeName.SizeName,
		sizeQuantity.Quantity,
		MAX(sizeQuantity.Price) AS Price
	FROM
		#TempItemSizeName sizeName
		JOIN #TempItemSizeQuantity sizeQuantity ON sizeName.ItemStyle = sizeQuantity.ItemStyle
			AND sizeName.SizeCode = sizeQuantity.Size
	GROUP BY
		sizeName.ItemStyle,
		sizeName.SizeName,
		sizeQuantity.Quantity
   -- Update order detail size sequenece number
   UPDATE [OD]
		SET [OD].SizeSortIndex = [SIZE].[SequeneceNumber]
	FROM 
		OrderDetail [OD]
		LEFT JOIN ItemStyle [STYLE] ON [STYLE].[Number] = [OD].[ItemStyleNumber]
		LEFT JOIN SalesOrders [SO] on [SO].[ID] = [STYLE].[SalesOrderID]
		LEFT JOIN Size [SIZE] ON [OD].[Size] = [Size].[Code]
				AND [SIZE].[CustomerID] = 'PU'
    WHERE [SO].[CustomerID] = 'PU'

