using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IItemStyleBarCodeRepository
    {
        ItemStyleBarCode Add(ItemStyleBarCode itemStyle);
        void Update(ItemStyleBarCode itemStyle);
        void Delete(ItemStyleBarCode itemStyle);
        IEnumerable<ItemStyleBarCode> GetItemStyles();
        IEnumerable<ItemStyleBarCode> GetItemStylesBySalesOrderID(string salesOrderID);
        ItemStyleBarCode GetItemStyle(string Number);
        bool IsExist(string Number);
        bool IsExist(string Number, out ItemStyle itemStyle);
    }
}
