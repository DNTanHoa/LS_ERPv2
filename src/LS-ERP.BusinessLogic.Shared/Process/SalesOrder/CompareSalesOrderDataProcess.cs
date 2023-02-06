using AutoMapper;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Dtos.SalesOrder;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public class CompareSalesOrderDataProcess
    {
        public static void CreateCompareItemList(List<SalesOrder> newSalesOrders,
            List<ItemStyle> itemStyleQuery, DateTime? confirmDate,
            ref List<SalesOrderCompareDto> compareItemList,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            Dictionary<string, SalesOrderCompareDto> dicCompare = new Dictionary<string, SalesOrderCompareDto>();
            try
            {
                if (newSalesOrders != null)
                {
                    DateTime dtConfirm;
                    if (confirmDate == null)
                    {
                        dtConfirm = DateTime.ParseExact("01/01/1900", "M/d/yyyy",
                            CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        dtConfirm = DateTime.ParseExact(confirmDate?.ToString("MM/dd/yyyy"), "M/d/yyyy",
                            CultureInfo.InvariantCulture);
                    }

                    //var dicPONumber = new Dictionary<string, int>();
                    var dicQtyPONum_NewItem = new Dictionary<string, decimal>(); // key = PONum 
                    var dicPONum_NewItem = new Dictionary<string, ItemStyle>(); // key = PONum 

                    var dicPONum_OldItem = new Dictionary<string, ItemStyle>();// key = PONum 

                    List<ItemStyle> oldItemStyleList = new List<ItemStyle>();
                    int countSalesOrder = newSalesOrders.Count;
                    List<ItemStyle> newItemStyle = new List<ItemStyle>();

                    foreach (var newSalesOrder in newSalesOrders)
                    {
                        if (countSalesOrder >= 2)
                        {
                            var itemOldStyle = itemStyleQuery.Where(x => x.SalesOrderID.Equals(newSalesOrder.ID));
                            foreach (var item in itemOldStyle)
                            {
                                oldItemStyleList.Add(item);
                            }
                        }
                        else
                        {
                            string orderPO = newSalesOrder.ID;

                            var itemStyle = itemStyleQuery.Where(x => x.SalesOrderID.Equals(newSalesOrder.ID));
                            foreach (var item in itemStyle)
                            {
                                oldItemStyleList.Add(item);
                            }
                            string year = orderPO.Substring(6, 4);
                            string oldOrder = "SO.HADDAD." + (int.Parse(year) - 1);

                            var itemOldStyle = itemStyleQuery.Where(x => x.SalesOrderID.Equals(oldOrder));
                            foreach (var item in itemOldStyle)
                            {
                                oldItemStyleList.Add(item);
                            }
                        }


                        foreach (var itemPO in newSalesOrder.ItemStyles)
                        {
                            decimal newQtyPONum = 0;

                            foreach (var item in itemPO.OrderDetails)
                            {
                                newQtyPONum += decimal.Parse(Math.Round((decimal)item.Quantity, 0).ToString());
                            }
                            newItemStyle.Add(itemPO);

                            if (dicQtyPONum_NewItem.TryGetValue(itemPO.PurchaseOrderNumber + itemPO.PurchaseOrderNumberIndex, out decimal rstPONum))
                            {
                                newQtyPONum = rstPONum + newQtyPONum;
                            }
                            dicQtyPONum_NewItem[itemPO.PurchaseOrderNumber + itemPO.PurchaseOrderNumberIndex] = newQtyPONum;
                        }
                    }
                    dicPONum_NewItem = newItemStyle.Distinct().ToDictionary(x => x.PurchaseOrderNumber + x.PurchaseOrderNumberIndex);

                    var config = new MapperConfiguration(
                            cfg => cfg.CreateMap<ItemStyle, ItemStyle>()
                            //.ForMember(d => d.Branch, o => o.Ignore())
                            );
                    var mapper = new Mapper(config);

                    dicPONum_OldItem = oldItemStyleList.Where(x => x.ItemStyleStatusCode != SalesOrderCompareDto.StatusCompare.Cancel.ToString())
                        .Distinct()
                        .ToDictionary(x => x.PurchaseOrderNumber + x.PurchaseOrderNumberIndex);

                    // UPDATE AND CANCEL
                    foreach (var oldItem in dicPONum_OldItem)
                    {
                        if (dicPONum_NewItem.TryGetValue(oldItem.Key, out ItemStyle rsListNewItem))
                        {
                            DateTime dtOldItem = DateTime.ParseExact(
                                oldItem.Value.ShipDate?.ToString("MM/dd/yyyy"), "M/d/yyyy",
                                CultureInfo.InvariantCulture);
                            DateTime dtNewItem = DateTime.ParseExact(
                                rsListNewItem.ShipDate?.ToString("MM/dd/yyyy"), "M/d/yyyy",
                                CultureInfo.InvariantCulture);

                            decimal oldQty = 0;
                            decimal newQty = 0;

                            foreach (var item in rsListNewItem.OrderDetails)
                            {
                                newQty += decimal.Parse(Math.Round((decimal)item.Quantity, 0).ToString());
                            }

                            foreach (var item in oldItem.Value.OrderDetails)
                            {
                                oldQty += decimal.Parse(Math.Round((decimal)item.Quantity, 0).ToString());
                            }

                            SalesOrderCompareDto compareItem = new SalesOrderCompareDto();
                            SetCompareObject(SalesOrderCompareDto.StatusCompare.Update, rsListNewItem, oldItem.Value, dtConfirm, ref compareItem, out errorMessage);

                            if (DateTime.Compare(dtConfirm, dtOldItem) >= 0 ||
                                DateTime.Compare(dtConfirm, dtNewItem) >= 0 ||
                                oldItem.Value.IsConfirmed == true)
                            {
                                if (oldItem.Value.IsConfirmed != true)
                                {
                                    if (newQty != oldQty)
                                    {
                                        compareItem.OldPlannedQty = oldQty;
                                        compareItem.NewPlannedQty = newQty;
                                        compareItem.StyleNumber = oldItem.Value.Number;
                                        compareItem.Status = SalesOrderCompareDto.StatusCompare.Update;
                                    }
                                }
                            }
                            else
                            {
                                if (newQty != oldQty && oldItem.Value.IsConfirmed != true)
                                {
                                    compareItem.OldPlannedQty = oldQty;
                                    compareItem.NewPlannedQty = newQty;
                                    compareItem.StyleNumber = oldItem.Value.Number;
                                    compareItem.Status = SalesOrderCompareDto.StatusCompare.Update;
                                }
                            }

                            if (compareItem != null && !String.IsNullOrEmpty(compareItem.PurchaseOrderNumber + compareItem.PurchaseOrderNumberIndex))
                            {
                                if (compareItem.Status != SalesOrderCompareDto.StatusCompare.Normal)
                                {
                                    compareItemList.Add(compareItem);
                                }
                            }

                        }
                        else // cancel
                        {
                            //Console.WriteLine("==================> cancel:  " + oldItem.Key);
                            if (oldItem.Value.IsConfirmed != true)
                            {
                                SalesOrderCompareDto compareItem = new SalesOrderCompareDto();
                                SetCompareObject(SalesOrderCompareDto.StatusCompare.Cancel, null, oldItem.Value, dtConfirm, ref compareItem, out errorMessage);
                                compareItem.Status = SalesOrderCompareDto.StatusCompare.Cancel;
                                compareItem.Confirm = false;

                                decimal oldQty = 0;
                                foreach (var it in oldItem.Value.OrderDetails)
                                {
                                    oldQty += (decimal)it.Quantity;
                                }
                                compareItem.OldPlannedQty = oldQty;

                                if (compareItem != null && !String.IsNullOrEmpty(compareItem.PurchaseOrderNumber + compareItem.PurchaseOrderNumberIndex))
                                {
                                    if (compareItem.Status != SalesOrderCompareDto.StatusCompare.Normal)
                                    {
                                        compareItemList.Add(compareItem);
                                    }
                                }
                            }
                        }
                    }

                    // NEW
                    foreach (var item in dicPONum_NewItem)
                    {
                        //Console.WriteLine("==================> new: " + item.Key);
                        //if (item.Key.Equals("0995511"))
                        //{
                        //    string str = "";
                        //}
                        if (!dicPONum_OldItem.TryGetValue(item.Key, out ItemStyle rsListNewItem))
                        {


                            decimal newQty = 0;
                            DateTime dtItem = DateTime.ParseExact(
                                item.Value.ShipDate?.ToString("MM/dd/yyyy"), "M/d/yyyy", CultureInfo.InvariantCulture);

                            foreach (var itemSize in item.Value.OrderDetails)
                            {
                                newQty += decimal.Parse(Math.Round((decimal)itemSize.Quantity, 0).ToString());
                            }

                            SalesOrderCompareDto compareItem = new SalesOrderCompareDto();
                            SetCompareObject(SalesOrderCompareDto.StatusCompare.New, item.Value, null, dtConfirm, ref compareItem, out errorMessage);

                            compareItem.Status = SalesOrderCompareDto.StatusCompare.New;
                            compareItem.NewPlannedQty = newQty;

                            if (DateTime.Compare(dtConfirm, dtItem) >= 0)
                            {
                                compareItem.Confirm = true;
                            }

                            if (compareItem != null && !String.IsNullOrEmpty(compareItem.PurchaseOrderNumber + compareItem.PurchaseOrderNumberIndex))
                            {
                                if (compareItem.Status != SalesOrderCompareDto.StatusCompare.Normal)
                                {
                                    compareItemList.Add(compareItem);
                                }
                            }

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public static void OldCreateCompareItemList(List<SalesOrder> newSalesOrders,
            List<ItemStyle> itemStyleQuery, DateTime? confirmDate,
            ref List<SalesOrderCompareDto> compareItemList,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            Dictionary<string, SalesOrderCompareDto> dicCompare = new Dictionary<string, SalesOrderCompareDto>();
            try
            {
                if (newSalesOrders != null)
                {
                    DateTime dtConfirm;
                    if (confirmDate == null)
                    {
                        dtConfirm = DateTime.ParseExact("01/01/1900", "M/d/yyyy",
                            CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        dtConfirm = DateTime.ParseExact(confirmDate?.ToString("MM/dd/yyyy"), "M/d/yyyy",
                            CultureInfo.InvariantCulture);
                    }

                    //var dicPONumber = new Dictionary<string, int>();
                    var dicQtyPONum_NewItem = new Dictionary<string, decimal>(); // key = PONum 
                    var dicGroupPONum_NewItem = new Dictionary<string, List<ItemStyle>>(); // key = PONum 

                    var dicGroupPONum_OldItem = new Dictionary<string, List<ItemStyle>>();// key = PONum 

                    List<ItemStyle> oldItemStyleList = new List<ItemStyle>();
                    int countSalesOrder = newSalesOrders.Count;
                    foreach (var newSalesOrder in newSalesOrders)
                    {
                        if (countSalesOrder >= 2)
                        {
                            var itemOldStyle = itemStyleQuery.Where(x => x.SalesOrderID.Equals(newSalesOrder.ID));
                            foreach (var item in itemOldStyle)
                            {
                                oldItemStyleList.Add(item);
                            }
                        }
                        else
                        {
                            string orderPO = newSalesOrder.ID;

                            var itemStyle = itemStyleQuery.Where(x => x.SalesOrderID.Equals(newSalesOrder.ID));
                            foreach (var item in itemStyle)
                            {
                                oldItemStyleList.Add(item);
                            }
                            string year = orderPO.Substring(6, 4);
                            string oldOrder = "SO.HA." + (int.Parse(year) - 1);

                            var itemOldStyle = itemStyleQuery.Where(x => x.SalesOrderID.Equals(oldOrder));
                            foreach (var item in itemOldStyle)
                            {
                                oldItemStyleList.Add(item);
                            }
                        }


                        foreach (var itemPO in newSalesOrder.ItemStyles)
                        {
                            decimal newQtyPONum = 0;

                            foreach (var item in itemPO.OrderDetails)
                            {
                                newQtyPONum += decimal.Parse(Math.Round((decimal)item.Quantity, 0).ToString());
                            }

                            // only PONum
                            List<ItemStyle> groupPO = new List<ItemStyle>();
                            if (dicGroupPONum_NewItem.TryGetValue(itemPO.PurchaseOrderNumber, out List<ItemStyle> rsGroup))
                            {
                                groupPO = rsGroup;
                            }
                            groupPO.Add(itemPO);
                            dicGroupPONum_NewItem[itemPO.PurchaseOrderNumber] = groupPO;


                            if (dicQtyPONum_NewItem.TryGetValue(itemPO.PurchaseOrderNumber, out decimal rstPONum))
                            {
                                newQtyPONum = rstPONum + newQtyPONum;
                            }
                            dicQtyPONum_NewItem[itemPO.PurchaseOrderNumber] = newQtyPONum;
                        }
                    }

                    var config = new MapperConfiguration(
                            cfg => cfg.CreateMap<ItemStyle, ItemStyle>()
                            //.ForMember(d => d.Branch, o => o.Ignore())
                            );
                    var mapper = new Mapper(config);

                    foreach (var itemPO in oldItemStyleList)
                    {
                        List<ItemStyle> groupPO = new List<ItemStyle>();
                        if (dicGroupPONum_OldItem.TryGetValue(itemPO.PurchaseOrderNumber, out List<ItemStyle> rsGroup))
                        {
                            groupPO = rsGroup;
                        }
                        groupPO.Add(itemPO);
                        dicGroupPONum_OldItem[itemPO.PurchaseOrderNumber] = groupPO;
                    }

                    // UPDATE AND CANCEL
                    foreach (var oldItem in dicGroupPONum_OldItem)
                    {
                        //Console.WriteLine("==================> update:  " + oldItem.Key);

                        //if (oldItem.Key.Equals("0995511"))
                        //{
                        //    string str = "";
                        //}

                        if (dicGroupPONum_NewItem.TryGetValue(oldItem.Key, out List<ItemStyle> rsListNewItem))
                        {
                            if (oldItem.Value.Count == 1 && rsListNewItem.Count == 1)
                            {
                                DateTime dtOldItem = DateTime.ParseExact(
                                    oldItem.Value[0].ShipDate?.ToString("MM/dd/yyyy"), "M/d/yyyy",
                                    CultureInfo.InvariantCulture);
                                DateTime dtNewItem = DateTime.ParseExact(
                                    rsListNewItem[0].ShipDate?.ToString("MM/dd/yyyy"), "M/d/yyyy",
                                    CultureInfo.InvariantCulture);

                                decimal oldQty = 0;
                                decimal newQty = 0;

                                foreach (var item in rsListNewItem[0].OrderDetails)
                                {
                                    newQty += decimal.Parse(Math.Round((decimal)item.Quantity, 0).ToString());
                                }

                                foreach (var item in oldItem.Value[0].OrderDetails)
                                {
                                    oldQty += decimal.Parse(Math.Round((decimal)item.Quantity, 0).ToString());
                                }

                                SalesOrderCompareDto compareItem = new SalesOrderCompareDto();
                                SetCompareObject(SalesOrderCompareDto.StatusCompare.Update, rsListNewItem[0], oldItem.Value[0], dtConfirm, ref compareItem, out errorMessage);
                                //compareItem.Status = SalesOrderCompareDto.StatusCompare.Update;


                                if (DateTime.Compare(dtConfirm, dtOldItem) >= 0 ||
                                    DateTime.Compare(dtConfirm, dtNewItem) >= 0 ||
                                    oldItem.Value[0].IsConfirmed == true)
                                {
                                    if (oldItem.Value[0].IsConfirmed != true)
                                    {
                                        if (newQty != oldQty)
                                        {
                                            compareItem.OldPlannedQty = oldQty;
                                            compareItem.NewPlannedQty = newQty;
                                            compareItem.StyleNumber = oldItem.Value[0].Number;
                                            compareItem.Status = SalesOrderCompareDto.StatusCompare.Update;
                                        }
                                    }
                                }
                                else
                                {
                                    if (newQty != oldQty && oldItem.Value[0].IsConfirmed != true)
                                    {
                                        compareItem.OldPlannedQty = oldQty;
                                        compareItem.NewPlannedQty = newQty;
                                        compareItem.StyleNumber = oldItem.Value[0].Number;
                                        compareItem.Status = SalesOrderCompareDto.StatusCompare.Update;
                                    }
                                }

                                if (compareItem != null && !String.IsNullOrEmpty(compareItem.PurchaseOrderNumber))
                                {
                                    if (compareItem.Status != SalesOrderCompareDto.StatusCompare.Normal)
                                    {
                                        compareItemList.Add(compareItem);
                                    }
                                }
                            }
                            else if (oldItem.Value.Count == 1 && rsListNewItem.Count > 1)
                            {
                                DateTime dtOldItem = DateTime.ParseExact(
                                    oldItem.Value[0].ShipDate?.ToString("MM/dd/yyyy"), "M/d/yyyy",
                                    CultureInfo.InvariantCulture);
                                DateTime dtETD_OldItem = DateTime.ParseExact(
                                    oldItem.Value[0].DeliveryDate?.ToString("MM/dd/yyyy"), "M/d/yyyy",
                                    CultureInfo.InvariantCulture);

                                int count = rsListNewItem.Count;

                                for (int i = 0; i < count; i++)
                                {
                                    DateTime dtNewItem = DateTime.ParseExact(
                                        rsListNewItem[i].ShipDate?.ToString("MM/dd/yyyy"), "M/d/yyyy",
                                        CultureInfo.InvariantCulture);
                                    DateTime dtETD_NewItem = DateTime.ParseExact(
                                        rsListNewItem[i].DeliveryDate?.ToString("MM/dd/yyyy"), "M/d/yyyy",
                                        CultureInfo.InvariantCulture);

                                    SalesOrderCompareDto compareItem = new SalesOrderCompareDto();
                                    SetCompareObject(SalesOrderCompareDto.StatusCompare.Update, rsListNewItem[i], oldItem.Value[0], dtConfirm, ref compareItem, out errorMessage);

                                    if (dtOldItem.CompareTo(dtNewItem) == 0 && dtETD_OldItem.CompareTo(dtETD_NewItem) == 0)
                                    {

                                        if (oldItem.Value[0].IsConfirmed != true)
                                        {

                                            decimal oldQty = 0;
                                            decimal newQty = 0;

                                            foreach (var item in rsListNewItem[i].OrderDetails)
                                            {
                                                newQty += decimal.Parse(Math.Round((decimal)item.Quantity, 0).ToString());
                                            }

                                            foreach (var item in oldItem.Value[0].OrderDetails)
                                            {
                                                oldQty += decimal.Parse(Math.Round((decimal)item.Quantity, 0).ToString());
                                            }

                                            if (newQty != oldQty)
                                            {
                                                compareItem.OldPlannedQty = oldQty;
                                                compareItem.NewPlannedQty = newQty;
                                                compareItem.StyleNumber = oldItem.Value[0].Number;
                                                compareItem.Status = SalesOrderCompareDto.StatusCompare.Update;

                                            }
                                        }

                                        if (compareItem != null && !String.IsNullOrEmpty(compareItem.PurchaseOrderNumber))
                                        {
                                            if (compareItem.Status != SalesOrderCompareDto.StatusCompare.Normal)
                                            {
                                                compareItemList.Add(compareItem);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        decimal oldQty = 0;
                                        decimal newQty = 0;

                                        if (oldItem.Value[0].IsConfirmed != true)
                                        {
                                            foreach (var item in rsListNewItem[i].OrderDetails)
                                            {
                                                newQty += decimal.Parse(Math.Round((decimal)item.Quantity, 0).ToString());
                                            }

                                            foreach (var item in oldItem.Value[0].OrderDetails)
                                            {
                                                oldQty += decimal.Parse(Math.Round((decimal)item.Quantity, 0).ToString());
                                            }

                                            if (newQty != oldQty)
                                            {
                                                compareItem.OldPlannedQty = oldQty;
                                                compareItem.NewPlannedQty = newQty;
                                                compareItem.StyleNumber = oldItem.Value[0].Number;
                                                compareItem.Status = SalesOrderCompareDto.StatusCompare.Update;
                                            }
                                        }

                                        if (compareItem.Status == SalesOrderCompareDto.StatusCompare.Update && oldQty == newQty && oldQty != 0)
                                        {
                                            if (DateTime.Compare(dtConfirm, dtOldItem) >= 0)
                                            {
                                                compareItem.Confirm = true;
                                            }

                                            if (compareItem != null && !String.IsNullOrEmpty(compareItem.PurchaseOrderNumber))
                                            {
                                                compareItemList.Add(compareItem);
                                            }
                                        }
                                        else
                                        {

                                            SalesOrderCompareDto compareItemNew = new SalesOrderCompareDto();
                                            SetCompareObject(SalesOrderCompareDto.StatusCompare.New, rsListNewItem[i], null, dtConfirm, ref compareItemNew, out errorMessage);

                                            compareItemNew.Status = SalesOrderCompareDto.StatusCompare.New;
                                            compareItemNew.NewPlannedQty = newQty;

                                            if (DateTime.Compare(dtConfirm, dtNewItem) >= 0)
                                            {
                                                compareItemNew.Confirm = true;
                                            }

                                            if (compareItemNew != null && !String.IsNullOrEmpty(compareItemNew.PurchaseOrderNumber))
                                            {
                                                if (compareItemNew.Status != SalesOrderCompareDto.StatusCompare.Normal)
                                                {
                                                    compareItemList.Add(compareItemNew);
                                                }
                                            }
                                        }
                                    }


                                }
                            }
                            else if (oldItem.Value.Count > 1 && rsListNewItem.Count == 1)
                            {
                                int count = oldItem.Value.Count;
                                bool check = false;
                                DateTime dtNewItem = DateTime.ParseExact(
                                    rsListNewItem[0].ShipDate?.ToString("MM/dd/yyyy"), "M/d/yyyy", CultureInfo.InvariantCulture);
                                DateTime dtETD_NewItem = DateTime.ParseExact(
                                    rsListNewItem[0].DeliveryDate?.ToString("MM/dd/yyyy"), "M/d/yyyy", CultureInfo.InvariantCulture);

                                // update
                                for (int i = 0; i < count; i++)
                                {
                                    DateTime dtOldItem = DateTime.ParseExact(
                                        oldItem.Value[i].ShipDate?.ToString("MM/dd/yyyy"), "M/d/yyyy", CultureInfo.InvariantCulture);
                                    DateTime dtETD_OldItem = DateTime.ParseExact(
                                        oldItem.Value[i].DeliveryDate?.ToString("MM/dd/yyyy"), "M/d/yyyy", CultureInfo.InvariantCulture);

                                    SalesOrderCompareDto compareItem = new SalesOrderCompareDto();
                                    SetCompareObject(SalesOrderCompareDto.StatusCompare.Update, rsListNewItem[0], oldItem.Value[i], dtConfirm, ref compareItem, out errorMessage);

                                    decimal oldQty = 0;
                                    decimal newQty = 0;

                                    foreach (var item in rsListNewItem[0].OrderDetails)
                                    {
                                        newQty += decimal.Parse(Math.Round((decimal)item.Quantity, 0).ToString());
                                    }

                                    foreach (var item in oldItem.Value[i].OrderDetails)
                                    {
                                        oldQty += decimal.Parse(Math.Round((decimal)item.Quantity, 0).ToString());
                                    }

                                    if (dtOldItem.CompareTo(dtNewItem) == 0 && dtETD_OldItem.CompareTo(dtETD_NewItem) == 0)
                                    {
                                        check = true;

                                        if (oldItem.Value[i].IsConfirmed != true)
                                        {
                                            if (newQty != oldQty)
                                            {
                                                compareItem.OldPlannedQty = oldQty;
                                                compareItem.NewPlannedQty = newQty;
                                                compareItem.StyleNumber = oldItem.Value[i].Number;
                                                compareItem.Status = SalesOrderCompareDto.StatusCompare.Update;

                                            }
                                        }

                                        if (compareItem != null && !String.IsNullOrEmpty(compareItem.PurchaseOrderNumber))
                                        {
                                            if (compareItem.Status != SalesOrderCompareDto.StatusCompare.Normal)
                                            {
                                                compareItemList.Add(compareItem);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (oldItem.Value[i].IsConfirmed != true)
                                        {
                                            if (newQty != oldQty)
                                            {
                                                compareItem.OldPlannedQty = oldQty;
                                                compareItem.NewPlannedQty = newQty;
                                                compareItem.StyleNumber = oldItem.Value[i].Number;
                                                compareItem.Status = SalesOrderCompareDto.StatusCompare.Update;

                                            }
                                        }

                                        if (compareItem.Status == SalesOrderCompareDto.StatusCompare.Update
                                            && oldQty == newQty && oldQty != 0)
                                        {
                                            check = true;
                                            if (DateTime.Compare(dtConfirm, dtOldItem) >= 0)
                                            {
                                                compareItem.Confirm = true;
                                            }

                                            if (compareItem != null && !String.IsNullOrEmpty(compareItem.PurchaseOrderNumber))
                                            {
                                                compareItemList.Add(compareItem);

                                            }
                                        }
                                    }
                                }

                                // new
                                if (!check)
                                {
                                    decimal newQty = 0;
                                    DateTime dtItem = DateTime.ParseExact(
                                                        rsListNewItem[0].ShipDate?.ToString("MM/dd/yyyy"), "M/d/yyyy",
                                                        CultureInfo.InvariantCulture);

                                    foreach (var item in rsListNewItem[0].OrderDetails)
                                    {
                                        newQty += decimal.Parse(Math.Round((decimal)item.Quantity, 0).ToString());
                                    }

                                    SalesOrderCompareDto compareItem = new SalesOrderCompareDto();
                                    SetCompareObject(SalesOrderCompareDto.StatusCompare.New, rsListNewItem[0], null, dtConfirm, ref compareItem, out errorMessage);

                                    compareItem.Status = SalesOrderCompareDto.StatusCompare.New;
                                    compareItem.NewPlannedQty = newQty;

                                    if (DateTime.Compare(dtConfirm, dtItem) >= 0)
                                    {
                                        compareItem.Confirm = true;
                                    }

                                    if (compareItem != null && !String.IsNullOrEmpty(compareItem.PurchaseOrderNumber))
                                    {
                                        if (compareItem.Status != SalesOrderCompareDto.StatusCompare.Normal)
                                        {
                                            compareItemList.Add(compareItem);
                                        }
                                    }
                                }

                            }
                            else
                            {
                                var oldItemListShort = oldItem.Value.OrderByDescending(x => x.ShipDate)
                                                                    .ThenByDescending(x => x.DeliveryDate).ToList();
                                rsListNewItem = rsListNewItem.OrderByDescending(x => x.ShipDate)
                                                             .ThenByDescending(x => x.DeliveryDate).ToList();
                                int count = oldItem.Value.Count;
                                int countNew = rsListNewItem.Count;

                                Queue<int> qOldItemDuplicated = new Queue<int>();
                                Queue<ItemStyle> qNewItem = new Queue<ItemStyle>();

                                //foreach (var item in oldItem.Value)
                                //{
                                //    qOldItem.Enqueue(item);
                                //}

                                //foreach (var item in rsListNewItem)
                                //{
                                //    qNewItem.Enqueue(item);
                                //}

                                for (int i = 0; i < countNew; i++)
                                {
                                    DateTime dtNewItem = DateTime.ParseExact(
                                        rsListNewItem[i].ShipDate?.ToString("MM/dd/yyyy"), "M/d/yyyy", CultureInfo.InvariantCulture);
                                    DateTime dtETD_NewItem = DateTime.ParseExact(
                                        rsListNewItem[i].DeliveryDate?.ToString("MM/dd/yyyy"), "M/d/yyyy", CultureInfo.InvariantCulture);

                                    decimal newQty = 0;
                                    bool checkDup = false;

                                    foreach (var item in rsListNewItem[i].OrderDetails)
                                    {
                                        newQty += decimal.Parse(Math.Round((decimal)item.Quantity, 0).ToString());
                                    }

                                    for (int j = 0; j < count; j++)
                                    {
                                        bool checkedItem = false;

                                        foreach (int item in qOldItemDuplicated)
                                        {
                                            if (item == j)
                                            {
                                                checkedItem = true;
                                                break;
                                            }
                                        }

                                        if (checkedItem)
                                            continue;

                                        var oldItemPop = oldItemListShort[j];


                                        DateTime dtOldItem = DateTime.ParseExact(
                                            oldItemPop.ShipDate?.ToString("MM/dd/yyyy"), "M/d/yyyy", CultureInfo.InvariantCulture);
                                        DateTime dtETD_OldItem = DateTime.ParseExact(
                                            oldItemPop.DeliveryDate?.ToString("MM/dd/yyyy"), "M/d/yyyy", CultureInfo.InvariantCulture);

                                        SalesOrderCompareDto compareItem = new SalesOrderCompareDto();
                                        SetCompareObject(SalesOrderCompareDto.StatusCompare.Update, rsListNewItem[i],
                                                           oldItemPop, dtConfirm, ref compareItem, out errorMessage);

                                        decimal oldQty = 0;

                                        foreach (var item in oldItemPop.OrderDetails)
                                        {
                                            oldQty += decimal.Parse(Math.Round((decimal)item.Quantity, 0).ToString());
                                        }

                                        if (dtOldItem.CompareTo(dtNewItem) == 0 && dtETD_OldItem.CompareTo(dtETD_NewItem) == 0)
                                        {
                                            if (oldItemPop.IsConfirmed != true)
                                            {
                                                if (newQty != oldQty)
                                                {
                                                    compareItem.OldPlannedQty = oldQty;
                                                    compareItem.NewPlannedQty = newQty;
                                                    compareItem.StyleNumber = oldItemPop.Number;
                                                    compareItem.Status = SalesOrderCompareDto.StatusCompare.Update;

                                                }
                                            }

                                            if (compareItem != null && !String.IsNullOrEmpty(compareItem.PurchaseOrderNumber))
                                            {
                                                if (compareItem.Status == SalesOrderCompareDto.StatusCompare.Update)
                                                {
                                                    checkDup = true;
                                                    compareItemList.Add(compareItem);

                                                    qOldItemDuplicated.Enqueue(j);
                                                    //if (qOldItem.Count > 0)
                                                    //    qOldItem.Dequeue();

                                                    break;
                                                }
                                                else
                                                {
                                                    if (compareItem.Status == SalesOrderCompareDto.StatusCompare.Normal)
                                                    {
                                                        checkDup = true;
                                                        qOldItemDuplicated.Enqueue(j);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else if (compareItem.Status == SalesOrderCompareDto.StatusCompare.Update && oldQty == newQty && oldQty != 0)
                                        {

                                            if (compareItem != null && !String.IsNullOrEmpty(compareItem.PurchaseOrderNumber))
                                            {

                                                checkDup = true;
                                                compareItemList.Add(compareItem);

                                                qOldItemDuplicated.Enqueue(j);
                                                //if (qOldItem.Count > 0)
                                                //    qOldItem.Dequeue();

                                                break;

                                            }
                                        }
                                        else
                                        {
                                            if (compareItem.Status == SalesOrderCompareDto.StatusCompare.Normal)
                                            {
                                                checkDup = true;
                                                qOldItemDuplicated.Enqueue(j);
                                                break;
                                            }
                                        }
                                    }

                                    //if (checkDup)
                                    //{
                                    //    if (qNewItem.Count > 0)
                                    //    {
                                    //        qNewItem.Dequeue();
                                    //    }    

                                    //}
                                    if (!checkDup)
                                    {
                                        qNewItem.Enqueue(rsListNewItem[i]);
                                    }
                                }

                                // new
                                while (qNewItem.Count > 0)
                                {
                                    var itemNew = qNewItem.Dequeue();
                                    decimal newQty = 0;
                                    DateTime dtItem = DateTime.ParseExact(
                                        itemNew.ShipDate?.ToString("MM/dd/yyyy"), "M/d/yyyy", CultureInfo.InvariantCulture);

                                    foreach (var item in itemNew.OrderDetails)
                                    {
                                        newQty += decimal.Parse(Math.Round((decimal)item.Quantity, 0).ToString());
                                    }

                                    SalesOrderCompareDto compareItem = new SalesOrderCompareDto();
                                    SetCompareObject(SalesOrderCompareDto.StatusCompare.New, itemNew, null, dtConfirm, ref compareItem, out errorMessage);

                                    compareItem.Status = SalesOrderCompareDto.StatusCompare.New;
                                    compareItem.NewPlannedQty = newQty;

                                    if (DateTime.Compare(dtConfirm, dtItem) >= 0)
                                    {
                                        compareItem.Confirm = true;
                                    }

                                    if (compareItem != null && !String.IsNullOrEmpty(compareItem.PurchaseOrderNumber))
                                    {
                                        if (compareItem.Status != SalesOrderCompareDto.StatusCompare.Normal)
                                        {
                                            compareItemList.Add(compareItem);
                                        }
                                    }
                                }
                            }
                        }
                        else // cancel
                        {
                            //Console.WriteLine("==================> cancel:  " + oldItem.Key);
                            foreach (var itemCancel in oldItem.Value)
                            {
                                if (itemCancel.IsConfirmed != true)
                                {

                                    SalesOrderCompareDto compareItem = new SalesOrderCompareDto();
                                    SetCompareObject(SalesOrderCompareDto.StatusCompare.Cancel, null, itemCancel, dtConfirm, ref compareItem, out errorMessage);
                                    compareItem.Status = SalesOrderCompareDto.StatusCompare.Cancel;
                                    compareItem.Confirm = false;

                                    decimal oldQty = 0;
                                    foreach (var it in itemCancel.OrderDetails)
                                    {
                                        oldQty += (decimal)it.Quantity;
                                    }
                                    compareItem.OldPlannedQty = oldQty;

                                    if (compareItem != null && !String.IsNullOrEmpty(compareItem.PurchaseOrderNumber))
                                    {
                                        if (compareItem.Status != SalesOrderCompareDto.StatusCompare.Normal)
                                        {
                                            compareItemList.Add(compareItem);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // NEW
                    foreach (var item in dicGroupPONum_NewItem)
                    {
                        //Console.WriteLine("==================> new: " + item.Key);
                        //if (item.Key.Equals("0995511"))
                        //{
                        //    string str = "";
                        //}
                        if (!dicGroupPONum_OldItem.TryGetValue(item.Key, out List<ItemStyle> rsListNewItem))
                        {

                            foreach (var itemNew in item.Value)
                            {
                                decimal newQty = 0;
                                DateTime dtItem = DateTime.ParseExact(
                                    itemNew.ShipDate?.ToString("MM/dd/yyyy"), "M/d/yyyy", CultureInfo.InvariantCulture);

                                foreach (var itemSize in itemNew.OrderDetails)
                                {
                                    newQty += decimal.Parse(Math.Round((decimal)itemSize.Quantity, 0).ToString());
                                }

                                SalesOrderCompareDto compareItem = new SalesOrderCompareDto();
                                SetCompareObject(SalesOrderCompareDto.StatusCompare.New, itemNew, null, dtConfirm, ref compareItem, out errorMessage);

                                compareItem.Status = SalesOrderCompareDto.StatusCompare.New;
                                compareItem.NewPlannedQty = newQty;

                                if (DateTime.Compare(dtConfirm, dtItem) >= 0)
                                {
                                    compareItem.Confirm = true;
                                }

                                if (compareItem != null && !String.IsNullOrEmpty(compareItem.PurchaseOrderNumber))
                                {
                                    if (compareItem.Status != SalesOrderCompareDto.StatusCompare.Normal)
                                    {
                                        compareItemList.Add(compareItem);
                                    }
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public static void CompareItemStyle(
            string userName,
            string customerID,
            string filePath,
            string fileName,
            List<ItemStyle> oldItemStyles,
            List<SalesOrderCompareDto> compareItemStyles,
            List<string> strNewSalesOrderID,
            ref List<Part> parts, IEnumerable<Size> sizes,
            out List<Part> newParts,
            out List<SalesOrder> newSalesOrders,
            out List<ItemStyle> newItemStyle,
            out List<ItemStyle> updateCancelItemStyle,
            out List<JobHead> updateJobHeads,
            out List<OrderDetail> updateOrderDetails,
            out string errorMessage)
        {
            newParts = new List<Part>();
            newItemStyle = new List<ItemStyle>();
            updateCancelItemStyle = new List<ItemStyle>();
            newSalesOrders = new List<SalesOrder>();
            updateJobHeads = new List<JobHead>();
            updateOrderDetails = new List<OrderDetail>();
            errorMessage = string.Empty;

            var dicSizes = sizes.ToDictionary(x => x.Code);
            var dicNewSalesOrders = new Dictionary<string, SalesOrder>();
            //var dicJobHeads = jobHeads.GroupBy(gb => gb.LSStyle + gb.GarmentSize.Replace(" ", "").ToUpper(), gb => gb)
            //                          .ToDictionary(x => x.Key, x => x.ToList());

            try
            {
                foreach (var salesOrderID in strNewSalesOrderID)
                {
                    SalesOrder salesOrder = new SalesOrder();
                    salesOrder.ID = salesOrderID;
                    salesOrder.SetCreateAudit(userName);
                    salesOrder.CustomerID = customerID;
                    salesOrder.OrderDate = DateTime.Now;
                    salesOrder.ConfirmDate = DateTime.Now;
                    salesOrder.DivisionID = "BU1";
                    salesOrder.CurrencyID = "USD";
                    salesOrder.PaymentTermCode = "8";
                    salesOrder.SalesOrderStatusCode = "Order";
                    salesOrder.PriceTermCode = "FOB";
                    salesOrder.PriceTermDescription = "FOB";
                    salesOrder.FileName = fileName;
                    salesOrder.SaveFilePath = filePath;

                    salesOrder.ItemStyles = new List<ItemStyle>();
                    dicNewSalesOrders.Add(salesOrderID, salesOrder);
                }

                var config = new MapperConfiguration(
                           cfg => cfg.CreateMap<SalesOrderCompareDto, ItemStyle>()
                           .ForMember(d => d.OrderDetails, o => o.Ignore())
                           );
                var mapper = new Mapper(config);

                var configOrderDetail = new MapperConfiguration(
                           cfg => cfg.CreateMap<OrderDetailsDto, OrderDetail>()
                           );
                var mapperOrderDetail = new Mapper(configOrderDetail);

                var dicOldItemStyles = oldItemStyles.ToDictionary(x => x.Number);

                foreach (var compareItemStyle in compareItemStyles)
                {
                    if (dicOldItemStyles.TryGetValue(compareItemStyle.StyleNumber, out ItemStyle oldItemStyle))
                    {
                        switch (compareItemStyle.Status)
                        {
                            case SalesOrderCompareDto.StatusCompare.Update:
                                {
                                    mapper.Map(compareItemStyle, oldItemStyle);
                                    oldItemStyle.ItemStyleStatusCode = ((int)SalesOrderCompareDto.StatusCompare.Update).ToString();

                                    oldItemStyle.DeliveryDate = compareItemStyle.ETD;
                                    oldItemStyle.UE = compareItemStyle.Size;

                                    int countSize = (int)compareItemStyle.OrderDetails?.Count;
                                    int? countNewSize = (int)oldItemStyle.OrderDetails?.Count;
                                    var compareOrderDtl = compareItemStyle.OrderDetails?.OrderByDescending(x => x.Size).ToList();
                                    var oldOrderDtl = oldItemStyle.OrderDetails?.OrderByDescending(x => x.Size).ToList();

                                    if (countNewSize == countSize)
                                    {
                                        decimal oldTotal = 0;

                                        for (int i = 0; i < countSize; i++)
                                        {
                                            long ID = oldOrderDtl[i].ID;
                                            decimal oldQty = (decimal)oldOrderDtl[i].Quantity;
                                            decimal reservationQty = oldOrderDtl[i].ReservedQuantity ?? 0;
                                            oldTotal += oldQty;
                                            mapperOrderDetail.Map(compareOrderDtl[i], oldOrderDtl[i]);
                                            oldOrderDtl[i].ID = ID;
                                            oldOrderDtl[i].OldQuantity = oldQty;
                                            oldOrderDtl[i].Price = compareOrderDtl[i].Price;
                                            oldOrderDtl[i].ReservedQuantity = reservationQty;

                                            if (dicSizes.TryGetValue(oldOrderDtl[i].Size, out Size rsSize))
                                            {
                                                oldOrderDtl[i].SizeSortIndex = rsSize.SequeneceNumber;
                                            }
                                            updateOrderDetails.Add(oldOrderDtl[i]);
                                        }
                                    }
                                    else
                                    {
                                        foreach (var itemCompareOrderDtl in compareOrderDtl)
                                        {
                                            if (oldItemStyle.OrderDetails != null && countNewSize != null && countNewSize > 0)
                                            {
                                                decimal oldTotal = 0;
                                                string notInSize = "-";
                                                foreach (var oldItemOrderDtl in oldOrderDtl)
                                                {
                                                    if (oldItemOrderDtl != null && oldItemOrderDtl.Size
                                                                                    .ToUpper()
                                                                                    .Replace(" ", "")
                                                                                    .Trim()
                                                                                    .Equals(itemCompareOrderDtl.Size.ToUpper()
                                                                                                                    .Replace(" ", "")
                                                                                                                    .Trim()))
                                                    {
                                                        notInSize += oldItemOrderDtl.Size + "-";
                                                        long ID = oldItemOrderDtl.ID;
                                                        decimal oldQty = (decimal)oldItemOrderDtl.Quantity;
                                                        decimal reservationQty = oldItemOrderDtl.ReservedQuantity ?? 0;
                                                        oldTotal += oldQty;
                                                        mapperOrderDetail.Map(itemCompareOrderDtl, oldItemOrderDtl);
                                                        oldItemOrderDtl.ID = ID;
                                                        oldItemOrderDtl.OldQuantity = oldQty;
                                                        oldItemOrderDtl.Price = itemCompareOrderDtl.Price;
                                                        oldItemOrderDtl.ReservedQuantity = reservationQty;

                                                        if (dicSizes.TryGetValue(oldItemOrderDtl.Size, out Size rsSize))
                                                        {
                                                            oldItemOrderDtl.SizeSortIndex = rsSize.SequeneceNumber;
                                                        }
                                                        updateOrderDetails.Add(oldItemOrderDtl);
                                                        break;
                                                    }
                                                    else if (oldItemOrderDtl != null && !oldItemOrderDtl.Size
                                                                                    .ToUpper()
                                                                                    .Replace(" ", "")
                                                                                    .Trim()
                                                                                    .Equals(itemCompareOrderDtl.Size.ToUpper()
                                                                                                                    .Replace(" ", "")
                                                                                                                    .Trim()))
                                                    {
                                                        notInSize += oldItemOrderDtl.Size + "-";
                                                    }
                                                }

                                                if (oldItemStyle.SalesOrder.CustomerID.Equals("HA"))
                                                {
                                                    if (!String.IsNullOrEmpty(oldItemStyle.UE) && !notInSize.Contains(itemCompareOrderDtl.Size)
                                                        && oldItemStyle.UE.Contains(itemCompareOrderDtl.Size) && countSize > countNewSize)
                                                    {
                                                        OrderDetail orderDetail = new OrderDetail();
                                                        mapperOrderDetail.Map(itemCompareOrderDtl, orderDetail);

                                                        if (dicSizes.TryGetValue(orderDetail.Size, out Size rsSize))
                                                        {
                                                            orderDetail.SizeSortIndex = rsSize.SequeneceNumber;
                                                        }
                                                        //updateOrderDetails.Add(orderDetail);
                                                        oldItemStyle.OrderDetails.Add(orderDetail);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                OrderDetail orderDetail = new OrderDetail();
                                                mapperOrderDetail.Map(itemCompareOrderDtl, orderDetail);

                                                if (dicSizes.TryGetValue(orderDetail.Size, out Size rsSize))
                                                {
                                                    orderDetail.SizeSortIndex = rsSize.SequeneceNumber;
                                                }

                                                oldItemStyle.OrderDetails.Add(orderDetail);
                                            }
                                        }
                                    }
                                    oldItemStyle.SetUpdateAudit(userName);
                                    updateCancelItemStyle.Add(oldItemStyle);
                                }
                                break;
                            case SalesOrderCompareDto.StatusCompare.Cancel:
                                {
                                    oldItemStyle.ItemStyleStatusCode = ((int)SalesOrderCompareDto.StatusCompare.Cancel).ToString();
                                    oldItemStyle.IsCalculateRequiredQuantity = false;

                                    foreach (var orderDetail in oldItemStyle.OrderDetails)
                                    {
                                        orderDetail.Quantity = 0;
                                        orderDetail.Price = 0;
                                        orderDetail.OldQuantity = 0;
                                        updateOrderDetails.Add(orderDetail);
                                    }
                                    oldItemStyle.SetUpdateAudit(userName);
                                    updateCancelItemStyle.Add(oldItemStyle);

                                }
                                break;
                            case SalesOrderCompareDto.StatusCompare.New:
                                {
                                    ItemStyle itemStyle = new ItemStyle();
                                    mapper.Map(compareItemStyle, itemStyle);
                                    itemStyle.ItemStyleStatusCode = ((int)SalesOrderCompareDto.StatusCompare.New).ToString();
                                    itemStyle.DeliveryDate = compareItemStyle.ETD;
                                    itemStyle.SetCreateAudit(userName);
                                    itemStyle.TotalQuantity = compareItemStyle.NewPlannedQty;
                                    //itemStyle.ItemStyleStatusCode = compareItemStyle.Status.ToString();

                                    foreach (var itemOrderDetail in compareItemStyle.OrderDetails)
                                    {
                                        OrderDetail orderDetail = new OrderDetail();

                                        mapperOrderDetail.Map(itemOrderDetail, orderDetail);
                                        orderDetail.ItemStyleNumber = itemStyle.Number;

                                        if (dicSizes.TryGetValue(orderDetail.Size, out Size rsSize))
                                        {
                                            orderDetail.SizeSortIndex = rsSize.SequeneceNumber;
                                        }

                                        if (itemStyle.OrderDetails == null)
                                        {
                                            itemStyle.OrderDetails = new List<OrderDetail>();
                                        }
                                        itemStyle.OrderDetails.Add(orderDetail);
                                    }

                                    if (dicNewSalesOrders.TryGetValue(compareItemStyle.SalesOrderID, out SalesOrder rsSalesOrder))
                                    {
                                        if (!String.IsNullOrEmpty(itemStyle.PurchaseOrderNumber + itemStyle.PurchaseOrderNumberIndex))
                                        {
                                            rsSalesOrder.ItemStyles.Add(itemStyle);
                                        }

                                        newSalesOrders.Add(rsSalesOrder);
                                    }
                                    else
                                    {
                                        newItemStyle.Add(itemStyle);
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        ItemStyle itemStyle = new ItemStyle();
                        mapper.Map(compareItemStyle, itemStyle);
                        itemStyle.ItemStyleStatusCode = ((int)SalesOrderCompareDto.StatusCompare.New).ToString();
                        itemStyle.DeliveryDate = compareItemStyle.ETD;
                        itemStyle.SetCreateAudit(userName);
                        itemStyle.TotalQuantity = compareItemStyle.NewPlannedQty;
                        //itemStyle.ItemStyleStatusCode = compareItemStyle.Status.ToString();

                        foreach (var itemOrderDetail in compareItemStyle.OrderDetails)
                        {
                            OrderDetail orderDetail = new OrderDetail();

                            mapperOrderDetail.Map(itemOrderDetail, orderDetail);
                            orderDetail.ItemStyleNumber = itemStyle.Number;

                            if (dicSizes.TryGetValue(orderDetail.Size, out Size rsSize))
                            {
                                orderDetail.SizeSortIndex = rsSize.SequeneceNumber;
                            }

                            if (itemStyle.OrderDetails == null)
                            {
                                itemStyle.OrderDetails = new List<OrderDetail>();
                            }
                            itemStyle.OrderDetails.Add(orderDetail);
                        }

                        if (dicNewSalesOrders.TryGetValue(compareItemStyle.SalesOrderID, out SalesOrder rsSalesOrder))
                        {
                            if (!String.IsNullOrEmpty(itemStyle.PurchaseOrderNumber + itemStyle.PurchaseOrderNumberIndex))
                            {
                                rsSalesOrder.ItemStyles.Add(itemStyle);
                            }

                            newSalesOrders.Add(rsSalesOrder);
                        }
                        else
                        {
                            newItemStyle.Add(itemStyle);
                        }
                    }
                }

                if (newSalesOrders != null && newSalesOrders.Count() > 0)
                {
                    foreach (var salesOrder in newSalesOrders)
                    {
                        foreach (var itemStyle in salesOrder.ItemStyles)
                        {
                            newItemStyle.Add(itemStyle);
                        }
                    }
                }

                EPPlusExtensions.GenerateLSStyleNonSeason("HA", userName, ref parts, newItemStyle, out newParts, out errorMessage);
            }
            catch (Exception ex)
            {

                errorMessage = ex.Message;
            }
        }

        private static void SetCompareObject(SalesOrderCompareDto.StatusCompare Status, ItemStyle itemStyle,
            ItemStyle oldItemStyle, DateTime dtConfirm, ref SalesOrderCompareDto compareItem, out string errorMessage)
        {
            errorMessage = string.Empty;
            var config = new MapperConfiguration(
                cfg => cfg.CreateMap<OrderDetail, OrderDetailsDto>()
                //.ForMember(d => d.Branch, o => o.Ignore())
                );
            var mapper = new Mapper(config);

            var configItemStyle = new MapperConfiguration(
                         cfg => cfg.CreateMap<ItemStyle, SalesOrderCompareDto>()
                         .ForMember(d => d.OrderDetails, o => o.Ignore())
                         );
            var mapperItemStyle = new Mapper(configItemStyle);
            try
            {
                switch (Status)
                {
                    case SalesOrderCompareDto.StatusCompare.New:
                        {
                            DateTime dateTime = DateTime.ParseExact(
                                itemStyle.ShipDate?.ToString("MM/dd/yyyy"), "M/d/yyyy", CultureInfo.InvariantCulture);

                            mapperItemStyle.Map(itemStyle, compareItem);
                            compareItem.StyleNumber = itemStyle.Number;
                            compareItem.SalesOrderID = "SO.HADDAD." + itemStyle.ShipDate.Value.Year;
                            compareItem.ETD = itemStyle.DeliveryDate;
                            compareItem.Price = decimal.Parse(Math.Round(
                                                decimal.Parse(itemStyle.OrderDetails?.First().Price?.ToString()), 3).ToString());

                            if (itemStyle.OrderDetails != null)
                            {
                                StringBuilder size = new StringBuilder();
                                List<OrderDetailsDto> orderDetailsDtos = new List<OrderDetailsDto>();
                                foreach (var item in itemStyle.OrderDetails)
                                {
                                    OrderDetailsDto orderDetailsDto = new OrderDetailsDto();
                                    mapper.Map(item, orderDetailsDto);
                                    orderDetailsDtos.Add(orderDetailsDto);
                                    size.Append(" " + item.Size);
                                }
                                compareItem.OrderDetails = orderDetailsDtos;
                                compareItem.Size = size.ToString().Trim().Replace(" ", " - ");
                            }

                            if (DateTime.Compare(dtConfirm, dateTime) >= 0)
                            {
                                compareItem.Confirm = true;
                            }
                        }

                        break;

                    case SalesOrderCompareDto.StatusCompare.Update:
                        {
                            DateTime dtNewItem = DateTime.ParseExact(
                                itemStyle.ShipDate?.ToString("MM/dd/yyyy"), "M/d/yyyy", CultureInfo.InvariantCulture);
                            DateTime dtOldItem = DateTime.ParseExact(
                                oldItemStyle.ShipDate?.ToString("MM/dd/yyyy"), "M/d/yyyy", CultureInfo.InvariantCulture);

                            DateTime dtETD_NewItem = DateTime.ParseExact(
                                itemStyle.DeliveryDate?.ToString("MM/dd/yyyy"), "M/d/yyyy", CultureInfo.InvariantCulture);
                            DateTime dtETD_OldItem = DateTime.ParseExact(
                                oldItemStyle.DeliveryDate?.ToString("MM/dd/yyyy"), "M/d/yyyy", CultureInfo.InvariantCulture);

                            bool checkUpdate = false;
                            compareItem.StyleNumber = oldItemStyle.Number;
                            compareItem.PurchaseOrderNumber = itemStyle.PurchaseOrderNumber;
                            compareItem.Packing = itemStyle.Packing;
                            compareItem.CustomerStyle = itemStyle.CustomerStyle;
                            compareItem.ContractNo = itemStyle.ContractNo;
                            compareItem.ShipDate = itemStyle.ShipDate;
                            compareItem.ETD = itemStyle.DeliveryDate;
                            compareItem.Division = itemStyle.Division;
                            compareItem.Season = itemStyle.Season;
                            compareItem.PCB = itemStyle.PCB;
                            compareItem.SalesOrderID = oldItemStyle.SalesOrderID;
                            compareItem.UE = oldItemStyle.UE;
                            compareItem.Description = oldItemStyle.Description;
                            compareItem.Brand = oldItemStyle.Brand;
                            compareItem.UnitID = oldItemStyle.UnitID;
                            compareItem.Year = oldItemStyle.Year;
                            compareItem.PurchaseOrderNumberIndex = oldItemStyle.PurchaseOrderNumberIndex;

                            compareItem.MSRP = oldItemStyle.MSRP;
                            compareItem.ContractDate = oldItemStyle.ContractDate;
                            compareItem.ShipMode = oldItemStyle.ShipMode;


                            string color = itemStyle.ColorCode;
                            compareItem.ColorCode = color;
                            compareItem.ColorName = itemStyle.ColorName;

                            decimal price = decimal.Parse(Math.Round(
                                            decimal.Parse(itemStyle.OrderDetails?.First().Price?.ToString()), 3).ToString());
                            //compareItem.Price = (float.Parse(Math.Round(price, 3).ToString()));
                            compareItem.Price = price;
                            compareItem.LabelName = itemStyle.LabelName;
                            compareItem.LabelCode = itemStyle.LabelCode;
                            compareItem.HangFlat = itemStyle.HangFlat;
                            compareItem.ProductionDescription = itemStyle.ProductionDescription;

                            if (!String.IsNullOrEmpty(itemStyle.PCB) && !String.IsNullOrEmpty(oldItemStyle.PCB)
                                && !itemStyle.PCB.Trim().Equals(oldItemStyle.PCB.Trim()))
                            {
                                compareItem.OldPCB = oldItemStyle.PCB;
                                checkUpdate = true;
                            }

                            if (!String.IsNullOrEmpty(itemStyle.Packing) && !String.IsNullOrEmpty(oldItemStyle.Packing)
                                && !itemStyle.Packing.Trim().Equals(oldItemStyle.Packing.Trim()))
                            {
                                compareItem.OldPacking = oldItemStyle.Packing;
                                checkUpdate = true;
                            }

                            if (!String.IsNullOrEmpty(itemStyle.CustomerStyle) &&
                                !itemStyle.CustomerStyle.Equals(oldItemStyle.CustomerStyle))
                            {
                                compareItem.OldCustomerStyle = oldItemStyle.CustomerStyle;
                                checkUpdate = true;
                            }

                            if (!String.IsNullOrEmpty(itemStyle.ContractNo) && !itemStyle.ContractNo.Equals(oldItemStyle.ContractNo))
                            {
                                compareItem.OldContractNo = oldItemStyle.ContractNo;
                                checkUpdate = true;
                            }

                            if (dtNewItem.CompareTo(dtOldItem) != 0)
                            {
                                compareItem.OldShipDate = oldItemStyle.ShipDate;
                                checkUpdate = true;

                                if (DateTime.Compare(dtConfirm, dtNewItem) >= 0)
                                {
                                    compareItem.Confirm = true;
                                }
                            }
                            else
                            {
                                if (dtNewItem.CompareTo(dtOldItem) == 0 && (DateTime.Compare(dtConfirm, dtNewItem) >= 0
                                                                            || DateTime.Compare(dtConfirm, dtOldItem) >= 0))
                                {
                                    checkUpdate = true;
                                    compareItem.Confirm = true;
                                }
                            }

                            if (dtETD_NewItem.CompareTo(dtETD_OldItem) != 0)
                            {
                                compareItem.OldETD = oldItemStyle.DeliveryDate;
                                checkUpdate = true;
                            }

                            if (!String.IsNullOrEmpty(itemStyle.Division) && !itemStyle.Division.Equals(oldItemStyle.Division))
                            {
                                compareItem.OldDivision = oldItemStyle.Division;
                                checkUpdate = true;
                            }

                            if (!String.IsNullOrEmpty(itemStyle.Season) && !itemStyle.Season.Equals(oldItemStyle.Season))
                            {
                                compareItem.OldSeason = oldItemStyle.Season;
                                checkUpdate = true;
                            }

                            if (!String.IsNullOrEmpty(itemStyle.LabelCode) && !itemStyle.LabelCode.Equals(oldItemStyle.LabelCode))
                            {
                                compareItem.OldLabelCode = oldItemStyle.LabelCode;
                                checkUpdate = true;
                            }

                            if (!String.IsNullOrEmpty(itemStyle.LabelName) && !itemStyle.LabelName.Equals(oldItemStyle.LabelName))
                            {
                                compareItem.OldLabelName = oldItemStyle.LabelName;
                                checkUpdate = true;
                            }

                            if (!String.IsNullOrEmpty(itemStyle.HangFlat) && !itemStyle.HangFlat.Equals(oldItemStyle.HangFlat))
                            {
                                compareItem.OldHangFlat = oldItemStyle.HangFlat;
                                checkUpdate = true;
                            }

                            if (!String.IsNullOrEmpty(itemStyle.ProductionDescription)
                                && !itemStyle.ProductionDescription.Equals(oldItemStyle.ProductionDescription))
                            {
                                compareItem.OldProductionDescription = oldItemStyle.ProductionDescription;
                                checkUpdate = true;
                            }

                            string oldColor = oldItemStyle.ColorCode;

                            if (!String.IsNullOrEmpty(color) && !String.IsNullOrEmpty(oldColor) && !color.Equals(oldColor))
                            {
                                compareItem.OldColor = oldColor;
                                checkUpdate = true;
                            }

                            decimal oldPrice = 0;

                            if (oldItemStyle.OrderDetails != null && oldItemStyle.OrderDetails.Count() > 0)
                            {
                                oldPrice = decimal.Parse(Math.Round(
                                              decimal.Parse(oldItemStyle.OrderDetails?.First().Price?.ToString()), 3).ToString());
                            }

                            //Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>> " + oldItemStyle.PurchaseOrderNumber);
                            if (price > 0 && oldPrice > 0 && price != oldPrice)
                            {
                                compareItem.OldPrice = oldPrice;
                                checkUpdate = true;
                            }

                            if (itemStyle.OrderDetails != null && oldItemStyle.OrderDetails != null)
                            {
                                StringBuilder size = new StringBuilder();
                                size.Append("");

                                List<OrderDetailsDto> orderDetailsDtos = new List<OrderDetailsDto>();
                                var newOrderDtlList = itemStyle.OrderDetails.OrderBy(x => x.SizeSortIndex);
                                foreach (var item in newOrderDtlList)
                                {
                                    if (size.ToString() == "")
                                    {
                                        size.Append(item.Size.Trim());
                                    }
                                    else
                                    {
                                        size.Append(" - " + item.Size.Trim());
                                    }

                                    OrderDetailsDto orderDetailsDto = new OrderDetailsDto();
                                    mapper.Map(item, orderDetailsDto);
                                    orderDetailsDtos.Add(orderDetailsDto);
                                }
                                compareItem.OrderDetails = orderDetailsDtos;

                                StringBuilder oldSize = new StringBuilder();
                                oldSize.Append("");
                                var oldOrderDtlList = oldItemStyle.OrderDetails.OrderBy(x => x.SizeSortIndex);
                                foreach (var item in oldOrderDtlList)
                                {
                                    if (oldSize.ToString() == "")
                                    {
                                        oldSize.Append(item.Size.Trim());
                                    }
                                    else
                                    {
                                        oldSize.Append(" - " + item.Size.Trim());
                                    }
                                }

                                compareItem.Size = size.ToString().Trim();
                                if (!size.ToString().Trim().Equals(oldSize.ToString().Trim()))
                                {
                                    compareItem.OldSize = oldSize.ToString().Trim();
                                    checkUpdate = true;
                                }
                            }


                            if (checkUpdate)
                            {
                                compareItem.Status = SalesOrderCompareDto.StatusCompare.Update;
                                compareItem.StyleNumber = oldItemStyle.Number;
                            }
                            else
                            {
                                compareItem.Status = SalesOrderCompareDto.StatusCompare.Normal;
                            }
                        }
                        break;

                    case SalesOrderCompareDto.StatusCompare.Cancel:
                        {
                            compareItem.StyleNumber = oldItemStyle.Number;
                            compareItem.OldETD = oldItemStyle.DeliveryDate;
                            compareItem.OldDivision = oldItemStyle.Division;
                            compareItem.OldSeason = oldItemStyle.Season;
                            compareItem.PurchaseOrderNumber = oldItemStyle.PurchaseOrderNumber;
                            compareItem.PurchaseOrderNumberIndex = oldItemStyle.PurchaseOrderNumberIndex;
                            compareItem.OldPacking = oldItemStyle.Packing;
                            compareItem.OldCustomerStyle = oldItemStyle.CustomerStyle;
                            compareItem.OldContractNo = oldItemStyle.ContractNo;
                            compareItem.OldShipDate = oldItemStyle.ShipDate;
                            compareItem.OldColor = oldItemStyle.ColorCode;
                            compareItem.SalesOrderID = oldItemStyle.SalesOrderID;
                            compareItem.Status = SalesOrderCompareDto.StatusCompare.Cancel;

                            if (oldItemStyle.OrderDetails?.Count > 0)
                            {
                                compareItem.OldPrice = decimal.Parse(Math.Round(
                                                        decimal.Parse(oldItemStyle.OrderDetails?.First().Price?.ToString()), 3)
                                                                      .ToString());
                            }
                            else
                            {
                                compareItem.OldPrice = 0;
                            }

                            compareItem.OldLabelCode = oldItemStyle.LabelCode;
                            compareItem.OldLabelName = oldItemStyle.LabelName;

                            compareItem.OldHangFlat = oldItemStyle.HangFlat;
                            compareItem.OldProductionDescription = oldItemStyle.ProductionDescription;

                            if (oldItemStyle.OrderDetails != null)
                            {
                                StringBuilder oldSize = new StringBuilder();
                                foreach (var item in oldItemStyle.OrderDetails)
                                {
                                    oldSize.Append(" " + item.Size);
                                }

                                compareItem.OldSize = oldSize.ToString().Trim().Replace(" ", " - ");
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

        }

        private static Dictionary<string, string> GetDict(string SizeColumns)
        {
            if (!String.IsNullOrEmpty(SizeColumns))
            {
                var sizes = SizeColumns.Split(',');
                var dict = new Dictionary<string, string>();
                foreach (var str in sizes)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        var pair = str.Split('$');
                        dict.Add(pair[0].ToUpper(), pair[1]);
                    }
                }
                return dict;
            }
            return null;
        }
    }

}
