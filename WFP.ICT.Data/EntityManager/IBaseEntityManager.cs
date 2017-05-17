using System;
using System.Collections.Generic;
using System.Linq;

namespace WFP.ICT.Data.EntityManager
{
    public interface IBaseEntityManager<EntityType> where EntityType : class, new()
    {
        IQueryable<EntityType> GetAll(string navigationalProperties = "");
        EntityType GetById(Guid id, string navigationalProperties = "");

        void Insert(IEnumerable<EntityType> entitylist, bool saveChanges = true);
        void Insert(EntityType entityObject, bool saveChanges = true);
        
        void Update(IEnumerable<EntityType> entityList, bool saveChanges = true);
        void Update(EntityType newEntity, bool saveChanges = true);

        void Delete(IEnumerable<EntityType> entitylist, bool saveChanges = true);
        void Delete(EntityType entityObject, bool saveChanges = true);
        void DeleteById(Guid id, bool saveChanges = true);

        int SaveChanges();
        bool SaveChangesSuccess { get; }
        void Dispose();
    }
}