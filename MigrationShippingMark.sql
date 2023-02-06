INSERT INTO [ShippingMark]
           ([Code]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt])
     SELECT DISTINCT
		 [Description]
		 ,'admin'
		 ,'admin'
		 ,GETDATE()
		 ,GETDATE()
	FROM [LSERP].[dbo].[ShippingMark]
GO