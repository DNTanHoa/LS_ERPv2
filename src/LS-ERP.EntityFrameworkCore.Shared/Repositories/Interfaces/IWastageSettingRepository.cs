using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IWastageSettingRepository
    {
        WastageSetting Add(WastageSetting wastageSetting);
        void Update(WastageSetting wastageSetting);
        void Delete(WastageSetting wastageSetting);
        IQueryable<WastageSetting> GetWastageSettings();
        IQueryable<WastageSetting> GetWastageSettings(string CustomerID);
        WastageSetting GetWastageSetting(long ID);
        bool IsExist(long ID);
        bool IsExist(long ID, out WastageSetting wastageSetting);
    }
}
