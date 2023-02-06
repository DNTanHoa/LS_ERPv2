using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories
{
    public class ShipmentRepository : IShipmentRepository
    {
        private readonly AppDbContext context;

        public ShipmentRepository(AppDbContext context)
        {
            this.context = context;
        }

        public Shipment Add(Shipment shipment)
        {
            return context.Shipment.Add(shipment).Entity;
        }

        public void Delete(Shipment shipment)
        {
            context.Shipment.Remove(shipment);
        }

        public Shipment GetShipment(string fileName)
        {
            return context.Shipment.FirstOrDefault(x => x.FileName == fileName);
        }

        public IQueryable<Shipment> GetShipments()
        {
            return context.Shipment;
        }

        public IQueryable<Shipment> GetShipments(string CustomerID)
        {
            return context.Shipment.Where(x => x.CustomerID == CustomerID);
        }

        public bool IsExist(string fileName)
        {
            var shipment = GetShipment(fileName);
            return shipment != null ? true : false;
        }

        public void Update(Shipment shipment)
        {
            context.Entry(shipment).State = EntityState.Modified;
        }
    }
}
