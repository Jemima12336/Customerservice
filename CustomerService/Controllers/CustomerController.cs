using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerService.Business;
using CustomerService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IBizManager<Customer> custBizManager;
        public CustomerController(IBizManager<Customer> custBizManager)
        {
            this.custBizManager = custBizManager;
        }
        [HttpGet]
        public IActionResult GetAllCustomers()
        {
            var customers = this.custBizManager.GetAll();
            if (customers == null)
            {
                return base.NotFound();
            }
            return base.Ok(customers);
        }
        [HttpGet("{id}", Name = "GetCustomerByID")]
        public IActionResult GetCustomerByID(String id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return base.BadRequest("Invalid customer ID");
            }
            var customerbyID = this.custBizManager.GetByID(id);
            if (customerbyID == null)
            {
                return base.NotFound();
            }
            return base.Ok(customerbyID);
        }
        [Authorize(Policy = Policies.Admin)]
        [HttpPost]
        public IActionResult AddCustomer(Customer customer)
        {
            if (customer == null)
            {
                return base.BadRequest();
            }
            this.custBizManager.Add(customer);
            return base.CreatedAtRoute(nameof(this.GetCustomerByID), new { id = customer.Id }, customer);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateCustomerByID(String id,[FromBody] Customer customer)
        {
            if (string.IsNullOrWhiteSpace(id) )
            {
                return base.BadRequest();
            }
            if(id!=customer.Id)
            {
                return base.BadRequest();
            }
            if(customer == null)
            {
                return base.BadRequest();
            }
        
            var updateByID=this.custBizManager.GetByID(id);
            if (updateByID == null)
            {
                return base.NotFound();
            }
            this.custBizManager.UpdateByID(id, customer);
            return base.NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteCustomerByID(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return base.BadRequest();
            }
            var Delete = this.custBizManager.DeleteByID(id);
            if (!Delete)
            {
                return base.NotFound();
            }
            return base.NoContent();
        }

    }
}
