using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AdminPanel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace AdminPanel.Controllers
{
    public class EmployeeController : Controller
    {
        [HttpGet]
        public IActionResult Index(string Pin)
        {
            try
            {

                if (HttpContext.Session.GetString("user") == "admin")
                {
                    if (Pin == null)
                    {


                        var client = new RestClient("https://possystem.conveyor.cloud/Product/employee_select");
                        client.Timeout = -1;
                        var request = new RestRequest(Method.POST);
                        IRestResponse response = client.Execute(request);
                        var result = JsonConvert.DeserializeObject<List<Employees>>(response.Content);
                        return View(result);
                    }
                    else
                    {
                        var client = new RestClient("https://possystem.conveyor.cloud/Product/employee_remove");
                        client.Timeout = -1;
                        var request = new RestRequest(Method.POST);
                        request.AlwaysMultipartFormData = true;
                        request.AddParameter("owner_pin", HttpContext.Session.GetString("pin"));
                        request.AddParameter("owner_password", HttpContext.Session.GetString("password"));
                        request.AddParameter("pin", Pin);
                        IRestResponse response = client.Execute(request);

                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            var errorResult = JObject.Parse(response.Content);
                            string errorText = errorResult.GetValue("message").ToString();
                            throw new Exception(errorText);
                        }


                        var client1 = new RestClient("https://possystem.conveyor.cloud/Product/employee_select");
                        client1.Timeout = -1;
                        var request1 = new RestRequest(Method.POST);
                        IRestResponse response1 = client1.Execute(request1);
                        var result1 = JsonConvert.DeserializeObject<List<Employees>>(response1.Content);
                        return View(result1);

                    }
                }
                else
                {
                    return RedirectToAction("login", "authorization");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        //თანამშრომლის დამატების გვერდის გახსნა
        [HttpGet]
        public IActionResult employee_add()
        {
            try
            {
                if (HttpContext.Session.GetString("user") == "admin")
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("login", "authorization");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //თანამშრომლის დამატება
        [HttpPost]
        public IActionResult employee_add(string Firstname, string Lastname, string Gender, string Pin, string Password, DateTime BirthDate)
        {
            try
            {
                var BirthDate1 = BirthDate.ToString("yyyy/MM/dd");
                if (HttpContext.Session.GetString("user") == "admin")
                {
                    var client = new RestClient("https://possystem.conveyor.cloud/Product/Cashier_add");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "text/plain");
                    request.AddParameter("text/plain", "[\r\n    {\r\n        " +
                        "\"owner_pin\": \""+ HttpContext.Session.GetString("pin") +"\",\r\n        " +
                        "\"owner_password\": \""+HttpContext.Session.GetString("password") +"\",\r\n        " +
                        "\"firstname\": \""+Firstname+"\",\r\n        \"lastname\": \""+Lastname+"\",\r\n        " +
                        "\"gender\": \""+Gender+"\",\r\n        \"pin\": \""+Pin+"\",\r\n        " +
                        "\"password\": \""+Password+"\",\r\n        " +
                        "\"birthdate\": \""+ BirthDate1 + "\"\r\n    }\r\n]", ParameterType.RequestBody);
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
                else
                {
                    return RedirectToAction("login", "authorization");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //თანამშრომლის რედაქტირების გვერდის ჩატვირთვა
        [HttpGet]
        public IActionResult employee_edit(string Pin)
        {
            try
            {
                if (HttpContext.Session.GetString("user") == "admin")
                {
                    var client = new RestClient("https://possystem.conveyor.cloud/Product/employee_select_by_pin");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AlwaysMultipartFormData = true;
                    request.AddParameter("pin", Pin);
                    IRestResponse response = client.Execute(request);

                    //სტატუსის შემოწმება
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var errorResult = JObject.Parse(response.Content);
                        string errorText = errorResult.GetValue("message").ToString();
                        throw new Exception(errorText);
                    }
                    var product = JsonConvert.DeserializeObject<List<Employees>>(response.Content);
                    //var product =response.Deserialize<Employees>();
                    return View(product[0]);
                }
                else
                {
                    return RedirectToAction("login", "authorization");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //თანამშრომლის რედაქტირება
        [HttpPost]
        public IActionResult employee_edit(string pin, string firstname, string lastname, string gender, string new_pin, string password, DateTime BirthDate)
        {
            try
            {
                if (HttpContext.Session.GetString("user") == "admin")
                {
                    var client = new RestClient("https://possystem.conveyor.cloud/Product/employee_update");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AlwaysMultipartFormData = true;
                    request.AddParameter("owner_pin", HttpContext.Session.GetString("pin"));
                    request.AddParameter("owner_password", HttpContext.Session.GetString("password"));
                    request.AddParameter("pin", pin);
                    request.AddParameter("firstname", firstname);
                    request.AddParameter("lastname", lastname);
                    request.AddParameter("gender", gender);
                    request.AddParameter("new_pin", new_pin);
                    request.AddParameter("password", password);
                    request.AddParameter("birthdate", BirthDate);
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



                        var client1 = new RestClient("https://possystem.conveyor.cloud/Product/employee_select_by_pin");
                        client1.Timeout = -1;
                        var request1 = new RestRequest(Method.POST);
                        request1.AlwaysMultipartFormData = true;
                        request1.AddParameter("pin", new_pin);
                        IRestResponse response1 = client1.Execute(request1);

                        //სტატუსის შემოწმება
                        if (response1.StatusCode != HttpStatusCode.OK)
                        {
                            var errorResult = JObject.Parse(response1.Content);
                            string errorText = errorResult.GetValue("message").ToString();
                            throw new Exception(errorText);
                        }
                        var product = JsonConvert.DeserializeObject<List<Employees>>(response1.Content);
                        //var product =response.Deserialize<Employees>();
                        return View(product[0]);
                        
                    }
                }
                else
                {
                    return RedirectToAction("login", "authorization");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
