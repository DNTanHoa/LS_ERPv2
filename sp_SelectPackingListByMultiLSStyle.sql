-- ================================================
-- Seach Packing List By LSStyles
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Hoa DNT
-- Create date: 2023-01-06
-- Description: Seach Packing List By LSStyles
-- EXEC sp_SelectPackingListByMultiLSStyle '307525-SS23-A10;309989-SS23-A42'
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_SelectPackingListByMultiLSStyle]
	-- Add the parameters for the stored procedure here
	@LSStyles NVARCHAR(4000)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   SELECT
		VALUE AS LSStyle INTO #TempSearchLSStyle
	FROM
		STRING_SPLIT(@LSStyles, ';')

	SELECT
		* 
	FROM 
		PackingList 
	WHERE 
	ID IN 
		(
			SELECT
				PKL.PackingListID
			FROM
				PackingLine PKL (NOLOCK)
				JOIN #TempSearchLSStyle TLS ON TLS.LSStyle = PKL.LSStyle 
		)
END
GO