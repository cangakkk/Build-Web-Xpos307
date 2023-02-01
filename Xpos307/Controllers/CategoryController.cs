using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xpos307.ViewModels;

namespace Xpos307.Controllers
{
    public class CategoryController : Controller
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static string apiUrl;
        //private IConfiguration config;

        public CategoryController(IConfiguration _config)
        {
            //config= _config;
            apiUrl = _config["ApiUrl"]; 
        }

        public async Task<IActionResult> Index(VMPage page)
        {
            VMResponse apiResponse = new VMResponse();
            if (string.IsNullOrEmpty(page.Name))
                    apiResponse = JsonConvert.DeserializeObject<VMResponse>
                                    (await httpClient.GetStringAsync(apiUrl + "api/Category/GetAll"));
            else
                    apiResponse = JsonConvert.DeserializeObject<VMResponse>
                                    (await httpClient.GetStringAsync(apiUrl + "api/Category/GetByName?Name=" + page.Name));

            List<VMTblCategory> data = JsonConvert.DeserializeObject<List<VMTblCategory>>
                                        (apiResponse.entity.ToString());

            if (data == null || apiResponse.Success == false)
            {
                string errorMag = apiResponse.message;
                return RedirectToAction("Index");
            }

            //Get Short Order
            ViewBag.OrderId = string.IsNullOrEmpty(page.orderBy) ? "id_desc" : "";
            ViewBag.OrderName = page.orderBy == "name" ? "name_desc" : "name";
            ViewBag.OrderDesc = page.orderBy == "decription" ? "decription_desc" : "decription";

            switch (page.orderBy)
            {
                case "name_desc":
                    data = data.OrderByDescending(p=>p.Name).ToList();
                    break;
                case "name":
                    data = data.OrderBy(p=>p.Name).ToList();
                    break;
                case "decription_desc":
                    data = data.OrderByDescending(p=>p.Description).ToList();
                    break;
                case "decription":
                    data = data.OrderBy(p => p.Description).ToList();
                    break;
                case "id_desc":
                    data = data.OrderByDescending(p => p.Id).ToList();
                    break;
                default : 
                    data =data.OrderBy(p => p.Id).ToList();
                    break;
            }

            ViewBag.Name = page.Name;
            ViewBag.Title = "Category Index";

            return View(data);
        }

        public IActionResult Add()
        {
            VMTblCategory dataView = new VMTblCategory();

            ViewBag.Title = "Add Category";
            return View(dataView);
        }

        [HttpPost]
        public async Task<IActionResult> Add(VMTblCategory dataView)
        {
            string jsonData = JsonConvert.SerializeObject(dataView);

            HttpContent content = new StringContent(jsonData, UnicodeEncoding.UTF8, "application/json");

            VMResponse apiResponse = JsonConvert.DeserializeObject<VMResponse>(await (
                                    await httpClient.PostAsync(apiUrl + "api/Category/Add",content)
                              ).Content.ReadAsStringAsync());

            if (!apiResponse.Success)
            {
                string errorMag = apiResponse.message;
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Edit(int categoryId)
        {
            VMResponse apiResponse = JsonConvert.DeserializeObject<VMResponse>(
                                    await httpClient.GetStringAsync(apiUrl + "api/Category/Get?CategoryID=" + categoryId));

            VMTblCategory data = JsonConvert.DeserializeObject<VMTblCategory>(apiResponse.entity.ToString());

            if (data == null || apiResponse.Success == false )
            {
                string errorMag = apiResponse.message;
                return RedirectToAction("Index");
            }

            ViewBag.Title = "Edit Category";
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(VMTblCategory dataView)
        {
            string jsonData = JsonConvert.SerializeObject(dataView);
            HttpContent content = new StringContent(jsonData, UnicodeEncoding.UTF8, "application/json");
            VMResponse apiResponse = JsonConvert.DeserializeObject<VMResponse>(await (
                                    await httpClient.PutAsync(apiUrl + "api/Category/Edit", content)
                              ).Content.ReadAsStringAsync());
            if (!apiResponse.Success)
            {
                string errorMag = apiResponse.message;
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int categoryId)
        {
            VMResponse apiResponse = JsonConvert.DeserializeObject<VMResponse>(
                                    await httpClient.GetStringAsync(apiUrl + "api/Category/Get?CategoryID=" + categoryId));

            VMTblCategory data = JsonConvert.DeserializeObject<VMTblCategory>(apiResponse.entity.ToString());
            if (data == null || apiResponse.Success == false)
            {
                string errorMag = apiResponse.message;
                return RedirectToAction("Index");
            }

            ViewBag.Title = "Delete Category";
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(VMTblCategory dataView)
        {

            VMResponse apiResponse = JsonConvert.DeserializeObject<VMResponse>(await (
                                    await httpClient.DeleteAsync(apiUrl + "api/Category/Delete?categoryId=" + dataView.Id)
                              ).Content.ReadAsStringAsync());

            if (!apiResponse.Success)
            {
                string errorMag = apiResponse.message;
            }

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Detail(int categoryId)
        {
            VMResponse apiResponse = JsonConvert.DeserializeObject<VMResponse>(
                                    await httpClient.GetStringAsync(apiUrl + "api/Category/Get?CategoryID=" + categoryId));

            VMTblCategory data = JsonConvert.DeserializeObject<VMTblCategory>(apiResponse.entity.ToString());

            if (data == null || apiResponse.Success == false)
            {
                string errorMag = apiResponse.message;
                return RedirectToAction("Index");
            }

            ViewBag.Title = "Detail Category";
            return View(data);
        }


    }
}
