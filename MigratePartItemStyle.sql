/****** Script for SelectTopNRows command from SSMS  ******/
DECLARE @CustomerID nvarchar(450) = 'DE'
INSERT INTO [dbo].[Part]
           ([Number]
           ,[GarmentColorCode]
           ,[GarmentColorName]
           ,[Season]
           ,[LastSequenceNumber]
           ,[CustomerID]
           ,[Character]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt]
           ,[ID])
	SELECT 
		SUB_3.CustomerStyle,
		SUB_3.GarmentColorCode,
		SUB_3.GarmentColorName,
		SUB_3.Season,
		MAX(SUB_3.LastSequenceNumber) + 1 as MaxSequenceNumber,
		@CustomerID,
		SUB_3.[Character],
		'admin',
		NULL,
		GETDATE(),
		NULL,
		NEWID()
	FROM
	(
		SELECT 
			SUB_2.CustomerStyle,
			SUBSTRING(Numbers,1,1) AS [Character],
			CAST(SUBSTRING(Numbers,2, LEN(Numbers)) AS  INT) AS LastSequenceNumber,
			SUB_2.Season,
			SUB_2.ItemColor AS GarmentColorCode,
			SUB_2.ItemColorName AS GarmentColorName
		FROM
		(
			SELECT 
				*
			FROM
			(
				SELECT [StyleNumber]
				  ,[LSStyle]
				  ,[CustomerStyle]
				  ,ItemColor
				  ,ItemColorName
				  ,[Description]
				  ,[Season]
				  ,VALUE AS NUMBERS
			  FROM 
				[LS_ERPv1.5].[dbo].[ItemStyle] [IS]
				JOIN [LS_ERPv1.5].[dbo].[SalesOrder] [SO] ON [SO].[OrderID] = [IS].[OrderID]
					AND [SO].[CustID] = @CustomerID
					AND [IS].[LSStyle] IS NOT NULL
					AND [IS].[LSStyle] LIKE '%-%'
					AND TRIM([IS].[Season]) <> ''
				CROSS APPLY STRING_SPLIT(LSStyle, '-')
			) SUB_1
			WHERE 
				TRIM(SUB_1.[CustomerStyle]) <> TRIM(SUB_1.NUMBERS)
				AND TRIM(SUB_1.[Season]) <> TRIM(SUB_1.NUMBERS)
				AND SUB_1.[LSStyle] LIKE  '%' + SUB_1.[Season] + '%'
				AND SUB_1.NUMBERS <> 'FOB'
				AND SUB_1.NUMBERS <> 'L2L'
		) SUB_2
	)SUB_3
	GROUP BY
		SUB_3.CustomerStyle,
		SUB_3.[Character],
		SUB_3.Season,
		SUB_3.GarmentColorCode,
		SUB_3.GarmentColorName
