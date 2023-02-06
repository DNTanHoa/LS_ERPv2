using LS_ERP.BusinessLogic.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public interface IOperationQueries
    {
        public IEnumerable<OperationDtos> GetByID(string ID);
        public IEnumerable<OperationDtos> GetAll();
        public IEnumerable<OperationDtos> GetByGroup(string group);
       
    }
}
