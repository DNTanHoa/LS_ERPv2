using LS_ERP.Identity.BusinessLogic.Command.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LS_ERP.Identity.BusinessLogic.Command
{
    public class LoginCommand
        : IRequest<LoginResult>
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string Signature { get; set; }
    }
}
