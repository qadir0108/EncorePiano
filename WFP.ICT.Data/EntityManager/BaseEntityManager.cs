using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WFP.ICT.Data.Entities;

namespace WFP.ICT.Data.EntityManager
{
    /// <summary>
    /// Base class for Entity Manager. Generic Repository Design Pattern
    /// </summary>
    public class BaseEntityManager<EntityType> : IDisposable, IBaseEntityManager<EntityType> where EntityType : class, iBaseEntity, new()
    {
        private static WFPICTContext _dbContext;
        public bool SaveChangesSuccess { get; protected set; }

        public WFPICTContext CurrentContext
        {
            get
            {
                var httpContext = HttpContext.Current;
                if (httpContext == null)
                {
                    return new WFPICTContext();
                } else
                {
                    const string key = "WFPICTContext";
                    if (httpContext.Items[key] != null)
                    {
                        return httpContext.Items[key] as WFPICTContext;
                    }
                    else
                    {
                        _dbContext = new WFPICTContext();
                        httpContext.Items[key] = _dbContext;
                    }
                }
                return _dbContext;
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        public BaseEntityManager()
        {
            if (_dbContext == null)
                _dbContext = CurrentContext;

        }

        /// <summary>
        /// public constructor that use existing object
        /// </summary>
        public BaseEntityManager(WFPICTContext dbContext)
        {
            if (dbContext != null)
                _dbContext = dbContext;
            else
            {
                string message="BaseEntityManager constructor, entity name "+EntityName()+": dbContext object can't be NULL !";
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Dispose Design Pattern
        /// </summary>
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (_dbContext != null)
                        _dbContext.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region data processing methods

        /// <summary>
        /// Return full list of entities from Db table
        /// </summary>
        public virtual IQueryable<EntityType> GetAll(string navigationalProperties = "")
        {
            IQueryable<EntityType> _retData;
            var navPropertiesList = navigationalProperties.Split(",".ToCharArray());
            try
            {
                if (String.IsNullOrEmpty(navigationalProperties))
                    _retData = _dbContext.Set<EntityType>();
                else
                {
                    var dbQuery = _dbContext.Set<EntityType>().Include(navPropertiesList[0]);
                    for (int i = 1; i < navPropertiesList.Length; i++)
                    {
                        dbQuery.Include(navPropertiesList[i]);
                    }
                    _retData = dbQuery;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                _retData=null;
            }

            return _retData.OrderBy(x => x.CreatedAt);
        }

        /// <summary>
        /// Get by id
        /// </summary>
        public EntityType GetById(Guid id, string navigationalProperties = "")
        {
            EntityType entity;
            var navPropertiesList = navigationalProperties.Split(",".ToCharArray());
            try
            {
                if (navigationalProperties.Equals(String.Empty))
                    entity = _dbContext.Set<EntityType>().Find(id);
                else
                {
                    var dbQuery = _dbContext.Set<EntityType>().Include(navPropertiesList[0]);
                    for (int i = 1; i < navPropertiesList.Length; i++ )
                    {
                        dbQuery.Include(navPropertiesList[i]);
                    }
                    entity = dbQuery.FirstOrDefault(x => x.Id == id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                entity = null;
            }
            return entity;
        }

        /// <summary>
        /// Save changes in the  current DbContext
        /// </summary>
        public int SaveChanges()
        {
            int retValue;
            try
            {
                SaveChangesSuccess = true;
                retValue = _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                SaveChangesSuccess = false;
                retValue = 0;
            }
            return retValue;
        }

        /// <summary>
        /// Add new record into database table
        /// </summary>
        public virtual void Insert(EntityType entityObject, bool saveChanges=true)
        {
            AddEntity(entityObject);
            if (saveChanges) SaveChanges();
        }

        /// <summary>
        /// Add list of new records into database table
        /// </summary>
        public virtual void Insert(IEnumerable<EntityType> entitylist, bool saveChanges = true)
        {
            foreach (EntityType entity in entitylist)
                AddEntity(entity);
            if (saveChanges) SaveChanges();
        }       

        /// <summary>
        /// Add new entity to DbSet object
        /// </summary>
        protected void AddEntity(EntityType entity)
        {
            CheckEntityId(entity);
            _dbContext.Set<EntityType>().Add(entity);            
        }

        /// <summary>
        /// Creates new Guid type value for empty Id
        /// </summary>
        protected bool CheckEntityId(EntityType entity)
        {
            bool ret = false;
            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
                ret = true;
            }
            entity.CreatedAt = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Delete record into database table
        /// </summary>
        public virtual void Delete(EntityType entityObject, bool saveChanges = true)
        {
            DeleteEntity(entityObject);
            if (saveChanges) SaveChanges();
        }

        /// <summary>
        /// Delete list of records into database table
        /// </summary>
        public virtual void Delete(IEnumerable<EntityType> entitylist, bool saveChanges = true)
        {
            _dbContext.Set<EntityType>().RemoveRange(entitylist);
            if (saveChanges) SaveChanges();
        }

        protected void DeleteEntity(EntityType entity)
        {
            if (_dbContext.Entry(entity).State == EntityState.Detached)
            {
                _dbContext.Set<EntityType>().Attach(entity);
            }
            _dbContext.Set<EntityType>().Remove(entity);
        }

        /// <summary>
        /// Delete entity by it primary key Id
        /// </summary>
        public void DeleteById(Guid id, bool saveChanges=true)
        {
            EntityType entity = GetById(id);
            if (entity != null)
                Delete(entity, saveChanges);
        }

        /// <summary>
        /// Update entity in the database
        /// </summary>
        public virtual void Update(EntityType newEntity, bool saveChanges = true)
        {
            EntityType oldEntity = GetById(newEntity.Id);
            var entry = _dbContext.Entry(oldEntity);
            entry.CurrentValues.SetValues(newEntity);
            entry.State = EntityState.Modified;
            if (saveChanges) SaveChanges();
        }

        /// <summary>
        /// Update list of entities
        /// </summary>
        public virtual void Update(IEnumerable<EntityType> entityList, bool saveChanges = true)
        {
            foreach (EntityType entity in entityList)
                Update(entity, saveChanges);
        }

        #endregion data processing methods

        #region Service Metods

        protected static string EntityName()
        {
            return typeof(EntityType).ToString();
        }

        #endregion Service Metods
        
    }
}
