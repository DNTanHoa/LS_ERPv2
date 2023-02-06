USE [ERPv2]
GO

INSERT INTO [dbo].[Item]
           ([ID]
           ,[Name]
           ,[Description]
           ,[Specify]
           ,[CreatedBy]
           ,[LastUpdatedBy]
           ,[CreatedAt]
           ,[LastUpdatedAt]
           ,[ColorCode]
           ,[ColorName]
           ,[CustomerID]
           ,[MaterialTypeCode]
           ,[Season])
    SELECT DISTINCT
		[PARTMTL].[ItemID],
		TRIM([PARTMTL].[ItemName]) AS [ItemName],
		TRIM([PARTMTL].[Position]) AS [Position],
		TRIM([PARTMTL].[Specify]) AS [Specify],
		TRIM([PARTMTL].[ItemColorCode]) AS [ItemColorCode],
		TRIM([PARTMTL].[ItemColorName]) AS [ItemColorName],
		CASE WHEN ([PARTMTL].[PerUnitID] <> 'YD' AND [PARTMTL].[MaterialTypeCode] = 'FB') THEN 'AC'
			 ELSE [PARTMTL].[MaterialTypeCode] END AS MaterialTypeCode
	FROM
		PartMaterial [PARTMTL]
		JOIN PartRevision [PARTREV] ON [PARTMTL].[PartRevisionID] = [PARTREV].[ID]
	WHERE
		[PARTMTL].[ItemID] IS NOT NULL AND TRIM([PARTMTL].[ItemID]) <> ''
GO


