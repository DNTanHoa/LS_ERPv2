DECLARE @CustomerID NVARCHAR(450) = 'DE'

--- Xóa dữ liệu reservation liên quan
DELETE FROM [ReservationEntry] WHERE ProductionBOMID IN 
(
	SELECT 
		ID 
	FROM [ProductionBOM] WHERE [ItemStyleNumber] IN 
	(
		SELECT
			Number
		FROM 
			[ItemStyle] [IS] (NOLOCK)
			JOIN [SalesOrders] [SO] (NOLOCK) ON [IS].[SalesOrderID] = [SO].[ID]
		WHERE
			[SO].[CustomerID] = @CustomerID
	)
)

---  Xóa dữ liệu ProBom hiện tại trên v2 của khách hàng tương ứng
DELETE FROM [ProductionBOM] WHERE [ItemStyleNumber] IN 
(
	SELECT
		Number
	FROM 
		[ItemStyle] [IS] (NOLOCK)
		JOIN [SalesOrders] [SO] (NOLOCK) ON [IS].[SalesOrderID] = [SO].[ID]
	WHERE
		[SO].[CustomerID] = @CustomerID
)

--- Lấy dữ liệu ProBom hiện tại trên v1.5 của khách hàng tương ứng
SET IDENTITY_INSERT [dbo].[ProductionBOM] ON

INSERT INTO [dbo].[ProductionBOM]
           ([ID]
		   ,[ItemStyleNumber]
           ,[PartMaterialID]
           ,[ExternalCode]
           ,[VendorID]
           ,[ItemID]
           ,[ItemName]
           ,[ItemColorCode]
           ,[ItemColorName]
           ,[Specify]
           ,[GarmentSize]
           ,[MaterialTypeCode]
           ,[MaterialTypeClass]
           ,[Position]
           ,[FabricWeight]
           ,[FabricWidth]
           ,[RequiredQuantity]
           ,[WastagePercent]
           ,[WastageQuantity]
           ,[LessPercent]
           ,[LessQuantity]
           ,[FreePercent]
           ,[FreeQuantity]
           ,[ConsumptionQuantity]
           ,[QuantityPerUnit]
           ,[WareHouseQuantity]
           ,[ReservedQuantity]
           ,[RemainQuantity]
           ,[IssuedQuantity]
           ,[LeadTime]
           ,[PerUnitID]
           ,[PriceUnitID]
           ,[Price]
           ,[CurrencyID]
           ,[JobHeadNumber]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt])
     SELECT 
		 [PROBOM].[ID]
		,[PROBOM].[StyleNumber]
		,NULL
		,[PROBOM].[ExCode]
		,[PROBOM].[VendID]
		,[PROBOM].[ItemID]
		,[PROBOM].[ItemName]
		,[PROBOM].[Color]
		,[PROBOM].[GridValue]
		,[PROBOM].[Specify]
		,[PROBOM].[GarmentSize]
		,[PROBOM].[MaterialClass]
		,[PROBOM].[MaterialClassType]
		,[PROBOM].[Position]
		,[PROBOM].[FabricWeight]
		,[PROBOM].[FabricWidth]
		,[PROBOM].[RequiredQTy]
		,[PROBOM].[Wastage]
		,[PROBOM].[WastageQty]
		,[PROBOM].[Less]
		,[PROBOM].[LessQty]
		,[PROBOM].[FreePercent]
		,[PROBOM].[FreePercentQty]
		,[PROBOM].[ConsumptionQty]
		,[PROBOM].[QtyPerUnit]
		,[PROBOM].[WareHouseQty]
		,[PROBOM].[ReservedQty]
		,[PROBOM].[RequiredQty] - ISNULL([PROBOM].[ReservedQty],0)
		,[PROBOM].[IssuedQty]
		,[PROBOM].[LeadTime]
		,[PROBOM].[PerUnit]
		,[PROBOM].[PriceUnit]
		,[PROBOM].[PurchPrice]
		,[PROBOM].[Currency]
		,[PROBOM].[JobHead]
		,[CREATEUSER].[UserName]
		,[CREATEUSER].[UserName]
		,[PROBOM].[CreatedDate]
		,[PROBOM].[UpdatedDate]
	FROM 
		[LS_ERPv1.5].[dbo].[ProBom] [PROBOM]
		LEFT JOIN [LS_ERPv1.5].[dbo].[Person] (NOLOCK) [CREATE] ON [CREATE].[ID] = [PROBOM].[Person]
		LEFT JOIN [LS_ERPv1.5].[dbo].[PermissionPolicyUser] (NOLOCK) [CREATEUSER] ON [CREATE].[User] = [CREATEUSER].[Oid]
		LEFT JOIN [LS_ERPv1.5].[dbo].[ItemStyle] [IS] (NOLOCK) ON [IS].[StyleNumber] = [PROBOM].[StyleNumber]
		JOIN [LS_ERPv1.5].[dbo].[SalesOrder] [SO] (NOLOCK) ON [IS].[OrderID] = [SO].[OrderID]
	WHERE
		[SO].[CustID] = @CustomerID

SET IDENTITY_INSERT [dbo].[ProductionBOM] OFF