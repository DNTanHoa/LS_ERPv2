USE [ERPv2]
GO
-- Migration Part - HA
INSERT INTO [dbo].[Part]
           ([Number]
           ,[GarmentColorCode]
           ,[GarmentColorName]
           ,[LastSequenceNumber]
           ,[CustomerID]
           ,[Character]
           ,[CreatedBy]
           ,[CreatedAt]
           ,[ID])
     SELECT 
		 [CustomerStyle]
        ,[ColorCode]
        ,[ColorName]
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
			sub_1.ColorCode,
			sub_1.ColorName,
			max(sub_1.LastSequenceNumber) as LastSequenceNumber
			--ROW_NUMBER() OVER (Partition by CustomerStyle, [Character], Season ORDER BY LastSequenceNumber DESC) as rn
		FROM
		(
			SELECT 
				CustomerStyle,
				SUBSTRING(Numbers,1,1) AS [Character],
				CAST(SUBSTRING(Numbers,2, LEN(Numbers)) AS  INT) AS LastSequenceNumber,
				Customer,
				ColorCode,
				ColorName
			FROM
			(
				SELECT
					ColorCode,
					ColorName,
					'HA' AS Customer,
					CustomerStyle,
					Value AS Numbers
				FROM 
					[dbo].[ItemStyle]
					CROSS APPLY STRING_SPLIT(LSStyle, '-')
				WHERE
					LSStyle LIKE '%-%'
					AND Season is not null AND ColorCode is not null
					AND SalesOrderID IN (SELECT ID FROM SalesOrders WHERE CustomerID = 'HA')
			) sub
			WHERE CustomerStyle <> Numbers
		) sub_1
		GROUP BY sub_1.CustomerStyle,
				 sub_1.[Character],
				 sub_1.Customer,
				 sub_1.ColorCode,
				 sub_1.ColorName
	) SUB_2
	