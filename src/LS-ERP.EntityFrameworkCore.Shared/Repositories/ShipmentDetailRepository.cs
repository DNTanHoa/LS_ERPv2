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
    public class ShipmentDetailRepository : IShipmentDetailRepository
    {
        private readonly AppDbContext context;

        public ShipmentDetailRepository(AppDbContext context)
        {
            this.context = context;
        }

        public ShipmentDetail Add(ShipmentDetail shipmentDetail)
        {
            return context.ShipmentDetail.Add(shipmentDetail).Entity;
        }

        public void Delete(ShipmentDetail shipmentDetail)
        {
            context.ShipmentDetail.Remove(shipmentDetail);
        }

        public IQueryable<ShipmentDetail> GetMatchingShipmentDetails(string customerPurchaseOrder)
        {
            return context.ShipmentDetail
                .Where(x => x.CustomerPurchaseOrderNumber == customerPurchaseOrder
                            && ((x.MatchedPO != null && x.MatchedPO == false) || x.MatchedPO == null));
        }

        public IQueryable<ShipmentDetail> GetShipmentDetails()
        {
            return context.ShipmentDetail.Where(x => x.MatchedPO != null || x.MatchedPO == false);
        }

        public IQueryable<ShipmentDetail> GetShipmentDetails(string customerID)
        {
            return context.ShipmentDetail
                .Include(x => x.Shipment)
                .Where(x => x.Shipment.CustomerID == customerID
                            && ((x.MatchedPO != null && x.MatchedPO == false) || x.MatchedPO == null));
        }

        public void Update(ShipmentDetail shipmentDetail)
        {
            context.Entry(shipmentDetail).State = EntityState.Modified;
        }
    }
}
