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
    public class WastageSettingRepository
        : IWastageSettingRepository
    {
        private readonly AppDbContext context;

        public WastageSettingRepository(AppDbContext context)
        {
            this.context = context;
        }

        public WastageSetting Add(WastageSetting wastageSetting)
        {
            return context.WastageSetting.Add(wastageSetting).Entity;
        }

        public void Delete(WastageSetting wastageSetting)
        {
            context.WastageSetting.Remove(wastageSetting);
        }

        public WastageSetting GetWastageSetting(long ID)
        {
            return context.WastageSetting.FirstOrDefault(x => x.ID == ID);
        }

        public IQueryable<WastageSetting> GetWastageSettings()
        {
            return context.WastageSetting;
        }

        public IQueryable<WastageSetting> GetWastageSettings(string CustomerID)
        {
            return context.WastageSetting.Where(x => x.CustomerID == CustomerID);
        }

        public bool IsExist(long ID)
        {
            var wastageSetting = GetWastageSetting(ID);
            return wastageSetting != null ? true : false;
        }

        public bool IsExist(long ID, out WastageSetting wastageSetting)
        {
            wastageSetting = null;
            wastageSetting = GetWastageSetting(ID);
            return wastageSetting != null ? true : false;
        }

        public void Update(WastageSetting wastageSetting)
        {
            context.Entry(wastageSetting).State = EntityState.Modified;
        }
    }
}
