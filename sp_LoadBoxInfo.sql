-- =============================================
-- Author:		Nguyen Xuan Lam
-- Create date: 2022-09-05
-- Description:	Load Box Info
-- =============================================
CREATE PROCEDURE [dbo].[sp_LoadBoxInfo]
	@CustomerID nvarchar(max),
	@Search nvarchar(max)
AS
BEGIN

	SELECT * 
	FROM BoxInfo (nolock)
	WHERE ISNULL(IsDeleted,0) <> 1

END
