using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IBoxDimensionRepository
    {
        BoxDimension Add(BoxDimension boxDimension);
        void Update(BoxDimension boxDimension);
        void Delete(BoxDimension boxDimension);
        IQueryable<BoxDimension> GetBoxDimensions();
        BoxDimension GetBoxDimension(string Code);
    }
}
