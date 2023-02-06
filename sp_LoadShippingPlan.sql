
-- =============================================
-- Author:		Nguyen Xuan Lam
-- Create date: 2022-09-07
-- Description:	Load Shipping Plan
-- =============================================
CREATE PROCEDURE sp_LoadShippingPlan
	@CustomerID NVARCHAR(1000),
	@Search NVARCHAR(1000)
AS
BEGIN
	
	SELECT * 
	FROM ShippingPlans (NOLOCK)
	WHERE ISNULL(CustomerID,'') LIKE N'%' + @CustomerID + '%'
	AND (ISNULL(Title,'') LIKE N'%' + @Search + '%'
		OR ISNULL(FilePath,'') LIKE N'%' + @Search + '%')
	AND ISNULL(IsDeleted,0) <> 1
END
GO
