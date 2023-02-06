using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IShipmentDetailRepository
    {
        ShipmentDetail Add(ShipmentDetail shipmentDetail);
        void Update(ShipmentDetail shipmentDetail);
        void Delete(ShipmentDetail shipmentDetail);
        IQueryable<ShipmentDetail> GetShipmentDetails();
        IQueryable<ShipmentDetail> GetShipmentDetails(string customerID);
        IQueryable<ShipmentDetail> GetMatchingShipmentDetails(string customerPurchaseOrder);
    }
}
