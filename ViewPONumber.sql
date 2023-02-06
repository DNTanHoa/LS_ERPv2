CREATE VIEW ViewPONumber
AS
	SELECT DISTINCT i.PurchaseOrderNumber, i.LSStyle, s.CustomerID
	FROM ItemStyle (NOLOCK) i
		INNER JOIN SalesOrders (NOLOCK) s ON i.SalesOrderID = s.ID
	
