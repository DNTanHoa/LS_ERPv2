using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IReceiptRepository
    {
        Receipt Add(Receipt receipt);
        void Update(Receipt receipt);
        void Delete(Receipt receipt);
        IEnumerable<Receipt> GetReceipts();
        Receipt GetReceipt(string number);
        bool IsExist(string number, out Receipt receipt);
        bool IsExist(string number);
    }
}
