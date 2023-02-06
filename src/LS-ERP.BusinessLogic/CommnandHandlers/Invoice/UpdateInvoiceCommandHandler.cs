using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class UpdateInvoiceCommandHandler
        : IRequestHandler<UpdateInvoiceCommand, UpdateInvoiceResult>
    {
        private readonly ILogger<UpdateInvoiceCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMediator mediator;
        private readonly IInvoiceRepository invoiceRepository;
        private readonly IPackingListRepository packingListRepository;
        private readonly IInvoiceDetailRepository invoiceDetailRepository;

        public UpdateInvoiceCommandHandler(ILogger<UpdateInvoiceCommandHandler> logger,
            SqlServerAppDbContext context,
            IMediator mediator,
            IInvoiceRepository invoiceRepository,
            IPackingListRepository packingListRepository,
            IInvoiceDetailRepository invoiceDetailRepository)
        {
            this.logger = logger;
            this.context = context;
            this.mediator = mediator;
            this.invoiceRepository = invoiceRepository;
            this.packingListRepository = packingListRepository;
            this.invoiceDetailRepository = invoiceDetailRepository;
        }

        public async Task<UpdateInvoiceResult> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute update invoice command", DateTime.Now.ToString());
            var result = new UpdateInvoiceResult();

            if (request.CustomerID == "GA" || request.CustomerID == "IFG")
            {
                /// Check & update separation packing list
                var checkPackingLists = context.PackingList
                    .AsNoTracking()
                    .Where(x => x.IsRevised == true && x.Invoice.ID == long.Parse(request.InvoiceID)).ToList();
                foreach (var data in checkPackingLists)
                {
                    var revisedList = context.PackingList
                        .Where(x => x.IsRevised == true && x.CustomerID == request.CustomerID).ToList();
                    if(CheckRevised(data,revisedList))
                    {
                        if(request.CustomerID == "GA")
                            GA_UpdateSeparatePackingList(data);
                        else if (request.CustomerID == "IFG")
                            IFG_UpdateSeparatePackingList(data);
                    }
                }
            }

            var invoice = invoiceRepository.GetInvoice(long.Parse(request.InvoiceID));
            var packingList = packingListRepository.GetPackingList(long.Parse(request.InvoiceID)).ToList();
            
            var ifgUnit = context.Unit?.FirstOrDefault(x => x.ID == "DZ");

            var partPrices = context.PartPrice
                .Where(x => x.CustomerID == request.CustomerID).ToList();

            var styles = packingList.SelectMany(x => x.ItemStyles).ToList();

            var salesOrders = context.SalesOrders
                    .Where(s => styles.Select(i => i.SalesOrderID).Contains(s.ID)).ToList();

            UpdateInvoiceProcessor.UpdateInvoice(invoice, packingList, request.UserName, ifgUnit,
                partPrices, salesOrders,
                out List<InvoiceDetail> deleteInvoiceDetail,
                out List<InvoiceDetail> newInvoiceDetail,
                out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            try
            {
                context.InvoiceDetail.RemoveRange(deleteInvoiceDetail);
                context.InvoiceDetail.AddRange(newInvoiceDetail);

                context.Invoice.Update(invoice);

                //invoice.InvoiceDetails = newInvoiceDetail;

                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
                result.Result = invoice;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
            }

            return result;
        }

        public bool CheckRevised(PackingList packingList, List<PackingList> revisedList)
        {
            var resultList = new List<PackingList>();
            var parent = packingList;
            var parentID = parent?.ParentPackingListID ?? 0;
            while (parentID > 0)
            {
                parent = revisedList.FirstOrDefault(x => x.ID == parentID);
                parentID = parent?.ParentPackingListID ?? 0;
            }

            parentID = parent.ID;
            while(parentID > 0)
            {
                var checkList = revisedList
                    .Where(x => (x.ParentPackingListID ?? 0) == parentID).ToList();

                resultList.AddRange(checkList.Where(x => x.IsSeparated != true).ToList());

                parentID = checkList?.FirstOrDefault(x => x.IsSeparated == true)?.ID ?? 0;
            }

            if (parent.TotalQuantity != resultList?.Sum(x => x.TotalQuantity))
                return true;
            else
                return false;
        }
        public void GA_UpdateSeparatePackingList(PackingList packingList)
        {
            var separates = new List<PackingList>();
            //decimal totQty = 0;
            var parent = packingList;
            var parentID = parent?.ParentPackingListID ?? 0; 

            while (parentID > 0)
            {
                parent = context.PackingList
                    .AsNoTracking()
                    .Include(x => x.ItemStyles)
                    .ThenInclude(i => i.PackingOverQuantities)
                    .Include(x => x.ItemStyles)
                    .ThenInclude(i => i.OrderDetails)
                    .Include(x => x.PackingLines)
                    .ThenInclude(l => l.BoxDimension)
                    .FirstOrDefault(x => x.ID == parentID);
                parentID = parent?.ParentPackingListID ?? 0;
            }

            parentID = parent.ID;
            while (parentID > 0)
            {
                var lists = context.PackingList
                    .AsNoTracking()
                    .Include(x => x.PackingLines)
                    .Where(x => (x.ParentPackingListID ?? 0) == parentID).ToList();

                separates.AddRange(lists.Where(x => x.IsSeparated != true).ToList());

                parentID = lists?.FirstOrDefault(x => x.IsSeparated == true)?.ID ?? 0;
            }

            // if (totQty != parent.TotalQuantity)
            //{
            var itemStyle = new Dictionary<string, string>();
            var separateQuantity = new Dictionary<string, decimal>();
            var overQuantities = new List<PackingOverQuantity>();
            var netWeights = new List<StyleNetWeight>();

            parent.ItemStyles.OrderBy(x => x.LSStyle).ToList().ForEach(x =>
            {
                itemStyle.Add(x.LSStyle, x.LSStyle);
                overQuantities.AddRange(x.PackingOverQuantities);
            });
                
            netWeights = context.StyleNetWeight.Where(x => parent.ItemStyles
                            .Select(x => x.CustomerStyle).Contains(x.CustomerStyle)).ToList();

            separates?.Where(p => p.ID != packingList.ID)
                ?.ToList()?.ForEach(x =>
            {
                x.PackingLines.ToList().ForEach(x =>
                {
                    var key = x.LSStyle + ";" + x.Size;
                    var value = (decimal)(x.QuantitySize * x.TotalCarton);
                    if(!separateQuantity.ContainsKey(key))
                    {
                        separateQuantity.Add(key,value);
                    }
                    else
                    {
                        separateQuantity[key] += value;
                    }
                });
            });

            var newPackingList = context.PackingList
                .Include(x => x.ItemStyles)
                .ThenInclude(i => i.OrderDetails)
                .Include(x => x.PackingLines)
                .ThenInclude(p => p.BoxDimension)
                .FirstOrDefault(x => x.ID == packingList.ID);
            foreach (var line in newPackingList.PackingLines)
            {
                var parentQty = parent.PackingLines
                    .Where(p => p.Size == line.Size && p.LSStyle == line.LSStyle)
                    .Sum(x => x.QuantitySize * x.TotalCarton);
                
                if (separateQuantity.ContainsKey(line?.LSStyle + ";" + line?.Size))
                {
                    line.TotalQuantity = parentQty - separateQuantity[line.LSStyle + ";" + line.Size];
                }
                else
                {
                    line.TotalQuantity = parentQty;
                }
                //line.TotalQuantity = parentQty - separateQuantity[line.LSStyle + ";" + line.Size];
            }

            var newPackingLines = SeparatePackingListProcess
                .SeparatePacking(newPackingList.PackingLines.ToList(), itemStyle, overQuantities, netWeights, true);

            newPackingList.PackingLines = new List<PackingLine>(newPackingLines);
            newPackingList.TotalQuantity = newPackingLines.Sum(x => x.QuantitySize * x.TotalCarton);

            //var newStyle = context.ItemStyle
            //    .Include(x => x.OrderDetails)
            //    .Where(x => parent.ItemStyles.Select(x => x.Number)
            //    .Contains(x.Number)).ToList();
            //newPackingList.ItemStyles = new List<ItemStyle>(newStyle);

            context.PackingList.Update(newPackingList);
            context.SaveChanges();
            //}
        }
        public void IFG_UpdateSeparatePackingList(PackingList packingList)
        {
            var separates = new List<PackingList>();
            //decimal totQty = 0;
            var parent = packingList;
            var parentID = parent?.ParentPackingListID ?? 0;

            while (parentID > 0)
            {
                parent = context.PackingList
                    .AsNoTracking()
                    .Include(x => x.ItemStyles)
                    .ThenInclude(i => i.PackingOverQuantities)
                    .Include(x => x.ItemStyles)
                    .ThenInclude(i => i.OrderDetails)
                    .Include(x => x.PackingLines)
                    .ThenInclude(l => l.BoxDimension)
                    .FirstOrDefault(x => x.ID == parentID);
                parentID = parent?.ParentPackingListID ?? 0;
            }

            parentID = parent.ID;
            while (parentID > 0)
            {
                var lists = context.PackingList
                    .AsNoTracking()
                    .Include(x => x.PackingLines)
                    .Where(x => (x.ParentPackingListID ?? 0) == parentID).ToList();

                separates.AddRange(lists.Where(x => x.IsSeparated != true).ToList());

                parentID = lists?.FirstOrDefault(x => x.IsSeparated == true)?.ID ?? 0;
            }


            var netWeights = context.StyleNetWeight.Where(x => parent.ItemStyles
                .Select(x => x.CustomerStyle).Contains(x.CustomerStyle)).ToList();

            var separateQuantity = new Dictionary<string, decimal>();
            separates?.Where(p => p.ID != packingList.ID)?.ToList()?.ForEach(x =>
            {
                x.PackingLines.ToList().ForEach(x =>
                {
                    var key = x.LSStyle + ";" + x.Size;
                    var value = (decimal)(x.QuantitySize * x.TotalCarton);
                    if (!separateQuantity.ContainsKey(key))
                    {
                        separateQuantity.Add(key, value);
                    }
                    else
                    {
                        separateQuantity[key] += value;
                    }
                });
            });

            var newPackingList = context.PackingList
                .Include(x => x.ItemStyles)
                .ThenInclude(i => i.OrderDetails)
                .Include(x => x.PackingLines)
                .ThenInclude(p => p.BoxDimension)
                .FirstOrDefault(x => x.ID == packingList.ID);
            foreach (var line in newPackingList.PackingLines)
            {
                var parentQty = parent.PackingLines
                   .Where(p => p.Size == line.Size && p.LSStyle == line.LSStyle)
                   .Sum(x => x.QuantitySize * x.TotalCarton);
                if (separateQuantity.ContainsKey(line?.LSStyle + ";" + line?.Size))
                {
                    line.TotalQuantity = parentQty - separateQuantity[line.LSStyle + ";" + line.Size];
                }
                else
                {
                    line.TotalQuantity = parentQty;
                }
            }

            var newPackingLines = IFG_SeparatePackingListProcess
                .SeparatePacking(newPackingList?.PackingLines?.ToList(), newPackingList?.ItemStyles?.ToList(), netWeights);

            newPackingList.PackingLines = new List<PackingLine>(newPackingLines);
            newPackingList.TotalQuantity = newPackingLines.Sum(x => x.QuantitySize * x.TotalCarton);

            
            context.PackingList.Update(newPackingList);
            context.SaveChanges();
        }
    }
}
