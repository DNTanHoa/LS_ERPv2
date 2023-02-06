using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Report
{
    public class PurchaseOrderReportDetail
    {
        public PurchaseOrderReport PurchaseOrderReport { get; set; }
        public string No { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Specify { get; set; }
        public string Season { get; set; }

        public string CustomerStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string ContractNo { get; set; }

        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public string UnitID { get; set; }
        public decimal? LessQuantity { get; set; }
        public decimal? FreeQuantity { get; set; }
        public decimal? Total { get; set; }

        public bool? RoundUp { get; set; }
        public bool? RoundDown { get; set; }

        public string ToDictionaryKey(string customerID)
        {
            string key = string.Empty;
            switch (customerID)
            {
                case "HA":
                    {
                        key = this.ItemID + this.ContractNo + this.Season
                            + this.GarmentSize
                            + this.ItemName + this.Specify + this.UnitID
                            + this.Price + this.ItemColorCode + this.ItemColorName;
                    }
                    break;
                case "DE":
                    {
                        if (!this.UnitID.Equals("PCS") &&
                            !this.UnitID.Equals("SETS"))
                        {
                            this.GarmentSize = "";

                            key = this.ItemID + this.ItemName
                            + this.Specify
                            + this.ItemColorCode + this.GarmentColorCode;
                        }
                        else
                        {
                            key = this.ItemID + this.GarmentColorCode + this.Season
                            + this.GarmentSize + this.CustomerStyle
                            + this.ItemName + this.Specify + this.UnitID
                            + this.Price + this.ItemColorCode + this.ItemColorName;
                        }
                    }
                    break;
                default:
                    {
                        key = this.ItemID + this.CustomerStyle + this.Season
                            + this.GarmentSize
                            + this.ItemName + this.Specify + this.UnitID
                            + this.Price + this.ItemColorCode + this.ItemColorName;
                    }
                    break;
                
            }
            return key;
        }


        public decimal RoundQuantity(PurchaseOrderReportDetail purchaseOrderReportDetail)
        {
            decimal quantity = 0;
            if (purchaseOrderReportDetail.RoundDown != null && (bool)purchaseOrderReportDetail.RoundDown)
            {
                quantity = Math.Floor((decimal)purchaseOrderReportDetail.Quantity);
            }
            else if (purchaseOrderReportDetail.RoundUp != null && (bool)purchaseOrderReportDetail.RoundUp)
            {
                quantity = Math.Ceiling((decimal)purchaseOrderReportDetail.Quantity);
            }
            else
            {
                quantity = purchaseOrderReportDetail.Quantity ?? 0;
            }
            return quantity;
        }
    }
}
