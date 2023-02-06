using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface ILabelPortRepository
    {
        LabelPort Add(LabelPort labelPort);
        void Update(LabelPort labelPort);
        void Delete(LabelPort labelPort);
        IQueryable<LabelPort> GetLabelPorts();
        IQueryable<LabelPort> GetLabelPorts(string customerID);
        LabelPort GetLabelPort(long ID);
    }
}
