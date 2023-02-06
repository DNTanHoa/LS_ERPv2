
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Kanban.Logic
{
    public interface IAuthenticateHelper
    {
        string RefreshToken { get; set; }
        string AccessToken { get; set; }
        DateTime? ExpiredDate { get; set; }
        HttpContext? context { get; set; }

        public void SetInforFromContext(HttpContext context);
        public void SetResponse(HttpContext context);
        public string GetAccessToken();
    }
}
