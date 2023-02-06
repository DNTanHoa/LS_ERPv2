using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IDivisionRepository
    {
        Division Add(Division division);
        void Update(Division division);
        void Delete(Division division);
        IEnumerable<Division> GetDivisions();
        Division GetDivision(string ID);
        bool IsExist(string ID);
        bool IsExist(string ID, out Division division);
    }
}
