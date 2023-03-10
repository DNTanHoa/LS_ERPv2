
-- =============================================
-- Author:		Nguyen Xuan Lam
-- Create date: 2022-09-01
-- Description:	Load Part Master
-- =============================================
CREATE PROCEDURE [dbo].[sp_LoadPartMaster] 
	@CustomerID nvarchar(max),
	@Search nvarchar(max)
AS
BEGIN
	
	SELECT * 
	FROM PartMaster
	WHERE ISNULL(CustomerID,'') LIKE N'%' + @CustomerID + '%'
	AND (ISNULL(ExternalID,'') LIKE N'%' + @Search + '%'
	OR ISNULL(CustomerStyle,'') LIKE N'%' + @Search + '%'
	OR ISNULL(GarmentColorCode,'') LIKE N'%' + @Search + '%')
	AND ISNULL(IsDeleted,0) <> 1

END
