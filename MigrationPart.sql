USE [ERPv2_Production]
GO
-- Migration Part
INSERT INTO [dbo].[Part]
           ([Number]
           ,[GarmentColorCode]
           ,[GarmentColorName]
           ,[Season]
           ,[LastSequenceNumber]
           ,[CustomerID]
           ,[Character]
           ,[CreatedBy]
           ,[CreatedAt]
           ,[ID])
     SELECT 
		 [CustomerStyle]
        ,[GarmentColorCode]
        ,[GarmentColorName]
        ,[Season]
        ,[LastSequenceNumber] + 1
        ,[Customer]
        ,[Character]
        ,'admin'
        ,GETDATE()
        ,UPPER(NEWID())
	 FROM 
	(
		SELECT
			sub_1.CustomerStyle,
			sub_1.[Character],
			sub_1.Customer,
			sub_1.Season,
			sub_1.GarmentColorCode,
			sub_1.GarmentColorName,
			max(sub_1.LastSequenceNumber) as LastSequenceNumber
			--ROW_NUMBER() OVER (Partition by CustomerStyle, [Character], Season ORDER BY LastSequenceNumber DESC) as rn
		FROM
		(
			SELECT 
				CustomerStyle,
				SUBSTRING(Numbers,1,1) AS [Character],
				CAST(SUBSTRING(Numbers,2, LEN(Numbers)) AS  INT) AS LastSequenceNumber,
				Customer,
				Season,
				GarmentColorCode,
				GarmentColorName
			FROM
			(
				SELECT
					ID,
					TRIM(Season) AS Season,
					GarmentColorCode,
					GarmentColorName,
					'PU' AS Customer,
					CustomerStyle,
					Value AS Numbers
				FROM 
					[dbo].[SalesContractDetail]
					CROSS APPLY STRING_SPLIT(LSStyle, '-')
				WHERE
					LSStyle LIKE '%-%'
			) sub
			WHERE CustomerStyle <> Numbers 
		) sub_1
		GROUP BY sub_1.CustomerStyle,
				 sub_1.[Character],
				 sub_1.Customer,
				 sub_1.Season,
				 sub_1.GarmentColorCode,
				 sub_1.GarmentColorName
	) SUB_2
	