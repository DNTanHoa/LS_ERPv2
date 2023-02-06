using LS_ERP.BusinessLogic.Dtos.PackingList;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Validator
{
    public  class PackingListValidator
    {
        private readonly IItemStyleRepository itemStyleRepository;

        public PackingListValidator(IItemStyleRepository itemStyleRepository)
        {
            this.itemStyleRepository = itemStyleRepository;
        }


        public bool IsValid(List<PackingListImportDto> packingListDtos, out string errorMessage)
        {
            var itemStyle = new ItemStyle();
            /// Check ItemStyle by PO number
            foreach (var data in packingListDtos)
            {
                itemStyle = itemStyleRepository.GetItemStylesByPONumber(data.PONumber)
                                    .OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                if(itemStyle == null)
                {
                    errorMessage = "Please import item style before";
                    return false;
                }
                else  /// Check size 
                {
                    if(itemStyle.OrderDetails.ToList().Find(x => x.Size.Trim().ToUpper() == data.POSize.Trim().ToUpper()) == null)
                    {
                        errorMessage = "Please check size order again";
                        return false;
                    }
                }
            }
            
            errorMessage = string.Empty;
            return true;
        }
    }
}
