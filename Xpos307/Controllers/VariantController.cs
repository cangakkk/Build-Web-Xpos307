using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xpos307.ViewModels;

namespace Xpos307.Controllers
{
    public class VariantController : Controller
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static string apiUrl;
        public VariantController(IConfiguration _config)
        {
            apiUrl = _config["ApiUrl"];
        }

        public async Task<IActionResult> Index(VMPage page)
        {
            VMResponse apiResponse = new VMResponse();

            if (string.IsNullOrEmpty(page.Name))
                apiResponse = JsonConvert.DeserializeObject<VMResponse>(await httpClient.GetStringAsync(apiUrl + "api/Variant/GetAll"));
            else
                apiResponse = JsonConvert.DeserializeObject<VMResponse>(await httpClient.GetStringAsync(apiUrl + "api/Variant/GetByName?Name=" + page.Name));

            List<VMTblVariant> data = JsonConvert.DeserializeObject<List<VMTblVariant>>(apiResponse.entity.ToString());

            if (data == null || apiResponse.Success == false)
            {
                string errorMag = apiResponse.message;
                return RedirectToAction("Index");
            }

            //Get Short Order
            ViewBag.OrderId = string.IsNullOrEmpty(page.orderBy) ? "id_desc" : "";
            ViewBag.OrderName = page.orderBy == "name" ? "name_desc" : "name";
            ViewBag.OrderCat = page.orderBy == "category" ? "category_desc" : "category";
            ViewBag.OrderDesc = page.orderBy == "decription" ? "decription_desc" : "decription";

            switch (page.orderBy)
            {
                case "name_desc":
                    data = data.OrderByDescending(p => p.Name).ToList();
                    break;
                case "name":
                    data = data.OrderBy(p => p.Name).ToList();
                    break;
                case "category_desc":
                    data = data.OrderByDescending(p => p.CategoryName).ToList();
                    break;
                case "category":
                    data = data.OrderBy(p => p.CategoryName).ToList();
                    break;
                case "decription_desc":
                    data = data.OrderByDescending(p => p.Description).ToList();
                    break;
                case "decription":
                    data = data.OrderBy(p => p.Description).ToList();
                    break;
                case "id_desc":
                    data = data.OrderByDescending(p => p.Id).ToList();
                    break;
                default:
                    data = data.OrderBy(p => p.Id).ToList();
                    break;
            }

            ViewBag.Name = page.Name;

            ViewBag.Title = "Variant Index";
            return View(data);
        }

        public async Task<IActionResult> Add()
        {
            VMResponse apiResponse = JsonConvert.DeserializeObject<VMResponse>(await httpClient.GetStringAsync(apiUrl + "api/Category/GetAll"));

            ViewBag.category = JsonConvert.DeserializeObject<List<VMTblCategory>>(apiResponse.entity.ToString());
            ViewBag.Title = "Add Category";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(VMTblVariant dataView)
        {
            string jsonData = JsonConvert.SerializeObject(dataView);

            HttpContent content = new StringContent(jsonData, UnicodeEncoding.UTF8, "application/json");

            var apiResponse = JsonConvert.DeserializeObject<VMResponse>(await (
                                    await httpClient.PostAsync(apiUrl + "api/Variant/Add", content)
                              ).Content.ReadAsStringAsync());

            if (!apiResponse.Success)
            {
                string errorMag = apiResponse.message;
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Delete(int variantId)
        {
            VMResponse apiResponse = JsonConvert.DeserializeObject<VMResponse>(
                                    await httpClient.GetStringAsync(apiUrl + "api/Variant/Get?VariantID=" + variantId));

            VMTblVariant data = JsonConvert.DeserializeObject<VMTblVariant>(apiResponse.entity.ToString());
            if (data == null || apiResponse.Success == false)
            {
                string errorMag = apiResponse.message;
                return RedirectToAction("Index");
            }

            ViewBag.Title = "Delete Variant";
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(VMTblVariant dataView)
        {

            var apiResponse = JsonConvert.DeserializeObject<VMResponse>(await (
                                    await httpClient.DeleteAsync(apiUrl + "api/Variant/Delete?variantId=" + dataView.Id + "&updateBy=" + dataView.UpdateBy)
                              ).Content.ReadAsStringAsync());

            if (!apiResponse.Success)
            {
                string errorMag = apiResponse.message;
            }

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Detail(int variantId)
        {
            VMResponse apiResponse = JsonConvert.DeserializeObject<VMResponse>(
                                    await httpClient.GetStringAsync(apiUrl + "api/Variant/Get?VariantID=" + variantId));

            VMTblVariant data = JsonConvert.DeserializeObject<VMTblVariant>(apiResponse.entity.ToString());

            if (data == null || apiResponse.Success == false)
            {
                string errorMag = apiResponse.message;
                return RedirectToAction("Index");
            }

            ViewBag.Title = "Detail Variant";
            return View(data);
        }

        public async Task<IActionResult> Edit(int variantId)
        {
            VMResponse apiVariantResponse = JsonConvert.DeserializeObject<VMResponse>(await httpClient.GetStringAsync(apiUrl + "api/Variant/Get?variantId=" + variantId));
            VMResponse apiCategoryResponse = JsonConvert.DeserializeObject<VMResponse>(await httpClient.GetStringAsync(apiUrl + "api/Category/GetAll"));

            if (apiVariantResponse == null || apiCategoryResponse == null)
            {
                string errMsg = "No Response from API!";
                return RedirectToAction("Index");
            }

            VMTblVariant data = JsonConvert.DeserializeObject<VMTblVariant>(apiVariantResponse.entity.ToString());
            ViewBag.category = JsonConvert.DeserializeObject<List<VMTblCategory>>(apiCategoryResponse.entity.ToString());

            return View(data);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(VMTblVariant dataView)
        {
            string jsonData = JsonConvert.SerializeObject(dataView);
            HttpContent content = new StringContent(jsonData, UnicodeEncoding.UTF8, "application/json");
            VMResponse apiResponse = JsonConvert.DeserializeObject<VMResponse>(await (
                                    await httpClient.PutAsync(apiUrl + "api/Variant/Edit", content)
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
