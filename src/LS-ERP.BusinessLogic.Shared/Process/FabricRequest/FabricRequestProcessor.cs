using AutoMapper;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public class FabricRequestProcessor
    {
        public static void Update(string userName, FabricRequest request, FabricRequest oldFabricRequest,
            out List<FabricRequestDetail> newRequestDetails,
            out List<FabricRequestDetail> updateRequestDetails,
            out List<FabricRequestDetail> deleteRequestDetails)
        {
            newRequestDetails = new List<FabricRequestDetail>();
            updateRequestDetails = new List<FabricRequestDetail>();
            deleteRequestDetails = new List<FabricRequestDetail>();

            var config = new MapperConfiguration(config =>
            {
                config.CreateMap<FabricRequestDetail, FabricRequestDetail>()
                .ForMember(x => x.FabricRequest, y => y.Ignore())
                .ForMember(x => x.CreatedAt, y => y.Ignore())
                .ForMember(x => x.CreatedBy, y => y.Ignore())
                .ForMember(x => x.LastUpdatedAt, y => y.Ignore())
                .ForMember(x => x.LastUpdatedBy, y => y.Ignore());

                config.CreateMap<FabricRequest, FabricRequest>()
                .ForMember(x => x.Details, y => y.Ignore())
                .ForMember(x => x.CreatedAt, y => y.Ignore())
                .ForMember(x => x.CreatedBy, y => y.Ignore())
                .ForMember(x => x.LastUpdatedAt, y => y.Ignore())
                .ForMember(x => x.LastUpdatedBy, y => y.Ignore());
            });

            var mapper = config.CreateMapper();

            var dicOldRequestDetails = oldFabricRequest.Details?.ToDictionary(x => x.ID);

            foreach (var itemRequest in request.Details)
            {
                if (dicOldRequestDetails != null && dicOldRequestDetails.TryGetValue(itemRequest.ID, out FabricRequestDetail rsRequestDetail))
                {
                    rsRequestDetail = mapper.Map<FabricRequestDetail, FabricRequestDetail>(itemRequest);
                    rsRequestDetail.SetUpdateAudit(userName);
                    updateRequestDetails.Add(rsRequestDetail);
                }
                else if (itemRequest.ID == 0)
                {
                    var newRequestDetail = mapper.Map<FabricRequestDetail, FabricRequestDetail>(itemRequest);
                    newRequestDetail.FabricRequestID = oldFabricRequest.ID;
                    newRequestDetail.SetCreateAudit(userName);
                    newRequestDetails.Add(newRequestDetail);
                }
            }

            var dicItemUpdate = updateRequestDetails.ToDictionary(x => x.ID);
            foreach (var itemOldRequest in oldFabricRequest.Details)
            {
                if (!dicItemUpdate.ContainsKey(itemOldRequest.ID))
                {
                    itemOldRequest.FabricRequest = null;
                    deleteRequestDetails.Add(itemOldRequest);
                }
            }

            //oldFabricRequest = mapper.Map<FabricRequest, FabricRequest>(request);
        }

        public static void UpdateFabricRequestDetail(string userName, List<IssuedGroupLine> issuedGroupLines,
            ref FabricRequest fabricRequest,
            ref List<FabricRequestDetail> oldRequestDetails)
        {
            decimal total = 0;

            foreach (var oldRequestDetail in oldRequestDetails)
            {
                if (oldRequestDetail.IssuedQuantity == null)
                {
                    oldRequestDetail.IssuedQuantity = 0;
                }

                oldRequestDetail.IssuedQuantity += issuedGroupLines
                    .Where(r => r.FabricRequestDetailID == oldRequestDetail.ID)
                    .Sum(s => s.IssuedQuantity);

                if (oldRequestDetail.SemiFinishedProductQuantity == null)
                {
                    oldRequestDetail.SemiFinishedProductQuantity = 0;
                }

                if (oldRequestDetail.StreakRequestQuantity == null)
                {
                    oldRequestDetail.StreakRequestQuantity = 0;
                }

                total += ((decimal)oldRequestDetail.RequestQuantity +
                    (decimal)oldRequestDetail.SemiFinishedProductQuantity +
                    (decimal)oldRequestDetail.StreakRequestQuantity) - (decimal)oldRequestDetail.IssuedQuantity;
            }

            if (total < 1)
            {
                fabricRequest.StatusID = "I";
                fabricRequest.Status = null;
            }
            else
            {
                fabricRequest.StatusID = "Issuing";
                fabricRequest.Status = null;
            }
        }
    }
}
