using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IReservationEntryRepository
    {
        ReservationEntry Add(ReservationEntry reservationEntry);
        void Update(ReservationEntry reservationEntry);
        void Delete(ReservationEntry reservationEntry);
        IEnumerable<ReservationEntry> GetReservationEntries();
        IQueryable<ReservationEntry> GetReservationEntries(List<long?> ProductionBOMIDs);
        IQueryable<ReservationEntry> GetReservationEntriesFollowOrderDetail(List<long?> OrderDetailIDs);
        ReservationEntry GetReservationEntry(long ID);
        bool IsExist(long ID, out ReservationEntry reservationEntry);
        bool IsExist(long ID);
    }
}
