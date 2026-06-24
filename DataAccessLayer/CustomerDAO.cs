using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects;

namespace DataAccessLayer
{
    public class CustomerDAO
    {
        private static CustomerDAO? instance = null;
        private static readonly object instanceLock = new object();

        private CustomerDAO() { }

        public static CustomerDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new CustomerDAO();
                    }
                    return instance;
                }
            }
        }

        public List<Customer> GetAllCustomers()
        {
            using var context = new FuminiHotelManagementContext();
            return context.Customers.ToList();
        }

        public Customer? GetCustomerById(int customerId)
        {
            using var context = new FuminiHotelManagementContext();
            return context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
        }

        public Customer? GetCustomerByEmail(string email)
        {
            using var context = new FuminiHotelManagementContext();
            return context.Customers.FirstOrDefault(c => c.EmailAddress != null && c.EmailAddress.Equals(email));
        }

        public void AddCustomer(Customer customer)
        {
            using var context = new FuminiHotelManagementContext();
            context.Customers.Add(customer);
            context.SaveChanges();
        }

        public void UpdateCustomer(Customer customer)
        {
            using var context = new FuminiHotelManagementContext();
            context.Customers.Update(customer);
            context.SaveChanges();
        }

        public void DeleteCustomer(Customer customer)
        {
            using var context = new FuminiHotelManagementContext();
            context.Customers.Remove(customer);
            context.SaveChanges();
        }
    }
}