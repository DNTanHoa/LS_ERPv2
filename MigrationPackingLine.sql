	USE [ERPv2]
	IF OBJECT_ID('tempdb.dbo.#TempItemSizeNamePL') IS NOT NULL
		DROP TABLE #TempItemSizeNamePL;

	IF OBJECT_ID('tempdb.dbo.#TempItemSizeQuantityPL') IS NOT NULL
		DROP TABLE #TempItemSizeQuantityPL;
	DELETE PackingLine
	GO
-- Get ItemSizeName
	SELECT
	sub1.LSStyle AS ItemStyle,
	sub1.StyleNumber,
	sub1.SizeName,
	sub1.SizeCode 
	INTO #TempItemSizeNamePL
		FROM
		(
			SELECT
				sub.LSStyle,
				sub.StyleNumber,
				SUBSTRING(SizeColumns, 0, CHARINDEX('$', SizeColumns)) AS SizeName,
				REVERSE(SUBSTRING(REVERSE(SizeColumns), 0, CHARINDEX('$', REVERSE(SizeColumns)))) AS SizeCode
			FROM
				(
					SELECT
						LSStyle,
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
		sub.ItemID AS ItemStyle
		,sub.SizeQty AS Size
		,sub.Quantity AS Quantity
		,sub.[SeqNo]
		,sub.[QtySize]
		,sub.[QtyPerPackage]
		,sub.[PackagesPerBox]
		,sub.[QtyPerCTNS]
		,sub.[QtyPCS]
		,sub.[NWeight]
		,sub.[GWeight]
		,sub.[Color]
		,sub.[PrePack]
		,sub.[Description]
		,sub.[PackNo]
		,sub.[DimCode]
		,sub.[Height]
		,sub.[Width]
		,sub.[Lenght]
		,sub.[InDimCode]
		,sub.[HeightIn]
		,sub.[WidthIn]
		,sub.[LenghtIn]
		,sub.[FromNo]
		,sub.[ToNo]
		,sub.[TotalCarton]
		INTO #TempItemSizeQuantityPL
	FROM
	(
		SELECT
		  ItemID
		  ,REPLACE(SizeQty, 'Q', 'S')SizeQty
		  ,Quantity
		  ,[SeqNo]
		  ,[QtySize]
		  ,[QtyPerPackage]
		  ,[PackagesPerBox]
		  ,[QtyPerCTNS]
		  ,[QtyPCS]
		  ,[NWeight]
		  ,[GWeight]
		  ,[Color]
		  ,[Size] AS PrePack
		  ,[Description]
		  ,[PackNo]
		  ,[DimCode]
		  ,[Height]
		  ,[Width]
		  ,[Lenght]
		  ,[InDimCode]
		  ,[HeightIn]
		  ,[WidthIn]
		  ,[LenghtIn]
		  ,[FromNo]
		  ,[ToNo]
		  ,[TotalCarton]
		FROM 
			[LSERP].[dbo].[InventTrans] invent
			UNPIVOT
			(
				Quantity
				FOR SizeQty in (Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8, Q9, Q10, Q11, Q12, Q13, Q14)

			) u
		WHERE Quantity > 0 AND ItemID IS NOT NULL
		--where RIGHT(Size, 1) = RIGHT(PriceQty, 1) and PricePerSize != 0 and Quantity != 0
	) sub
	WHERE
		sub.ItemID IN 
		(
			SELECT
				LSStyle
			FROM
				ItemStyleConvertv1
			WHERE
				OrderID IN (SELECT REPLACE(OrderID,'_PU','') as OrderID FROM [dbo].SalesOrders  WHERE CustomerID = 'PU')
		)
		AND Quantity > 0

GO
--DELETE [dbo].[PackingLine]
INSERT INTO [dbo].[PackingLine]
           ([SequenceNo]
           ,[LSStyle]
           ,[QuantitySize]
           ,[QuantityPerPackage]
           ,[PackagesPerBox]
           ,[QuantityPerCarton]
           ,[TotalQuantity]
           ,[NetWeight]
           ,[GrossWeight]
           ,[Color]
           ,[PrePack]
           ,[Size]
           ,[Quantity]
           ,[PackingListCode]
           ,[BoxDimensionCode]
           ,[Length]
           ,[Width]
           ,[Height]
           ,[InnerBoxDimensionCode]
           ,[InnerLength]
           ,[InnerWidth]
           ,[InnerHeight]
           ,[FromNo]
           ,[ToNo]
           ,[TotalCarton]
           ,[PackingListID]
           ,[CreatedBy]
           ,[CreatedAt])
	SELECT DISTINCT
		sizeQuantity.[SeqNo]
		,sizeName.ItemStyle
		,sizeQuantity.[QtySize]
		,sizeQuantity.[QtyPerPackage]
		,sizeQuantity.[PackagesPerBox]
		,sizeQuantity.[QtyPerCTNS]
		,sizeQuantity.[QtyPCS]
		,sizeQuantity.[NWeight]
		,sizeQuantity.[GWeight]
		,sizeQuantity.[Color]
		,sizeQuantity.[PrePack]
		,sizeName.SizeName
		,sizeQuantity.Quantity
		,sizeQuantity.[PackNo]
		,bd.Code
		,sizeQuantity.[Lenght]
		,sizeQuantity.[Width]
		,sizeQuantity.[Height]	
		,bd2.Code
		,sizeQuantity.[LenghtIn]
		,sizeQuantity.[WidthIn]
		,sizeQuantity.[HeightIn]
		,sizeQuantity.[FromNo]
		,sizeQuantity.[ToNo]
		,sizeQuantity.[TotalCarton]
		,pl.ID
		,'admin'
		,GETDATE()
	FROM
		#TempItemSizeNamePL sizeName
		JOIN #TempItemSizeQuantityPL sizeQuantity ON sizeName.ItemStyle = sizeQuantity.ItemStyle
			AND sizeName.SizeCode = sizeQuantity.Size
		JOIN PackingList pl ON pl.PackingListCode = sizeQuantity.PackNo
		LEFT JOIN BoxDimension bd ON bd.Code = sizeQuantity.[DimCode]
		LEFT JOIN BoxDimension bd2 ON bd2.Code = sizeQuantity.[InDimCode]
GO
DELETE ItemStylePackingList
--- INSERT STYLE NUMBER - PACKING LIST ID
INSERT INTO [dbo].[ItemStylePackingList]
           ([ItemStylesNumber]
           ,[PackingListsID])
     SELECT DISTINCT
		sizeName.StyleNumber
		,pl.ID
	FROM
		#TempItemSizeNamePL sizeName
		JOIN #TempItemSizeQuantityPL sizeQuantity ON sizeName.ItemStyle = sizeQuantity.ItemStyle
			AND sizeName.SizeCode = sizeQuantity.Size
		JOIN PackingList pl ON pl.PackingListCode = sizeQuantity.PackNo
GO