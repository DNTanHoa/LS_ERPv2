USE [ERPv2]
GO
UPDATE OrderDetail
SET OrderDetail.Price = (SELECT DISTINCT [LS_ERPv1.5].[dbo].[ItemStyle].FOBPrice
						 FROM [LS_ERPv1.5].[dbo].[ItemStyle]
						 WHERE OrderID IN (SELECT OrderID FROM [LS_ERPv1.5].[dbo].SalesOrder WHERE CustID = 'HA')
						 AND [LS_ERPv1.5].[dbo].[ItemStyle].FOBPrice IS NOT NULL
						 AND OrderDetail.ItemStyleNumber = [LS_ERPv1.5].[dbo].[ItemStyle].StyleNumber
						 )
WHERE OrderDetail.ItemStyleNumber IN (SELECT DISTINCT [LS_ERPv1.5].[dbo].[ItemStyle].StyleNumber
			 FROM [LS_ERPv1.5].[dbo].[ItemStyle]
			 INNER JOIN OrderDetail ON OrderDetail.ItemStyleNumber = [LS_ERPv1.5].[dbo].[ItemStyle].StyleNumber
			 WHERE OrderID IN (SELECT OrderID FROM [LS_ERPv1.5].[dbo].SalesOrder WHERE CustID = 'HA'))