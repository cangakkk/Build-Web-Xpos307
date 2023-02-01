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
    public class ProductController : ControllerBase
    {
        private readonly XPOS307Context db;
        private readonly ProductRepo productRepo;
        public ProductController(XPOS307Context _db)
        {
            db = _db;
            productRepo = new ProductRepo(db);
        }

        [HttpGet("GetAll")]
        public VMResponse GetAll()
        {
            return productRepo.GetAll();
        }
        
        [HttpGet("[action]")]
        public VMResponse Get(int ProductID)
        {
            return productRepo.GetByID(ProductID);
        }

        [HttpPut("[action]")]
        public VMResponse Edit(VMTblProduct dataView)
        {
            return productRepo.Edit(dataView);
        }

        [HttpPost("[action]")]
        public VMResponse Add(VMTblProduct dataView)
        {
            return productRepo.Add(dataView);
        }

        [HttpDelete("[action]")]
        public VMResponse Delete(int productId, int updateBy)
        {
            return productRepo.Delete(productId, updateBy);
        }

        [HttpGet("[action]")]
        public VMResponse GetByName(string Name)
        {
            return productRepo.GetByName(Name);
        }
    }

}
