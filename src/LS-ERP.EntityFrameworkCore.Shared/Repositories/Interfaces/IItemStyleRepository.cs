using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IItemStyleRepository
    {
        ItemStyle Add(ItemStyle itemStyle);
        void Update(ItemStyle itemStyle);
        void Delete(ItemStyle itemStyle);
        IQueryable<ItemStyle> GetItemStyles();
        IQueryable<ItemStyle> GetItemStyles(string CustomerID);
        IQueryable<ItemStyle> GetOnlyItemStylesFollowContractNo(List<string> contractNos);
        IQueryable<ItemStyle> GetItemStylesForContractInfo(List<string> contractNos);
        IQueryable<ItemStyle> GetItemStylesFollowSalesOrderID(List<string> OrderID);
        IQueryable<ItemStyle> GetItemStylesFollowLSStyle(List<string> LSStyle);
        IQueryable<ItemStyle> GetItemStylesFollowLSStyleBarcodes(List<string> LSStyle, string customerID);
        IQueryable<ItemStyle> GetOnlyItemStylesFollowSalesOrderID(List<string> OrderID); // for update LSStyle
        IQueryable<ItemStyle> GetItemStylesForCompareData(List<string> OrderID);
        IQueryable<ItemStyle> GetItemStylesFollowSalesOrderID(string customerID);
        IQueryable<ItemStyle> GetItemStyles(List<string> numbers);
        ItemStyle GetItemStyle(string number);
        bool IsExist(string number);
        bool IsExist(string number, out ItemStyle itemStyle);
        ///Using import packing list HA
        IQueryable<ItemStyle> GetItemStylesByPONumber(string PONumber);
    }
}
