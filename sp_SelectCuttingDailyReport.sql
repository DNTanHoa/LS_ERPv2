USE [ERPv2_Production]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Nguyen Vinh Truong
-- Create date: 2022-09-11
-- Description:	Get cutting output daily report
-- =============================================
CREATE PROCEDURE [dbo].[sp_SelectCuttingDailyReport]
	-- Add the parameters for the stored procedure here
	@CompanyID NVARCHAR(4000),
	@ProduceDate DATETIME
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;
		select b.WorkCenterName,b.LSStyle,b.[set],b.CustomerName,b.TargetQuantity,b.GarmentColor,sum(b.AllocQuantity) as Quantity,b.Remark
	from
	(
	select c.WorkCenterName,at.LSStyle,c.IsPrint,c.Quantity,c.[set],at.AllocQuantity,d.TargetQuantity,d.CustomerName,d.GarmentColor,d.Remark
	from CuttingOutput c
		 left join AllocTransaction at on at.CuttingOutputID = c.ID and at.lot is null
		 left join DailyTarget d on d.LSStyle = at.LSStyle 
	where c.IsPrint = 0
		and c.ProduceDate = @ProduceDate
		and c.WorkCenterID like '%'+ @CompanyID +'%'
	) b
	group by 
	b.WorkCenterName,b.LSStyle,b.[set],b.CustomerName,b.TargetQuantity,b.GarmentColor,b.Remark
		
END
