using System.Collections.Generic;
using BusinessObjects;
using Repositories;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepo;

        public CustomerService()
        {
            _customerRepo = new CustomerRepository();
        }

        public List<Customer> GetAllCustomers() => _customerRepo.GetAllCustomers();
        public Customer? GetCustomerById(int customerId) => _customerRepo.GetCustomerById(customerId);
        public Customer? GetCustomerByEmail(string email) => _customerRepo.GetCustomerByEmail(email);
        public void AddCustomer(Customer customer) => _customerRepo.AddCustomer(customer);
        public void UpdateCustomer(Customer customer) => _customerRepo.UpdateCustomer(customer);
        public void DeleteCustomer(Customer customer) => _customerRepo.DeleteCustomer(customer);

        public bool IsAdmin(string email, string password)
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            string adminEmail = config["AdminAccount:Email"] ?? "admin@FUMiniHotelSystem.com";
            string adminPassword = config["AdminAccount:Password"] ?? "@@abc123@@";

            return email == adminEmail && password == adminPassword;
        }

        public Customer? Login(string email, string password)
        {
            var customer = _customerRepo.GetCustomerByEmail(email);
            if (customer != null && customer.Password == password)
            {
                return customer;
            }
            return null;
        }
    }
}
