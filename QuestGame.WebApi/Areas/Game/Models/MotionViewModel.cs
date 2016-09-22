﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuestGame.WebApi.Areas.Game.Models
{
    public class MotionViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        //public string NextStageTitle { get; set; }
        public int OwnerStageId { get; set; }
        public int? NextStageId { get; set; }
    }
}