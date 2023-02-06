DECLARE @Username NVARCHAR(4000) = 'admin'
--- Migrate Job
INSERT INTO [dbo].[JobHead]
           ([Number]
           ,[GarmentColorCode]
           ,[GarmentSize]
           ,[CustomerStyle]
           ,[LSStyle]
           ,[Config]
           ,[PartRevisionID]
           ,[ProductionQuantity]
           ,[RequestDueDate]
           ,[StartDate]
           ,[DueDate]
           ,[Confirmed]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt])
	SELECT 
			[JobNum]
		   ,[Color]
		   ,[Size]
		   ,[CustomerStyle]
		   ,[LSStyle]
		   ,[Config]
		   ,[PartRev]
		   ,[ProdQty]
		   ,[ReqDueDate]
		   ,[StartDate]
		   ,[DueDate]
		   ,[JobFirm]
		   ,@Username
           ,NULL
           ,GETDATE()
           ,NULL
	FROM [LS_ERPv1.5].[dbo].[JobHead]