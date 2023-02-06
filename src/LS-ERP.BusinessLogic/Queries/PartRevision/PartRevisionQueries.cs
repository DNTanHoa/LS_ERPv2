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
    public class PartRevisionQueries : IPartRevisionQueries
    {
        private readonly IPartRevisionRepository partRevisionRepository;
        private readonly IMapper mapper;

        public PartRevisionQueries(IPartRevisionRepository partRevisionRepository,
            IMapper mapper)
        {
            this.partRevisionRepository = partRevisionRepository;
            this.mapper = mapper;
        }

        public PartRevisionDetailDtos GetDetailDtos(long ID)
        {
            var partRevision = partRevisionRepository.GetPartRevision(ID);

            if(partRevision != null)
            {
                return mapper.Map<PartRevisionDetailDtos>(partRevision);
            }

            return null;
        }

        public IEnumerable<PartRevisionSummaryDtos> GetSummaryDtos()
        {
            return partRevisionRepository.GetPartRevisions()
                .Select(x => mapper.Map<PartRevisionSummaryDtos>(x));
        }
    }
}
