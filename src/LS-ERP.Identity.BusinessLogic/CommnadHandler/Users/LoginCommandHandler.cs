using LS_ERP.Identity.BusinessLogic.Command;
using LS_ERP.Identity.BusinessLogic.Command.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LS_ERP.Identity.BusinessLogic.CommnadHandler
{
    public class LoginCommandHandler
        : IRequestHandler<LoginCommand, LoginResult>
    {
        private readonly ILogger<LoginCommandHandler> logger;

        public LoginCommandHandler(ILogger<LoginCommandHandler> logger)
        {
            this.logger = logger;
        }

        public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
