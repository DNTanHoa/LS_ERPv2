using LS_ERP.BusinessLogic.Dtos.Division;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public interface IDivisionQueries
    {
        IEnumerable<DivisionDtos> GetDivisions();
        DivisionDtos GetDivision(string ID);
    }
}
