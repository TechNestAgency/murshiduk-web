using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {

        public mlaraEntities _db = null;
        public DbSet<T> table = null;

        public GenericRepository()
        {
            this._db = new mlaraEntities();
            this._db.Configuration.ValidateOnSaveEnabled = false;
            this._db.Configuration.ProxyCreationEnabled = false;
            this.table = _db.Set<T>();
        }

        public GenericRepository(mlaraEntities _db)
        {
            this._db = _db;
            this.table = _db.Set<T>();
        }

        public void Delete(int id)
        {
            var data = table.Find(id);
            table.Remove(data);

        }

        public void Delete(long id)
        {
            var data = table.Find(id);
            table.Remove(data);
        }

        public IEnumerable<T> GetAll()
        {
            return table.ToList();
        }

        public T GetById(int id)
        {
            return table.Find(id);
        }

        public T GetById(long id)
        {
            return table.Find(id);
        }

        public void Insert(T obj)
        {
            table.Add(obj);
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(T obj)
        {
            table.Attach(obj);
            _db.Entry(obj).State = EntityState.Modified;
        }
    }
}