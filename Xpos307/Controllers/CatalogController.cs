using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xpos307.ViewModels;

namespace Xpos307.Controllers
{
    public class CatalogController : Controller
    {
        private readonly HttpClient httpClient= new HttpClient();
        private readonly string apiUrl;
        private string errMsg;
        public CatalogController(IConfiguration _config)
        {
            apiUrl = _config["ApiUrl"];

        }
        public async Task<IActionResult> Index(VMPage page)
        {
            VMResponse apiResponse;

            if (string.IsNullOrEmpty(page.Name))
                apiResponse = JsonConvert.DeserializeObject<VMResponse>(await httpClient.GetStringAsync(apiUrl + "api/Product/GetAll"));
            else
                apiResponse = JsonConvert.DeserializeObject<VMResponse>(await httpClient.GetStringAsync(apiUrl + "api/Product/GetByName?Name=" + page.Name));

            List<VMTblProduct> data = JsonConvert.DeserializeObject<List<VMTblProduct>>
                                        (apiResponse.entity.ToString()).Where(p=>p.Stock>0).ToList();

            if (apiResponse.Success == false)
            {
                errMsg = "Product API tidak ditemukan";
                return RedirectToAction("Index","Home");
            }
            else if(!apiResponse.Success|| apiResponse.entity == null)
            {
                errMsg = apiResponse.message;

            }
            ViewBag.title = "Product Catalog";
            return View(data);
        }

        public IActionResult OrderPreview(List<VMTblOrderDetail> listCart, int totalQty, decimal amount)
        {
            ViewBag.TotalQty = totalQty;
            ViewBag.Amount = amount;

            return View(listCart);
        }

        [HttpPost]
        public async Task<VMResponse> SubmitOrder(List<VMTblOrderDetail> orderCart, int totalItem, decimal totalAmount, bool isCheckOut)
        {
            string jsonData = JsonConvert.SerializeObject(orderCart);

            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            VMResponse apiResponse = JsonConvert.DeserializeObject<VMResponse>(await(
                                    await httpClient.PostAsync(apiUrl + "api/Order/AddOrder?totalItem=" + totalItem + "&totalAmount=" + totalAmount+ "&isCheckOut=" + isCheckOut, content)
                              ).Content.ReadAsStringAsync());
           
            return apiResponse;
        }

        //[HttpPost]
        //public async Task<IActionResult> SaveOrder(List<VMTblOrderDetail> orderCart, int totalItem, decimal totalAmount)
        //{
        //    string jsonData = JsonConvert.SerializeObject(orderCart);
        //    HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        //    VMResponse apiResponse = JsonConvert.DeserializeObject<VMResponse>(await (
        //                            await httpClient.PostAsync(apiUrl + "api/Order/SaveOrder?totalItems=" + totalItem + "&totalAmount=" + totalAmount, content)
        //                      ).Content.ReadAsStringAsync());

        //    if (apiResponse == null)
        //    {
        //        errMsg = "API tidak bisa ditemukan";
        //        return RedirectToAction("Index,Home");
        //    }
        //    else if (!apiResponse.Success)
        //    {
        //        errMsg = apiResponse.message;
        //    }
        //    return View();
        //}

        public async Task<IActionResult> OrderHistory()
        {
            VMResponse apiResponse = JsonConvert.DeserializeObject<VMResponse>(await(
                                   await httpClient.GetAsync(apiUrl + "api/Order/GetAllTrx")
                             ).Content.ReadAsStringAsync());
            
            if (apiResponse == null)
            {
                errMsg = "API tidak bisa ditemukan";
                return RedirectToAction("Index","Home");
            }
            else if (!apiResponse.Success)
            {
                errMsg = apiResponse.message;
            }
            List<VMTblOrderHeader> data = JsonConvert.DeserializeObject<List<VMTblOrderHeader>>(apiResponse.entity.ToString());
            
            ViewBag.Title = "Order History";
            return View(data);
        }

        public async Task<IActionResult> OrderDetails(int orderHeaderId)
        {
            VMResponse apiResponse = JsonConvert.DeserializeObject<VMResponse>(await (
                                   await httpClient.GetAsync(apiUrl + "api/Order/GetTrxDetailById?orderHeaderId="+ orderHeaderId)
                             ).Content.ReadAsStringAsync());

            if (apiResponse == null)
            {
                errMsg = "API tidak bisa ditemukan";
                return RedirectToAction("Index", "Home");
            }
            else if (!apiResponse.Success)
            {
                errMsg = apiResponse.message;
            }
            List<VMTblOrderDetail> data = JsonConvert.DeserializeObject<List<VMTblOrderDetail>>(apiResponse.entity.ToString());

            ViewBag.Title = "Order History";

            return View(data);
        }

        public async Task<VMResponse> UpdateOrderHead(int orderHeaderId, int totalQty, decimal Amount, bool isCheckOut, int userId)
        { 
            HttpContent content = new StringContent("", Encoding.UTF8, "application/json");

            VMResponse apiResponse = JsonConvert.DeserializeObject<VMResponse>(await (
                                    await httpClient.PutAsync(apiUrl + "api/Order/UptadeOrderHead?orderHeaderId=" + orderHeaderId 
                                    + "&totalQty=" + totalQty 
                                    + "&Amount=" + Amount 
                                    + "&isCheckOut=" + isCheckOut 
                                    + "&userId=3"
                                    , content)
                              ).Content.ReadAsStringAsync());

            return apiResponse;
        }
    }
}
