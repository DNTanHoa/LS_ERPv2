using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace LS_ERP.BusinessLogic.Queries
{
    public interface ISizeQueries
    {

        IEnumerable<SizeDtos> GetByCustomerId(string customerID);
        IEnumerable<SizeLSStyleDtos> GetByLSStyle(string lsStyle);

    }
}
