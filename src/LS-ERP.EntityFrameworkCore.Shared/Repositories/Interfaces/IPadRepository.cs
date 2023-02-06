using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IPadRepository
    {
        Pad Add(Pad pad);
        void Update(Pad pad);
        void Delete(Pad pad);
        IQueryable<Pad> GetPads();
        Pad GetPad(string Code);
    }
}
