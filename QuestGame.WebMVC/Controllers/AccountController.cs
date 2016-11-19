﻿using QuestGame.Common.Helpers;
using QuestGame.WebMVC.Constants;
using QuestGame.WebMVC.Models;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using QuestGame.Domain.DTO;
using AutoMapper;
using QuestGame.WebMVC.Attributes;

namespace QuestGame.WebMVC.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(IMapper mapper)
            : base(mapper)
        { }

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

                return RedirectToAction("SendEmail", new { id = answer.Body });
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

        [CustomAuthorize]
        [HTTPExceptionAttribute]
        [HttpGet]
        public async Task<ActionResult> UserProfile(string id)
        {
            using (var client = RestHelper.Create())
            {
                var response = await client.GetAsync(ApiMethods.AccontUserById + id);
                response.EnsureSuccessStatusCode();

                var answer = await response.Content.ReadAsAsync<ApplicationUserDTO>();
                var model = mapper.Map<ApplicationUserDTO, UserViewModel>(answer);

                return View(model);
            }
        }

        [CustomAuthorize]
        [HttpPost]
        [HTTPExceptionAttribute]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserProfileEdit(UserViewModel model, HttpPostedFileBase file)
        {
            var user = mapper.Map<UserViewModel, ApplicationUserDTO>(model);
            var filePath = await UploadFile(file, ApiMethods.AccountUploadFile);
            if (!string.IsNullOrEmpty(filePath))
                user.UserProfile.AvatarUrl = filePath;

            using (var client = RestHelper.Create(SessionUser.Token))
            {
                var response = await client.PostAsJsonAsync(ApiMethods.AccontEditUser, user);
                response.EnsureSuccessStatusCode();

                return RedirectToAction("UserProfile", new { id = model.Id });
            }
        }

        [HttpGet]
        [HTTPExceptionAttribute]
        public async Task<ActionResult> SendEmail(string id)
        {
            string emailToken;

            // Получить токен
            using (var client = RestHelper.Create())
            {
                var response = await client.GetAsync(ApiMethods.AccontEmailToken + id);
                response.EnsureSuccessStatusCode();

                emailToken = await response.Content.ReadAsAsync<string>();
            }

            // Подготовить письмо
            var email = new ConfirmEmailTemplate
            {
                ActionUrl = Url.Action("ConfirmEmail", "Account", new { userId = id, code = HttpUtility.UrlEncode(emailToken) }, protocol: Request.Url.Scheme)
            };

            using (var client = RestHelper.Create())
            {
                var param = new Dictionary<string, string>
                {
                    { "userId", id },
                    { "subject", InfoMessages.EmailConfirmTitle },
                    { "body", email.TransformText() }
                };

                var response = await client.PostAsJsonAsync(ApiMethods.AccontSendEmailToken, param);
                response.EnsureSuccessStatusCode();
            }

            ViewBag.Title = InfoMessages.EmailConfirmTitle;
            ViewBag.Message = InfoMessages.EmailSendConfirm;

            return View("ActionResultInfo");
        }
        
        [HttpGet]
        [HTTPExceptionAttribute]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            using (var client = RestHelper.Create())
            {
                var response = await client.GetAsync(ApiMethods.AccontConfirmEmail + userId + "&code=" + code);
                response.EnsureSuccessStatusCode();

                ViewBag.Title = InfoMessages.EmailConfirmTitle;
                ViewBag.Message = InfoMessages.EmailConfirmAccepted;

                return View("ActionResultInfo");
            }
        }

        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [HTTPExceptionAttribute]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var client = RestHelper.Create())
            {
                var response = await client.GetAsync(ApiMethods.AccontUserByEmail + model.Email);
                response.EnsureSuccessStatusCode();
                var user = await response.Content.ReadAsAsync<ApplicationUserDTO>();

                response = await client.GetAsync(ApiMethods.AccontResetToken + user.Id);
                response.EnsureSuccessStatusCode();
                var code = await response.Content.ReadAsAsync<string>();

                // Подготовить письмо
                var email = new PasswordResetTemplate
                {
                    ActionUrl = Url.Action("NewPassword", "Account", new { userId = user.Id, code = HttpUtility.UrlEncode(code) }, protocol: Request.Url.Scheme)
                };

                var param = new Dictionary<string, string>
                {
                    { "userId", user.Id },
                    { "subject", InfoMessages.PasswordResetTitle },
                    { "body", email.TransformText() }
                };

                response = await client.PostAsJsonAsync(ApiMethods.AccontSendResetToken, param);
                response.EnsureSuccessStatusCode();
            }

            ViewBag.Title = InfoMessages.PasswordResetTitle;
            ViewBag.Message = InfoMessages.PasswordSendToken;
            return View("ActionResultInfo");
        }

        [HttpGet]
        public ActionResult NewPassword(string userId, string code)
        {
            if (userId == null || string.IsNullOrWhiteSpace(code))
            {
                ViewBag.Title = InfoMessages.PasswordResetTitle;
                ViewBag.Message = ErrorMessages.AccountUserNotFound;
                return View("ActionResultInfo");
            }

            var model = new ResetPasswordRequestModel
            {
                Id = userId,
                ResetToken = HttpUtility.UrlDecode(code)
            };

            return View(model);
        }

        [HttpPost]
        [HTTPExceptionAttribute]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NewPassword(ResetPasswordRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = ErrorMessages.InternalServerError;
                return View(model);
            }

            using (var client = RestHelper.Create())
            {
                var modelDTO = mapper.Map<ResetPasswordRequestModel, ResetPasswordDTO>(model);
                var response = await client.PostAsJsonAsync(ApiMethods.AccontResetPassword, modelDTO);
                response.EnsureSuccessStatusCode();
            }

            ViewBag.Title = InfoMessages.PasswordResetTitle;
            ViewBag.Message = InfoMessages.PasswordConfirmTrue;
            return View("ActionResultInfo");
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [HTTPExceptionAttribute]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var param = mapper.Map<ChangePasswordViewModel, ChangePasswordDTO>(model);

            using (var client = RestHelper.Create(SessionUser.Token))
            {
                var response = await client.PostAsJsonAsync(ApiMethods.AccountChangePassword, param);
                response.EnsureSuccessStatusCode();
            }

            ViewBag.Title = InfoMessages.PasswordChangeTitle;
            ViewBag.Message = InfoMessages.PasswordConfirmTrue;
            return View("ActionResultInfo");
        }
    }
}