﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using QuestGame.WebApi.Models;
using System.Net.Http.Headers;


namespace QuestGame.WebApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult Login()
        {
            ViewBag.Message = "Авторизация";
            return View();
        }

        [HttpPost]
        public ActionResult Login( UserLogin user )
        {

            if (!ModelState.IsValid)
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:9243");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = client.PostAsJsonAsync(@"/Token", user).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.Message = "Авторизация пройдена";
                    }
                    else
                    {
                        ViewBag.Message = "Не авторизирован";
                    }
                }

                ViewBag.Message = "Данные не верны";
            }

            return View();
        }
    }
}
