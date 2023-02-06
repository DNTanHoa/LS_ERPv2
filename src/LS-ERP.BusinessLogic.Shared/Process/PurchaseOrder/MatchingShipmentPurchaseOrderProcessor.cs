using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public class MatchingShipmentPurchaseOrderProcessor
    {
        public static void Matching(List<ShipmentDetail> shipmentDetails, ref List<PurchaseOrderLine> purchaseOrderLines,
            out List<ShipmentDetail> updateShipmentDetails,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            updateShipmentDetails = new List<ShipmentDetail>();
            try
            {
                var dicShipmentDetails = new Dictionary<string, ShipmentDetail>(); //shipmentDetails.ToDictionary(x => x.CustomerPurchaseOrderNumber + x.ContractNo + x.MaterialCode + x.Color);
                foreach (var shipmentDetail in shipmentDetails)
                {
                    if (!dicShipmentDetails.ContainsKey(shipmentDetail.KeyShipmentDetail()))
                    {
                        dicShipmentDetails[shipmentDetail.KeyShipmentDetail()] = shipmentDetail;
                    }
                }

                foreach (var purchaseOrderLine in purchaseOrderLines)
                {
                    var keyPOLine = purchaseOrderLine.KeyMatchingShipment();

                    if (dicShipmentDetails.TryGetValue(keyPOLine, out ShipmentDetail rsValue))
                    {
                        purchaseOrderLine.DeliveryNote = rsValue.DeliveryNo;
                        purchaseOrderLine.MatchingShipment = true;
                        rsValue.MatchedPO = true;
                        updateShipmentDetails.Add(rsValue);
                    }

                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

        }

    }
}
