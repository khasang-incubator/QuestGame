﻿using AutoMapper;
using QuestGame.Common.Helpers;
using QuestGame.Domain.DTO;
using QuestGame.WebMVC.Attributes;
using QuestGame.WebMVC.Constants;
using QuestGame.WebMVC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace QuestGame.WebMVC.Controllers
{
    [CustomAuthorize]
    public class BaseController : Controller
    {
        protected IMapper mapper;

        public BaseController(IMapper mapper)
        {
            this.mapper = mapper;
        }

        protected ApplicationUserDTO SessionUser
        {
            get
            {
                return Session["User"] as ApplicationUserDTO;
            }
        }

        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        protected ActionResult UploadFile(HttpPostedFileBase file)
        {
            if (file != null)
            {
                string fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/Temp/"), fileName);
                try
                {
                    file.SaveAs(path);         // сохраняем файл в папку Files в проекте
                }
                catch
                {
                    ViewBag.Message = "Ошибка локального сохранения";
                }

                RestHelper.UploadFile(ApiMethods.BaseUploadFile, path);

                System.IO.File.Delete(path);   // удаляем файл из папки Files в проекте

                ViewBag.Message = ErrorMessages.BaseSuccessUploadFile;
            }
            else
            {
                ViewBag.Message = "Имя файла null";
            }
            return RedirectToAction("Index");
        }
    }
}