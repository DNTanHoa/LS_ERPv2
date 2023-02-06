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
    public class BoxDimensionRepository : IBoxDimensionRepository
    {
        private readonly AppDbContext context;

        public BoxDimensionRepository(AppDbContext context)
        {
            this.context = context;
        }
        public BoxDimension Add(BoxDimension boxDimension)
        {
            return context.BoxDimension.Add(boxDimension).Entity;
        }

        public void Delete(BoxDimension boxDimension)
        {
            context.BoxDimension.Remove(boxDimension);
        }

        public BoxDimension GetBoxDimension(string Code)
        {
            return context.BoxDimension
               .FirstOrDefault(x => x.Code == Code);
        }

        public IQueryable<BoxDimension> GetBoxDimensions()
        {
            return context.BoxDimension;
        }

        public void Update(BoxDimension boxDimension)
        {
            context.Entry(boxDimension).State = EntityState.Modified;
        }
    }
}
