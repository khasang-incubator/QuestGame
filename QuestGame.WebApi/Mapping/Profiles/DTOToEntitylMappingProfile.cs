﻿using AutoMapper;
using QuestGame.Domain.DTO;
using QuestGame.Domain.Entities;
using QuestGame.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuestGame.WebApi.Mapping.Profiles
{
    public class DTOToEntitylMappingProfile : Profile
    {
        public DTOToEntitylMappingProfile()
        {
            CreateMap<QuestFullDTO, Quest>().ForMember(x => x.Owner, y => y.Ignore());
            CreateMap<QuestDTO, Quest>().ForMember(x => x.Owner, y => y.Ignore());
            CreateMap<StageFullDTO, Stage>();
            CreateMap<MotionDTO, Motion>();
            CreateMap<UserProfileDTO, UserProfile>();
        }

        public override string ProfileName
        {
            get { return "DTOToEntitylMappingProfile"; }
        }
    }
}