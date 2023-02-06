using AutoMapper;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.BusinessLogic.Dtos.SalesOrder;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries.SalesOrder
{
    public class SalesOrderQueries : ISalesOrderQueries
    {
        private readonly ISalesOrderRepository salesOrderRepository;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;

        public SalesOrderQueries(ISalesOrderRepository salesOrderRepository,
            SqlServerAppDbContext context,
            IMapper mapper)
        {
            this.salesOrderRepository = salesOrderRepository;
            this.context = context;
            this.mapper = mapper;
        }

        public List<PurchaseForSalesOrderDto> GetPurchaseForOrder(string CustomerID, 
            string style, string salesOrderID)
        {
            var results = new List<PurchaseForSalesOrderDto>();

            var orders = context.SalesOrders
                .Where(x => (x.ID.Contains(salesOrderID) || string.IsNullOrEmpty(salesOrderID)) &&
                            (x.CustomerID == CustomerID || string.IsNullOrEmpty(CustomerID))).ToList();
            
            var itemStyles = context.ItemStyle
                .Include(x => x.SalesOrder)
                .Where(x => orders.Select(x => x.ID).Contains(x.SalesOrderID) &&
                            (x.LSStyle.Contains(style) || string.IsNullOrEmpty(style)))
                .ToList();

            results = context.PurchaseOrderLine
                .Include(x => x.PurchaseOrder)
                .Where(x => itemStyles.Select(s => s.LSStyle).Contains(x.LSStyle))
                .GroupBy(x => new 
                {
                    x.CustomerStyle,
                    x.LSStyle,
                    x.PurchaseOrder.OrderDate,
                    x.PurchaseOrder.VendorID,
                    x.PurchaseOrder.Number,
                    x.PurchaseOrder.CreatedBy,
                    x.SalesOrderID,
                    x.PurchaseOrderID
                })
                .Select(x => new PurchaseForSalesOrderDto()
                {
                    CustomerStyle = x.Key.CustomerStyle,
                    LSStyle = x.Key.LSStyle,
                    PurchaseOrderDate = x.Key.OrderDate,
                    VenderID = x.Key.VendorID,
                    PurchaseOrderID = x.Key.PurchaseOrderID,
                    PurchaseOrderNumber = x.Key.Number,
                    PurchaseOrderMS = x.Key.CreatedBy,
                    SalesOrderID = x.Key.SalesOrderID,
                }).ToList();

            foreach(var result in results)
            {
                var saleOrder = orders.FirstOrDefault(x => x.ID == result.SalesOrderID);
                if(saleOrder != null)
                {
                    result.OrderDate = saleOrder.OrderDate;
                    result.CustomerID = saleOrder.CustomerID;
                    result.SalesOrderMS = saleOrder.CreatedBy;
                }
            }

            return results;
        }

        public SalesOrderDetailDtos GetSalesOrderByID(string OrderID)
        {
            var saleOrder = salesOrderRepository.GetSalesOrder(OrderID);

            if (saleOrder != null)
            {
                return mapper.Map<SalesOrderDetailDtos>(saleOrder);
            }

            return null;
        }

        public IEnumerable<SalesOrderSummaryDtos> GetSalesOrderSummaries()
        {
            return salesOrderRepository.GetSalesOrders()
                .Select(x => mapper.Map<SalesOrderSummaryDtos>(x));
        }
    }
}
