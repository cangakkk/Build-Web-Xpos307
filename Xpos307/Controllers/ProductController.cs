using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xpos307.AddOns;
using Xpos307.ViewModels;

namespace Xpos307.Controllers
{
    public class ProductController : Controller
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static string apiUrl;
        private readonly string imgFolder;
        private readonly IWebHostEnvironment webHostEnvironment;
        private int pageSize;

        public ProductController(IConfiguration _config, IWebHostEnvironment _webHostEnvironment)
        {
            apiUrl = _config["ApiUrl"];
            imgFolder = _config["ImangeFolder"];
            webHostEnvironment = _webHostEnvironment;
            pageSize = int.Parse(_config["PageSize"]);
        }

        public string uploadFile(IFormFile file)
        {
            string uniqFileName = null;

            if(file != null)
            {
                uniqFileName = Guid.NewGuid().ToString() + "-" + file.FileName;
                string uploadFolder = webHostEnvironment.WebRootPath + "\\" + imgFolder+ "\\" +uniqFileName;

                using (FileStream filestream = new FileStream(uploadFolder,FileMode.Create))
                {
                    file.CopyTo(filestream);
                }
            }
            return uniqFileName;
        }

        public async Task<IActionResult> Index(VMPage page)
        {
            VMResponse apiResponse = new VMResponse();

            if(string.IsNullOrEmpty(page.Name))
                 apiResponse = JsonConvert.DeserializeObject<VMResponse>(await httpClient.GetStringAsync(apiUrl + "api/Product/GetAll"));
            else
                 apiResponse = JsonConvert.DeserializeObject<VMResponse>(await httpClient.GetStringAsync(apiUrl + "api/Product/GetByName?Name="+page.Name));

            List<VMTblProduct> data = JsonConvert.DeserializeObject<List<VMTblProduct>>(apiResponse.entity.ToString());

            if (data == null || apiResponse.Success == false)
            {
                string errorMag = apiResponse.message;
                return RedirectToAction("Index");
            }

            //Get Short Order
            ViewBag.OrderId = string.IsNullOrEmpty(page.orderBy) ? "id_desc" : "";
            ViewBag.OrderName = page.orderBy == "name" ? "name_desc" : "name";
            ViewBag.OrderVar = page.orderBy == "variant" ? "variant_desc" : "variant";
            ViewBag.OrderCat = page.orderBy == "category" ? "category_desc" : "category";

            switch (page.orderBy)
            {
                case "name_desc":
                    data = data.OrderByDescending(p => p.Name).ToList();
                    break;
                case "name":
                    data = data.OrderBy(p => p.Name).ToList();
                    break;
                case "variant_desc":
                    data = data.OrderByDescending(p => p.VariantName).ToList();
                    break;
                case "variant":
                    data = data.OrderBy(p => p.VariantName).ToList();
                    break;
                case "category_desc":
                    data = data.OrderByDescending(p => p.CategoryName).ToList();
                    break;
                case "category":
                    data = data.OrderBy(p => p.CategoryName).ToList();
                    break;
                case "id_desc":
                    data = data.OrderByDescending(p => p.Id).ToList();
                    break;
                default:
                    data = data.OrderBy(p => p.Id).ToList();
                    break;
            }

            ViewBag.Title = "Product Index";
            ViewBag.Name = page.Name;
            ViewBag.Showdata = page.showData;

            int pgSize = page.showData ?? pageSize;

            return View(Pagination<VMTblProduct>.CreateAsync(data, page.pageNumber ?? 1, pgSize));

            //return View(data);
        }

        public async Task<IActionResult> Add()
        {
            VMResponse apiCategoryResponse = JsonConvert.DeserializeObject<VMResponse>(await httpClient.GetStringAsync(apiUrl + "api/Category/GetAll"));
            VMResponse apiVariantResponse = JsonConvert.DeserializeObject<VMResponse>(await httpClient.GetStringAsync(apiUrl + "api/Variant/GetAll"));

            ViewBag.category = JsonConvert.DeserializeObject<List<VMTblCategory>>(apiCategoryResponse.entity.ToString());
            ViewBag.variant = JsonConvert.DeserializeObject<List<VMTblVariant>>(apiVariantResponse.entity.ToString());

            ViewBag.ApiUrl = apiUrl + "api/Variant/GetByCategory";

            ViewBag.Title = "Product Add";

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(VMTblProduct dataView)
        {
            //Upload Image File 
            dataView.Image = uploadFile(dataView.ImageFile);
            //dataView.Images = uploadedFilename;
            dataView.ImageFile = null;

            //mempersiapkan data json untuk kirim API
            string jsonData = JsonConvert.SerializeObject(dataView);
            HttpContent content= new StringContent(jsonData, Encoding.UTF8, "application/json");
            var apiResponse = JsonConvert.DeserializeObject<VMResponse>(await (
                                   await httpClient.PostAsync(apiUrl + "api/Product/Add", content)
                             ).Content.ReadAsStringAsync());
            if (!apiResponse.Success)
            {
                string errorMag = apiResponse.message;
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail(int ProductID)
        {
            VMResponse apiResponse = JsonConvert.DeserializeObject<VMResponse>(
                                    await httpClient.GetStringAsync(apiUrl + "api/Product/Get?ProductID=" + ProductID));

            VMTblProduct data = JsonConvert.DeserializeObject<VMTblProduct>(apiResponse.entity.ToString());

            if (data == null || apiResponse.Success == false)
            {
                string errorMag = apiResponse.message;
                return RedirectToAction("Index");
            }

            ViewBag.Title = "Detail Variant";
            return View(data);
        }

        public async Task<IActionResult> Delete(int productID)
        {
            VMResponse apiResponse = JsonConvert.DeserializeObject<VMResponse>(
                                    await httpClient.GetStringAsync(apiUrl + "api/Product/Get?productId=" + productID));

            VMTblProduct data = JsonConvert.DeserializeObject<VMTblProduct>(apiResponse.entity.ToString());

            if (data == null || apiResponse.Success == false)
            {
                string errorMag = apiResponse.message;
                return RedirectToAction("Index");
            }

            ViewBag.Title = "Delete Variant";
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(VMTblProduct dataView)
        {

            VMResponse apiResponse = JsonConvert.DeserializeObject<VMResponse>(await (
                                    await httpClient.DeleteAsync(apiUrl + "api/Product/Delete?productId=" + dataView.Id + "&updateBy=" + dataView.UpdateBy)
                              ).Content.ReadAsStringAsync());

            if (!apiResponse.Success)
            {
                string errorMag = apiResponse.message;
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int ProductID)
        {
            VMResponse apiCategoryResponse = JsonConvert.DeserializeObject<VMResponse>(await httpClient.GetStringAsync(apiUrl + "api/Category/GetAll"));
          

            VMResponse apiResponse = JsonConvert.DeserializeObject<VMResponse>(
                                    await httpClient.GetStringAsync(apiUrl + "api/Product/Get?ProductID=" + ProductID));

            VMTblProduct data = JsonConvert.DeserializeObject<VMTblProduct>(apiResponse.entity.ToString());

            if (data == null || apiResponse.Success == false)
            {
                string errorMag = apiResponse.message;
                return RedirectToAction("Index");
            }

            ViewBag.category = JsonConvert.DeserializeObject<List<VMTblCategory>>(apiCategoryResponse.entity.ToString());

            ViewBag.ApiUrl = apiUrl + "api/Variant/GetByCategory";

            ViewBag.Title = "Detail Variant";
            return View(data);

        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(VMTblProduct dataView)
        {
            //Upload Image File 
            dataView.Image = uploadFile(dataView.ImageFile);
            //dataView.Images = uploadedFilename;
            dataView.ImageFile = null;

            string jsonData = JsonConvert.SerializeObject(dataView);
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var apiResponse = JsonConvert.DeserializeObject<VMResponse>(await (
                                   await httpClient.PutAsync(apiUrl + "api/Product/Edit", content)
                             ).Content.ReadAsStringAsync());

            if (!apiResponse.Success)
            {
                string errorMag = apiResponse.message;
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}
