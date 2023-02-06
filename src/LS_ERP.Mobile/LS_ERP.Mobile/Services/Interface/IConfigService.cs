using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Mobile.Services.Interface
{
    public interface  IConfigService
    {
        Task<string> GetHostErpApi();
        Task<string> GetHostHelpDeskApi();
    }
}
