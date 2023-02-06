USE [ERPv2]
GO
INSERT INTO  [dbo].[PackingList]
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
  LEFT JOIN  [dbo].ShippingMark sp ON sp.Code = [LSERP].[dbo].[PackingList].ShipMark2
  LEFT JOIN  [dbo].ShippingMark sp1 ON sp1.Code = [LSERP].[dbo].[PackingList].ShipMark1
  WHERE OrderNo IN (SELECT REPLACE(ID, '_PU','') AS ID FROM [dbo].SalesOrders WHERE CustomerID = 'PU')

GO
UPDATE [dbo].[PackingList]
SET CompanyCode = 'LS'
