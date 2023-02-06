USE [ERPv2_Production]
GO
/****** Object:  StoredProcedure [dbo].[sp_SelectCuttingStatus]    Script Date: 07/16/2022 1:23:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Nguyen Vinh Truong
-- Create date: 2022-07-07
-- Description:	Get cutting output status report
-- =============================================
CREATE PROCEDURE [dbo].[sp_SelectCuttingStatus]
	-- Add the parameters for the stored procedure here
	@CompanyID NVARCHAR(4000),
	@FromDate DATETIME,
	@ToDate DATETIME,
	@CustomerName NVARCHAR(4000)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;

		select s1.PSDD,s1.MergeBlockLSStyle,s1.MergeLSStyle,s1.IsCanceled,s1.Remark,s1.LSStyle,s1.[Set],s1.Size,s1.GarmentColor, s1.OrderQuantity,s1.Quantity,s1.IsFull
				,s1.ContrastColor
				,s1.FabricContrastName
				,a.AllocQuantity,a.CuttingOutputID
				,c.WorkCenterName	
				,c.ProduceDate
		from 
			(SELECT d.PSDD,d.CustomerName,d.MergeBlockLSStyle,d.MergeLSStyle,d.IsCanceled,d.Remark,alloc.LSStyle,alloc.[Set],alloc.Size,d.GarmentColor, alloc.OrderQuantity,alloc.Quantity,alloc.IsFull
				,alloc.FabricContrastName		
				,f.ContrastColor	    
				FROM AllocDailyOutput alloc
					left join DailyTarget d on d.ID = alloc.TargetID
					left join FabricContrast f on f.ID = alloc.FabricContrastID 
				where alloc.Operation = 'CUTTING'
					and d.ID = alloc.TargetID
					and d.FromDate >= @FromDate
					and d.ToDate <= @ToDate
					and d.Operation = 'CUTTING'
					and d.CustomerName = @CustomerName
					and d.CompanyID = @CompanyID
					) s1
		left join AllocTransaction a		  
		on s1.LSStyle = a.LSStyle and s1.Size = a.Size and s1.[Set] = a.[Set] and a.Lot is null and a.IsRetured = 0 and a.FabricContrastName = s1.FabricContrastName
		left join CuttingOutput c on c.ID = a.CuttingOutputID	  
	
	order by s1.LSStyle,s1.size,s1.FabricContrastName 
END
