using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IEntitySequenceNumberRepository
    {
        EntitySequenceNumber Add(EntitySequenceNumber entitySequenceNumber);
        void Update(EntitySequenceNumber entitySequenceNumber);
        void Delete(EntitySequenceNumber entitySequenceNumber);
        IEnumerable<EntitySequenceNumber> GetEntitySequenceNumbers();
        EntitySequenceNumber GetEntitySequenceNumber(string Code);
        bool IsExist(string Code);
        bool IsExist(string Code, out EntitySequenceNumber entitySequenceNumber);
        string GetNextNumberByCode(string Code, out EntitySequenceNumber entitySequenceNumber);
    }
}
