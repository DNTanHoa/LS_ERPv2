using AutoMapper;
using Microsoft.Extensions.Configuration;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultils.Extensions;
using Ultils.Helpers;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.BusinessLogic.Queries
{
    public class DeliveryNoteQueries : IDeliveryNoteQueries
    {
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        public DeliveryNoteQueries(SqlServerAppDbContext context, IMapper mapper, IConfiguration configuration)
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        public IEnumerable<DeliveryNoteDtos> GetByID(string ID)
        {
            var deliveryNote = context.DeliveryNote.Where(x => x.ID == ID && x.IsDeleted == false).ToList();
            var result = new List<DeliveryNoteDtos>();
            var resultDeliveryNote = deliveryNote.Select(x => mapper.Map<DeliveryNoteDtos>(x));
            foreach (var res in resultDeliveryNote)
            {
                res.Details = new List<DeliveryNoteDetail>();                
                res.Details = getDeleveryNoteDetail(res);
                res.LSStyleDetails = new List<LSStyleDetailDtos>();
                res.LSStyleDetails = getLSStyleDetails(res);
                var company = context.Company.Where(x => x.Code == res.CompanyID).FirstOrDefault();
                res.CompanyName = company?.OrtherName;
                res.Address = company?.Address;
                result.Add(res);
            }
            return result;
        }
        public IEnumerable<DeliveryNoteDtos> GetByCompany(string companyID, DateTime fromDate, DateTime toDate)
        {
            var deliveryNotes = context.DeliveryNote.Where(d=>d.CompanyID == companyID
                                                            && d.IsDeleted == false
                                                            && d.CreatedAt.Value.Date >= fromDate
                                                            && d.CreatedAt.Value.Date <= toDate
                                                            ).ToList().OrderByDescending(o=>o.CreatedAt).ToList();
            var result = deliveryNotes.Select(x => mapper.Map<DeliveryNoteDtos>(x));
            //foreach(var res in result)
            //{
            //    res.Details = getDeleveryNoteDetail(res);
            //    res.LSStyleDetails = getLSStyleDetails(res);
            //}    
            return result;
        }
        public IEnumerable<DeliveryNoteDtos> GetForScanQrcode(string companyID, string type)
        {
            var result = context.DeliveryNote.Where(d => d.CompanyID == companyID
                                                            && d.IsDeleted == false
                                                            && d.Status == "NEW" 
                                                            && d.Type == type
                                                            ).ToList().OrderByDescending(o => o.CreatedAt).ToList();

            return result.Select(x => mapper.Map<DeliveryNoteDtos>(x));
        }

        public List<LSStyleDetailDtos> getLSStyleDetails(DeliveryNoteDtos deliveryNoteDtos)
        {
            var result = new List<LSStyleDetailDtos>();
            var cuttingCards = context.CuttingCard.Where(c => deliveryNoteDtos.Details.Select(s => s.CuttingCardID ).ToList().Contains(c.ID)).ToList();
            foreach(var cutttingCard in cuttingCards)
            {
                var lsstyle = (from c in context.CuttingOutput.Where(x => x.ID == cutttingCard.CuttingOutputID)
                               from at in context.AllocTransaction.Where(x => x.CuttingOutputID == c.ID && x.Lot == null)
                               select new LSStyleDetailDtos
                               {
                                   LSStyle = at.LSStyle,
                                   FabricContrastName = at.FabricContrastName,
                                   Set = at.Set,
                                   Size = at.Size,
                                   Quantity = at.AllocQuantity
                               }
                              ).AsEnumerable().GroupBy(g => new { g.LSStyle, g.FabricContrastName, g.Set, g.Size })
                              .Select(s => new LSStyleDetailDtos
                              {
                                  LSStyle = s.Key.LSStyle,
                                  FabricContrastName = s.Key.FabricContrastName,
                                  Set = s.Key.Set,
                                  Size = s.Key.Size,
                                  Quantity = s.Sum(t => t.Quantity)
                              }).ToList();
                result.AddRange(lsstyle);
            }    
            result = result.AsEnumerable().GroupBy(g => new { g.LSStyle, g.FabricContrastName, g.Set, g.Size })
                              .Select(s => new LSStyleDetailDtos
                              {
                                  LSStyle = s.Key.LSStyle,
                                  FabricContrastName = s.Key.FabricContrastName,
                                  Set = s.Key.Set,
                                  Size = s.Key.Size,
                                  Quantity = s.Sum(t => t.Quantity)
                              }).ToList();
            return result.OrderBy(o=>o.LSStyle).ToList();
        }
        public List<DeliveryNoteDetail> getDeleveryNoteDetail(DeliveryNoteDtos deliveryNoteDtos)
        {
            var result = new List<DeliveryNoteDetail>();
            result = context.DeliveryNoteDetail.Where(d => d.DeliveryNoteID == deliveryNoteDtos.ID && d.IsDeleted == false)
                .OrderBy(o=>o.MergeBlockLSStyle).ThenBy(o=>o.FabricContrastName).ThenBy(o=>o.Set).ThenBy(o=>o.Size).ToList();
            //result = result.AsEnumerable()
            //        .GroupBy(g => new
            //        {
            //            g.CuttingCardID,
            //            g.MergeBlockLSStyle,
            //            g.MergeLSStyle,
            //            g.FabricContrastName,
            //            g.Set,
            //            g.Size
            //        }).Select(s => new DeliveryNoteDetail
            //        {
            //            CuttingCardID = s.Key.CuttingCardID,
            //            MergeBlockLSStyle = s.Key.MergeBlockLSStyle,
            //            MergeLSStyle = s.Key.MergeLSStyle,
            //            FabricContrastName = s.Key.FabricContrastName,
            //            Set = s.Key.Set,
            //            Size = s.Key.Size,
            //            AllocQuantity = s.Sum(t=>t.AllocQuantity)
            //        }).ToList();
            return result;
        }
        public IEnumerable<DeliveryNoteDetailDtos> GetReport(string companyID, DateTime fromDate, DateTime toDate, string keyWord)
        {
            var result = (from d in context.DeliveryNote.Where(x => x.CompanyID == companyID && x.IsDeleted == false)
                          from dt in context.DeliveryNoteDetail.Where(x => x.DeliveryNoteID == d.ID
                                                                     && x.IsSend == true
                                                                     &&
                                                                     ((x.SendDate.Date >= fromDate.Date
                                                                        && x.SendDate.Date <= toDate.Date
                                                                        && string.IsNullOrEmpty(keyWord))
                                                                      || (x.MergeBlockLSStyle.ToUpper().Contains(keyWord) && !string.IsNullOrEmpty(keyWord))
                                                                     )
                                                                     && x.IsDeleted == false
                                                                     )
                          select new DeliveryNoteDetailDtos
                          {
                              ID = dt.ID,
                              CompanyID = d.CompanyID,
                              DeliveryNoteID = d.ID,
                              CuttingCardID = dt.CuttingCardID,
                              MergeBlockLSStyle = dt.MergeBlockLSStyle,
                              MergeLSStyle = dt.MergeLSStyle,
                              FabricContrastName = dt.FabricContrastName,
                              Set = dt.Set,
                              Size = dt.Size,
                              WorkCenterName = dt.WorkCenterName,
                              TableNO = dt.TableNO,
                              TotalPackage = dt.TotalPackage,
                              ProduceDate = dt.ProduceDate,
                              AllocQuantity = dt.AllocQuantity,
                              Status = dt.Status,
                              IsSend = dt.IsSend,
                              SendDate = dt.SendDate,
                              IsReceived = dt.IsReceived,
                              ReceivedDate = dt.IsReceived ? dt.ReceivedDate : null,
                              CardType = dt.CardType,
                              DeliveryNote = d
                          }).ToList().OrderByDescending(o=>o.SendDate).ThenBy(o=>o.MergeBlockLSStyle).ToList();
            return result;
        }
        public IEnumerable<LSStyleDetailReportDtos> GetReceiveReport(string companyID, DateTime fromDate, DateTime toDate, string keyWord)
        {
            var connectionString = configuration.GetSection("ConnectionString")
               .GetSection("DbConnection").Value ?? string.Empty;
            SqlParameter[] parameters =
            {
                new SqlParameter("@CompanyID",companyID ?? string.Empty),             
                new SqlParameter("Keyword", keyWord ?? string.Empty),
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };
            DataTable table = SqlHelper.FillByReader(connectionString,
                "sp_SelectPrintReportStatus", parameters);
            var result = table.AsListObject<LSStyleDetailReportDtos>();
            return result;
        }
    }   
}
