﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Security;
//using System.Web.Http;
using System.Web.Mvc;
using QuestGame.WebApi.Models;
using System.Text;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace QuestGame.WebApi.Controllers
{
    public class RegisterController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Страница регистрации";

            return View( new UserInvite() );
        }

        [HttpPost]
        public async Task<ActionResult> CreateUser( UserInvite user )
        {
            //string parsedContent = Newtonsoft.Json.JsonConvert.SerializeObject(user);
 
            using ( var client = new HttpClient() )
            {
                client.BaseAddress = new Uri("http://localhost:9243");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.PostAsJsonAsync(@"api/Account/Register", user).Result;

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Message = "Успешная регистрация";
                }
                else
                {
                    ViewBag.Message = "Что-то пошло не так";
                }
            }

            return View();
        }
    }
}
