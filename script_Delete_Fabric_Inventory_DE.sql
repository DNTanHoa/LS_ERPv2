--- === DELETE FABRIC - DE INVENTORY === ---

-- === ISSUED ENTRY === --
select * 
--- delete 
from MaterialTransaction where StorageCode='FB'
and IssuedNumber in (select Number from Issued where CustomerID='DE' and StorageCode='FB')

select * 
--- delete 
from IssuedLine 
where IssuedNumber in (select Number from Issued where CustomerID='DE' and StorageCode='FB')

select * 
--- delete 
from IssuedGroupLine 
where IssuedNumber in (select Number from Issued where CustomerID='DE' and StorageCode='FB')

select * 
--- delete
from Issued 
where CustomerID='DE' and StorageCode='FB' 

-- === RECEIPT ENTRY === --
select * 
--- delete 
from MaterialTransaction where StorageCode='FB'
and ReceiptNumber in (select Number from Receipt where CustomerID='DE' and StorageCode='FB')

--select * 
----- delete 
--from ReceiptLine 
--where ReceiptGroupLineID in 
--	(select ID 
--	from ReceiptGroupLine 
--	where ReceiptNumber in (select Number 
--							from Receipt
--							where CustomerID='DE' and StorageCode='FB'))
select * 
--- delete 
from ReceiptGroupLine 
where ReceiptNumber in (select Number 
						from Receipt
						where CustomerID='DE' and StorageCode='FB')

select * 
--- delete
from Receipt 
where CustomerID='DE' and StorageCode='FB' 

-- === STORAGE DETAIL == --
select * 
--- delete 
from MaterialTransaction where StorageCode='FB'
and StorageDetailID in (select ID from StorageDetail where CustomerID='DE' and StorageCode='FB')

select * 
--- delete 
from StorageDetail where CustomerID='DE' and StorageCode='FB'

-- == FABRIC PURCHASE ORDER == ---
select *
-- update f set OnHandQuantity=0, ReceivedQuantity=0
from FabricPurchaseOrder f
where CustomerID='DE' 