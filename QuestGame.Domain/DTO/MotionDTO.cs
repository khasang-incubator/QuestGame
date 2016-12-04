﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestGame.Domain.DTO
{
    public class MotionDTO
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int? NextStageId { get; set; }
        public int OwnerStageId { get; set; }
    }

    public class MotionEditDTO : MotionDTO
    {
        public Dictionary<int, string> RedirectStagesList { get; set; }
    }
}
