﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuestGame.WebMVC.Constants
{
    public class InfoMessages
    {
        public const string EmailSendConfirm = "На почту отправлено сообщение. Пройдите по ссылке из письма, чтобы закончить регистрацию.";
        public const string EmailConfirmTitle = "Подтверждение аккаунта";
        public const string EmailConfirmAccepted = "Email успешно подтвержден!";

        public const string PasswordConfirmTrue = "Пароль успешно изменен!";
        public const string PasswordSendToken = "Письмо смены пароля отправлено. Чтобы сменить пароль пройдите по ссылке из письма.";
        public const string PasswordResetTitle = "Сброс пароля";
        public const string PasswordChangeTitle = "Смена пароля";
    }
}