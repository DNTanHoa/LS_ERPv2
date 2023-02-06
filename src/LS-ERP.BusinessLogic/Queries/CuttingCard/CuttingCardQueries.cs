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


namespace LS_ERP.BusinessLogic.Queries
{
    public class CuttingCardQueries : ICuttingCardQueries
    {
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        public CuttingCardQueries(SqlServerAppDbContext context, IMapper mapper, IConfiguration configuration)
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        public IEnumerable<CuttingCardDtos> GetByCuttingOutputID(int cuttingOutputID)
        {

            var result = context.CuttingCard.Where(x=>x.CuttingOutputID == cuttingOutputID).ToList();
            foreach (var r in result)
            {
                r.QRCodeBase = QRCodeHelper.GenerateQRCodeBase64(r.ID);
            }
            return result.Select(x => mapper.Map<CuttingCardDtos>(x));
        }
        public IEnumerable<CuttingCardDtos> GetByID(string ID)
        {
            var cuttingCards = context.CuttingCard.Where(x => x.ID == ID && x.Operation == "CUTTING").ToList();
            var result = new List<CuttingCardDtos>();
            var cards =  cuttingCards.Select(x => mapper.Map<CuttingCardDtos>(x));
            foreach(var res in cards)
            {
                var deliveryNoteDetail = context.DeliveryNoteDetail.Where(x => x.CuttingCardID == res.ID && x.IsDeleted == false).FirstOrDefault();
                if(deliveryNoteDetail != null)
                {
                    res.DeliveryNote = context.DeliveryNote.Where(x => x.ID == deliveryNoteDetail.DeliveryNoteID).FirstOrDefault();                   
                }
                result.Add(res);
            }    
            return result;
        }
        public IEnumerable<CuttingCardDtos> GetLocationByID(string ID)
        {
            var parrentID = "";
            var masterCard = context.CuttingCard.Where(x => x.ID == ID).FirstOrDefault();
            if(masterCard!=null)
            {
                parrentID = masterCard.ParentID;
            }    
            var result = context.CuttingCard.Where(x => x.ParentID == parrentID && x.Operation == "CUTTING").ToList();

            return result.Select(x => mapper.Map<CuttingCardDtos>(x)).ToList();
        }
        public IEnumerable<CuttingCardDtos> GetByMasterID(string parrentID)
        {
            var result = context.CuttingCard.Where(x => x.ParentID == parrentID && x.Operation == "CUTTING").ToList();
            foreach (var r in result)
            {
                r.QRCodeBase = QRCodeHelper.GenerateQRCodeBase64(r.ID);
            }

            return result.Select(x => mapper.Map<CuttingCardDtos>(x)).ToList().OrderByDescending(x => x.CardTypeID);

            // export for user
            //var result = context.CuttingCard.Where(x => x.MergeBlockLSStyle == "345934-SS23-B" && x.CardTypeID =="O7").ToList();
            //foreach (var r in result)
            //{
            //    r.QRCodeBase = QRCodeHelper.GenerateQRCodeBase64(r.ID);
            //}
            //return result.Select(x => mapper.Map<CuttingCardDtos>(x)).ToList().OrderByDescending(x => x.WorkCenterName).ThenBy(x=>x.TableNO).ThenBy(x=>x.Size);
        }
        public IEnumerable<CuttingCardDtos> GetByMultiMasterID(List<string> ListParrentID)
        {
            var result = context.CuttingCard.Where(x => ListParrentID.Contains(x.ParentID) && x.Operation == "CUTTING").ToList();
            foreach (var r in result)
            {
                r.QRCodeBase = QRCodeHelper.GenerateQRCodeBase64(r.ID);
            }

            return result.Select(x => mapper.Map<CuttingCardDtos>(x)).ToList().OrderByDescending(x => x.CardTypeID);
        }
        public IEnumerable<CuttingCardDtos> GetByCompanyID(string companyId, string operation, string keyword, DateTime produceDate)
        {
            var result = from cc in context.CuttingCard.Where(cc => cc.Operation == operation
                                                              && cc.ProduceDate.Date >= produceDate.Date
                                                              && !cc.CardType.Equals("MASTER")
                                                              && (cc.MergeBlockLSStyle.Contains(keyword)
                                                                || cc.MergeLSStyle.Contains(keyword)
                                                                || cc.Size.Contains(keyword)
                                                                || cc.FabricContrastColor.Contains(keyword)
                                                                || cc.WorkCenterName.Contains(keyword)
                                                                || string.IsNullOrEmpty(keyword)))
                         from co in context.CuttingOutput.Where(co=>co.ID == cc.CuttingOutputID && co.WorkCenterID.Contains(companyId))                         
                         select cc;
            return result.Select(x => mapper.Map<CuttingCardDtos>(x));
        }
        public IEnumerable<ComplingReportStatusDtos> GetComplingReportStatus(string companyID, string customerName, string keyword, DateTime fromDate,DateTime toDate)
        {
            var connectionString = configuration.GetSection("ConnectionString")
               .GetSection("DbConnection").Value ?? string.Empty;
            SqlParameter[] parameters =
            {
                new SqlParameter("@CompanyID",companyID ?? string.Empty),
                new SqlParameter("CustomerName",customerName ?? string.Empty),
                new SqlParameter("Keyword", keyword ?? string.Empty),
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };
            DataTable table = SqlHelper.FillByReader(connectionString,
                "sp_SelectComplingReportStatus", parameters);
            var result = table.AsListObject<ComplingReportStatusDtos>();
            return result;
        }
        public IEnumerable<CuttingCardDtos> GetMasterByCompanyID(string companyId, string operation, string keyword,DateTime produceDate)
        {
            var result = from cc in context.CuttingCard.Where(cc => cc.Operation == operation
                                                              && cc.ProduceDate.Date >= produceDate.Date
                                                              && cc.CardType == "MASTER"
                                                              && (cc.MergeBlockLSStyle.Contains(keyword)
                                                                || cc.MergeLSStyle.Contains(keyword)
                                                                || cc.Size.Contains(keyword)
                                                                || cc.FabricContrastColor.Contains(keyword)
                                                                || cc.WorkCenterName.Contains(keyword)
                                                                || string.IsNullOrEmpty(keyword)))
                         from co in context.CuttingOutput.Where(co => co.ID == cc.CuttingOutputID && co.WorkCenterID.Contains(companyId))
                         select cc;
            return result.Select(x => mapper.Map<CuttingCardDtos>(x)).ToList().OrderByDescending(x => x.CreatedAt);
        }
        public IEnumerable<CuttingCardDtos> GetByCompanyIDForSupperMarket(string companyId, string operation, string keyword)
        {
            var result = from cc in context.CuttingCard.Where(cc => cc.CurrentOperation == operation
                                                              && !cc.CardType.Equals("MASTER")
                                                              && (cc.MergeBlockLSStyle.Contains(keyword)
                                                                || cc.MergeLSStyle.Contains(keyword)
                                                                || cc.Size.Contains(keyword)
                                                                || cc.FabricContrastColor.Contains(keyword)
                                                                || cc.WorkCenterName.Contains(keyword)
                                                                || string.IsNullOrEmpty(keyword)))
                         from co in context.CuttingOutput.Where(co => co.ID == cc.CuttingOutputID && co.WorkCenterID.Contains(companyId))
                         select cc;
            return result.Select(x => mapper.Map<CuttingCardDtos>(x)).ToList().OrderByDescending(x => x.ProduceDate);
        }
    }   
}
