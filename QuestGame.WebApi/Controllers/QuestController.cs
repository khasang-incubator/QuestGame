﻿using AutoMapper;
using QuestGame.Domain.DTO;
using QuestGame.Domain.Entities;
using QuestGame.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Threading.Tasks;
using QuestGame.Common.Interfaces;
using System.Web;
using QuestGame.WebApi.Constants;

namespace QuestGame.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Quest")]
    public class QuestController : BaseController
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                logger.Error("Quest | GetAll | ", ex.ToString());
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        [Route("GetById")]
        public QuestDTO GetById(int? id)
        {
            try
            {
                var quest = dataManager.Quests.GetById(id);
                var response = mapper.Map<Quest, QuestDTO>(quest);
                return response;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                logger.Error("Quest | GetById | ", ex.ToString());
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            
        }

        [HttpGet]
        [Route("GetByActive")]
        public IEnumerable<QuestDTO> GetByActive()
        {
            try
            {
                var quest = dataManager.Quests.GetByActive().ToList();
                var response = mapper.Map<IEnumerable<Quest>, IEnumerable<QuestDTO>>(quest);
                return response;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                logger.Error("Quest | GetByActive | ", ex.ToString());
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        [Route("GetByUserName")]
        public IEnumerable<QuestDTO> GetByUserName(string userName)
        {
            try
            {
                var quest = dataManager.Quests.GetByUserName(userName).ToList();
                var response = mapper.Map<IEnumerable<Quest>, IEnumerable<QuestDTO>>(quest);
                return response;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                logger.Error("Quest | GetByUserName | ", ex.ToString());
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        [Route("Create")]
        public IHttpActionResult Create(QuestDTO quest)
        {
            try
            {
                var model = mapper.Map<QuestDTO, Quest>(quest);
                var owner = dataManager.Users.GetAll().FirstOrDefault(x => x.UserName == quest.Owner);
                if (owner == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                
                model.Owner = owner;
                model.Date = DateTime.Now;
                model.Cover = new Image
                {
                    Name = ConfigSettings.GetServerFilePath(ConfigSettings.NoImage),
                    Prefix = string.Empty
                };

                dataManager.Quests.Add(model);
                dataManager.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                logger.Error("Quest | Add | ", ex.ToString());
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }            
        }

        [HttpPut]
        [Route("Update")]
        public IHttpActionResult Update(QuestDTO quest)
        {
            try
            {
                var questEntity = dataManager.Quests.GetById(quest.Id);
                var model = mapper.Map<QuestDTO, Quest>(quest, questEntity);

                model.Cover = new Image
                {
                    Name = quest.Cover,
                    Prefix = ConfigSettings.QuestPrefixFile,
                };

                var owner = dataManager.Users.GetAll().FirstOrDefault(x => x.UserName == quest.Owner);
                model.Owner = owner;
                dataManager.Quests.Update(model);
                dataManager.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                logger.Error("Quest | Update | ", ex.ToString());
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }            
        }

        [HttpDelete]
        [Route("Delete")]
        public IHttpActionResult Delete(QuestDTO quest)
        {
            try
            {
                var model = mapper.Map<QuestDTO, Quest>(quest);
                dataManager.Quests.Delete(model);
                dataManager.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                logger.Error("Quest | Delete | ", ex.ToString());
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }             
        }

        /// <summary>
        /// Загрузка обложки квеста
        /// </summary>
        [HttpPost]
        [Route("UploadFile")]
        public async Task<string> UploadFile()
        {
            try
            {
                var result = await Upload(ConfigSettings.QuestPrefixFile);
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                logger.Error("Quest | UploadFile | ", ex.ToString());
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }
    }
}


