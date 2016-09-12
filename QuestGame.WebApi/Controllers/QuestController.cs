﻿using AutoMapper;
using Newtonsoft.Json;
using QuestGame.Domain;
using QuestGame.Domain.DTO;
using QuestGame.Domain.Entities;
using QuestGame.Domain.Interfaces;
using QuestGame.WebApi.Mapping;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using QuestGame.Common.Interfaces;
using System.Web;
using System.Threading;
using Microsoft.AspNet.Identity;
using QuestGame.WebApi.Areas.Design.Models;

namespace QuestGame.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Quest")]
    public class QuestController : ApiController
    {
        IDataManager dataManager;
        IMapper mapper;
        ILoggerService logger;

        public QuestController(IDataManager dataManager, IMapper mapper, ILoggerService logger)
        {
            this.dataManager = dataManager;
            this.mapper = mapper;
            this.logger = logger;

            logger.Information("Request | QuestController | from {0} | user: {1}", HttpContext.Current.Request.UserHostAddress, User.Identity.Name);
        }

        [HttpGet]
        [Route("GetAll")]
        public IEnumerable<QuestDTO> GetAll()
        {
            try
            {
                var quests = dataManager.Quests.GetAll().ToList();

                var response = mapper.Map<IEnumerable<Quest>, IEnumerable<QuestDTO>>(quests);           
                return response;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }            
        }

        [HttpGet]
        [Route("GetByUser")]
        public IEnumerable<QuestDTO> GetByUser()
        {
            var principal = Thread.CurrentPrincipal;
            var identity = principal.Identity;
            var id = identity.GetUserId();

            try
            {
                var quests = dataManager.Quests.GetByUser(id.ToString()).ToList();

                var response = mapper.Map<IEnumerable<Quest>, IEnumerable<QuestDTO>>(quests);

                return response;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        [HttpGet]
        [Route("GetById")]
        public QuestDTO GetById(int id)
        {
            var quest = dataManager.Quests.GetById(id);
            var response = mapper.Map<Quest, QuestDTO>(quest);

            return response;
        }

        [HttpPost]
        [Route("Add")]
        public IHttpActionResult Add(QuestDTO quest)
        {
            var model = mapper.Map<QuestDTO, Quest>(quest);

            try
            {
                //var owner = dataManager.UserManager.FindByNameAsync(quest.Owner);
                var owner = dataManager.Users.GetAll().FirstOrDefault(x => x.UserName == quest.Owner);
                if (owner == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                model.Owner = owner;
                model.Date = DateTime.Now;

                dataManager.Quests.Add(model);
                dataManager.Save();
                return Ok();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }

        [HttpPut]
        [Route("Update")]
        public void Update(QuestDTO model)
        {
            var questOriginal = dataManager.Quests.GetById(model.Id);
            var questResult = mapper.Map<QuestDTO, Quest>(model, questOriginal);

            dataManager.Quests.Update(questResult);
            dataManager.Save();
        }

        [HttpDelete]
        [Route("Delete")]
        public void Delete(int id)
        {
            var quest = dataManager.Quests.GetById(id);

            dataManager.Quests.Delete(quest);
            dataManager.Save();
        }
    }    
}

    
