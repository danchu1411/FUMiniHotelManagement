using System.Collections.Generic;
using BusinessObjects;

namespace Services
{
    public interface ICustomerService
    {
        List<Customer> GetAllCustomers();
        Customer? GetCustomerById(int customerId);
        Customer? GetCustomerByEmail(string email);
        void AddCustomer(Customer customer);
        void UpdateCustomer(Customer customer);
        void DeleteCustomer(Customer customer);

        // Business Logic for Auth
        Customer? Login(string email, string password);
        bool IsAdmin(string email, string password);
    }
}