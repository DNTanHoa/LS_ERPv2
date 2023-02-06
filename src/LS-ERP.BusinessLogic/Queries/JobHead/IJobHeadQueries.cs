using LS_ERP.BusinessLogic.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public interface IJobHeadQueries
    {
        IEnumerable<JobHeadSummaryDtos> GetSummaryDtos();
        IEnumerable<JobHeadSummaryDtos> GetSummaryDtos(string style);
        (IEnumerable<JobHeadSummaryDtos>, int totalPage, int totalCount)
            GetJobHeadSummaryDtosPaging(string keyword, int pageIndex, int pageSize);
        JobHeadDetailDtos GetDetailDtos(string number);

        IEnumerable<JobHeadSummaryDtos> Filter(string customerID, string style,
            DateTime fromDate, DateTime toDate);
    }
}
