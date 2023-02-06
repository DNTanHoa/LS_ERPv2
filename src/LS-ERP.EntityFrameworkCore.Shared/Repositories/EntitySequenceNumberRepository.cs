using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories
{
    public class EntitySequenceNumberRepository
        : IEntitySequenceNumberRepository
    {
        private readonly AppDbContext context;

        public EntitySequenceNumberRepository(AppDbContext context)
        {
            this.context = context;
        }

        public EntitySequenceNumber Add(EntitySequenceNumber entitySequenceNumber)
        {
            return context.EntitySequenceNumber.Add(entitySequenceNumber).Entity;
        }

        public void Delete(EntitySequenceNumber entitySequenceNumber)
        {
            context.EntitySequenceNumber.Remove(entitySequenceNumber);
        }

        public EntitySequenceNumber GetEntitySequenceNumber(string Code)
        {
            return context.EntitySequenceNumber.FirstOrDefault(x => x.Code == Code);
        }

        public IEnumerable<EntitySequenceNumber> GetEntitySequenceNumbers()
        {
            return context.EntitySequenceNumber;
        }

        public string GetNextNumberByCode(string Code, out EntitySequenceNumber sequenceNumber)
        {
            sequenceNumber = GetEntitySequenceNumber(Code);

            if(sequenceNumber != null)
            {
                var lastNumber = sequenceNumber.LastNumber;
                var nextNumber = sequenceNumber.Prefix + sequenceNumber.Subfix
                    + lastNumber.ToString().PadLeft((int)sequenceNumber.NumberLength, '0');
                sequenceNumber.LastNumber += 1;

                return nextNumber;
            }

            return null;
        }

        public bool IsExist(string Code)
        {
            throw new NotImplementedException();
        }

        public bool IsExist(string Code, out EntitySequenceNumber entitySequenceNumber)
        {
            throw new NotImplementedException();
        }

        public void Update(EntitySequenceNumber entitySequenceNumber)
        {
            throw new NotImplementedException();
        }
    }
}
