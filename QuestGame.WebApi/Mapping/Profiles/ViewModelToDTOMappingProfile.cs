﻿using AutoMapper;
using QuestGame.Domain.DTO;
using QuestGame.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuestGame.WebApi.Areas.Game.Models;
using QuestGame.WebApi.Areas.Design.Models;

namespace QuestGame.WebApi.Mapping.Profiles
{
    public class ViewModelToDTOMappingProfile : Profile
    {
        public ViewModelToDTOMappingProfile()
        {
            CreateMap<QuestViewModel, QuestFullDTO>();
            CreateMap<QuestViewModel, QuestDTO>();
            CreateMap<NewItemViewModel, QuestFullDTO>();
            CreateMap<NewItemViewModel, QuestDTO>();
            CreateMap<NewItemViewModel, StageDTO>();
            CreateMap<NewItemViewModel, MotionDTO>().ForMember(x => x.Description, y => y.MapFrom(v => v.Title));
            CreateMap<StageViewModel, StageFullDTO>();
            CreateMap<StageViewModel, StageDTO>();
            CreateMap<MotionViewModel, MotionDTO>();
        }

        public override string ProfileName
        {
            get { return "ViewModelToDTOMappingProfile"; }
        }
    }
}