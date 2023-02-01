using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xpos307.ViewModels;

namespace Xpos307.Controllers
{
    public class CustomerController : Controller
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static string apiUrl;
        private readonly IWebHostEnvironment webHostEnvironment;

        public CustomerController(IConfiguration _config, IWebHostEnvironment _webHostEnvironment)
        {
            apiUrl = _config["ApiUrl"];
            webHostEnvironment= _webHostEnvironment;
        }

        public async Task<IActionResult> Index(VMPage page)
        {
            VMResponse apiResponse = new VMResponse();

            if (string.IsNullOrEmpty(page.Name))
                apiResponse = JsonConvert.DeserializeObject<VMResponse>(await httpClient.GetStringAsync(apiUrl + "api/Customer/GetAll"));
            else
                apiResponse = JsonConvert.DeserializeObject<VMResponse>(await httpClient.GetStringAsync(apiUrl + "api/Customer/GetByName?Name=" + page.Name));

            List<VMTblCustomer> data = JsonConvert.DeserializeObject<List<VMTblCustomer>>(apiResponse.entity.ToString());

            if (data == null || apiResponse.Success == false)
            {
                string errorMag = apiResponse.message;
                return RedirectToAction("Index");
            }

            //Get Short Order
            //ViewBag.OrderId = string.IsNullOrEmpty(page.orderBy) ? "id_desc" : "";
            //ViewBag.OrderName = page.orderBy == "name" ? "name_desc" : "name";
            //ViewBag.OrderVar = page.orderBy == "variant" ? "variant_desc" : "variant";
            //ViewBag.OrderCat = page.orderBy == "category" ? "category_desc" : "category";

            //switch (page.orderBy)
            //{
            //    case "name_desc":
            //        data = data.OrderByDescending(p => p.Name).ToList();
            //        break;
            //    case "name":
            //        data = data.OrderBy(p => p.Name).ToList();
            //        break;
            //    case "variant_desc":
            //        data = data.OrderByDescending(p => p.VariantName).ToList();
            //        break;
            //    case "variant":
            //        data = data.OrderBy(p => p.VariantName).ToList();
            //        break;
            //    case "category_desc":
            //        data = data.OrderByDescending(p => p.CategoryName).ToList();
            //        break;
            //    case "category":
            //        data = data.OrderBy(p => p.CategoryName).ToList();
            //        break;
            //    case "id_desc":
            //        data = data.OrderByDescending(p => p.Id).ToList();
            //        break;
            //    default:
            //        data = data.OrderBy(p => p.Id).ToList();
            //        break;
            //}

            ViewBag.Title = "Customer Index";
            
            return View(data);
        }

        public IActionResult Add()
        {
            VMTblCustomer dataView = new VMTblCustomer();

            ViewBag.Title = "Add Customer";
            return View(dataView);
        }

        [HttpPost]
        public async Task<IActionResult> Add(VMTblCustomer dataView)
        {
            string jsonData = JsonConvert.SerializeObject(dataView);

            HttpContent content = new StringContent(jsonData, UnicodeEncoding.UTF8, "application/json");

            VMResponse apiResponse = JsonConvert.DeserializeObject<VMResponse>(await (
                                    await httpClient.PostAsync(apiUrl + "api/Customer/Add", content)
                              ).Content.ReadAsStringAsync());

            if (!apiResponse.Success)
            {
                string errorMag = apiResponse.message;
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Detail(int CustomerId)
        {
            VMResponse apiResponse = JsonConvert.DeserializeObject<VMResponse>(
                                    await httpClient.GetStringAsync(apiUrl + "api/Customer/Get?CustomerId=" + CustomerId));

            VMTblCustomer data = JsonConvert.DeserializeObject<VMTblCustomer>(apiResponse.entity.ToString());

            if (data == null || apiResponse.Success == false)
            {
                string errorMag = apiResponse.message;
                return RedirectToAction("Index");
            }

            ViewBag.Title = "Detail Customer";
            return View(data);
        }

        public async Task<IActionResult> Delete(int CustomerId)
        {
            VMResponse apiResponse = JsonConvert.DeserializeObject<VMResponse>(
                                    await httpClient.GetStringAsync(apiUrl + "api/Customer/Get?CustomerId=" + CustomerId));

            VMTblCustomer data = JsonConvert.DeserializeObject<VMTblCustomer>(apiResponse.entity.ToString());
            if (data == null || apiResponse.Success == false)
            {
                string errorMag = apiResponse.message;
                return RedirectToAction("Index");
            }

            ViewBag.Title = "Delete Customer";
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(VMTblCustomer dataView)
        {
            VMResponse apiResponse = JsonConvert.DeserializeObject<VMResponse>(await (
                                    await httpClient.DeleteAsync(apiUrl + "api/Customer/Delete?customerId=" + dataView.Id + "&updateBy="+ dataView.UpdateBy)
                              ).Content.ReadAsStringAsync());

            if (!apiResponse.Success)
            {
                string errorMag = apiResponse.message;
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int CustomerId)
        {
            VMResponse apiResponse = JsonConvert.DeserializeObject<VMResponse>(
                                    await httpClient.GetStringAsync(apiUrl + "api/Customer/Get?CustomerId=" + CustomerId));

            VMTblCustomer data = JsonConvert.DeserializeObject<VMTblCustomer>(apiResponse.entity.ToString());

            if (data == null || apiResponse.Success == false)
            {
                string errorMag = apiResponse.message;
                return RedirectToAction("Index");
            }

            ViewBag.Title = "Edit Customer";
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(VMTblCustomer dataView)
        {
            string jsonData = JsonConvert.SerializeObject(dataView);
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            VMResponse apiResponse = JsonConvert.DeserializeObject<VMResponse>(await (
                                    await httpClient.PutAsync(apiUrl + "api/Customer/Edit", content)
                              ).Content.ReadAsStringAsync());
            if (!apiResponse.Success)
            {
                string errorMag = apiResponse.message;
            }

            return RedirectToAction("Index");
        }
    }
}
