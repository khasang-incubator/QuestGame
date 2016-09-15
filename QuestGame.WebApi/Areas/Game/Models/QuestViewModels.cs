using QuestGame.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuestGame.WebApi.Areas.Game.Models
{
    public class QuestViewModel
    {
        public int Id { get; set; }

        [Display(Name = "��������")]
        public string Title { get; set; }

        [Display(Name = "���� ��������")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Date { get; set; }

        [Display(Name = "�������")]
        public int Rate { get; set; }

        [Display(Name = "�����������")]
        public bool Active { get; set; }

        [Display(Name = "�����")]
        public string Owner { get; set; }

        public IDictionary<int, string> Stages { get; set; }

        public QuestViewModel()
        {
            Stages = new Dictionary<int, string>();
        }
    }

    public class StageViewModel
    {
        public int Id { get; set; }

        [Display(Name = "��������")]
        public string Title { get; set; }

        [Display(Name = "����")]
        public string Body { get; set; }

        public IDictionary<int, string> Motions { get; set; }

        public StageViewModel()
        {
            Motions = new Dictionary<int, string>();
        }
    }

    public class MotionViewModel
    {
        public int Id { get; set; }

        [Display(Name = "��������")]
        public string Description { get; set; }

        public int NextStageId { get; set; }
    }

    public class NewQuestViewModel
    {
        [Required]
        [Display(Name = "��������")]
        public string Title { get; set; }

        [Display(Name = "�����")]
        public string Owner { get; set; }
    }
}