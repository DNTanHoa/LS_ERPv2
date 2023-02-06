using Common.Model;
using LS_ERP.BusinessLogic.Commands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class DeleteJobOperationCommandHandler
        : IRequestHandler<DeleteJobOperationCommand, CommonCommandResult>
    {
        public Task<CommonCommandResult> Handle(DeleteJobOperationCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
