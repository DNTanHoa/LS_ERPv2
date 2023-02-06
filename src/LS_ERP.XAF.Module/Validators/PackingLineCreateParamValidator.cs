using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Validators
{
    public class PackingLineCreateParamValidator
    {
        public static bool IsValid(PackingLineCreateParam param, string customerID, out string errorMessage)
        {
            errorMessage = string.Empty;

            /// PU & IFG
            if(customerID == "PU" || customerID == "IFG")
            {
                switch (param.PackingType)
                {
                    case PackingType.SolidSizeSolidColor:

                        if (param.RemainQuantity)
                        {
                            return true;
                        }

                        if (param.TotalQuantity < 0)
                        {
                            errorMessage = "Invalid total quantity input";
                            return false;
                        }
                        else if (param.TotalQuantity == 0 && param.PackingRatios.Find(x => x.Ratio > 0) == null)
                        {
                            errorMessage = "Invalid ratio over size input";
                            return false;
                        }

                        break;
                    //case PackingType.SolidSizeAssortedColor:

                    //    if (param.TotalQuantity <= 0)
                    //    {
                    //        errorMessage = "Invalid total quantity input";
                    //        return false;
                    //    }

                    //    break;
                    case PackingType.AssortedSizeSolidColor:

                        if (param.QuantityPackagePerCarton <= 0)
                        {
                            errorMessage = "Invalid total quantity input";
                            return false;
                        }

                        break;
                        //case PackingType.AssortedSizeAssortedColor:
                        //    break;
                }

                foreach (var orderDetial in param.OrderDetails)
                {
                    if (orderDetial.ShipQuantity > orderDetial.BalanceQuantity)
                    {
                        errorMessage = "Invalid ship quantity input (greater than balance quantity)";
                        return false;
                    }
                }
            }
            else if(customerID == "HM")
            {
                switch (param.PackingType)
                {
                    case PackingType.SolidSizeAssortedColor:
                        if (param.QuantityPackagePerCarton <= 0)
                        {
                            errorMessage = "Invalid ratio over size input";
                            return false;
                        }

                        if (param.PackingRatios.Sum(x => x.TotalQuantity) <= 0)
                        {
                            errorMessage = "Invalid total quantity input";
                            return false;
                        }
                        break;

                    case PackingType.AssortedSizeSolidColor:
                        if (param.QuantityPackagePerCarton <= 0)
                        {
                            errorMessage = "Invalid quantity per carton input";
                            return false;
                        }

                        if (param.PackingRatios.Sum(x => x.Ratio) <= 0)
                        {
                            errorMessage = "Invalid ratio input";
                            return false;
                        }
                        break;
                }
                //foreach (var data in param.PackingRatios)
                //{
                //    if (data.Ratio < 0)
                //    {
                //        errorMessage = "Invalid ratio over size input";
                //        return false;
                //    }

                //    if (data.TotalQuantity < 0)
                //    {
                //        errorMessage = "Invalid total quantity input";
                //        return false;
                //    }

                //    if ((data.Ratio > 0 && data.TotalQuantity == 0) || (data.Ratio == 0 && data.TotalQuantity > 0))
                //    {
                //        errorMessage = "Invalid total quantity & ratio input";
                //        return false;
                //    }
                //}
            }
            if (customerID == "GA")
            {
                switch (param.PackingType)
                {
                    case PackingType.SolidSizeSolidColor:

                        if (param.RemainQuantity)
                        {
                            return true;
                        }

                        if (param.TotalQuantity < 0)
                        {
                            errorMessage = "Invalid total quantity input";
                            return false;
                        }
                        else if (param.TotalQuantity == 0 && param.PackingRatios.Find(x => x.Ratio > 0) == null)
                        {
                            errorMessage = "Invalid ratio over size input";
                            return false;
                        }

                        break;
                   
                    case PackingType.AssortedSizeSolidColor:

                        if (param.QuantityPackagePerCarton <= 0)
                        {
                            errorMessage = "Invalid total quantity input";
                            return false;
                        }

                        break;
                }

                foreach (var orderDetial in param.OrderDetails)
                {
                    if (orderDetial.ShipQuantity > orderDetial.BalanceQuantity)
                    {
                        errorMessage = "Invalid ship quantity input (greater than balance quantity)";
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
