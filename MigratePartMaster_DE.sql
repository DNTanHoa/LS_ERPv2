
insert into PartMaster
select UPPER(NEWID()),*,'Admin','Admin',GETDATE(),GETDATE(),0
from(
	select distinct b.BarCode,i.ColorCode,i.ColorName,b.Size,i.Season,i.CustomerStyle,s.CustomerID
	from ItemStyle i 
		join ItemStyleBarCode b on  i.Number = b.ItemStyleNumber
		join SalesOrders s on i.SalesOrderID=s.ID 
	where s.CustomerID='DE') a

