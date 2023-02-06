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
    public class ReservationEntryRepository : IReservationEntryRepository
    {
        private readonly AppDbContext context;
        public ReservationEntryRepository(AppDbContext context)
        {
            this.context = context;
        }
        public ReservationEntry Add(ReservationEntry reservationEntry)
        {
            return context.ReservationEntry.Add(reservationEntry).Entity;
        }
        public void Delete(ReservationEntry reservationEntry)
        {
            context.ReservationEntry.Remove(reservationEntry);
        }
        public IEnumerable<ReservationEntry> GetReservationEntries()
        {
            return context.ReservationEntry;
        }
        public IQueryable<ReservationEntry> GetReservationEntries(List<long?> ProductionBOMIDs)
        {
            return context.ReservationEntry
                .Include(x => x.PurchaseOrderLine)
                .Where(x => ProductionBOMIDs.Contains(x.ProductionBOMID));
        }

        public IQueryable<ReservationEntry> GetReservationEntriesFollowOrderDetail(List<long?> OrderDetailIDs)
        {
            return context.ReservationEntry
                .Where(x => OrderDetailIDs.Contains(x.OrderDetailID));
        }

        public ReservationEntry GetReservationEntry(long ID)
        {
            return context.ReservationEntry.FirstOrDefault(x => x.ID == ID);
        }
        public bool IsExist(long ID, out ReservationEntry reservationEntry)
        {
            reservationEntry = null;
            reservationEntry = GetReservationEntry(ID);
            return reservationEntry != null ? true : false;
        }
        public bool IsExist(long ID)
        {
            var reservationEntry = GetReservationEntry(ID);
            return reservationEntry != null ? true : false;
        }
        public void Update(ReservationEntry reservationEntry)
        {
            context.Entry(reservationEntry).State = EntityState.Modified;
        }
    }
}
