﻿using System;
using System.ComponentModel.DataAnnotations;

namespace QuestGame.Domain.Entities
{
    public abstract class Content
    {
        [Key]
        public int OwnerId { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public string Video { get; set; }
        public DateTime ModifyDate { get; set; }

        public Content()
        {
            this.ModifyDate = DateTime.Now;
        }
    }
}