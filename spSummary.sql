DECLARE @CustomerID NVARCHAR(100) = 'DE';
DECLARE @Style NVARCHAR(100) = '311815';
DECLARE @Season NVARCHAR(100) = 'SS22';
DECLARE @Keyword NVARCHAR(1000) = '';

SELECT 
	--SUB.SalesOrderID,
	--SUB.ForecastTitle,
	SUB.ItemCode,
	SUB.ItemID,
	SUB.ItemName,
	SUB.ItemColorCode,
	SUB.ItemColorName,
	SUB.Specify,
	SUB.Position,
	SUB.Season,
	SUB.CustomerStyle,
	SUB.GarmentColorCode,
	SUB.GarmentColorName,
	SUB.GarmentSize,
	SUB.PriceUnitID,
	SUM(SUB.ConsumptionQuantity) AS ConsumptionQuantity,
	SUM(SUB.RequiredQuantity) AS RequiredQuantity,
	SUM(SUB.ReservedQuantity) AS ReservedQuantity,
	SUM(SUB.PurchaseQuantity) AS PurchaseQuantity,
	SUM(SUB.ReceiptQuantity) AS ReceiptQuantity,
	SUM(SUB.IssuedQuantity) AS IssuedQuantity 
FROM
(
	SELECT
		'' AS SalesOrderID,
		'' AS ForecastTitle,
		BOM.ItemCode,
		BOM.ItemID,
		BOM.ItemName,
		BOM.ItemColorCode,
		BOM.ItemColorName,
		BOM.Specify,
		BOM.Position,
		STYLE.Season,
		STYLE.ColorCode AS GarmentColorCode,
		STYLE.ColorName AS GarmentColorName,
		'' AS GarmentSize,
		STYLE.CustomerStyle,
		'' AS LSStyle,
		BOM.PerUnitID,
		BOM.PriceUnitID,
		BOM.QuantityPerUnit,
		BOM.ConsumptionQuantity,
		BOM.RequiredQuantity,
		BOM.ReservedQuantity AS ReservedQuantity,
		0 AS PurchaseQuantity,
		0 AS ReceiptQuantity,
		0 AS IssuedQuantity
	FROM
		ProductionBOM (NOLOCK) BOM
		JOIN ItemStyle (NOLOCK) STYLE ON BOM.ItemStyleNumber = STYLE.Number
			AND (STYLE.Season = @Season OR @Season = '' OR @Season IS NULL)
			AND (STYLE.CustomerStyle LIKE '%' + @Style + '%' OR @Style IS NULL OR @Style = '')
		JOIN SalesOrders (NOLOCK) SO ON SO.ID = STYLE.SalesOrderID
			AND SO.CustomerID = @CustomerID

	UNION ALL

	SELECT 
		'' AS SalesOrderID,
		'' AS ForecastTitle,
		POLINE.ItemCode,
		POLINE.ItemID,
		POLINE.ItemName,
		POLINE.ItemColorCode,
		POLINE.ItemColorName,
		POLINE.Specify,
		POLINE.Position,
		POLINE.Season,
		POLINE.GarmentColorCode AS GarmentColorCode,
		POLINE.GarmentColorName AS GarmentColorName,
		'' AS GarmentSize,
		POLINE.CustomerStyle,
		'' AS LSStyle,
		POLINE.UnitID,
		POLINE.SecondUnitID,
		0 AS QuantityPerUnit,
		0 AS ConsumptionQuantity,
		0 AS RequiredQuantity,
		0 AS ReservedQuantity,
		POLINE.Quantity AS PurchaseQuantity,
		0 AS ReceiptQuantity,
		0 AS IssuedQuantity
	FROM
		PurchaseOrderLine (NOLOCK) POLINE
		JOIN SalesOrders (NOLOCK) SO ON SO.ID = POLINE.SalesOrderID
			AND SO.CustomerID = @CustomerID
			AND (POLINE.Season = @Season OR @Season = '' OR @Season IS NULL)
			AND (POLINE.CustomerStyle LIKE '%' + @Style + '%' OR @Style IS NULL OR @Style = '')

	UNION ALL

	SELECT 
		'' AS SalesOrderID,
		'' AS ForecastTitle,
		POGROUPLINE.ItemCode,
		POGROUPLINE.ItemID,
		POGROUPLINE.ItemName,
		POGROUPLINE.ItemColorCode,
		POGROUPLINE.ItemColorName,
		POGROUPLINE.Specify,
		POGROUPLINE.Position,
		POGROUPLINE.Season,
		POGROUPLINE.GarmentColorCode AS GarmentColorCode,
		POGROUPLINE.GarmentColorName AS GarmentColorName,
		'' AS GarmentSize,
		POGROUPLINE.CustomerStyle,
		'' AS LSStyle,
		POGROUPLINE.UnitID,
		POGROUPLINE.UnitID,
		0 AS QuantityPerUnit,
		0 AS ConsumptionQuantity,
		0 AS RequiredQuantity,
		0 AS ReservedQuantity,
		0 AS PurchaseQuantity,
		RG.ReceiptQuantity AS ReceiptQuantity,
		0 AS IssuedQuantity
	FROM
		ReceiptgroupLine (NOLOCK) RG
		JOIN PurchaseOrderGroupLine (NOLOCK) POGROUPLINE ON POGROUPLINE.ID = RG.PurchaseOrderGroupLineID
		JOIN PurchaseOrder (NOLOCK) PO ON PO.ID = POGROUPLINE.PurchaseOrderID
			AND PO.CustomerID = @CustomerID
			AND (POGROUPLINE.Season = @Season OR @Season = '' OR @Season IS NULL)
			AND (POGROUPLINE.CustomerStyle LIKE '%' + @Style + '%' OR @Style IS NULL OR @Style = '')

) SUB
GROUP BY
	--SUB.SalesOrderID,
	--SUB.ForecastTitle,
	SUB.ItemCode,
	SUB.ItemID,
	SUB.ItemName,
	SUB.ItemColorCode,
	SUB.ItemColorName,
	SUB.Specify,
	SUB.Position,
	SUB.Season,
	SUB.GarmentColorCode,
	SUB.GarmentColorName,
	SUB.GarmentSize,
	SUB.CustomerStyle,
	SUB.PriceUnitID
ORDER BY
	--SUB.LSStyle,
	--SUB.SalesOrderID,
	SUB.ItemID,
	SUB.ItemName