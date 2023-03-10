USE [ERPv2]
-- =============================================
-- Author:		Nguyen Xuan Lam
-- Create date: 2022-08-31
-- Description:	Load Loading Plan
-- =============================================
CREATE PROCEDURE [dbo].[sp_LoadLoadingPlan] 
	@CustomerID nvarchar(max),
	@ContainerNumber nvarchar(max),
	@OrderNumber nvarchar(max)
AS
BEGIN

	SELECT *
	FROM LoadingPlan
	WHERE ISNULL(CustomerID,'') LIKE N'%' + @CustomerID + '%' 
	AND ISNULL(ContainerNumber,'') LIKE N'%' + @ContainerNumber + '%'
	AND ISNULL(OrderNumber,'') LIKE N'%' + @OrderNumber + '%'

END
