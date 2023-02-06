using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IShipmentRepository
    {
        Shipment Add(Shipment shipment);
        void Update(Shipment shipment);
        void Delete(Shipment shipment);
        IQueryable<Shipment> GetShipments();
        IQueryable<Shipment> GetShipments(string CustomerID);
        Shipment GetShipment(string fileName);
        bool IsExist(string fileName);
    }
}
