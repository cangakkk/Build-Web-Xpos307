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
    public class CategoryController : ControllerBase
    {
        private readonly XPOS307Context db;
        private readonly CategoryRepo categoryRepo;
        public CategoryController(XPOS307Context _db)
        {
            db = _db;
            categoryRepo = new CategoryRepo(db);
        }
        [HttpGet("GetAll")]
        public VMResponse GetAll()
        {
            return categoryRepo.GetAll();
        }

        [HttpGet("[action]")]
        public VMResponse Get(int CategoryID)
        {
            return categoryRepo.GetByID(CategoryID);
        }

        [HttpPut("[action]")]
        public VMResponse Edit(VMTblCategory dataView)
        {
            return categoryRepo.Edit(dataView);
        }

        [HttpPost("[action]")]
        public VMResponse Add(VMTblCategory dataView)
        {
            return categoryRepo.Add(dataView);
        }

        [HttpDelete("[action]")]
        public VMResponse Delete(int categoryId, int updateBy)
        {
            return categoryRepo.Delete(categoryId, updateBy);
        }

        [HttpGet("[action]")]
        public VMResponse GetByName(string Name)
        {
            return categoryRepo.GetByName(Name);
        }
    }
}

   

