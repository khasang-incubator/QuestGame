﻿using QuestGame.Common.Helpers;
using QuestGame.WebMVC.Constants;
using QuestGame.WebMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using QuestGame.Domain.DTO;


using AutoMapper;

using System.Web.Configuration;

namespace QuestGame.WebMVC.Controllers
{
    public class AccountController : Controller
    {
        IMapper mapper;

        public AccountController(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public ActionResult Register()
        {
            var model = new RegisterViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                View(model);
            }

            using (var client = RestHelper.Create())
            {
                var response = await client.PostAsJsonAsync(ApiMethods.AccontRegister, model);
                var answer = await response.Content.ReadAsAsync<RegisterResponse>();

                if (answer.Success)
                {
                    ViewBag.ErrorMessage = ErrorMessages.AccountSuccessRegister;
                }
                else
                {
                    ViewBag.ErrorMessage = ErrorMessages.AccountFailRegister;
                    return RedirectToAction("Index", "Home");
                }

                return RedirectToAction("SendEmail");
            }
        }

        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                View(model);
            }

            using (var client = RestHelper.Create())
            {
                var response = await client.PostAsJsonAsync(ApiMethods.AccountLogin, model);

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    ViewBag.ErrorMessage = ErrorMessages.AccountFailLogin;
                    return View();
                }

                var answer = await response.Content.ReadAsAsync<ApplicationUserDTO>();

                //Записать пользователя в сесию
                Session["User"] = answer;

                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult LogOff()
        {
            Session["User"] = null;
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<ActionResult> UserProfile(string id)
        {
            var currentUser = Session["User"] as ApplicationUserDTO;


            using (var client = RestHelper.Create())
            {
                var response = await client.GetAsync(ApiMethods.AccontUser + id);
                var answer = await response.Content.ReadAsAsync<ApplicationUserDTO>();

                var model = mapper.Map<ApplicationUserDTO, UserViewModel>(answer);

                model.UserProfile.avatarUrl = "http://vignette3.wikia.nocookie.net/shokugekinosoma/images/6/60/No_Image_Available.png/revision/latest?cb=20150708082716";

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserProfileEdit(UserViewModel model)
        {
            var user = mapper.Map<UserViewModel, ApplicationUserDTO>(model);

            var currentUser = Session["User"] as ApplicationUserDTO;

            using (var client = RestHelper.Create(currentUser.Token))
            {
                var response = await client.PostAsJsonAsync(ApiMethods.AccontEditUser, user);

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    ViewBag.ErrorMessage = "Неудачная попытка редактирования!";
                }

                return RedirectToAction("UserProfile", new { id = model.Id });
            }
        }


        [HttpGet]
        public async Task<ActionResult> SendEmail(string id)
        {
            string emailToken;

            using (var client = RestHelper.Create())
            {
                var response = await client.GetAsync(@"/api/Account/GetEmailToken?id=" + id);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Home");
                }

                emailToken = await response.Content.ReadAsAsync<string>();
            }

            var param = new Dictionary<string, string>();

            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = id, code = HttpUtility.UrlEncode(emailToken) }, protocol: Request.Url.Scheme);

            var emailBody = "Для завершения регистрации перейдите по ссылке:: <a href=\"" + callbackUrl + "\">завершить регистрацию</a>";

            param.Add("userId", id);
            param.Add("subject", "Подтверждение email");
            param.Add("body", emailBody);

            using (var client = RestHelper.Create())
            {
                var response = await client.PostAsJsonAsync(@"api/Account/SendEmailToken", param);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Message = "На почту отправлено сообщение";

            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            using (var client = RestHelper.Create())
            {
                var response = await client.GetAsync(@"/api/Account/ConfirmEmail?id=" + userId + "&code=" + code);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        ViewBag.Message = "Пользователь не найден";
                        break;
                    case HttpStatusCode.BadRequest:
                        ViewBag.Message = "Данные не корректны";
                        break;
                    case HttpStatusCode.InternalServerError:
                        ViewBag.Message = "Возникла ошибка. Попробуйте еще раз";
                        break;
                    case HttpStatusCode.OK:
                        var currentUser = Session["User"] as ApplicationUserDTO;
                        if (currentUser != null)
                        {
                            currentUser.EmailConfirmed = true;
                        }                        
                        ViewBag.Message = "Email успешно подтвержден!";
                        break;
                }

                return View();
            }
        }
    }
}