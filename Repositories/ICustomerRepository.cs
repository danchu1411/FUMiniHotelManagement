using System.Collections.Generic;
using BusinessObjects;

namespace Repositories
{
    public interface ICustomerRepository
    {
        List<Customer> GetAllCustomers();
        Customer? GetCustomerById(int customerId);
        Customer? GetCustomerByEmail(string email);
        void AddCustomer(Customer customer);
        void UpdateCustomer(Customer customer);
        void DeleteCustomer(Customer customer);
    }
}
