using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories
{
    public class StorageRepository : IStorageRepository
    {
        private readonly AppDbContext context;

        public StorageRepository(AppDbContext context)
        {
            this.context = context;
        }

        public Storage Add(Storage storage)
        {
            return context.Storage.Add(storage).Entity;
        }

        public void Delete(Storage storage)
        {
            context.Storage.Remove(storage);
        }

        public Storage GetStorage(string code)
        {
            return context.Storage
                .Include(x => x.StorageDetails)
                .FirstOrDefault(x => x.Code == code);
        }

        public IEnumerable<Storage> GetStorages()
        {
            return context.Storage;
        }

        public bool IsExist(string code, out Storage storage)
        {
            storage = null;
            storage = context.Storage.FirstOrDefault(x => x.Code == code);
            return storage != null ? true : false;
        }

        public bool IsExist(string code)
        {
            var storage = context.Storage.FirstOrDefault(x => x.Code == code);
            return storage != null ? true : false;
        }

        public void Update(Storage storage)
        {
            context.Entry(storage).State = EntityState.Modified;
        }
    }
}
