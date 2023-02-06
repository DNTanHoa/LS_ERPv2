SELECT 
	*
FROM 
	PurchaseOrder 
WHERE 
	Number IN ('DKSS22 ADE 330172(A46)', 'DKSS22 RFID 330172(A46)', 'PO2021-7SANCE')

--- Cập nhật Production BOM về giá trị ban đầu
UPDATE ProductionBOM
	SET
		ReservedQuantity = 0,
		RemainQuantity = RequiredQuantity
WHERE
	ID IN 
	(
		SELECT 
			ProductionBOMID
		FROM
			ReservationEntry
		WHERE
			PurchaseOrderLineID IN 
			(
				SELECT ID FROM PurchaseOrderLine 
				WHERE PurchaseOrderID IN (SELECT ID FROM PurchaseOrder WHERE Number IN ('DKSS22 ADE 330172(A46)', 'DKSS22 RFID 330172(A46)', 'PO2021-7SANCE'))
			)
	)
--- Xóa reservation
DELETE FROM 
	ReservationEntry 
WHERE 
	PurchaseOrderLineID IN 
	(
		SELECT ID FROM PurchaseOrderLine 
		WHERE PurchaseOrderID IN (SELECT ID FROM PurchaseOrder WHERE Number IN ('DKSS22 ADE 330172(A46)', 'DKSS22 RFID 330172(A46)', 'PO2021-7SANCE'))
	)

--- Xóa Line
DELETE FROM 
	PurchaseOrderLine 
WHERE 
	PurchaseOrderID IN (SELECT ID FROM PurchaseOrder WHERE Number IN ('DKSS22 ADE 330172(A46)', 'DKSS22 RFID 330172(A46)', 'PO2021-7SANCE'))

--- Xóa Group Line
DELETE FROM PurchaseOrderGroupLine
WHERE
	PurchaseOrderID IN (SELECT ID FROM PurchaseOrder WHERE Number IN ('DKSS22 ADE 330172(A46)', 'DKSS22 RFID 330172(A46)', 'PO2021-7SANCE'))