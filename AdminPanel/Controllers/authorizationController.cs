using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace AdminPanel.Controllers
{
    public class authorizationController : Controller
    {
        public IActionResult login()
        {
            return View();
        }

        [HttpPost]
        public  IActionResult login(string pin, string password)
        {
            
            var client = new RestClient("https://localhost:44303/Product/admin_authorization?pin="+pin+"&password="+password);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AlwaysMultipartFormData = true;
            IRestResponse response = client.Execute(request);
            var responsee = response.Content;
            if (response.StatusCode==HttpStatusCode.OK)
            {
                HttpContext.Session.SetString("user", "admin");
                HttpContext.Session.SetString("pin", pin);
                HttpContext.Session.SetString("password", password);
                return RedirectToAction("Index", "Product");
            }
            else
            {
                ViewBag.login_text = "pin or password is incorect!";
                return View();
            }
            

        }
        public IActionResult logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("login", "authorization");
        }
    }
}
