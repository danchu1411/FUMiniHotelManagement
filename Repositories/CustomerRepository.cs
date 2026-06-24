using System.Collections.Generic;
using BusinessObjects;
using DataAccessLayer;

namespace Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        public List<Customer> GetAllCustomers() => CustomerDAO.Instance.GetAllCustomers();
        public Customer? GetCustomerById(int customerId) => CustomerDAO.Instance.GetCustomerById(customerId);
        public Customer? GetCustomerByEmail(string email) => CustomerDAO.Instance.GetCustomerByEmail(email);
        public void AddCustomer(Customer customer) => CustomerDAO.Instance.AddCustomer(customer);
        public void UpdateCustomer(Customer customer) => CustomerDAO.Instance.UpdateCustomer(customer);
        public void DeleteCustomer(Customer customer) => CustomerDAO.Instance.DeleteCustomer(customer);
    }
}
