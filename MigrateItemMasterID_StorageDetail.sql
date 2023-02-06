--- Migrate ItemMasterID from PartMaster for FG
UPDATE d SET d.ItemMasterID = m.ID
FROM StorageDetail d INNER JOIN PartMaster m ON	
	TRIM(ISNULL(d.GarmentColorCode,'')) = TRIM(ISNULL(m.GarmentColorCode,'')) AND
	TRIM(ISNULL(d.GarmentColorName,'')) = TRIM(ISNULL(m.GarmentColorName,'')) AND
	TRIM(ISNULL(d.GarmentSize,'')) = TRIM(ISNULL(m.GarmentSize,'')) AND
	TRIM(ISNULL(d.Season,'')) = TRIM(ISNULL(m.Season,'')) AND
	TRIM(ISNULL(d.CustomerStyle,'')) = TRIM(ISNULL(m.CustomerStyle,'')) AND
	d.CustomerID=m.CustomerID 
WHERE d.StorageCode='FG'
