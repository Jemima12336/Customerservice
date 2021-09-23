using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerService.Business;
using CustomerService.Models;

namespace CustomerService.Business
{
    public class custBizManager:IBizManager<Customer>
    {
        private static IList<Customer> _repository = new List<Customer>();
       public IList<Customer> GetAll()
        {
            return _repository;
        }
        public Customer GetByID(string id)
        {
            return _repository.Where(cust => cust.Id == id).FirstOrDefault();
        }
        public void Add(Customer customer)
        {
            Random rnd = new Random();
            customer.Id = rnd.Next(10000000, 99999999).ToString();
            _repository.Add(customer);
        }
        public bool DeleteByID(string id)
        {
            var customer= _repository.Where(cust => cust.Id == id).FirstOrDefault();
            if (customer != null)
            {
                _repository.Remove(customer);
                return true;
            }
            return false;
        }
        public void UpdateByID(string id,Customer customer)
        {
            var targetcustomer = _repository.FirstOrDefault(cust => cust.Id == id);
            if (targetcustomer != null)
            {
                targetcustomer.LastName=customer.LastName;
                targetcustomer.FirstName = customer.FirstName;
                targetcustomer.DOB=customer.DOB;
                targetcustomer.SSN=customer.SSN;

            }
        }
    }
}
