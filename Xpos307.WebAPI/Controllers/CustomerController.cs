using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Xpos307.DataModels;
using Xpos307.Repo;
using Xpos307.ViewModels;

namespace Xpos307.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly XPOS307Context db;
        private readonly CustomerRepo customerRepo;
        public CustomerController(XPOS307Context _db)
        {
            db = _db;
            customerRepo = new CustomerRepo(db);
        }
        [HttpGet("[action]")]
        public VMResponse GetAll()
        {
            return customerRepo.GetAll();
        }

        [HttpGet("[action]")]
        public VMResponse Get(int CustomerId)
        {
            return customerRepo.GetByID(CustomerId);
        }

        [HttpPut("[action]")]
        public VMResponse Edit(VMTblCustomer dataView)
        {
            return customerRepo.Edit(dataView);
        }
        [HttpPost("[action]")]
        public VMResponse Add(VMTblCustomer dataView)
        {
            return customerRepo.Add(dataView);
        }

        [HttpDelete("[action]")]
        public VMResponse Delete(int customerId, int updateBy)
        {
            return customerRepo.Delete(customerId, updateBy);
        }
    }
}
