using LS_ERP.BusinessLogic.Events;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Helpers;
using MediatR;
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
    public class ReceiptCreatedEventHandler : INotificationHandler<ReceiptCreatedEvent>
    {
        private readonly ILogger<ReceiptCreatedEventHandler> logger;
        private readonly SqlServerAppDbContext database;
        private readonly IConfiguration configuration;

        public ReceiptCreatedEventHandler(ILogger<ReceiptCreatedEventHandler> logger,
            SqlServerAppDbContext database,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.database = database;
            this.configuration = configuration;
        }
        public Task Handle(ReceiptCreatedEvent notification, 
            CancellationToken cancellationToken)
        {
            try
            {
                /// Lấy thông tin purchase order để cập nhật thông tin
                var connectionString = configuration.GetSection("ConnectionString")
                .GetSection("DbConnection").Value ?? string.Empty;
                var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@ReceiptNumber", notification.ReceiptNumber),
                    new SqlParameter("@StorageCode", notification.StorageCode),
                    new SqlParameter("@UserName", notification.UserName),
                };
                var result = SqlHelper.ExecuteNonQuery(connectionString,
                    "sp_CreateReceipt", sqlParameter);
            }
            catch(Exception ex)
            {
                logger.LogError("{@time} - Receipt created event handler for request {@request} " +
                    "has error with message {@mesage}",
                    DateTime.Now.ToString(), JsonConvert.SerializeObject(notification), ex.InnerException?.Message);
                LogHelper.Instance.Error("{@time} - Receipt created event handler for request {@request} " +
                    "has error with message {@mesage}",
                    DateTime.Now.ToString(), JsonConvert.SerializeObject(notification), ex.InnerException?.Message);
            }
            return Task.CompletedTask;
        }
    }
}
