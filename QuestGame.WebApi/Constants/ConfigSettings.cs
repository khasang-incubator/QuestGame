﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using QuestGame.Common.Helpers;

namespace QuestGame.WebApi.Constants
{
    public class ConfigSettings
    {

        #region Ключи параметров

        public const string BaseUrlKey = "WebApiServiceBaseUrl";
        public const string MailPathKey = "MailPath";
        public const string FilePathKey = "FilePath";

        #endregion

        #region Настройки по умолчанию

        public const string WebApiServiceBaseUrl = @"http://localhost:9243/";
        public const string PathMail = @"..\..\Content\Temp\";
        public const string FilePath = @"~\Content\Images\";

        public const string QuestPrefixFile = @"Quests\";
        public const string StagePrefixFile = @"Stages\";
        public const string AvatarPrefixFile = @"Avatars\";

        #endregion

        public static string GetAbsFilePath()
        {
            var relativePath = CommonHelper.GetParamOrDefaultValue(FilePathKey, FilePath);
            var absPath = HttpContext.Current.Server.MapPath(relativePath);
            return absPath;
        }
    }
}