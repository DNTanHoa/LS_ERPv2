USE [ERPv2]
GO

DECLARE @UserName NVARCHAR(4000) = 'admin'

--- Migrate forecast group
INSERT INTO [dbo].[ForecastGroup]
           ([ID]
           ,[CustomerID]
           ,[Description]
           ,[ForecastFrom]
           ,[ForecastTo]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt])
	SELECT
		 CAST([FCG].[ID] AS NVARCHAR(100))
		,[FCG].[Customer]
		,[FCG].[Desc]
		,NULL
		,NULL
		,@UserName
		,NULL
		,GETDATE()
		,NULL
	FROM
		[LS_ERPv1.5].[dbo].[ForecastGroup] [FCG]

--- Migrate Forecast Entry
INSERT INTO [dbo].[ForecastEntry]
           ([ForecastGroupID]
           ,[Name]
           ,[WeekID]
           ,[EntryDate]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt])
	SELECT
		[ForecastGroup]
	   ,[Name]
	   ,NULL
	   ,[Date]
	   ,@UserName
	   ,NULL
	   ,GETDATE()
	   ,NULL
	FROM
		[LS_ERPv1.5].[dbo].[CreationWeek] [CreationWeek]

--- Migrate week
INSERT INTO [dbo].[Week]
           ([ID]
           ,[Name]
           ,[FromDate]
           ,[ToDate])
	SELECT 
		 [WeekID]
		,[WeekID]
		,NULL
		,NULL
	FROM [LS_ERPv1.5].[dbo].[Week]

--- Migrate forecast overall
INSERT INTO [dbo].[ForecastOverall]
           ([ID]
           ,[ForecastEntryID]
           ,[LSCode]
           ,[GarmentColorCode]
           ,[GarmentColorName]
           ,[Zone]
           ,[Division]
           ,[Brand]
           ,[Season]
           ,[LabelCode]
           ,[LabelName]
           ,[PurchaseOrderNumber]
           ,[CustomerStyle]
           ,[ContractNo]
           ,[Distribution]
           ,[ForecastWeekID]
           ,[ForecastWeekTitle]
           ,[ShipTo]
           ,[ShippingMethod]
           ,[ShipPack]
           ,[PackRatio]
           ,[DeliveryDate]
           ,[PlannedQuantity]
           ,[IsBomPulled]
           ,[IsQuantityCalculated]
           ,[IsQuantityBalanced])
     SELECT
	   [FCOverall].[ID]
	  ,[FCentry].[ID]
      ,[FCOverall].[LSCode]
	  ,[FCOverall].[ModelCode]
      ,[FCOverall].[ModelName]
      ,[FCOverall].[Zone]
	  ,[FCOverall].[Division]
      ,[FCOverall].[Branch]
      ,[FCOverall].[Season]
      ,[FCOverall].[Label]
      ,[FCOverall].[Label]
      ,[FCOverall].[PONum]
      ,[FCOverall].[IMAN]
      ,[FCOverall].[ContractNo]
      ,[FCOverall].[Distribution]
      ,[Week].[ID]
	  ,[FCoverall].[WeekString]
	  ,[FCOverall].[ShipTo]
      ,[FCOverall].[ShipMode]
      ,[FCOverall].[ShipPack]
      ,[FCOverall].[PackRatio]
      ,[FCOverall].[DeliveryDate]
      ,[FCOverall].[PlannedQty]
	  ,[FCOverall].[BomPulled]
      ,[FCOverall].[QtyCalculated]
      ,[FCOverall].[QtyBalanced]
	 FROM
	   [LS_ERPv1.5].[dbo].[ForecastOverall] [FCOverall]
	   LEFT JOIN [ERPv2].[dbo].[ForecastEntry] [FCEntry] ON [FCOverall].[CreationWeek] = [FCentry].[Name]
	   LEFT JOIN [ERPv2].[dbo].[Week] [Week] ON [Week].[ID] = [FCOverall].[Week]

--- Migrate forecast detail
INSERT INTO [dbo].[ForecastDetail]
           ([ForecastOverallID]
           ,[GarmentSize]
           ,[PlannedQuantity]
           ,[Price])
	SELECT
		 [FCDetail].[ForecastOverall]
		,[FCDetail].[Size]
		,[FCDetail].[PlannedQty]
		,[FCDetail].[Price]
	FROM
		[LS_ERPv1.5].[dbo].[ForecastDetail] [FCDetail]


--- Migrate forcast material

INSERT INTO [dbo].[ForecastMaterial]
           ([ItemID]
           ,[ItemName]
           ,[MaterialClassType]
           ,[Specify]
           ,[ItemColorName]
           ,[GarmentSize]
           ,[Position]
           ,[MaterialTypeCode]
           ,[CurrencyID]
           ,[PerUnitID]
           ,[PriceUnitID]
           ,[VendorID]
           ,[QuantityPerUnit]
           ,[RequiredQuantity]
           ,[WastageQuantity]
           ,[Price]
           ,[FabricWeight]
           ,[FabricWidth]
           ,[ForecastDetailID]
           ,[ForecastOverallID]
           ,[GarmentColorCode]
           ,[GarmentColorName]
           ,[ItemColorCode]
           ,[LeadTime]
		   ,[CutWidth])
     SELECT
		    [FCMaterial].[ItemID]
           ,[FCMaterial].[ItemName]
           ,NULL -- for the materail class type usse to calculate wastage
           ,[FCMaterial].[Specify]
           ,[FCMaterial].[GridValue]
           ,[FCMaterial].[GarmentSize]
           ,[FCMaterial].[Position]
           ,[FCMaterial].[MaterialClass]
           ,[FCMaterial].[Currency]
           ,[FCMaterial].[PerUnit]
           ,[FCMaterial].[PriceUnit]
           ,[FCMaterial].[VendID]
           ,[FCMaterial].[QtyPerUnit]
           ,[FCMaterial].[RequiredQty]
           ,[FCMaterial].[Wastage]
           ,[FCMaterial].[PurchPrice]
           ,[FCMaterial].[FabricWeight]
           ,[FCMaterial].[FabricWidth]
           ,[FCMaterial].[ForecastDetail]
           ,[FCMaterial].[ForecastOverall]
           ,NULL--[FCMaterial].[GarmentColorCode]
           ,NULL--[FCMaterial].[GarmentColorName]
           ,[FCMaterial].[Color]
           ,NULL -- Leaad time
		   ,[FCMaterial].[CutWidth]
	 FROM
		[LS_ERPv1.5].[dbo].[ForecastMtl] [FCMaterial]

  
GO
