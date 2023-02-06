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
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext context;

        public CustomerRepository(AppDbContext context)
        {
            this.context = context;
        }

        public Customer Add(Customer customer)
        {
            return context.Customer.Add(customer).Entity;
        }

        public void Delete(Customer customer)
        {
            context.Customer.Remove(customer);
        }

        public Customer GetCustomer(string ID)
        {
            return context.Customer
                .Include(x => x.Currency)
                .Include(x => x.PaymentTerm)
                .Include(x => x.PriceTerm)
                .Include(x => x.Division)
                .Include(x => x.Brands)
                .FirstOrDefault(x => x.ID == ID);
        }

        public IEnumerable<Customer> GetCustomers()
        {
            return context.Customer
                .Include(x => x.Currency)
                .Include(x => x.PaymentTerm)
                .Include(x => x.PriceTerm)
                .Include(x => x.Division)
                .Include(x => x.Brands)
                .ToList();
        }

        public bool IsExist(string ID, out Customer customer)
        {
            customer = null;
            customer = context.Customer.FirstOrDefault(x => x.ID == ID);
            return customer != null ? true : false; 
        }

        public bool IsExist(string ID)
        {
            var customer = context.Customer.FirstOrDefault(x => x.ID == ID);
            return customer != null ? true : false;
        }

        public void Update(Customer customer)
        {
            context.Entry(customer).State = EntityState.Modified;
        }
    }
}
