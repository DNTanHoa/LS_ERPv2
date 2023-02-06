using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IJobHeadRepository
    {
        JobHead Add(JobHead jobHead);
        void Update(JobHead jobHead);
        void Delete(JobHead jobHead);
        IQueryable<JobHead> GetJobHeads();
        IQueryable<JobHead> GetJobHeads(string customerID, string style,
            DateTime fromDate, DateTime toDate);
        IQueryable<JobHead> SearchJobHeads(string keywords);
        IQueryable<JobHead> GetJobHeads(string style);
        IQueryable<JobHead> GetJobHeads(List<string> customerStyles);
        JobHead GetJobHead(string number);
        bool IsExist(string number, out JobHead jobHead);
        bool IsExist(string number);
    }
}
