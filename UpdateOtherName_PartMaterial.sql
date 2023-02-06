
--------- Other Name for RFID ---------
select *
-- update p set OtherName = N'RFID'
from PartMaterial p 
	join PartRevision r on p.PartRevisionID=r.ID
where p.ItemName like '%RFID%' 
and r.CustomerID='DE' and isnull(p.OtherName,'')=''

select *
-- update b set OtherName = N'RFID'
from ProductionBOM b 
	join PartMaterial p on b.PartMaterialID=p.ID
	join PartRevision r on p.PartRevisionID=r.ID
where b.ItemName like '%RFID%' 
and r.CustomerID='DE' and isnull(b.OtherName,'')=''

select *
-- update f set OtherName = N'RFID'
from ForecastMaterial f
	join PartMaterial p on f.PartMaterialID=p.ID
	join PartRevision r on p.PartRevisionID=r.ID
where f.ItemName like '%RFID%' 
and r.CustomerID='DE' and isnull(f.OtherName,'')=''

--------- Other Name for ADEQUAT ---------
select *
-- update p set OtherName = N'ADEQUAT'
from PartMaterial p
	join PartRevision r on p.PartRevisionID=r.ID
where (p.ItemName like '%ADEQUAT%' or Replace(p.ItemName,' ','') like '%LFTADQ%' or Replace(p.ItemName,' ','') like '%STKADQ%')
and r.CustomerID='DE' and isnull(OtherName,'')='' 

select *
-- update b set OtherName = N'ADEQUAT'
from ProductionBOM b
	join PartMaterial p on b.PartMaterialID=p.ID
	join PartRevision r on p.PartRevisionID=r.ID
where (b.ItemName like '%ADEQUAT%' or Replace(b.ItemName,' ','') like '%LFTADQ%' or Replace(b.ItemName,' ','') like '%STKADQ%')
and r.CustomerID='DE' and isnull(b.OtherName,'')=''

select *
-- update f set OtherName = N'ADEQUAT'
from ForecastMaterial f
	join PartMaterial p on f.PartMaterialID=p.ID
	join PartRevision r on p.PartRevisionID=r.ID
where (f.ItemName like '%ADEQUAT%' or Replace(f.ItemName,' ','') like '%LFTADQ%' or Replace(f.ItemName,' ','') like '%STKADQ%')
and r.CustomerID='DE' and isnull(f.OtherName,'')=''

--------- Other Name for THREAD ---------
select *
-- update p set OtherName = N'THREAD'
from PartMaterial p
	join PartRevision r on p.PartRevisionID=r.ID
where p.VendorID='Hunglong' 
and r.CustomerID='DE' and isnull(p.OtherName,'')=''

select *
-- update b set OtherName = N'THREAD'
from ProductionBOM b
	join PartMaterial p on b.PartMaterialID=p.ID
	join PartRevision r on p.PartRevisionID=r.ID
where b.VendorID='Hunglong' 
and r.CustomerID='DE' and isnull(b.OtherName,'')=''

select *
-- update f set OtherName = N'THREAD'
from ForecastMaterial f
	join PartMaterial p on f.PartMaterialID=p.ID
	join PartRevision r on p.PartRevisionID=r.ID
where f.VendorID='Hunglong' 
and r.CustomerID='DE' and isnull(f.OtherName,'')=''

-------- Other Name for CARE + SIZE LABEL  ---------
select *
-- update p set OtherName = N'CARE + SIZE LABEL'
from PartMaterial p
	join PartRevision r on p.PartRevisionID=r.ID
where (Replace(p.ItemName,' ','') like '%CARELABEL%' or Replace(p.ItemName,' ','') like '%CARE+SIZE%' or (p.ItemName like '%LBL%' and p.VendorID in ('TAGTIME','VIETPHU')))
and r.CustomerID='DE' and isnull(OtherName,'')=''

select *
-- update b set OtherName =N'CARE + SIZE LABEL'
from ProductionBOM b
	join PartMaterial p on b.PartMaterialID=p.ID
	join PartRevision r on p.PartRevisionID=r.ID
where (Replace(b.ItemName,' ','') like '%CARELABEL%' or Replace(b.ItemName,' ','') like '%CARE+SIZE%' or (b.ItemName like '%LBL%' and b.VendorID in ('TAGTIME','VIETPHU')))
and r.CustomerID='DE' and isnull(b.OtherName,'')=''

select *
-- update f set OtherName = N'CARE + SIZE LABEL'
from ForecastMaterial f
	join PartMaterial p on f.PartMaterialID=p.ID
	join PartRevision r on p.PartRevisionID=r.ID
where (Replace(f.ItemName,' ','') like '%CARELABEL%' or Replace(f.ItemName,' ','') like '%CARE+SIZE%' or (f.ItemName like '%LBL%' and f.VendorID in ('TAGTIME','VIETPHU')))
and r.CustomerID='DE' and isnull(f.OtherName,'')=''

--------- Other Name for ELASTIC ---------
select *
-- update p set OtherName = N'ELASTIC'
from PartMaterial p
	join PartRevision r on p.PartRevisionID=r.ID
where (p.ItemName like '%ELASTIC%' or (p.ItemName like 'EL%' and p.VendorID in('HUAYAN','LIENCHAU','YOUNGLION')))
and r.CustomerID='DE' and isnull(p.OtherName,'')=''

select *
-- update b set OtherName = N'ELASTIC'
from ProductionBOM b
	join PartMaterial p on b.PartMaterialID=p.ID
	join PartRevision r on p.PartRevisionID=r.ID
where (b.ItemName like '%ELASTIC%' or (b.ItemName like 'EL%' and b.VendorID in('HUAYAN','LIENCHAU','YOUNGLION')))
and r.CustomerID='DE' and isnull(b.OtherName,'')=''

select *
-- update f set OtherName = N'ELASTIC'
from ForecastMaterial f
	join PartMaterial p on f.PartMaterialID=p.ID
	join PartRevision r on p.PartRevisionID=r.ID
where (f.ItemName like '%ELASTIC%' or (f.ItemName like 'EL%' and f.VendorID in('HUAYAN','LIENCHAU','YOUNGLION')))
and r.CustomerID='DE' and isnull(f.OtherName,'')=''

--------- Other Name for TRACEABILITY LABEL ---------
select *
-- update p set OtherName = N'TRACEABILITY LABEL'
from PartMaterial p
	join PartRevision r on p.PartRevisionID=r.ID
where Replace(p.ItemName,' ','') like '%TRACEABILITYLABEL%' 
and r.CustomerID='DE' and isnull(p.OtherName,'')=''

select *
-- update b set OtherName = N'TRACEABILITY LABEL'
from ProductionBOM b
	join PartMaterial p on b.PartMaterialID=p.ID
	join PartRevision r on p.PartRevisionID=r.ID
where Replace(b.ItemName,' ','') like '%TRACEABILITYLABEL%' 
and r.CustomerID='DE' and isnull(b.OtherName,'')=''

select *
-- update f set OtherName = N'TRACEABILITY LABEL'
from ForecastMaterial f
	join PartMaterial p on f.PartMaterialID=p.ID
	join PartRevision r on p.PartRevisionID=r.ID
where Replace(f.ItemName,' ','') like '%TRACEABILITYLABEL%' 
and r.CustomerID='DE' and isnull(f.OtherName,'')=''

--------- Other Name for LAMINET ---------
select *
-- update p set OtherName = N'LAMINET'
from PartMaterial p
	join PartRevision r on p.PartRevisionID=r.ID
where p.VendorID in ('UNION')
and r.CustomerID='DE' and isnull(OtherName,'')=''

select *
-- update b set OtherName = N'LAMINET'
from ProductionBOM b
	join PartMaterial p on b.PartMaterialID=p.ID
	join PartRevision r on p.PartRevisionID=r.ID
where b.VendorID in ('UNION')
and r.CustomerID='DE' and isnull(b.OtherName,'')=''

select *
-- update f set OtherName = N'LAMINET'
from ForecastMaterial f
	join PartMaterial p on f.PartMaterialID=p.ID
	join PartRevision r on p.PartRevisionID=r.ID
where f.VendorID in ('UNION')
and r.CustomerID='DE' and isnull(f.OtherName,'')=''

--------- Other Name for HEAT TRANSFER ---------
select *
-- update p set OtherName = N'HEAT TRANSFER'
from PartMaterial p
	join PartRevision r on p.PartRevisionID=r.ID
where (Replace(p.ItemName,' ','') like '%HEATTRANSFER%' or Replace(p.ItemName,' ','') like '%SIZEHT%')
and r.CustomerID='DE' and isnull(p.OtherName,'')=''

select *
-- update b set OtherName = N'HEAT TRANSFER'
from ProductionBOM b
	join PartMaterial p on b.PartMaterialID=p.ID
	join PartRevision r on p.PartRevisionID=r.ID
where (Replace(b.ItemName,' ','') like '%HEATTRANSFER%' or Replace(b.ItemName,' ','') like '%SIZEHT%')
and r.CustomerID='DE' and isnull(b.OtherName,'')=''

select *
-- update f set OtherName = N'HEAT TRANSFER'
from ForecastMaterial f
	join PartMaterial p on f.PartMaterialID=p.ID
	join PartRevision r on p.PartRevisionID=r.ID
where (Replace(f.ItemName,' ','') like '%HEATTRANSFER%' or Replace(f.ItemName,' ','') like '%SIZEHT%')
and r.CustomerID='DE' and isnull(f.OtherName,'')=''

--------- Other Name for ZIPPER ---------
select *
-- update p set OtherName = N'ZIPPER'
from PartMaterial p
	join PartRevision r on p.PartRevisionID=r.ID
where p.VendorID in ('YKK','MAX ZIPPER')
and r.CustomerID='DE' and isnull(p.OtherName,'')=''

select *
-- update b set OtherName = N'ZIPPER'
from ProductionBOM b
	join PartMaterial p on b.PartMaterialID=p.ID
	join PartRevision r on p.PartRevisionID=r.ID
where b.VendorID in ('YKK','MAX ZIPPER')
and r.CustomerID='DE' and isnull(b.OtherName,'')=''

select *
-- update f set OtherName = N'ZIPPER'
from ForecastMaterial f
	join PartMaterial p on f.PartMaterialID=p.ID
	join PartRevision r on p.PartRevisionID=r.ID
where f.VendorID in ('YKK','MAX ZIPPER')
and r.CustomerID='DE' and isnull(f.OtherName,'')=''

-------- Other Name for HANGER ---------
select *
-- update p set OtherName = N'HANGER'
from PartMaterial p
	join PartRevision r on p.PartRevisionID=r.ID
where Replace(p.ItemName,' ','') like '%HANGUP%'
and r.CustomerID='DE' and isnull(p.OtherName,'')=''

select *
-- update b set OtherName = N'HANGER'
from ProductionBOM b
	join PartMaterial p on b.PartMaterialID=p.ID
	join PartRevision r on p.PartRevisionID=r.ID
where Replace(b.ItemName,' ','') like '%HANGUP%'
and r.CustomerID='DE' and isnull(b.OtherName,'')=''

select *
-- update f set OtherName = N'HANGER'
from ForecastMaterial f
	join PartMaterial p on f.PartMaterialID=p.ID
	join PartRevision r on p.PartRevisionID=r.ID
where Replace(f.ItemName,' ','') like '%HANGUP%'
and r.CustomerID='DE' and isnull(f.OtherName,'')=''