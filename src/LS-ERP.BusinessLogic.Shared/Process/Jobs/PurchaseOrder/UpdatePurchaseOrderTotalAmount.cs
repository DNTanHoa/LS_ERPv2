using Hangfire;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process.Jobs
{
    public class UpdatePurchaseOrderTotalAmount
    {
        private readonly ILogger<UpdatePurchaseOrderTotalAmount> logger;
        private readonly SqlServerAppDbContext context;

        public UpdatePurchaseOrderTotalAmount(ILogger<UpdatePurchaseOrderTotalAmount> logger,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        [JobDisplayName("Update total amount purchase order")]
        [AutomaticRetry(Attempts = 3)]
        public Task Execute()
        {
            try
            {
                var purchaseOrders = context.PurchaseOrder
                    .Where(x => x.TotalAmount == null || x.TotalAmount == 0)
                    .ToList();
                foreach (var po in purchaseOrders)
                {
                    var totalAmount = context.PurchaseOrderGroupLine
                   .Where(x => x.PurchaseOrderID == po.ID)
                   .Sum(x => !string.IsNullOrEmpty(po.CurrencyID)
                                    ? (po.CurrencyExchangeTypeID == "USD/VND"
                                        ? (po.CurrencyExchangeValue * x.Price * x.Quantity)
                                        : (x.Price * x.Quantity))
                                        : 0
                                        );
                    po.TotalAmount = (decimal)(totalAmount);
                    context.Update(po);
                }
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                logger.LogError("Purchase order event handler has error with message {@message}",
                    ex.InnerException?.Message);
                LogHelper.Instance.Error("Purchase order event handler has error with message {@message}",
                    ex.InnerException?.Message);
            }

            return Task.CompletedTask;
        }
    }
}
