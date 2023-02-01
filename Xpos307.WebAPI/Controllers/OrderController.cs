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
    public class OrderController : ControllerBase
    {
        private readonly XPOS307Context db;
        private readonly OrderRepo orderRepo;
        public OrderController(XPOS307Context _db)
        {
            db = _db;
            orderRepo = new OrderRepo(db);
        }
        [HttpGet("[action]")]
        public VMResponse GetAllTrxDetail()
        {
            return orderRepo.GetAllTrxDetail();
        }

        [HttpGet("[action]")]
        public VMResponse GetAllTrx()
        {
            return orderRepo.GetAllTrx();
        }

        [HttpGet("[action]")]
        public VMResponse GetTrxById(int OrderID)
        {
            return orderRepo.GetTrxById(OrderID);
        }

        [HttpGet("[action]")]
        public VMResponse GetTrxDetailById(int orderHeaderId)
        {
            return orderRepo.GetTrxDetailById(orderHeaderId);
        }

        [HttpPut("[action]")]
        public VMResponse Edit(VMTblOrderDetail dataView)
        {
            return orderRepo.Edit(dataView);
        }

        [HttpPost("[action]")]
        public VMResponse Add(VMTblOrderDetail dataView)
        {
            return orderRepo.Add(dataView);
        }

        [HttpDelete("[action]")]
        public VMResponse Delete(int orderId, int updateBy)
        {
            return orderRepo.Delete(orderId, updateBy);
        }

        [HttpPost("[action]")]
        public VMResponse AddOrder(List<VMTblOrderDetail> orderChart, int totalItem, int totalAmount,bool isCheckOut)
        {
            return orderRepo.AddOrder(orderChart, totalItem, totalAmount, isCheckOut);
        }

        [HttpPut("[action]")]
        public VMResponse UptadeOrderHead(int orderHeaderId, int totalQty, decimal Amount, bool isCheckOut,int userId)
        {
            return orderRepo.UptadeOrderHead(orderHeaderId,totalQty,Amount,isCheckOut,userId);
        }

        //[HttpPost("[action]")]
        //public VMResponse SaveOrder(List<VMTblOrderDetail> orderChart, int totalItems, int totalAmount)
        //{
        //    return orderRepo.SaveOrder(orderChart, totalItems, totalAmount);
        //}
    }
}
