IF OBJECT_ID('tempdb.dbo.#TempItemSizeNameOPL') IS NOT NULL
		DROP TABLE #TempItemSizeNameOPL;

	IF OBJECT_ID('tempdb.dbo.#TempItemSizeQuantityOPL') IS NOT NULL
		DROP TABLE #TempItemSizeQuantityOPL;

-- Get ItemSizeName
	SELECT
	sub1.StyleNumber AS StyleID,
	sub1.SizeName,
	sub1.SizeCode 
	INTO #TempItemSizeNameOPL
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
		sub.PackNo
		,sub.Color
		,sub.ColorName
		,sub.StyleID 
		,sub.Size 
		,sub.Quantity
		INTO #TempItemSizeQuantityOPL
	FROM
	(
		SELECT
		  StyleID
		  ,PackNo
		  ,Size
		  ,Quantity
		  ,Color
		  ,ColorName
		FROM 
			[LSERP].[dbo].PackedStyleDim dim
			UNPIVOT
			(
				Quantity
				FOR Size in (S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14)

			) u
		WHERE Quantity >= 0 AND StyleID IS NOT NULL
	) sub
GO

	-- Create PackingOverQuantity 
INSERT INTO [dbo].[PackingOverQuantity]
           ([PackingListCode]
           ,[ColorCode]
           ,[ColorName]
           ,[Size]
           ,[Quantity]
           ,[PackingListID]
           ,[ItemStyleNumber]
           ,[CreatedBy]
           ,[CreatedAt])
	SELECT
		sizeQuantity.PackNo,
		sizeQuantity.Color,
		sizeQuantity.ColorName,
		sizeName.SizeName,
		sizeQuantity.Quantity,
		pl.ID,
		sizeName.StyleID,
		'admin',
		GETDATE()
	FROM
		#TempItemSizeNameOPL sizeName
		JOIN #TempItemSizeQuantityOPL sizeQuantity ON sizeName.StyleID = sizeQuantity.StyleID
			AND sizeName.SizeCode = sizeQuantity.Size
		JOIN PackingList pl ON pl.PackingListCode = sizeQuantity.PackNo
	GROUP BY
		sizeName.StyleID,
		sizeName.SizeName,
		sizeQuantity.Quantity,
		sizeQuantity.Color,
		sizeQuantity.ColorName,
		sizeQuantity.PackNo,
		pl.ID

	-- Update order detail size sequenece number
	UPDATE [POQ]
		SET SizeSortIndex = [SIZE].[SequeneceNumber]
	FROM 
		PackingOverQuantity [POQ]
		LEFT JOIN ItemStyle [STYLE] ON [STYLE].[Number] = [POQ].[ItemStyleNumber]
		LEFT JOIN SalesOrders [SO] on [SO].[ID] = [STYLE].[SalesOrderID]
		LEFT JOIN Size [SIZE] ON [POQ].[Size] = [Size].[Code]
				AND [SIZE].[CustomerID] = 'PU'