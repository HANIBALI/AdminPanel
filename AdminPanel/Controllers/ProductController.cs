using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AdminPanel.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace AdminPanel.Controllers
{
    public class ProductController : Controller
    {
        //[HttpGet]
        //public IActionResult Index()
        //{
        //    var client = new RestClient("https://localhost:44303/Product/SelectProduct");
        //    client.Timeout = -1;
        //    var request = new RestRequest(Method.POST);
        //    IRestResponse response = client.Execute(request);
        //    var result = JsonConvert.DeserializeObject<List<products>>(response.Content);
        //    return View(result);
        //}
        [HttpGet]
        public IActionResult Index(string barcode)
        {
            try
            {
                if (barcode == null)
                {
                    var client2 = new RestClient("https://localhost:44303/Product/SelectProduct");
                    client2.Timeout = -1;
                    var request2 = new RestRequest(Method.POST);
                    IRestResponse response2 = client2.Execute(request2);
                    var result2 = JsonConvert.DeserializeObject<List<products>>(response2.Content);
                    return View(result2);
                }
                //პროდუქტის წაშლა
                else
                {
                    var client = new RestClient("https://localhost:44303/Product/product_remove");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "text/plain");
                    request.AddParameter("text/plain", "[\r\n    {\r\n        \"barcode\": \"" + barcode + "\"\r\n    }\r\n]", ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var errorResult = JObject.Parse(response.Content);
                        string errorText = errorResult.GetValue("message").ToString();
                        throw new Exception(errorText);
                    }
                    //თავიდან ჩატვირთვა
                    var client1 = new RestClient("https://localhost:44303/Product/SelectProduct");
                    client1.Timeout = -1;
                    var request1 = new RestRequest(Method.POST);
                    IRestResponse response1 = client1.Execute(request1);
                    var result = JsonConvert.DeserializeObject<List<products>>(response1.Content);
                    return View(result);
                }
               
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
                
            }
        }

        public products get_product_info_by_barcode(string barcode)
        {
            var client = new RestClient("https://localhost:44303/Product/GetByBarcodeForAdmin");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AlwaysMultipartFormData = true;
            request.AddParameter("barcode", barcode);
            IRestResponse response = client.Execute(request);
            //სტატუსის შემოწმება
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var errorResult = JObject.Parse(response.Content);
                string errorText = errorResult.GetValue("message").ToString();
                throw new Exception(errorText);
            }

            var product = JsonConvert.DeserializeObject<products>(response.Content);
            return product;
        }
        //პროდუქტის რედაქტირება
        [HttpGet]
        public IActionResult product_edit(string barcode)
        {
            try
            {

                var product = get_product_info_by_barcode(barcode);
                return View(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        //პროდუქტის რედაქტირება
        [HttpPost]
        public IActionResult product_edit(string barcode,string new_barcode,string name,decimal price,string measurement,decimal quantity)
        {
            try
            {
                var client = new RestClient("https://localhost:44303/Product/product_update");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AlwaysMultipartFormData = true;
                request.AddParameter("barcode", barcode);
                request.AddParameter("new_barcode", new_barcode);
                request.AddParameter("name", name);
                request.AddParameter("quantity", quantity);
                request.AddParameter("measurement", measurement);
                request.AddParameter("price", price);
                IRestResponse response = client.Execute(request);
                //სტატუსის შემოწმება
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var errorResult = JObject.Parse(response.Content);
                    string errorText = errorResult.GetValue("message").ToString();
                    throw new Exception(errorText);
                }
                else
                {
                    var result_json = JObject.Parse(response.Content);
                    string result_text = result_json.GetValue("message").ToString();
                    ViewBag.editMessage = result_text;
                    var product = get_product_info_by_barcode(new_barcode);
                    return View(product);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        //პროდუქტის დამატება
        [HttpGet]
        public IActionResult product_add()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        //პროდუქტის დამატება
        [HttpPost]
        public IActionResult product_add(string barcode,string name, decimal price, string measurement, decimal quantity)
        {
            try
            {
                var client = new RestClient("https://localhost:44303/Product/product_add");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "text/plain");
                request.AddParameter("text/plain", "[\r\n    {\r\n        " +
                    "\"barcode\":\""+barcode+"\",\r\n        " +
                    "\"name\":\"" + name + "\",\r\n        " +
                    "\"price\":\"" + price + "\",\r\n        " +
                    "\"measurement\":\"" + measurement + "\",\r\n        " +
                    "\"quantity\":\"" + quantity + "\"\r\n    }\r\n]", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                //სტატუსის შემოწმება
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var errorResult = JObject.Parse(response.Content);
                    string errorText = errorResult.GetValue("message").ToString();
                    throw new Exception(errorText);
                }
                else
                {
                    var result_json = JObject.Parse(response.Content);
                    string result_text = result_json.GetValue("message").ToString();
                    ViewBag.addMessage = result_text;
                    return View();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

    }
}
