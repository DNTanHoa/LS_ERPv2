using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IUnitRepository
    {
        Unit Add(Unit unit);
        void Update(Unit unit);
        void Delete(Unit unit);
        IEnumerable<Unit> GetUnits();
        Unit GetUnit(string ID);
        bool IsExist(string ID, out Unit Unit);
        bool IsExist(string ID);
    }
}
