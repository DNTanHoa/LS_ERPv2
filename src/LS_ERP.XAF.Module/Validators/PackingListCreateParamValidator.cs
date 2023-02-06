using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Validators
{
    public class PackingListCreateParamValidator
    {
        public static bool IsValid(List<ItemStyle> newStyleValidators, IObjectSpace objectSpace, out string errorMessage)
        {
            errorMessage = string.Empty;
            bool checkSize = false;
            errorMessage = "Please input net weight for size: ";
            var customerStyle = "";
            foreach (var itemStyle in newStyleValidators.OrderBy(x => x.CustomerStyle))
            {
                if(customerStyle != itemStyle.CustomerStyle)
                {
                    var orderDetailCriteria = CriteriaOperator.Parse("[ItemStyleNumber] " +
                   "IN (" + string.Join(",", "'" + itemStyle.Number + "'") + ")");
                    var orderDetails = objectSpace.GetObjects<OrderDetail>(orderDetailCriteria).ToList();

                    var styleNetWeightCriteria = CriteriaOperator.Parse("[CustomerStyle] " +
                            "IN (" + string.Join(",", "'" + itemStyle.CustomerStyle + "'") + ")");
                    var styleNetWeights = objectSpace.GetObjects<StyleNetWeight>(styleNetWeightCriteria).ToList();

                    foreach (var orderDetail in orderDetails)
                    {
                        if (styleNetWeights.Find(x => x.Size == orderDetail.Size) == null)
                        {
                            checkSize = true;
                            errorMessage += orderDetail.Size + " , ";
                        }
                    }

                    var str = errorMessage.Substring(errorMessage.Length - 3, 3);
                    if (errorMessage.Substring(errorMessage.Length - 3, 3).Trim() == ",")
                    {
                        errorMessage = errorMessage.Substring(0, errorMessage.Length - 2);
                        errorMessage += "(CustomerStyle: " + itemStyle.CustomerStyle + ") & ";
                    }

                    customerStyle = itemStyle.CustomerStyle;
                }
            }
            
            if (checkSize)
            {
                errorMessage = errorMessage.Substring(0,errorMessage.Length - 3);
                return false;
            }

            return true;
        }
    }
}
