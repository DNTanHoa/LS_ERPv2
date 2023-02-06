INSERT INTO  [ERPv2].[dbo].[PackingList]
           ([PackingListCode]
           ,[PackingListDate]
           ,[CustomerID]
		   ,[SalesOrderID]
           ,[DeliveryGroup]
           ,[ShippingMethodCode]
           ,[ShippingMark1Code]
           ,[ShippingMark2Code]
		   ,[RatioRight]
           ,[RatioLeft]
           ,[RatioBottom]
           ,[LSStyles]
           ,[TotalQuantity]
           ,[Confirm]          
           ,[CreatedBy]
           ,[CreatedAt])
     SELECT 
       [PackNo]
      ,[PackingDate]
	  ,[CustID]
      ,CONCAT([OrderNo],'_PU')
	  ,[DeliveryGroup]
      ,[ShipMode]       
      ,sp1.Code
      ,sp.Code
      ,[RatioRight]
      ,[RatioLeft]
      ,[RatioBottom]
	  ,[ListStyles] 
      ,[TotalQty]
	  ,[Confirm]
	  ,[UserBlock]
      ,[CreateDate]
      
  FROM  [LSERP].[dbo].[PackingList]
  LEFT JOIN  [ERPv2].[dbo].ShippingMark sp ON sp.Code = [LSERP].[dbo].[PackingList].ShipMark2
  LEFT JOIN  [ERPv2].[dbo].ShippingMark sp1 ON sp1.Code = [LSERP].[dbo].[PackingList].ShipMark1
  WHERE OrderNo IN (SELECT REPLACE(ID, '_PU','') AS ID FROM [ERPv2].[dbo].SalesOrders WHERE CustomerID = 'PU')

GO


INSERT INTO  [ERPv2].[dbo].[PackingList]
           ([PackingListCode]
           ,[PackingListDate]
           ,[CustomerID]
		   ,[SalesOrderID]
           ,[DeliveryGroup]
           ,[ShippingMethodCode]
           ,[ShippingMark1Code]
           ,[ShippingMark2Code]
		   ,[RatioRight]
           ,[RatioLeft]
           ,[RatioBottom]
           ,[LSStyles]
           ,[TotalQuantity]
           ,[Confirm]          
           ,[CreatedBy]
           ,[CreatedAt])
		SELECT 
			   [PackNo]
			  ,[PackingDate]
			  ,[CustID]
			  ,[OrderNo]
			  ,[DeliveryGroup]
			  ,[ShipMode]       
			  ,[ShipMark1]
			  ,[ShipMark2]
			  ,[RatioRight]
			  ,[RatioLeft]
			  ,[RatioBottom]
			  ,[ListStyles] 
			  ,[TotalQty]
			  ,[Confirm]
			  ,[UserBlock]
			  ,[CreateDate]
      
		  FROM  [LSERP].[dbo].[PackingList] 
		  LEFT JOIN  [ERPv2].[dbo].ShippingMark sp ON sp.Code = [LSERP].[dbo].[PackingList].ShipMark2
		  LEFT JOIN  [ERPv2].[dbo].ShippingMark sp1 ON sp1.Code = [LSERP].[dbo].[PackingList].ShipMark1
		  WHERE OrderNo IN (SELECT ID FROM [ERPv2].[dbo].SalesOrders WHERE CustomerID <> 'PU')
		  AND CustID <> 'PU'