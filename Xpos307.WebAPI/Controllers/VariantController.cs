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
    public class VariantController : ControllerBase
    {
        private readonly XPOS307Context db;
        private readonly VariantRepo variantRepo;
        public VariantController(XPOS307Context _db)
        {
            db = _db;
            variantRepo = new VariantRepo(db);
        }
        [HttpGet("GetAll")]
        public VMResponse GetAll()
        {
            return variantRepo.GetAll();
        }

        [HttpGet("[action]")]
        public VMResponse Get(int VariantID)
        {
            return variantRepo.GetById(VariantID);
        }

        [HttpGet("[action]")]
        public VMResponse GetByCategory(int categoryId)
        {
            return variantRepo.GetByCategory(categoryId);
        }

        [HttpPut("[action]")]
        public VMResponse Edit(VMTblVariant dataView)
        {
            return variantRepo.Edit(dataView);
        }

        [HttpPost("[action]")]
        public VMResponse Add(VMTblVariant dataView)
        {
            return variantRepo.Add(dataView);
        }
        [HttpDelete("[action]")]
        public VMResponse Delete(int variantId, int updateBy)
        {
            return variantRepo.Delete(variantId, updateBy);
        }

        [HttpGet("[action]")]
        public VMResponse GetByName(string Name)
        {
            return variantRepo.GetByName(Name);
        }
    }
}
