using Common;
using LS_ERP.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Mobile.Services.Interface
{
    public interface ICuttingCardService
    {
        Task<CommonResponseModel<List<CuttingCardModel>>> GetCuttingCard(string Id);
        Task<CommonResponseModel<List<CuttingCardModel>>> GetLocationCuttingCard(string Id);
        Task<CommonResponseModel<CuttingCardModel>> UpdateCuttingCard(CuttingCardModel cuttingCard);
        Task<CommonResponseModel<CuttingCardModel>> UpdateBulkCuttingCard(BulkCuttingCardModel bulkCuttingCard);
    }
}
