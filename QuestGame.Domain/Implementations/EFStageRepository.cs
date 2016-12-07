﻿using QuestGame.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestGame.Domain.Entities;

namespace QuestGame.Domain.Implementations
{
    public class EFStageRepository : IStageRepository
    {
        private IApplicationDbContext dbContext;

        public EFStageRepository(IApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Add(Stage item)
        {
            dbContext.Stages.Add(item);
        }

        public void Delete(object id)
        {
            Delete(GetById((int)id));
        }

        public void Delete(Stage item)
        {
            dbContext.Stages.Remove(item);
        }

        public IEnumerable<Stage> GetAll()
        {
            return dbContext.Stages;
        }

        public Stage GetById(object id)
        {
            return dbContext.Stages.Find((int)id);
        }

        public IEnumerable<Stage> GetByMotionId(int id)
        {
            return dbContext.Motions.Find(id).OwnerStage.OwnerQuest.Stages;
        }

        public void Update(Stage item)
        {
            dbContext.EntryObj(item);
        }

        public IEnumerable<Stage> GetByQuestId(int id)
        {
            return dbContext.Stages.Where(x => x.QuestId == id);
        }
    }
}
