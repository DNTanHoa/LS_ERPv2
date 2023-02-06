INSERT INTO [ERPv2].[dbo].[StyleNetWeight]
           ([CustomerStyle]
           ,[Size]
           ,[NetWeight]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt])
     SELECT
       [CustStyle]
      ,[Size]
      ,[NetWeight]
	  ,'admin'
	  ,'admin'
	  ,GETDATE()
	  ,GETDATE()
	FROM [LSERP].[dbo].[StyleNetWeight]
GO