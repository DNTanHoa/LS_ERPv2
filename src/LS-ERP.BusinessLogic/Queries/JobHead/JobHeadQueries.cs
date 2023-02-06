using AutoMapper;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public class JobHeadQueries : IJobHeadQueries
    {
        private readonly IJobHeadRepository jobHeadRepository;
        private readonly IMapper mapper;

        public JobHeadQueries(IJobHeadRepository jobHeadRepository,
            IMapper mapper)
        {
            this.jobHeadRepository = jobHeadRepository;
            this.mapper = mapper;
        }

        public JobHeadDetailDtos GetDetailDtos(string number)
        {
            var jobHead = jobHeadRepository.GetJobHead(number);

            if(jobHead != null)
            {
                return mapper.Map<JobHeadDetailDtos>(jobHead);
            }

            return null;
        }

        public (IEnumerable<JobHeadSummaryDtos>, int totalPage, int totalCount) GetJobHeadSummaryDtosPaging(string keyword, int pageIndex, int pageSize)
        {
            var jobHead = jobHeadRepository.SearchJobHeads(keyword);
            int totalCount = jobHead.Count();
            int totalPage = (totalCount + pageSize - 1) / pageSize;
            var data = jobHead.Skip(pageIndex * pageSize).Take(pageSize)
                .Select(x => mapper.Map<JobHeadSummaryDtos>(x))
                .ToList();
            return (data, totalPage, totalCount);
        }

        public IEnumerable<JobHeadSummaryDtos> GetSummaryDtos()
        {
            return jobHeadRepository.GetJobHeads()
                .Select(x => mapper.Map<JobHeadSummaryDtos>(x));
        }

        public IEnumerable<JobHeadSummaryDtos> GetSummaryDtos(string style)
        {
            return jobHeadRepository.GetJobHeads(style)
                .Select(x => mapper.Map<JobHeadSummaryDtos>(x));
        }

        public IEnumerable<JobHeadSummaryDtos> Filter(string customerID, string style, 
            DateTime fromDate, DateTime toDate)
        {
            return jobHeadRepository.GetJobHeads(customerID, style, fromDate, toDate)
                .Select(x => mapper.Map<JobHeadSummaryDtos>(x));
        }
    }
}
