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
    public class UnitQueries : IUnitQueries
    {
        private readonly IUnitRepository unitRepository;
        private readonly IMapper mapper;

        public UnitQueries(IUnitRepository unitRepository,
            IMapper mapper)
        {
            this.unitRepository = unitRepository;
            this.mapper = mapper;
        }

        public IEnumerable<UnitSelectDtos> GetSelectUnits()
        {
            return unitRepository.GetUnits()
                .Select(x => mapper.Map<UnitSelectDtos>(x));
        }

        public UnitDtos GetUnit(string ID)
        {
            var unit = unitRepository.GetUnit(ID);

            if(unit != null)
            {
                return mapper.Map<UnitDtos>(unit);
            }

            return null;
        }

        public IEnumerable<UnitDtos> GetUnits()
        {
            return unitRepository.GetUnits()
                .Select(x => mapper.Map<UnitDtos>(x));
        }
    }
}
