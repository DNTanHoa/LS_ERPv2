using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Customer Add(Customer customer);
        void Update(Customer customer);
        void Delete(Customer customer);
        IEnumerable<Customer> GetCustomers();
        Customer GetCustomer(string ID);
        bool IsExist(string ID, out Customer customer);
        bool IsExist(string ID);
    }
}
