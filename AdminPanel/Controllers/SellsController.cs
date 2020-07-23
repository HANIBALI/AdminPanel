using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminPanel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace AdminPanel.Controllers
{
    public class SellsController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("user") == "admin")
            {
                var client = new RestClient("https://possystem.conveyor.cloud/Product/SelectFromSales");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                IRestResponse response = client.Execute(request);
                var result = JsonConvert.DeserializeObject<List<Sells>>(response.Content);
                //return Ok(response.Content);
                return View(result);
            }
            else
            {
                return RedirectToAction("login", "authorization");
            }
        }
    }
}
