using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Extensions
{
    public static class OrderDetailExtension
    {
        public static void CalculateQuantity(this OrderDetail orderDetail)
        {
            if (orderDetail.ReservationEntries != null &&
                orderDetail.ReservationEntries.Any())
            {
                orderDetail.ReservedQuantity = orderDetail.ReservationEntries
                                                    .Where(x => !string.IsNullOrEmpty(x.JobHeadNumber))
                                                    .Sum(x => x.ReservedQuantity);

                orderDetail.ConsumedQuantity = orderDetail.ReservationEntries
                                                    .Where(x => string.IsNullOrEmpty(x.JobHeadNumber))
                                                    .Sum(x => x.ReservedQuantity);
            }
        }
    }
}
