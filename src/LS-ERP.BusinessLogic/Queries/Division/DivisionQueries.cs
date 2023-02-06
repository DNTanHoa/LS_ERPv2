using AutoMapper;
using LS_ERP.BusinessLogic.Dtos.Division;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public class DivisionQueries : IDivisionQueries
    {
        private readonly IDivisionRepository divisionRepository;
        private readonly IMapper mapper;

        public DivisionQueries(IDivisionRepository divisionRepository,
            IMapper mapper)
        {
            this.divisionRepository = divisionRepository;
            this.mapper = mapper;
        }

        public DivisionDtos GetDivision(string ID)
        {
            var division = divisionRepository.GetDivision(ID);

            if(division != null)
            {
                return mapper.Map<DivisionDtos>(division);
            }

            return null;
        }

        public IEnumerable<DivisionDtos> GetDivisions()
        {
            var divisions = divisionRepository.GetDivisions();
            return divisions.Select(x => mapper.Map<DivisionDtos>(x));
        }
    }
}
