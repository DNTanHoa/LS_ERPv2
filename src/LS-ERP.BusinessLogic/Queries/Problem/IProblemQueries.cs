using LS_ERP.BusinessLogic.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public interface IProblemQueries
    {
        public IEnumerable<ProblemDtos> GetAll();
        
    }
}
