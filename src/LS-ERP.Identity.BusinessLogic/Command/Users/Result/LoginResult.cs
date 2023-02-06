using LS_ERP.Identity.EntittyFramework.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LS_ERP.Identity.BusinessLogic.Command.Result
{
    public class LoginResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public User User { get; set; }
    }
}
