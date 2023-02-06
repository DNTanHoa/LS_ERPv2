using Common;
using LS_ERP.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Mobile.Services.Interface
{
    public interface IDeliveryNoteService
    {
        Task<CommonResponseModel<List<DeliveryNoteModel>>> GetDeliveryNote(string companyID, string type);
    }
}
