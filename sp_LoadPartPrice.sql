-- =============================================
-- Author:		LamNX
-- Create date: 2022-11-09
-- Description:	Load Part Price
-- =============================================
CREATE PROCEDURE [dbo].[sp_LoadPartPrice] 
	@CustomerID NVARCHAR(1000),
	@Search NVARCHAR(max)
AS
BEGIN
	
	SELECT * 
	FROM PartPrice (NOLOCK)
	WHERE ISNULL(CustomerID,'') LIKE IIF(ISNULL(@CustomerID,'')='', N'%', @CustomerID)
		AND (ISNULL(StyleNO,'') LIKE N'%' + @Search + '%'
		OR ISNULL(Season,'') LIKE N'%' + @Search + '%'
		OR ISNULL(GarmentColorCode,'') LIKE N'%' + @Search + '%'
		OR ISNULL(ProductionType,'') LIKE N'%' + @Search + '%')
		AND ISNULL(IsDeleted,0) <> 1
		AND CustomerID IS NOT NULL

END
