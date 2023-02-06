using AutoMapper;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultils.Extensions;
using Ultils.Helpers;

namespace LS_ERP.BusinessLogic.Queries
{
    public class PurchaseOrderQueries : IPurchaseOrderQueries
    {
        private readonly IPurchaseOrderRepository purchaseOrderRepository;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public PurchaseOrderQueries(IPurchaseOrderRepository purchaseOrderRepository,
            SqlServerAppDbContext context,
            IMapper mapper,
            IConfiguration configuration)
        {
            this.purchaseOrderRepository = purchaseOrderRepository;
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        public IEnumerable<PurchaseOrderSummaryDtos> GetPurchaseOrders()
        {
            return purchaseOrderRepository.GetPurchaseOrders()
                .Select(x => mapper.Map<PurchaseOrderSummaryDtos>(x));
        }

        public PurchaseOrderDetailDtos GetPurchaseOrder(string Number)
        {
            var data = purchaseOrderRepository.GetPurchaseOrder(Number);

            if (data != null)
            {
                return mapper.Map<PurchaseOrderDetailDtos>(data);
            }
            return null;
        }

        public IEnumerable<PurchaseOrderInforDtos> GetPurchaseOrderInfors(string purchaseOrderId)
        {
            var result = new List<PurchaseOrderInforDtos>();

            var purchaseOrder = context.PurchaseOrder
                .FirstOrDefault(x => x.ID == purchaseOrderId);

            var purchaseOrderGroupLines = context.PurchaseOrderGroupLine
                .Where(x => x.PurchaseOrderID == purchaseOrderId).ToList();

            var receipts = context.Receipt
                .Where(x => x.PurchaseOrderNumber == purchaseOrder.Number)
                .ToList();

            var receiptGroupLines = context.ReceiptGroupLine
                .Where(x => receipts.Select(r => r.Number).Contains(x.ReceiptNumber))
                .ToList();

            foreach (var groupLine in purchaseOrderGroupLines)
            {
                var infor = new PurchaseOrderInforDtos()
                {
                    ItemCode = groupLine.ItemCode,
                    ItemID = groupLine.ItemID,
                    ItemName = groupLine.ItemName,
                    ItemColorCode = groupLine.ItemColorCode,
                    ItemColorName = groupLine.ItemColorName,
                    Specify = groupLine.Specify,
                    Position = groupLine.Position,
                    GarmentColorCode = groupLine.GarmentColorCode,
                    GarmentColorName = groupLine.GarmentColorName,
                    GarmentSize = groupLine.GarmentSize
                };
                result.Add(infor);
            }
            return result;
        }

        public IEnumerable<PurchaseOrderReportDtos> GetPurchaseOrderReport(string CustomerID, string VendorID, DateTime FromDate, DateTime ToDate)
        {
            return purchaseOrderRepository.GetPurchaseOrders(CustomerID, VendorID, FromDate, ToDate)
            //.Join(context.PurchaseOrderGroupLine,
            //               Po => Po.ID,
            //               GroupLine => GroupLine.PurchaseOrderID,
            //               (Po, GroupLine) => new { Po, GroupLine })
            //.Select(x => new PurchaseOrderReportDtos
            //{
            //    Number = x.Po.Number,
            //    OrderDate = x.Po.OrderDate,
            //    EstimateShipDate = x.Po.EstimateShipDate,
            //    VendorConfirmedDate = x.Po.VendorConfirmedDate,
            //    ShipDate = x.Po.ShipDate,
            //    CustomerID = x.Po.CustomerID,
            //    InvoiceNo = x.Po.InvoiceNo,
            //    Reason = x.Po.Reason,
            //    PaymentTermCode = x.Po.PaymentTermCode,
            //    PurchaseOrderStatusCode = x.Po.PurchaseOrderStatusCode,
            //    Description = x.Po.Description,
            //    Remark = x.Po.Remark,
            //    CompanyCode = x.Po.CompanyCode,
            //    IncoTermCode = x.Po.IncoTermCode,
            //    VendorID = x.Po.VendorID,
            //    SupplierCNUFCode = x.Po.SupplierCNUFCode,
            //    CurrencyID = x.Po.CurrencyID,
            //    CurrencyExchangeTypeID = x.Po.CurrencyExchangeTypeID,
            //    CurrencyExchangeValue = x.Po.CurrencyExchangeValue,
            //    Discount = x.Po.Discount,
            //    IsIncludedTax = x.Po.IsIncludedTax,
            //    TaxCode = x.Po.TaxCode,
            //    Percentage = x.Po.Percentage,
            //    ShipTo = x.Po.ShipTo,
            //    ShippingMethodCode = x.Po.ShippingMethodCode,
            //    ShippingTermCode = x.Po.ShippingTermCode,
            //    Price = x.GroupLine.Price,
            //    Quantity = x.GroupLine.Quantity,
            //    Amount = x.Po.CurrencyExchangeValue > 0
            //                ? (x.Po.CurrencyExchangeValue * x.GroupLine.Price * x.GroupLine.Quantity)
            //                : (x.GroupLine.Price * x.GroupLine.Quantity)
            //});
            .Select(x => mapper.Map<PurchaseOrderReportDtos>(x));
        }

        public List<PurchaseOrderLineReceivedDtos> GetPurchaseOrderReceived(string customerID, string storageCode, DateTime? fromDate, DateTime? toDate)
        {
            var connectionString = configuration.GetSection("ConnectionString")
               .GetSection("DbConnection").Value ?? string.Empty;
            SqlParameter[] parameters =
            {
                new SqlParameter("@CustomerID", customerID),
                new SqlParameter("@StorageCode",storageCode),
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate",toDate)
            };

            if (customerID == null)
            {
                parameters[0].Value = DBNull.Value;
                parameters[0].Direction = ParameterDirection.Input;
            }

            if (storageCode == null)
            {
                parameters[1].Value = DBNull.Value;
                parameters[1].Direction = ParameterDirection.Input;
            }

            if (fromDate == null)
            {
                parameters[2].Value = DBNull.Value;
                parameters[2].Direction = ParameterDirection.Input;
            }

            if (toDate == null)
            {
                parameters[3].Value = DBNull.Value;
                parameters[3].Direction = ParameterDirection.Input;
            }

            DataTable table = SqlHelper.FillByReader(connectionString,
                "sp_SelectPurchaseReceived", parameters);
            return table.AsListObject<PurchaseOrderLineReceivedDtos>();
        }
    }
}
