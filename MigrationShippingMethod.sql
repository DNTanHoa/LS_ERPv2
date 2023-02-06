INSERT INTO [ERPv2].[dbo].[ShippingMethod]
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
		  ,NULL
		  ,NULL
		  ,NULL
		  ,'admin'
		  ,'admin'
		  ,GETDATE()
		  ,GETDATE()
	FROM [LSERP].[dbo].[ShippingMethod]