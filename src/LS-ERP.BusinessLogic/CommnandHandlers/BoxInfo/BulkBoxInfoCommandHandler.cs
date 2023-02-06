using AutoMapper;
using Common.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.EntityFrameworkCore.Entities;
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
    public class BulkBoxInfoCommandHandler
         : IRequestHandler<BulkBoxInfoCommand, CommonCommandResult>
    {
        private readonly ILogger<BulkBoxInfoCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;
        private readonly IMediator mediator;

        public BulkBoxInfoCommandHandler(ILogger<BulkBoxInfoCommandHandler> logger,
            SqlServerAppDbContext context,
            IMapper mapper,
            IMediator mediator)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
            this.mediator = mediator;
        }
        public Task<CommonCommandResult> Handle(BulkBoxInfoCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult();
            var boxInfos = new List<BoxInfo>(); 
            var deleteBoxInfos = new List<BoxInfo>();

            try
            {
                if(request.Data.Any())
                {
                    deleteBoxInfos = context.BoxInfo.Where(x => x.IsDeleted != true).ToList();
                    deleteBoxInfos.ForEach(x =>
                    {
                        x.IsDeleted = true;
                        x.SetUpdateAudit(request.UserName);
                    });
                }

                foreach (var data in request.Data)
                {
                    var boxInfo = new BoxInfo();
                    boxInfo.ID = data.ID;
                    boxInfo.TagID=data.TagID;
                    boxInfo.GarmentColorCode= data.GarmentColorCode;
                    boxInfo.ItemCode=data.ItemCode;
                    boxInfo.Description = data.Description;
                    boxInfo.GarmentSize=data.GarmentSize;
                    boxInfo.CustomerStyle=data.CustomerStyle;
                    boxInfo.QuantityPerBox=data.QuantityPerBox;
                    boxInfo.CustomerID = request.CustomerID;
                    boxInfo.SetCreateAudit(request.UserName);

                    boxInfos.Add(boxInfo);
                }

                context.BoxInfo.AddRange(boxInfos);
                context.BoxInfo.UpdateRange(deleteBoxInfos);
                context.SaveChanges();

                result.Success = true;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
            }

            return Task.FromResult(result);
        }
    }
}
