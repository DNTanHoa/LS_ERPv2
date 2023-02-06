using LS_ERP.BusinessLogic.Events;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ultils.Helpers;

namespace LS_ERP.BusinessLogic.EventHandlers
{
    public class PurchaseOrderCreatedEventHandler 
        : INotificationHandler<PurchaseOrderCreatedEvent>
    {
        private readonly ILogger<PurchaseOrderCreatedEventHandler> logger;
        private readonly SqlServerAppDbContext database;
        private readonly IConfiguration configuration;

        public PurchaseOrderCreatedEventHandler(ILogger<PurchaseOrderCreatedEventHandler> logger,
            SqlServerAppDbContext database,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.database = database;
            this.configuration = configuration;
        }

        public Task Handle(PurchaseOrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                /// Lấy thông tin purchase order để cập nhật thông tin
                var connectionString = configuration.GetSection("ConnectionString")
                .GetSection("DbConnection").Value ?? string.Empty;
                var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@PurchaseOrderNumber", notification.PurchaseOrderNumber),
                    new SqlParameter("@UserName", notification.UserName),
                };
                var result = SqlHelper.ExecuteNonQuery(connectionString,
                    "sp_UpdatePurchaseOrderToRelatedTable", sqlParameter);
            }
            catch (Exception ex)
            {
                logger.LogError("{@time} - Purchase order created event handler for request {@request} " +
                    "has error with message {@mesage}",
                    DateTime.Now.ToString(), JsonConvert.SerializeObject(notification), ex.InnerException?.Message);
                LogHelper.Instance.Error("{@time} - Purchase order created event handler for request {@request} " +
                    "has error with message {@mesage}",
                    DateTime.Now.ToString(), JsonConvert.SerializeObject(notification), ex.InnerException?.Message);
            }

            return Task.CompletedTask;
        }
    }
}
