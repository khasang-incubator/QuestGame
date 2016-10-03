﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuestGame.WebApi.Models
{
    public class UserViewModel
    {
        [System.Web.Mvc.HiddenInput(DisplayValue = false)]
        public string Id { get; set; }

        [Display(Name = "Обращение")]
        public string NickName { get; set; }

        [System.Web.Mvc.HiddenInput(DisplayValue = false)]
        public string UserName { get; set; }

        [Display(Name = "Адрес электронной почты")]
        public string Email { get; set; }

        public UserProfileViewModel UserProfile { get; set; }
    }
}