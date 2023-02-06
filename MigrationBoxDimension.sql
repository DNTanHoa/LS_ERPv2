INSERT INTO [ERPv2].[dbo].[BoxDimension]
           (
		    [Code]
           ,[Description]
           ,[Length]
           ,[Width]
           ,[Height]
           ,[Weight]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt])
     SELECT DISTINCT
       [Code]
      ,[Description]
      ,[Length]
      ,[Width]
      ,[Height]
      ,[Weight]
	  ,'admin'
	  ,'admin'
	  ,GETDATE()
	  ,GETDATE()
    FROM [LSERP].[dbo].[BoxDimension]