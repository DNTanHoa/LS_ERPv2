DECLARE @Username NVARCHAR(4000) = 'admin'

--- Migrate InventItemGroup To MaterialType
INSERT INTO [dbo].[MaterialType]
           ([Code]
           ,[Name]
           ,[SortOrder]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt])
	SELECT
		 [ITEMGROUP].[ItemGroupID]
		,[ITEMGROUP].[Name]
		,[ITEMGROUP].[ShortOrder]
		,@Username
		,NULL
		,GETDATE()
		,NULL
	FROM
		[LS_ERPv1.5].[dbo].[InventItemGroup] [ITEMGROUP] (NOLOCK)

--- Init Part Material Status
INSERT INTO [dbo].[PartMaterialStatus]([Code],[Name],[CanEdit],[CanDelete],[CreatedBy],[LastUpdatedBy],[CreatedAt],[LastUpdatedAt])
     VALUES ('0','Unknown',1,1,@Username,NULL,GETDATE(),NULL)
INSERT INTO [dbo].[PartMaterialStatus]([Code],[Name],[CanEdit],[CanDelete],[CreatedBy],[LastUpdatedBy],[CreatedAt],[LastUpdatedAt])
     VALUES ('1','Submitted',0,0,@Username,NULL,GETDATE(),NULL)
INSERT INTO [dbo].[PartMaterialStatus]([Code],[Name],[CanEdit],[CanDelete],[CreatedBy],[LastUpdatedBy],[CreatedAt],[LastUpdatedAt])
     VALUES ('2','Rejected',1,1,@Username,NULL,GETDATE(),NULL)
INSERT INTO [dbo].[PartMaterialStatus]([Code],[Name],[CanEdit],[CanDelete],[CreatedBy],[LastUpdatedBy],[CreatedAt],[LastUpdatedAt])
     VALUES ('3','Approved',0,0,@Username,NULL,GETDATE(),NULL)

--- Migrate Unit
INSERT INTO [dbo].[Unit]
           ([ID]
           ,[Name]
           ,[Text]
           ,[Rouding]
           ,[DecUnit]
           ,[Factor]
           ,[RoundingPurchase]
           ,[RoundUp]
           ,[RoundDown]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt])
	SELECT
		    [UNIT].[UnitID]
           ,[UNIT].[UnitID]
           ,[UNIT].[UnitID]
           ,[UNIT].[Rounding]
           ,[UNIT].[DecUnit]
           ,[UNIT].[Factor]
           ,[UNIT].[RoundingPurchase]
           ,[UNIT].[RoundUp]
           ,[UNIT].[RoundDown]
           ,@Username
           ,NULL
           ,GETDATE()
           ,NULL
	FROM
		[LS_ERPv1.5].[dbo].[Unit] [UNIT] (NOLOCK)

--- Migrate Vendor
INSERT INTO [dbo].[Vendor]
           ([ID]
           ,[Name]
           ,[Phone]
           ,[Email]
           ,[Fax]
           ,[Address]
           ,[Description]
           ,[Country]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt]
           ,[CurrencyID])
	SELECT
		    [VENDOR].[VendID]
           ,[VENDOR].[VendName]
           ,[VENDOR].[Phone]
           ,[VENDOR].[Email]
           ,[VENDOR].[Fax]
           ,[VENDOR].[Address]
           ,[VENDOR].[Description]
           ,[VENDOR].[Country]
           ,@Username
           ,NULL
           ,GETDATE()
           ,NULL
           ,[VENDOR].[CurrencyID]
	FROM
		[LS_ERPv1.5].[dbo].[Vendor] [VENDOR] (NOLOCK)

--- Migrate Part Revision
SET IDENTITY_INSERT [dbo].[PartRevision] ON

INSERT INTO [dbo].[PartRevision]
           ([RevisionNumber]
           ,[ID]
           ,[PartNumber]
           ,[EffectDate]
           ,[IsConfirmed]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt]
           ,[Season])
     SELECT
		 [PARTREVISION].[RevNum]
		,[PARTREVISION].[ID]
		,[PARTREVISION].[PartNum]
		,[PARTREVISION].[EffectDate]
		,[PARTREVISION].[Confirm]
		,[CREATEUSER].[UserName]
		,[CREATEUSER].[UserName]
		,[PARTREVISION].[EffectDate]
		,[PARTREVISION].[EffectDate]
		,[PARTREVISION].[Season]
	 FROM
		[LS_ERPv1.5].[dbo].[PartRev] [PARTREVISION] (NOLOCK)
		LEFT JOIN [LS_ERPv1.5].[dbo].[Person] (NOLOCK) [CREATE] ON [CREATE].[ID] = [PARTREVISION].[Person]
		LEFT JOIN [LS_ERPv1.5].[dbo].[PermissionPolicyUser] (NOLOCK) [CREATEUSER] ON [CREATE].[User] = [CREATEUSER].[Oid]

SET IDENTITY_INSERT [dbo].[PartRevision] OFF

--- Migrate Part Material
INSERT INTO [dbo].[PartMaterial]
           ([PartNumber]
           ,[Position]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt]
           ,[CurrencyID]
           ,[CutWidth]
           ,[ExternalCode]
           ,[FabricWeight]
           ,[FabricWidth]
           ,[FreePercent]
           ,[GarmentColorCode]
           ,[GarmentColorName]
           ,[GarmentSize]
           ,[ItemColorCode]
           ,[ItemColorName]
           ,[ItemID]
           ,[ItemName]
           ,[ItemStyleNumber]
           ,[LabelCode]
           ,[LabelName]
           ,[LeadTime]
           ,[LessPercent]
           ,[MaterialTypeCode]
           ,[OverPercent]
           ,[PartMaterialStatusCode]
           ,[PartRevisionID]
           ,[PerUnitID]
           ,[Price]
           ,[PriceUnitID]
           ,[QuantityPerUnit]
           ,[VendorID]
           ,[WastagePercent]
		   ,[Specify])
	SELECT
		    [PARTMATERIAL].[PartRev]
           ,[PARTMATERIAL].[Position]
           ,[CREATEUSER].[UserName]
           ,[CREATEUSER].[UserName]
           ,[PARTMATERIAL].[CreateDate]
           ,[PARTMATERIAL].[UpdateDate]
           ,[PARTMATERIAL].[Currency]
           ,[PARTMATERIAL].[CutWidth]
           ,[PARTMATERIAL].[ExCode]
           ,[PARTMATERIAL].[FabricWeight]
           ,[PARTMATERIAL].[FabricsWidth]
           ,[PARTMATERIAL].[FreePercent]
           ,[PARTMATERIAL].[ColorGroup]
           ,[PARTMATERIAL].[ColorGarmentName]
           ,[PARTMATERIAL].[GarmentSize]
           ,[PARTMATERIAL].[Color]
           ,[PARTMATERIAL].[GridValue]
           ,[PARTMATERIAL].[ItemID]
           ,[PARTMATERIAL].[ItemName]
           ,[PARTMATERIAL].[StyleNumber]
           ,[PARTMATERIAL].[Label]
           ,[PARTMATERIAL].[Label]
           ,[PARTMATERIAL].[LeadTime]
           ,[PARTMATERIAL].[Less]
           ,[PARTMATERIAL].[MaterialClass]
           ,[PARTMATERIAL].[Over]
           ,[PARTMATERIALSTATUS].[Code]
           ,[PARTMATERIAL].[PartRev]
           ,[PARTMATERIAL].[PerUnit]
           ,[PARTMATERIAL].[PurchPrice]
           ,[PARTMATERIAL].[PriceUnit]
           ,[PARTMATERIAL].[QtyPerUnit]
           ,[PARTMATERIAL].[VendID]
           ,[PARTMATERIAL].[Wastage]
		   ,[PARTMATERIAL].[Specify]
	 FROM
		[LS_ERPv1.5].[dbo].[PartMtl] [PARTMATERIAL] (NOLOCK)
		LEFT JOIN [LS_ERPv1.5].[dbo].[Person] (NOLOCK) [CREATE] ON [CREATE].[ID] = [PARTMATERIAL].[Person]
		LEFT JOIN [LS_ERPv1.5].[dbo].[PermissionPolicyUser] (NOLOCK) [CREATEUSER] ON [CREATE].[User] = [CREATEUSER].[Oid]
		LEFT JOIN [PartMaterialStatus] [PARTMATERIALSTATUS] (NOLOCK) ON [PARTMATERIALSTATUS].[Code] = CAST([PARTMATERIAL].[Status] AS NVARCHAR(100))

--- Migrate Part Revision Log
USE [ERPv2]
GO

--- Migreate part revision log
INSERT INTO [dbo].[PartRevisionLog]
           ([Code]
           ,[ActivityDate]
           ,[Description]
           ,[PartRevisionLogReferenceCode]
           ,[PartRevisionID]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt])
     SELECT
		[PRLOG].[Code]
       ,[PRLOG].[ActivityDate]
       ,[PRLOG].[Description]
       ,[PRLOG].[RefLog]
       ,[PRLOG].[PartRev]
       ,[CREATEUSER].[UserName]
       ,NULL
       ,GETDATE()
       ,NULL
	 FROM
		[LS_ERPv1.5].[dbo].[PartRevLog] [PRLOG]
		LEFT JOIN [LS_ERPv1.5].[dbo].[Person] (NOLOCK) [CREATE] ON [CREATE].[ID] = [PRLOG].[CreateBy]
		LEFT JOIN [LS_ERPv1.5].[dbo].[PermissionPolicyUser] (NOLOCK) [CREATEUSER] ON [CREATE].[User] = [CREATEUSER].[Oid]

--- Migrate part revision detail
INSERT INTO [dbo].[PartRevisionLogDetail]
           ([PartNumber]
           ,[PartRevisionID]
           ,[ExternalCode]
           ,[PartMaterialStatusCode]
           ,[ItemID]
           ,[ItemName]
           ,[ItemColorCode]
           ,[ItemColorName]
           ,[ItemStyleNumber]
           ,[Position]
           ,[LabelCode]
           ,[LabelName]
           ,[MaterialTypeCode]
           ,[MaterialTypeClass]
           ,[Specify]
           ,[GarmentSize]
           ,[GarmentColorCode]
           ,[GarmentColorName]
           ,[VendorID]
           ,[PerUnitID]
           ,[PriceUnitID]
           ,[CurrencyID]
           ,[Price]
           ,[QuantityPerUnit]
           ,[LeadTime]
           ,[LessPercent]
           ,[FreePercent]
           ,[WastagePercent]
           ,[OverPercent]
           ,[FabricWeight]
           ,[FabricWidth]
           ,[CutWidth]
           ,[PartRevisionLogCode]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt])
	SELECT
		 NULL
        ,[PRLOGDETAIL].[PartRev]
        ,[PRLOGDETAIL].[ExCode]
        ,NULL
        ,[PRLOGDETAIL].[ItemID]
        ,[PRLOGDETAIL].[ItemName]
        ,[PRLOGDETAIL].[Color]
        ,[PRLOGDETAIL].[GridValue]
        ,[PRLOGDETAIL].[StyleNumber]
        ,[PRLOGDETAIL].[Position]
        ,[PRLOGDETAIL].[Label]
        ,[PRLOGDETAIL].[Label]
        ,[PRLOGDETAIL].[MaterialClass]
        ,NULL---[PRLOGDETAIL].[MaterialTypeClass]
        ,[PRLOGDETAIL].[Specify]
        ,[PRLOGDETAIL].[GarmentSize]
        ,[PRLOGDETAIL].[ColorGroup]
        ,NULL---[PRLOGDETAIL].[ColorGarmentName]
        ,[PRLOGDETAIL].[VendID]
        ,[PRLOGDETAIL].[PerUnit]
        ,[PRLOGDETAIL].[PriceUnit]
        ,[PRLOGDETAIL].[Currency]
        ,NULL---[PRLOGDETAIL].[Price]
        ,[PRLOGDETAIL].[QtyPerUnit]
        ,[PRLOGDETAIL].[LeadTime]
        ,[PRLOGDETAIL].[Less]
        ,NULL---[PRLOGDETAIL].[Free]
        ,[PRLOGDETAIL].[Wastage]
        ,[PRLOGDETAIL].[Over]
        ,[PRLOGDETAIL].[FabricWeight]
        ,[PRLOGDETAIL].[FabricsWidth]
        ,[PRLOGDETAIL].[CutWidth]
        ,[PRLOGDETAIL].[RefLog]
        ,[CREATEUSER].[UserName]
		,NULL
		,GETDATE()
		,NULL
	FROM
		[LS_ERPv1.5].[dbo].[PartRevLogDetail] [PRLOGDETAIL]
		JOIN [LS_ERPv1.5].[dbo].[PartRevLog] [PRLOG] ON [PRLOGDETAIL].[RefLog] = [PRLOG].[Code]
		LEFT JOIN [LS_ERPv1.5].[dbo].[Person] (NOLOCK) [CREATE] ON [CREATE].[ID] = [PRLOG].[CreateBy]
		LEFT JOIN [LS_ERPv1.5].[dbo].[PermissionPolicyUser] (NOLOCK) [CREATEUSER] ON [CREATE].[User] = [CREATEUSER].[Oid]
GO
