using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IStorageRepository
    {
        Storage Add(Storage storage);
        void Update(Storage storage);
        void Delete(Storage storage);
        IEnumerable<Storage> GetStorages();
        Storage GetStorage(string code);
        bool IsExist(string code, out Storage storage);
        bool IsExist(string code);
    }
}
