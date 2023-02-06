using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Mobile.Services
{
    public interface IQrScanningService
    {
        Task<string> ScanAsync();
    }
}
