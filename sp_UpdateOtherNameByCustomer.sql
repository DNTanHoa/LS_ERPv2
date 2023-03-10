--============================================
-- Author:		Nguyen Xuan Lam
-- Create date: 2022-08-20
-- Description:	Update Other Name Before Create Item Master
-- =============================================
CREATE PROCEDURE [dbo].[sp_UpdateOtherNameByCustomer]
	@CustomerID nvarchar(max),
	@Msg int output
AS
BEGIN
	--DECLARE @MSG INT
	SET NOCOUNT ON;

    BEGIN TRAN UpdateOtherName
    BEGIN TRY
		IF @CustomerID = 'DE'
		BEGIN
			--------- Other Name for RFID ---------
			update p set OtherName = N'RFID'
			from PartMaterial p 
				join PartRevision r on p.PartRevisionID=r.ID
			where p.ItemName like '%RFID%' 
			and r.CustomerID=@CustomerID and isnull(p.OtherName,'')=''

			update b set OtherName = N'RFID'
			from ProductionBOM b 
				join PartMaterial p on b.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where b.ItemName like '%RFID%' 
			and r.CustomerID=@CustomerID and isnull(b.OtherName,'')=''

			update f set OtherName = N'RFID'
			from ForecastMaterial f
				join PartMaterial p on f.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where f.ItemName like '%RFID%' 
			and r.CustomerID=@CustomerID and isnull(f.OtherName,'')=''

			update d set OtherName = N'RFID'
			from StorageDetail d
			where d.ItemName like '%RFID%' 
			and d.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update t set OtherName = N'RFID'
			from MaterialTransaction t
				join PurchaseOrder p on Replace(Trim(p.Number),' ','')=Replace(Trim(t.PurchaseOrderNumber),' ','')
			where t.ItemName like '%RFID%' 
			and p.CustomerID=@CustomerID and isnull(t.OtherName,'')=''

			update l set OtherName = N'RFID'
			from PurchaseOrderGroupLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where l.ItemName like '%RFID%' 
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set OtherName = N'RFID'
			from PurchaseOrderLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where l.ItemName like '%RFID%' 
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set OtherName = N'RFID'
			from IssuedLine l
				join Issued i on l.IssuedNumber=i.Number
			where l.ItemName like '%RFID%' 
			and i.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update d set OtherName = N'RFID'
			from MaterialRequestDetails d
				join MaterialRequests r on r.ID=d.MaterialRequestId
			where d.ItemName like '%RFID%' 
			and r.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update l set OtherName = N'RFID'
			from ReceiptGroupLine l
				join Receipt r on r.Number=l.ReceiptNumber
			where l.ItemName like '%RFID%' 
			and r.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			--------- Other Name for ADEQUAT ---------
			update p set OtherName = N'ADEQUAT'
			from PartMaterial p
				join PartRevision r on p.PartRevisionID=r.ID
			where (p.ItemName like '%ADEQUAT%' or Replace(p.ItemName,' ','') like '%LFTADQ%' or Replace(p.ItemName,' ','') like '%STKADQ%')
			and r.CustomerID=@CustomerID and isnull(OtherName,'')='' 

			update b set OtherName = N'ADEQUAT'
			from ProductionBOM b
				join PartMaterial p on b.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where (b.ItemName like '%ADEQUAT%' or Replace(b.ItemName,' ','') like '%LFTADQ%' or Replace(b.ItemName,' ','') like '%STKADQ%')
			and r.CustomerID=@CustomerID and isnull(b.OtherName,'')=''

			update f set OtherName = N'ADEQUAT'
			from ForecastMaterial f
				join PartMaterial p on f.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where (f.ItemName like '%ADEQUAT%' or Replace(f.ItemName,' ','') like '%LFTADQ%' or Replace(f.ItemName,' ','') like '%STKADQ%')
			and r.CustomerID=@CustomerID and isnull(f.OtherName,'')=''

			update d set OtherName = N'ADEQUAT'
			from StorageDetail d
			where (d.ItemName like '%ADEQUAT%' or Replace(d.ItemName,' ','') like '%LFTADQ%' or Replace(d.ItemName,' ','') like '%STKADQ%')
			and d.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update t set OtherName = N'ADEQUAT'
			from MaterialTransaction t
				join PurchaseOrder p on Replace(Trim(p.Number),' ','')=Replace(Trim(t.PurchaseOrderNumber),' ','')
			where (t.ItemName like '%ADEQUAT%' or Replace(t.ItemName,' ','') like '%LFTADQ%' or Replace(t.ItemName,' ','') like '%STKADQ%')
			and p.CustomerID=@CustomerID and isnull(t.OtherName,'')=''

			update l set OtherName = N'ADEQUAT'
			from PurchaseOrderGroupLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where (l.ItemName like '%ADEQUAT%' or Replace(l.ItemName,' ','') like '%LFTADQ%' or Replace(l.ItemName,' ','') like '%STKADQ%')
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set OtherName = N'ADEQUAT'
			from PurchaseOrderLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where (l.ItemName like '%ADEQUAT%' or Replace(l.ItemName,' ','') like '%LFTADQ%' or Replace(l.ItemName,' ','') like '%STKADQ%')
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set OtherName = N'ADEQUAT'
			from IssuedLine l
				join Issued i on l.IssuedNumber=i.Number
			where (l.ItemName like '%ADEQUAT%' or Replace(l.ItemName,' ','') like '%LFTADQ%' or Replace(l.ItemName,' ','') like '%STKADQ%')
			and i.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update d set OtherName = N'ADEQUAT'
			from MaterialRequestDetails d
				join MaterialRequests r on r.ID=d.MaterialRequestId
			where (d.ItemName like '%ADEQUAT%' or Replace(d.ItemName,' ','') like '%LFTADQ%' or Replace(d.ItemName,' ','') like '%STKADQ%')
			and r.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update l set OtherName = N'ADEQUAT'
			from ReceiptGroupLine l
				join Receipt r on r.Number=l.ReceiptNumber
			where (l.ItemName like '%ADEQUAT%' or Replace(l.ItemName,' ','') like '%LFTADQ%' or Replace(l.ItemName,' ','') like '%STKADQ%')
			and r.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			--------- Other Name for STITCHING THREAD ---------
			update p set OtherName = N'STITCHING THREAD',DefaultThreadName=t.FullName
			from PartMaterial p
				join PartRevision r on p.PartRevisionID=r.ID
				join StitchingThread t on r.CustomerID=t.CustomerID
			where p.VendorID in ('Hunglong','THESEUS')
			and r.CustomerID=@CustomerID and isnull(p.OtherName,'')=''
			and Replace(p.ItemName,' ','') Like '%'+ Trim(t.shortname) + '%'

			update b set OtherName = N'STITCHING THREAD',DefaultThreadName=t.FullName
			from ProductionBOM b
				join PartMaterial p on b.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
				join StitchingThread t on r.CustomerID=t.CustomerID
			where b.VendorID in ('Hunglong','THESEUS')
			and r.CustomerID=@CustomerID and isnull(b.OtherName,'')=''
			and Replace(b.ItemName,' ','') Like '%'+ Trim(t.shortname) + '%'

			update f set OtherName = N'STITCHING THREAD',DefaultThreadName=t.FullName
			from ForecastMaterial f
				join PartMaterial p on f.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
				join StitchingThread t on r.CustomerID=t.CustomerID
			where f.VendorID in ('Hunglong','THESEUS')
			and r.CustomerID=@CustomerID and isnull(f.OtherName,'')=''
			and Replace(f.ItemName,' ','') Like '%'+ Trim(t.shortname) + '%'

			update d set OtherName = N'STITCHING THREAD',DefaultThreadName=t.FullName
			from StorageDetail d
				join PurchaseOrder p on Replace(Trim(p.Number),' ','')=Replace(Trim(d.PurchaseOrderNumber),' ','')
				join StitchingThread t on d.CustomerID=t.CustomerID
			where p.VendorID in ('Hunglong','THESEUS')
			and d.CustomerID=@CustomerID and isnull(d.OtherName,'')=''
			and Replace(d.ItemName,' ','') Like '%'+ Trim(t.shortname) + '%'

			update t set OtherName = N'STITCHING THREAD',DefaultThreadName=s.FullName
			from MaterialTransaction t
				join PurchaseOrder p on Replace(Trim(p.Number),' ','')=Replace(Trim(t.PurchaseOrderNumber),' ','')
				join StitchingThread s on s.CustomerID=p.CustomerID
			where p.VendorID in ('Hunglong','THESEUS')
			and p.CustomerID=@CustomerID and isnull(t.OtherName,'')=''
			and Replace(t.ItemName,' ','') Like '%'+ Trim(s.shortname) + '%'

			update l set OtherName = N'STITCHING THREAD',DefaultThreadName=t.FullName
			from PurchaseOrderGroupLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
				join StitchingThread t on p.CustomerID=t.CustomerID
			where p.VendorID in ('Hunglong','THESEUS')
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''
			and Replace(l.ItemName,' ','') Like '%'+ Trim(t.shortname) + '%'

			update l set OtherName = N'STITCHING THREAD',DefaultThreadName=t.FullName
			from PurchaseOrderLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
				join StitchingThread t on p.CustomerID=t.CustomerID
			where p.VendorID in ('Hunglong','THESEUS')
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''
			and Replace(l.ItemName,' ','') Like '%'+ Trim(t.shortname) + '%'

			update l set OtherName = N'STITCHING THREAD',DefaultThreadName=t.FullName
			from ReceiptGroupLine l
				join Receipt r on r.Number=l.ReceiptNumber
				join StitchingThread t on r.CustomerID=t.CustomerID
			where r.VendorID in ('Hunglong','THESEUS')
			and r.CustomerID=@CustomerID and isnull(l.OtherName,'')=''
			and Replace(l.ItemName,' ','') Like '%'+ Trim(t.shortname) + '%'

			update l set l.DefaultThreadName=t.FullName, l.OtherName= N'STITCHING THREAD'
			from IssuedLine l 
				join Issued i on l.IssuedNumber=i.Number
				join StitchingThread t On i.CustomerID = t.CustomerID
			where isnull(l.DefaultThreadName,'')='' and isnull(l.OtherName,'')=''
				and Replace(l.ItemName,' ','') Like N'%'+ Trim(t.shortname) + '%' 
				and i.CustomerID=@CustomerID 

			update d set d.DefaultThreadName=t.FullName,  d.OtherName= N'STITCHING THREAD'
			from MaterialRequestDetails d 
				join MaterialRequests r On d.MaterialRequestId=r.Id
				join StitchingThread t On r.CustomerID=t.CustomerID
			where isnull(d.DefaultThreadName,'')='' and isnull(d.OtherName,'')=''
				and Replace(d.ItemName,' ','') Like N'%'+ Trim(t.shortname) + '%' 
				and r.CustomerID=@CustomerID

			-------- Other Name for SIZE + CARE LABEL  ---------
			update p set OtherName = N'SIZE + CARE LABEL'
			from PartMaterial p
				join PartRevision r on p.PartRevisionID=r.ID
			where (Replace(p.ItemName,' ','') like '%CARELABEL%' or Replace(p.ItemName,' ','') like '%CARE+SIZE%' or Replace(p.ItemName,' ','') like '%SIZELABEL%' or (p.ItemName like '%LBL%' and p.VendorID in ('TAGTIME','VIETPHU')))
			and r.CustomerID=@CustomerID and isnull(OtherName,'')=''

			update b set OtherName =N'SIZE + CARE LABEL'
			from ProductionBOM b
				join PartMaterial p on b.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where (Replace(b.ItemName,' ','') like '%CARELABEL%' or Replace(b.ItemName,' ','') like '%CARE+SIZE%' or Replace(b.ItemName,' ','') like '%SIZELABEL%' or (b.ItemName like '%LBL%' and b.VendorID in ('TAGTIME','VIETPHU')))
			and r.CustomerID=@CustomerID and isnull(b.OtherName,'')=''

			update f set OtherName = N'SIZE + CARE LABEL'
			from ForecastMaterial f
				join PartMaterial p on f.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where (Replace(f.ItemName,' ','') like '%CARELABEL%' or Replace(f.ItemName,' ','') like '%CARE+SIZE%' or Replace(f.ItemName,' ','') like '%SIZELABEL%' or (f.ItemName like '%LBL%' and f.VendorID in ('TAGTIME','VIETPHU')))
			and r.CustomerID=@CustomerID and isnull(f.OtherName,'')=''

			update d set OtherName = N'SIZE + CARE LABEL'
			from StorageDetail d
				join PurchaseOrder p on Replace(Trim(p.Number),' ','')=Replace(Trim(d.PurchaseOrderNumber),' ','')
			where (Replace(d.ItemName,' ','') like '%CARELABEL%' or Replace(d.ItemName,' ','') like '%CARE+SIZE%' or Replace(d.ItemName,' ','') like '%SIZELABEL%' or (d.ItemName like '%LBL%' and p.VendorID in ('TAGTIME','VIETPHU')))
			and d.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update t set OtherName = N'SIZE + CARE LABEL'
			from MaterialTransaction t
				join PurchaseOrder p on Replace(Trim(p.Number),' ','')=Replace(Trim(t.PurchaseOrderNumber),' ','')
			where (Replace(t.ItemName,' ','') like '%CARELABEL%' or Replace(t.ItemName,' ','') like '%CARE+SIZE%' or Replace(t.ItemName,' ','') like '%SIZELABEL%' or (t.ItemName like '%LBL%' and p.VendorID in ('TAGTIME','VIETPHU')))
			and p.CustomerID=@CustomerID and isnull(t.OtherName,'')=''

			update l set OtherName = N'SIZE + CARE LABEL'
			from PurchaseOrderGroupLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where (Replace(l.ItemName,' ','') like '%CARELABEL%' or Replace(l.ItemName,' ','') like '%CARE+SIZE%' or Replace(l.ItemName,' ','') like '%SIZELABEL%' or (l.ItemName like '%LBL%' and p.VendorID in ('TAGTIME','VIETPHU')))
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set OtherName = N'SIZE + CARE LABEL'
			from PurchaseOrderLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where (Replace(l.ItemName,' ','') like '%CARELABEL%' or Replace(l.ItemName,' ','') like '%CARE+SIZE%' or Replace(l.ItemName,' ','') like '%SIZELABEL%' or (l.ItemName like '%LBL%' and p.VendorID in ('TAGTIME','VIETPHU')))
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set OtherName = N'SIZE + CARE LABEL'
			from IssuedLine l
				join Issued i on l.IssuedNumber=i.Number
			where (Replace(l.ItemName,' ','') like '%CARELABEL%' or Replace(l.ItemName,' ','') like '%CARE+SIZE%' or Replace(l.ItemName,' ','') like '%SIZELABEL%' or Replace(l.ItemName,' ','') like '%LBLUNFLD%')
			and i.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update d set OtherName = N'SIZE + CARE LABEL'
			from MaterialRequestDetails d
				join MaterialRequests r on r.ID=d.MaterialRequestId
			where (Replace(d.ItemName,' ','') like '%CARELABEL%' or Replace(d.ItemName,' ','') like '%CARE+SIZE%' or Replace(d.ItemName,' ','') like '%SIZELABEL%' or Replace(d.ItemName,' ','') like '%LBLUNFLD%')
			and r.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update l set OtherName = N'SIZE + CARE LABEL'
			from ReceiptGroupLine l
				join Receipt r on r.Number=l.ReceiptNumber
			where (Replace(l.ItemName,' ','') like '%CARELABEL%' or Replace(l.ItemName,' ','') like '%CARE+SIZE%' or Replace(l.ItemName,' ','') like '%SIZELABEL%' or (l.ItemName like '%LBL%' and r.VendorID in ('TAGTIME','VIETPHU')))
			and r.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			--------- Other Name for ELASTIC ---------
			update p set OtherName = N'ELASTIC'
			from PartMaterial p
				join PartRevision r on p.PartRevisionID=r.ID
			where (p.ItemName like '%ELASTIC%' or (p.ItemName like 'EL%' and p.VendorID in('HUAYAN','LIENCHAU','YOUNGLION')))
			and r.CustomerID=@CustomerID and isnull(p.OtherName,'')=''

			update b set OtherName = N'ELASTIC'
			from ProductionBOM b
				join PartMaterial p on b.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where (b.ItemName like '%ELASTIC%' or (b.ItemName like 'EL%' and b.VendorID in('HUAYAN','LIENCHAU','YOUNGLION')))
			and r.CustomerID=@CustomerID and isnull(b.OtherName,'')=''

			update f set OtherName = N'ELASTIC'
			from ForecastMaterial f
				join PartMaterial p on f.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where (f.ItemName like '%ELASTIC%' or (f.ItemName like 'EL%' and f.VendorID in('HUAYAN','LIENCHAU','YOUNGLION')))
			and r.CustomerID=@CustomerID and isnull(f.OtherName,'')=''

			update d set OtherName = N'ELASTIC'
			from StorageDetail d
				join PurchaseOrder p on Replace(Trim(p.Number),' ','')=Replace(Trim(d.PurchaseOrderNumber),' ','')
			where (d.ItemName like '%ELASTIC%' or (d.ItemName like 'EL%' and p.VendorID in('HUAYAN','LIENCHAU','YOUNGLION')))
			and p.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update t set OtherName = N'ELASTIC'
			from MaterialTransaction t
				join PurchaseOrder p on Replace(Trim(p.Number),' ','')=Replace(Trim(t.PurchaseOrderNumber),' ','')
			where (t.ItemName like '%ELASTIC%' or (t.ItemName like 'EL%' and p.VendorID in('HUAYAN','LIENCHAU','YOUNGLION')))
			and p.CustomerID=@CustomerID and isnull(t.OtherName,'')=''

			update l set OtherName = N'ELASTIC'
			from PurchaseOrderGroupLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where (l.ItemName like '%ELASTIC%' or (l.ItemName like 'EL%' and p.VendorID in('HUAYAN','LIENCHAU','YOUNGLION')))
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set OtherName = N'ELASTIC'
			from PurchaseOrderLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where (l.ItemName like '%ELASTIC%' or (l.ItemName like 'EL%' and p.VendorID in('HUAYAN','LIENCHAU','YOUNGLION')))
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set OtherName = N'ELASTIC'
			from IssuedLine l
				join Issued i on l.IssuedNumber=i.Number
			where (l.ItemName like '%ELASTIC%' or l.ItemName like 'ELNECKTAPE%')
			and i.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update d set OtherName = N'ELASTIC'
			from MaterialRequestDetails d
				join MaterialRequests r on r.ID=d.MaterialRequestId
			where (d.ItemName like '%ELASTIC%' or d.ItemName like 'ELNECKTAPE%')
			and r.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update l set OtherName = N'ELASTIC'
			from ReceiptGroupLine l
				join Receipt r on r.Number=l.ReceiptNumber
			where (l.ItemName like '%ELASTIC%' or (l.ItemName like 'EL%' and r.VendorID in('HUAYAN','LIENCHAU','YOUNGLION')))
			and r.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			--------- Other Name for TRACEABILITY LABEL ---------
			update p set OtherName = N'TRACEABILITY LABEL'
			from PartMaterial p
				join PartRevision r on p.PartRevisionID=r.ID
			where Replace(p.ItemName,' ','') like '%TRACEABILITYLABEL%' 
			and r.CustomerID=@CustomerID and isnull(p.OtherName,'')=''

			update b set OtherName = N'TRACEABILITY LABEL'
			from ProductionBOM b
				join PartMaterial p on b.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where Replace(b.ItemName,' ','') like '%TRACEABILITYLABEL%' 
			and r.CustomerID=@CustomerID and isnull(b.OtherName,'')=''

			update f set OtherName = N'TRACEABILITY LABEL'
			from ForecastMaterial f
				join PartMaterial p on f.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where Replace(f.ItemName,' ','') like '%TRACEABILITYLABEL%' 
			and r.CustomerID=@CustomerID and isnull(f.OtherName,'')=''

			update d set OtherName = N'TRACEABILITY LABEL'
			from StorageDetail d
			where Replace(d.ItemName,' ','') like '%TRACEABILITYLABEL%' 
			and d.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update t set OtherName = N'TRACEABILITY LABEL'
			from MaterialTransaction t
				join PurchaseOrder p on Replace(Trim(p.Number),' ','')=Replace(Trim(t.PurchaseOrderNumber),' ','')
			where Replace(t.ItemName,' ','') like '%TRACEABILITYLABEL%' 
			and p.CustomerID=@CustomerID and isnull(t.OtherName,'')=''

			update l set OtherName = N'TRACEABILITY LABEL'
			from PurchaseOrderGroupLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where Replace(l.ItemName,' ','') like '%TRACEABILITYLABEL%' 
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set OtherName = N'TRACEABILITY LABEL'
			from PurchaseOrderLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where Replace(l.ItemName,' ','') like '%TRACEABILITYLABEL%' 
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set OtherName = N'TRACEABILITY LABEL'
			from IssuedLine l
				join Issued i on l.IssuedNumber=i.Number
			where Replace(l.ItemName,' ','') like '%TRACEABILITYLABEL%' 
			and i.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update d set OtherName = N'TRACEABILITY LABEL'
			from MaterialRequestDetails d
				join MaterialRequests r on r.ID=d.MaterialRequestId
			where Replace(d.ItemName,' ','') like '%TRACEABILITYLABEL%' 
			and r.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update l set OtherName = N'TRACEABILITY LABEL'
			from ReceiptGroupLine l
				join Receipt r on r.Number=l.ReceiptNumber
			where Replace(l.ItemName,' ','') like '%TRACEABILITYLABEL%' 
			and r.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			--------- Other Name for LAMINET ---------
			update p set OtherName = N'LAMINET'
			from PartMaterial p
				join PartRevision r on p.PartRevisionID=r.ID
			where p.VendorID in ('UNION')
			and r.CustomerID=@CustomerID and isnull(OtherName,'')=''

			update b set OtherName = N'LAMINET'
			from ProductionBOM b
				join PartMaterial p on b.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where b.VendorID in ('UNION')
			and r.CustomerID=@CustomerID and isnull(b.OtherName,'')=''

			update f set OtherName = N'LAMINET'
			from ForecastMaterial f
				join PartMaterial p on f.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where f.VendorID in ('UNION')
			and r.CustomerID=@CustomerID and isnull(f.OtherName,'')=''

			update d set OtherName = N'LAMINET'
			from StorageDetail d
				join PurchaseOrder p on Replace(Trim(p.Number),' ','')=Replace(Trim(d.PurchaseOrderNumber),' ','')
			where p.VendorID in ('UNION')
			and p.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update t set OtherName = N'LAMINET'
			from MaterialTransaction t
				join PurchaseOrder p on Replace(Trim(p.Number),' ','')=Replace(Trim(t.PurchaseOrderNumber),' ','')
			where p.VendorID in ('UNION')
			and p.CustomerID=@CustomerID and isnull(t.OtherName,'')=''

			update l set OtherName = N'LAMINET'
			from PurchaseOrderGroupLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where p.VendorID in ('UNION')
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set OtherName = N'LAMINET'
			from PurchaseOrderLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where p.VendorID in ('UNION')
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set OtherName = N'LAMINET'
			from IssuedLine l
				join Issued i on l.IssuedNumber=i.Number
			where l.ItemName like N'%LAMINET%'
			and i.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update d set OtherName = N'LAMINET'
			from MaterialRequestDetails d
				join MaterialRequests r on r.ID=d.MaterialRequestId
			where d.ItemName like N'%LAMINET%'
			and r.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update l set OtherName = N'LAMINET'
			from ReceiptGroupLine l
				join Receipt r on r.Number=l.ReceiptNumber
			where r.VendorID in ('UNION')
			and r.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			--------- Other Name for SIZE HT ---------
			update p set OtherName = N'SIZE HT'
			from PartMaterial p
				join PartRevision r on p.PartRevisionID=r.ID
			where Replace(p.ItemName,' ','') like '%SIZEHT%'
			and r.CustomerID=@CustomerID and isnull(p.OtherName,'')=''

			update b set OtherName = N'SIZE HT'
			from ProductionBOM b
				join PartMaterial p on b.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where Replace(p.ItemName,' ','') like '%SIZEHT%'
			and r.CustomerID=@CustomerID and isnull(b.OtherName,'')=''

			update f set OtherName = N'SIZE HT'
			from ForecastMaterial f
				join PartMaterial p on f.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where Replace(p.ItemName,' ','') like '%SIZEHT%'
			and r.CustomerID=@CustomerID and isnull(f.OtherName,'')=''

			update d set OtherName = N'SIZE HT'
			from StorageDetail d
			where Replace(d.ItemName,' ','') like '%SIZEHT%'
			and d.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update t set OtherName = N'SIZE HT'
			from MaterialTransaction t
				join PurchaseOrder p on Replace(Trim(p.Number),' ','')=Replace(Trim(t.PurchaseOrderNumber),' ','')
			where Replace(t.ItemName,' ','') like '%SIZEHT%'
			and p.CustomerID=@CustomerID and isnull(t.OtherName,'')=''

			update l set OtherName = N'SIZE HT'
			from PurchaseOrderGroupLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where Replace(l.ItemName,' ','') like '%SIZEHT%'
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set OtherName = N'SIZE HT'
			from PurchaseOrderLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where Replace(l.ItemName,' ','') like '%SIZEHT%'
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set OtherName = N'SIZE HT'
			from IssuedLine l
				join Issued i on l.IssuedNumber=i.Number
			where  Replace(l.ItemName,' ','') like '%SIZEHT%'
			and i.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update d set OtherName = N'SIZE HT'
			from MaterialRequestDetails d
				join MaterialRequests r on r.ID=d.MaterialRequestId
			where  Replace(d.ItemName,' ','') like '%SIZEHT%'
			and r.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update l set OtherName = N'SIZE HT'
			from ReceiptGroupLine l
				join Receipt r on r.Number=l.ReceiptNumber
			where Replace(l.ItemName,' ','') like '%SIZEHT%'
			and r.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			--------- Other Name for HEAT TRANSFER ---------
			update p set OtherName = N'HEAT TRANSFER'
			from PartMaterial p
				join PartRevision r on p.PartRevisionID=r.ID
			where (Replace(p.ItemName,' ','') like '%HEATTRANSFER%' or p.ItemName like 'HT%' or p.ItemName like '%HTL%')
			and r.CustomerID=@CustomerID and isnull(p.OtherName,'')=''

			update b set OtherName = N'HEAT TRANSFER'
			from ProductionBOM b
				join PartMaterial p on b.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where (Replace(b.ItemName,' ','') like '%HEATTRANSFER%' or b.ItemName like 'HT%' or b.ItemName like '%HTL%')
			and r.CustomerID=@CustomerID and isnull(b.OtherName,'')=''

			update f set OtherName = N'HEAT TRANSFER'
			from ForecastMaterial f
				join PartMaterial p on f.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where (Replace(f.ItemName,' ','') like '%HEATTRANSFER%' or f.ItemName like 'HT%' or f.ItemName like '%HTL%')
			and r.CustomerID=@CustomerID and isnull(f.OtherName,'')=''

			update d set OtherName = N'HEAT TRANSFER'
			from StorageDetail d
			where (Replace(d.ItemName,' ','') like '%HEATTRANSFER%' or d.ItemName like 'HT%' or d.ItemName like '%HTL%')
			and d.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update t set OtherName = N'HEAT TRANSFER'
			from MaterialTransaction t
				join PurchaseOrder p on Replace(Trim(p.Number),' ','')=Replace(Trim(t.PurchaseOrderNumber),' ','')
			where (Replace(t.ItemName,' ','') like '%HEATTRANSFER%' or t.ItemName like 'HT%' or t.ItemName like '%HTL%')
			and p.CustomerID=@CustomerID and isnull(t.OtherName,'')=''

			update l set OtherName = N'HEAT TRANSFER'
			from PurchaseOrderGroupLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where (Replace(l.ItemName,' ','') like '%HEATTRANSFER%' or l.ItemName like 'HT%' or l.ItemName like '%HTL%')
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set OtherName = N'HEAT TRANSFER'
			from PurchaseOrderLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where (Replace(l.ItemName,' ','') like '%HEATTRANSFER%' or l.ItemName like 'HT%' or l.ItemName like '%HTL%')
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set OtherName = N'HEAT TRANSFER'
			from IssuedLine l
				join Issued i on l.IssuedNumber=i.Number
			where (Replace(l.ItemName,' ','') like '%HEATTRANSFER%' or l.ItemName like 'HT%' or l.ItemName like '%HTL%')
			and i.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update d set OtherName = N'HEAT TRANSFER'
			from MaterialRequestDetails d
				join MaterialRequests r on r.ID=d.MaterialRequestId
			where (Replace(d.ItemName,' ','') like '%HEATTRANSFER%' or d.ItemName like 'HT%' or d.ItemName like '%HTL%')
			and r.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update l set OtherName = N'HEAT TRANSFER'
			from ReceiptGroupLine l
				join Receipt r on r.Number=l.ReceiptNumber
			where (Replace(l.ItemName,' ','') like '%HEATTRANSFER%' or l.ItemName like 'HT%' or l.ItemName like '%HTL%')
			and r.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			--------- Other Name for ZIPPER ---------
			update p set OtherName = N'ZIPPER'
			from PartMaterial p
				join PartRevision r on p.PartRevisionID=r.ID
			where p.VendorID in ('YKK','MAX ZIPPER','SBS')
			and r.CustomerID=@CustomerID and isnull(p.OtherName,'')=''

			update b set OtherName = N'ZIPPER'
			from ProductionBOM b
				join PartMaterial p on b.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where b.VendorID in ('YKK','MAX ZIPPER','SBS')
			and r.CustomerID=@CustomerID and isnull(b.OtherName,'')=''

			update f set OtherName = N'ZIPPER'
			from ForecastMaterial f
				join PartMaterial p on f.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where f.VendorID in ('YKK','MAX ZIPPER','SBS')
			and r.CustomerID=@CustomerID and isnull(f.OtherName,'')=''

			update d set OtherName = N'ZIPPER'
			from StorageDetail d
				join PurchaseOrder p on Replace(Trim(p.Number),' ','')=Replace(Trim(d.PurchaseOrderNumber),' ','')
			where p.VendorID in ('YKK','MAX ZIPPER','SBS')
			and p.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update t set OtherName = N'ZIPPER'
			from MaterialTransaction t
				join PurchaseOrder p on Replace(Trim(p.Number),' ','')=Replace(Trim(t.PurchaseOrderNumber),' ','')
			where p.VendorID in ('YKK','MAX ZIPPER','SBS')
			and p.CustomerID=@CustomerID and isnull(t.OtherName,'')=''

			update l set OtherName = N'ZIPPER'
			from PurchaseOrderGroupLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where p.VendorID in ('YKK','MAX ZIPPER','SBS')
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set OtherName = N'ZIPPER'
			from PurchaseOrderLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where p.VendorID in ('YKK','MAX ZIPPER','SBS')
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set OtherName = N'ZIPPER'
			from IssuedLine l
				join Issued i on l.IssuedNumber=i.Number
			where (Replace(l.ItemName,' ','') like '%COIL%' or Replace(l.ItemName,' ','') like '%Puller%' or
				Replace(l.ItemName,' ','') like '%ZVIS%' or Replace(l.ItemName,' ','') like '%ZCMET%' or
				Replace(l.ItemName,' ','') like '%ZIPPER%') and i.CustomerID=@CustomerID and isnull(l.OtherName,'')=''
			and i.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update d set OtherName = N'ZIPPER'
			from MaterialRequestDetails d
				join MaterialRequests r on r.ID=d.MaterialRequestId
			where (Replace(d.ItemName,' ','') like '%COIL%' or Replace(d.ItemName,' ','') like '%Puller%' or
				Replace(d.ItemName,' ','') like '%ZVIS%' or Replace(d.ItemName,' ','') like '%ZCMET%' or
				Replace(d.ItemName,' ','') like '%ZIPPER%')	and r.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update l set OtherName = N'ZIPPER'
			from ReceiptGroupLine l
				join Receipt r on r.Number=l.ReceiptNumber
			where r.VendorID in ('YKK','MAX ZIPPER','SBS')
			and r.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			-------- Other Name for HANGER ---------
			update p set OtherName = N'HANGER'
			from PartMaterial p
				join PartRevision r on p.PartRevisionID=r.ID
			where Replace(p.ItemName,' ','') like '%HANGUP%'
			and r.CustomerID=@CustomerID and isnull(p.OtherName,'')=''

			update b set OtherName = N'HANGER'
			from ProductionBOM b
				join PartMaterial p on b.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where Replace(b.ItemName,' ','') like '%HANGUP%'
			and r.CustomerID=@CustomerID and isnull(b.OtherName,'')=''

			 update f set OtherName = N'HANGER'
			from ForecastMaterial f
				join PartMaterial p on f.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where Replace(f.ItemName,' ','') like '%HANGUP%'
			and r.CustomerID=@CustomerID and isnull(f.OtherName,'')=''

			update d set OtherName = N'HANGER'
			from StorageDetail d
			where Replace(d.ItemName,' ','') like '%HANGUP%'
			and d.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update t set OtherName = N'HANGER'
			from MaterialTransaction t
				join PurchaseOrder p on Replace(Trim(p.Number),' ','')=Replace(Trim(t.PurchaseOrderNumber),' ','')
			where Replace(t.ItemName,' ','') like '%HANGUP%'
			and p.CustomerID=@CustomerID and isnull(t.OtherName,'')=''

			update l set OtherName = N'HANGER'
			from PurchaseOrderGroupLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where Replace(l.ItemName,' ','') like '%HANGUP%'
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set OtherName = N'HANGER'
			from PurchaseOrderLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where Replace(l.ItemName,' ','') like '%HANGUP%'
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set OtherName = N'HANGER'
			from IssuedLine l
				join Issued i on l.IssuedNumber=i.Number
			where Replace(l.ItemName,' ','') like '%HANGUP%'
			and i.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update d set OtherName = N'HANGER'
			from MaterialRequestDetails d
				join MaterialRequests r on r.ID=d.MaterialRequestId
			where Replace(d.ItemName,' ','') like '%HANGUP%'
			and r.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update l set OtherName = N'HANGER'
			from ReceiptGroupLine l
				join Receipt r on r.Number=l.ReceiptNumber
			where Replace(l.ItemName,' ','') like '%HANGUP%'
			and r.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			-------- Other Name for DRAWSTRINGS ---------
			update p set OtherName = N'DRAWSTRINGS'
			from PartMaterial p
				join PartRevision r on p.PartRevisionID=r.ID
			where (Replace(p.ItemName,' ','') like '%DRAWCORD%' or Replace(p.ItemName,' ','') like '%DRAWSTRING%' or Replace(p.ItemName,' ','') like '%DRAWROUND%' or Replace(p.ItemName,' ','') like '%RIBBONTUBULAR%')
			and r.CustomerID=@CustomerID and isnull(p.OtherName,'')=''

			update b set OtherName = N'DRAWSTRINGS'
			from ProductionBOM b
				join PartMaterial p on b.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where (Replace(b.ItemName,' ','') like '%DRAWCORD%' or Replace(b.ItemName,' ','') like '%DRAWSTRING%' or Replace(b.ItemName,' ','') like '%DRAWROUND%' or Replace(b.ItemName,' ','') like '%RIBBONTUBULAR%')
			and r.CustomerID=@CustomerID and isnull(b.OtherName,'')=''

			 update f set OtherName = N'DRAWSTRINGS'
			from ForecastMaterial f
				join PartMaterial p on f.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where (Replace(f.ItemName,' ','') like '%DRAWCORD%' or Replace(f.ItemName,' ','') like '%DRAWSTRING%' or Replace(f.ItemName,' ','') like '%DRAWROUND%' or Replace(f.ItemName,' ','') like '%RIBBONTUBULAR%')
			and r.CustomerID=@CustomerID and isnull(f.OtherName,'')=''

			update d set OtherName = N'DRAWSTRINGS'
			from StorageDetail d
			where (Replace(d.ItemName,' ','') like '%DRAWCORD%' or Replace(d.ItemName,' ','') like '%DRAWSTRING%' or Replace(d.ItemName,' ','') like '%DRAWROUND%' or Replace(d.ItemName,' ','') like '%RIBBONTUBULAR%')
			and d.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update t set OtherName = N'DRAWSTRINGS'
			from MaterialTransaction t
				join PurchaseOrder p on Replace(Trim(p.Number),' ','')=Replace(Trim(t.PurchaseOrderNumber),' ','')
			where (Replace(t.ItemName,' ','') like '%DRAWCORD%' or Replace(t.ItemName,' ','') like '%DRAWSTRING%' or Replace(t.ItemName,' ','') like '%DRAWROUND%' or Replace(t.ItemName,' ','') like '%RIBBONTUBULAR%')
			and p.CustomerID=@CustomerID and isnull(t.OtherName,'')=''

			update l set OtherName = N'DRAWSTRINGS'
			from PurchaseOrderGroupLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where (Replace(l.ItemName,' ','') like '%DRAWCORD%' or Replace(l.ItemName,' ','') like '%DRAWSTRING%' or Replace(l.ItemName,' ','') like '%DRAWROUND%' or Replace(l.ItemName,' ','') like '%RIBBONTUBULAR%')
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set OtherName = N'DRAWSTRINGS'
			from PurchaseOrderLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where (Replace(l.ItemName,' ','') like '%DRAWCORD%' or Replace(l.ItemName,' ','') like '%DRAWSTRING%' or Replace(l.ItemName,' ','') like '%DRAWROUND%' or Replace(l.ItemName,' ','') like '%RIBBONTUBULAR%')
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set OtherName = N'DRAWSTRINGS'
			from IssuedLine l
				join Issued i on l.IssuedNumber=i.Number
			where (Replace(l.ItemName,' ','') like '%DRAWCORD%' or Replace(l.ItemName,' ','') like '%DRAWSTRING%' or Replace(l.ItemName,' ','') like '%DRAWROUND%' or Replace(l.ItemName,' ','') like '%RIBBONTUBULAR%')
			and i.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update d set OtherName = N'DRAWSTRINGS'
			from MaterialRequestDetails d
				join MaterialRequests r on r.ID=d.MaterialRequestId
			where (Replace(d.ItemName,' ','') like '%DRAWCORD%' or Replace(d.ItemName,' ','') like '%DRAWSTRING%' or Replace(d.ItemName,' ','') like '%DRAWROUND%' or Replace(d.ItemName,' ','') like '%RIBBONTUBULAR%')
			and r.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update l set OtherName = N'DRAWSTRINGS'
			from ReceiptGroupLine l
				join Receipt r on r.Number=l.ReceiptNumber
			where  (Replace(l.ItemName,' ','') like '%DRAWCORD%' or Replace(l.ItemName,' ','') like '%DRAWSTRING%' or Replace(l.ItemName,' ','') like '%DRAWROUND%' or Replace(l.ItemName,' ','') like '%RIBBONTUBULAR%')
			and r.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			--------- Other Name for LOOP ---------
			update p set OtherName = N'LOOP'
			from PartMaterial p
				join PartRevision r on p.PartRevisionID=r.ID
			where Replace(p.ItemName,' ','') like '%SATINDOUBLEFACE%'
			and r.CustomerID=@CustomerID and isnull(p.OtherName,'')=''

			update b set OtherName = N'LOOP'
			from ProductionBOM b
				join PartMaterial p on b.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where Replace(p.ItemName,' ','') like '%SATINDOUBLEFACE%'
			and r.CustomerID=@CustomerID and isnull(b.OtherName,'')=''

			update f set OtherName = N'LOOP'
			from ForecastMaterial f
				join PartMaterial p on f.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where Replace(p.ItemName,' ','') like '%SATINDOUBLEFACE%'
			and r.CustomerID=@CustomerID and isnull(f.OtherName,'')=''

			update d set OtherName = N'LOOP'
			from StorageDetail d
			where Replace(d.ItemName,' ','') like '%SATINDOUBLEFACE%'
			and d.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update t set OtherName = N'LOOP'
			from MaterialTransaction t
				join PurchaseOrder p on Replace(Trim(p.Number),' ','')=Replace(Trim(t.PurchaseOrderNumber),' ','')
			where Replace(t.ItemName,' ','') like '%SATINDOUBLEFACE%'
			and p.CustomerID=@CustomerID and isnull(t.OtherName,'')=''

			update l set OtherName = N'LOOP'
			from PurchaseOrderGroupLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where Replace(l.ItemName,' ','') like '%SATINDOUBLEFACE%'
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set OtherName = N'LOOP'
			from PurchaseOrderLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where Replace(l.ItemName,' ','') like '%SATINDOUBLEFACE%'
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set OtherName = N'LOOP'
			from IssuedLine l
				join Issued i on l.IssuedNumber=i.Number
			where Replace(l.ItemName,' ','') like '%SATINDOUBLEFACE%'
			and i.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update d set OtherName = N'LOOP'
			from MaterialRequestDetails d
				join MaterialRequests r on r.ID=d.MaterialRequestId
			where Replace(d.ItemName,' ','') like '%SATINDOUBLEFACE%'
			and r.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update l set OtherName = N'LOOP'
			from ReceiptGroupLine l
				join Receipt r on r.Number=l.ReceiptNumber
			where Replace(l.ItemName,' ','') like '%SATINDOUBLEFACE%'
			and r.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			--------- Other Name for INSIDE LOOP ---------
			update p set OtherName = N'INSIDE LOOP'
			from PartMaterial p
				join PartRevision r on p.PartRevisionID=r.ID
			where Replace(p.ItemName,' ','') like '%GROSGRAIN%'
			and r.CustomerID=@CustomerID and isnull(p.OtherName,'')=''

			update b set OtherName = N'INSIDE LOOP'
			from ProductionBOM b
				join PartMaterial p on b.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where Replace(p.ItemName,' ','') like '%GROSGRAIN%'
			and r.CustomerID=@CustomerID and isnull(b.OtherName,'')=''

			update f set OtherName = N'INSIDE LOOP'
			from ForecastMaterial f
				join PartMaterial p on f.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where Replace(p.ItemName,' ','') like '%GROSGRAIN%'
			and r.CustomerID=@CustomerID and isnull(f.OtherName,'')=''

			update d set OtherName = N'INSIDE LOOP'
			from StorageDetail d
			where Replace(d.ItemName,' ','') like '%GROSGRAIN%'
			and d.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update t set OtherName = N'INSIDE LOOP'
			from MaterialTransaction t
				join PurchaseOrder p on Replace(Trim(p.Number),' ','')=Replace(Trim(t.PurchaseOrderNumber),' ','')
			where Replace(t.ItemName,' ','') like '%GROSGRAIN%'
			and p.CustomerID=@CustomerID and isnull(t.OtherName,'')=''

			update l set OtherName = N'INSIDE LOOP'
			from PurchaseOrderGroupLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where Replace(l.ItemName,' ','') like '%GROSGRAIN%'
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set OtherName = N'INSIDE LOOP'
			from PurchaseOrderLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where Replace(l.ItemName,' ','') like '%GROSGRAIN%'
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set OtherName = N'INSIDE LOOP'
			from IssuedLine l
				join Issued i on l.IssuedNumber=i.Number
			where Replace(l.ItemName,' ','') like '%GROSGRAIN%'
			and i.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update d set OtherName = N'INSIDE LOOP'
			from MaterialRequestDetails d
				join MaterialRequests r on r.ID=d.MaterialRequestId
			where Replace(d.ItemName,' ','') like '%GROSGRAIN%'
			and r.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update l set OtherName = N'INSIDE LOOP'
			from ReceiptGroupLine l
				join Receipt r on r.Number=l.ReceiptNumber
			where Replace(l.ItemName,' ','') like '%GROSGRAIN%'
			and r.CustomerID=@CustomerID and isnull(l.OtherName,'')=''
		END
		ELSE IF @CustomerID = 'IFG'
		BEGIN
			-------- Other Name for THREAD ---------
			update p set OtherName = N'THREAD',DefaultThreadName=t.FullName
			from PartMaterial p
				join PartRevision r on p.PartRevisionID=r.ID
				join StitchingThread t on r.CustomerID=t.CustomerID
			where p.VendorID in ('Hunglong')
			and r.CustomerID=@CustomerID and isnull(p.OtherName,'')=''
			and Replace(p.ItemName,' ','') Like '%'+ Trim(t.shortname) + '%'

			update b set OtherName = N'THREAD',DefaultThreadName=t.FullName
			from ProductionBOM b
				join PartMaterial p on b.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
				join StitchingThread t on r.CustomerID=t.CustomerID
			where b.VendorID in ('Hunglong')
			and r.CustomerID=@CustomerID and isnull(b.OtherName,'')=''
			and Replace(b.ItemName,' ','') Like '%'+ Trim(t.shortname) + '%'

			update f set OtherName = N'THREAD',DefaultThreadName=t.FullName
			from ForecastMaterial f
				join PartMaterial p on f.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
				join StitchingThread t on r.CustomerID=t.CustomerID
			where f.VendorID in ('Hunglong')
			and r.CustomerID=@CustomerID and isnull(f.OtherName,'')=''
			and Replace(f.ItemName,' ','') Like '%'+ Trim(t.shortname) + '%'

			update d set OtherName = N'THREAD',DefaultThreadName=t.FullName
			from StorageDetail d
				join PurchaseOrder p on Replace(Trim(p.Number),' ','')=Replace(Trim(d.PurchaseOrderNumber),' ','')
				join StitchingThread t on d.CustomerID=t.CustomerID
			where p.VendorID in ('Hunglong')
			and d.CustomerID=@CustomerID and isnull(d.OtherName,'')=''
			and Replace(d.ItemName,' ','') Like '%'+ Trim(t.shortname) + '%'

			update t set OtherName = N'THREAD',DefaultThreadName=s.FullName
			from MaterialTransaction t
				join PurchaseOrder p on Replace(Trim(p.Number),' ','')=Replace(Trim(t.PurchaseOrderNumber),' ','')
				join StitchingThread s on s.CustomerID=p.CustomerID
			where p.VendorID in ('Hunglong')
			and p.CustomerID=@CustomerID and isnull(t.OtherName,'')=''
			and Replace(t.ItemName,' ','') Like '%'+ Trim(s.shortname) + '%'

			update l set OtherName = N'THREAD',DefaultThreadName=t.FullName
			from PurchaseOrderGroupLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
				join StitchingThread t on p.CustomerID=t.CustomerID
			where p.VendorID in ('Hunglong')
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''
			and Replace(l.ItemName,' ','') Like '%'+ Trim(t.shortname) + '%'

			update l set OtherName = N'THREAD',DefaultThreadName=t.FullName
			from PurchaseOrderLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
				join StitchingThread t on p.CustomerID=t.CustomerID
			where p.VendorID in ('Hunglong')
			and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''
			and Replace(l.ItemName,' ','') Like '%'+ Trim(t.shortname) + '%'

			update l set OtherName = N'THREAD',DefaultThreadName=t.FullName
			from ReceiptGroupLine l
				join Receipt r on r.Number=l.ReceiptNumber
				join StitchingThread t on r.CustomerID=t.CustomerID
			where r.VendorID in ('Hunglong')
			and r.CustomerID=@CustomerID and isnull(l.OtherName,'')=''
			and Replace(l.ItemName,' ','') Like '%'+ Trim(t.shortname) + '%'

			update l set l.DefaultThreadName=t.FullName,l.OtherName= N'THREAD'
			from IssuedLine l 
				join Issued i on l.IssuedNumber=i.Number
				join StitchingThread t On i.CustomerID = t.CustomerID
			where isnull(l.DefaultThreadName,'')='' and isnull(l.OtherName,'')=''
				and Replace(l.ItemName,' ','') Like N'%'+ Trim(t.shortname) + '%' 
				and i.CustomerID=@CustomerID 

			update d set d.DefaultThreadName=t.FullName,d.OtherName= N'THREAD'
			from MaterialRequestDetails d 
				join MaterialRequests r On d.MaterialRequestId=r.Id
				join StitchingThread t On r.CustomerID=t.CustomerID
			where isnull(d.DefaultThreadName,'')='' and isnull(d.OtherName,'')=''
				and Replace(d.ItemName,' ','') Like N'%'+ Trim(t.shortname) + '%' 
				and r.CustomerID=@CustomerID

			------ Other Name for Fabric ---------
			update p set OtherName = N'FABRIC'
			from PartMaterial p
				join PartRevision r on p.PartRevisionID=r.ID
			where p.MaterialTypeCode = 'FB'
				and r.CustomerID=@CustomerID and isnull(p.OtherName,'')=''

			update b set OtherName = N'FABRIC'
			from ProductionBOM b
				join PartMaterial p on b.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where b.MaterialTypeCode = 'FB'
				and r.CustomerID=@CustomerID and isnull(b.OtherName,'')=''

			update f set OtherName = N'FABRIC'
			from ForecastMaterial f
				join PartMaterial p on f.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where f.MaterialTypeCode = 'FB'
				and r.CustomerID=@CustomerID and isnull(f.OtherName,'')=''

			update d set OtherName = N'FABRIC'
			from StorageDetail d
			where d.StorageCode='FB'
				and d.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update t set OtherName = N'FABRIC'
			from MaterialTransaction t
				join PurchaseOrder p on Replace(Trim(p.Number),' ','')=Replace(Trim(t.PurchaseOrderNumber),' ','')
			where t.StorageCode='FB'
				and p.CustomerID=@CustomerID and isnull(t.OtherName,'')=''

			update l set OtherName = N'FABRIC'
			from PurchaseOrderGroupLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''
				and p.VendorID in (	select VendorID 
									from PartMaterial p
										join PartRevision r on p.PartRevisionID=r.ID
									where MaterialTypeCode='FB' and r.CustomerID=@CustomerID)
			
			update l set OtherName = N'FABRIC'
			from PurchaseOrderLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''
				and p.VendorID in (	select VendorID 
									from PartMaterial p
										join PartRevision r on p.PartRevisionID=r.ID
									where MaterialTypeCode='FB' and r.CustomerID=@CustomerID)

			update l set OtherName = N'FABRIC'
			from ReceiptGroupLine l
				join Receipt r on r.Number=l.ReceiptNumber
			where r.StorageCode='FB'
				and r.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set l.OtherName= N'FABRIC'
			from IssuedLine l 
				join Issued i on l.IssuedNumber=i.Number
			where i.StorageCode='FB'
				and isnull(l.OtherName,'')='' and i.CustomerID=@CustomerID 

			update d set d.OtherName= N'FABRIC'
			from MaterialRequestDetails d 
				join MaterialRequests r On d.MaterialRequestId=r.Id
			where r.StorageCode='FB'
				and isnull(d.OtherName,'')='' and r.CustomerID=@CustomerID

			------ Other Name for Others ---------
			update p set OtherName = N'OTHERS'
			from PartMaterial p
				join PartRevision r on p.PartRevisionID=r.ID
			where isnull(p.OtherName,'') not in ('THREAD','FABRIC')
				and r.CustomerID=@CustomerID and isnull(p.OtherName,'')=''

			update b set OtherName = N'OTHERS'
			from ProductionBOM b
				join PartMaterial p on b.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where isnull(b.OtherName,'') not in ('THREAD','FABRIC')
				and r.CustomerID=@CustomerID and isnull(b.OtherName,'')=''

			update f set OtherName = N'OTHERS'
			from ForecastMaterial f
				join PartMaterial p on f.PartMaterialID=p.ID
				join PartRevision r on p.PartRevisionID=r.ID
			where isnull(f.OtherName,'') not in ('THREAD','FABRIC')
				and r.CustomerID=@CustomerID and isnull(f.OtherName,'')=''

			update d set OtherName = N'OTHERS'
			from StorageDetail d
			where isnull(d.OtherName,'') not in ('THREAD','FABRIC')
				and d.CustomerID=@CustomerID and isnull(d.OtherName,'')=''

			update t set OtherName = N'OTHERS'
			from MaterialTransaction t
				join PurchaseOrder p on Replace(Trim(p.Number),' ','')=Replace(Trim(t.PurchaseOrderNumber),' ','')
			where isnull(t.OtherName,'') not in ('THREAD','FABRIC')
				and p.CustomerID=@CustomerID and isnull(t.OtherName,'')=''

			update l set OtherName = N'OTHERS'
			from PurchaseOrderGroupLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where isnull(l.OtherName,'') not in ('THREAD','FABRIC')
				and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''
			
			update l set OtherName = N'OTHERS'
			from PurchaseOrderLine l
				join PurchaseOrder p on p.ID=l.PurchaseOrderID
			where isnull(l.OtherName,'') not in ('THREAD','FABRIC')
				and p.CustomerID=@CustomerID and isnull(l.OtherName,'')=''
				
			update l set OtherName = N'OTHERS'
			from ReceiptGroupLine l
				join Receipt r on r.Number=l.ReceiptNumber
			where isnull(l.OtherName,'') not in ('THREAD','FABRIC')
				and r.CustomerID=@CustomerID and isnull(l.OtherName,'')=''

			update l set l.OtherName= N'OTHERS'
			from IssuedLine l 
				join Issued i on l.IssuedNumber=i.Number
			where isnull(l.OtherName,'') not in ('THREAD','FABRIC')
				and isnull(l.OtherName,'')='' and i.CustomerID=@CustomerID 

			update d set d.OtherName= N'OTHERS'
			from MaterialRequestDetails d 
				join MaterialRequests r On d.MaterialRequestId=r.Id
			where isnull(d.OtherName,'') not in ('THREAD','FABRIC')
				and isnull(d.OtherName,'')='' and r.CustomerID=@CustomerID
		END	

		SET @Msg= 1
		COMMIT TRANSACTION UpdateOtherName
    END TRY
    BEGIN CATCH
        SET @Msg= 0
        ROLLBACK TRANSACTION UpdateOtherName
    END CATCH

	SELECT @Msg
END
