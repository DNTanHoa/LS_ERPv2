using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories
{
    public class UnitRepository : IUnitRepository
    {
        private readonly AppDbContext context;

        public UnitRepository(AppDbContext context)
        {
            this.context = context;
        }

        public Unit Add(Unit unit)
        {
            return context.Add(unit).Entity;
        }

        public void Delete(Unit unit)
        {
            context.Unit.Remove(unit);
        }

        public Unit GetUnit(string ID)
        {
            return context.Unit
                .FirstOrDefault(x => x.ID == ID);
        }

        public IEnumerable<Unit> GetUnits()
        {
            return context.Unit.ToList();
        }

        public bool IsExist(string ID, out Unit unit)
        {
            unit = null;
            unit = context.Unit.FirstOrDefault(x => x.ID == ID);
            return unit != null ? true : false;
        }

        public bool IsExist(string ID)
        {
            var unit = context.Unit.FirstOrDefault(x => x.ID == ID);
            return unit != null ? true : false;
        }

        public void Update(Unit unit)
        {
            context.Entry(unit).State = EntityState.Modified;
        }
    }
}
