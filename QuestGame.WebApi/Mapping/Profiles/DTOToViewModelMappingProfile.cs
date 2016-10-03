﻿using AutoMapper;
using QuestGame.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuestGame.WebApi.Areas.Game.Models;
using System.Collections.Specialized;
using QuestGame.WebApi.Models;

namespace QuestGame.WebApi.Mapping.Profiles
{
    public class DTOToViewModelMappingProfile : Profile
    {
        public DTOToViewModelMappingProfile()
        {
            CreateMap<QuestFullDTO, QuestViewModel>().ForMember(x => x.Stages, y => y.MapFrom(pr => pr.Stages.Select(n => n.Title)));
            CreateMap<QuestDTO, QuestViewModel>();
            CreateMap<QuestDTO, UserQuestsViewModel>();

            CreateMap<StageDTO, StageViewModel>();
            CreateMap<MotionDTO, MotionViewModel>();

            CreateMap<UserDTO, UserViewModel>();
            CreateMap<UserProfileDTO, UserProfileViewModel>();
        }

        public override string ProfileName
        {
            get { return "DTOToViewModelMappingProfile"; }
        }
    }
}