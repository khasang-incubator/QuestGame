﻿using System.Web.Mvc;
using QuestGame.WebApi.Models;
using System.Threading.Tasks;
using Serilog;
using System.Net.Http;
using QuestGame.Domain.Implementations;
using QuestGame.Domain.Interfaces;
using QuestGame.WebApi.Models.UserViewModels;
using AutoMapper;
using System.Collections.Generic;

namespace QuestGame.WebApi.Controllers
{
    public class RegisterController : Controller
    {
        ILogger myLogger = null;

        public RegisterController()
        {
            myLogger = new LoggerConfiguration()
            .WriteTo.RollingFile("myapp-Log.txt")
            .CreateLogger();
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Страница регистрации";

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index( UserRegisterVM  user )
        {
            ViewBag.Title = "Регистрация нового пользователя";

            if (!ModelState.IsValid)
            {
                return View();
            }

            user.UserName = user.Email;

            IRequest client = new DirectRequest();
            var response = await client.PostRequestAsync(@"api/Account/Register", user);

            var result = await response.Content.ReadAsAsync<IEnumerable<string>>();

            if (response.IsSuccessStatusCode)
                {
                    ViewBag.Message = "Успешная регистрация";
                }
                else
                {
                    ViewBag.Message = "Что-то пошло не так";
                }

            return View("Index", user);  // Вставить страницу на проиль нового пользователя
        }
    }
}
