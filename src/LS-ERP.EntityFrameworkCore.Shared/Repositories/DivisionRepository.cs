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
    public class DivisionRepository : IDivisionRepository
    {
        private readonly AppDbContext context;

        public DivisionRepository(AppDbContext context)
        {
            this.context = context;
        }

        public Division Add(Division division)
        {
            return context.Add(division).Entity;
        }

        public void Delete(Division division)
        {
            context.Division.Remove(division);
        }

        public Division GetDivision(string ID)
        {
            return context.Division.FirstOrDefault(x => x.ID == ID);
        }

        public IEnumerable<Division> GetDivisions()
        {
            return context.Division.ToList();
        }

        public bool IsExist(string ID)
        {
            var division = GetDivision(ID);
            return division != null ? true : false;
        }

        public bool IsExist(string ID, out Division division)
        {
            division = null;
            division = GetDivision(ID);
            return division != null ? true : false;
        }

        public void Update(Division division)
        {
            context.Entry(division).State = EntityState.Modified;
        }
    }
}
