using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.SessionState;
using Ellemy.CQRS.Example.Query;

namespace Ellemy.CQRS.Example.Web.Infrastructure
{
    public class InMemoryCacheRepository<T> : IRepository<T> where T:class 
    {
        private readonly HttpSessionState _cache;
        private const string keyFormat = "{0}_{1}";

        public InMemoryCacheRepository()
        {
            _cache = HttpContext.Current.Session;
            if (_cache[typeof(T).Name] == null)
                _cache[typeof (T).Name] = new List<T>();
        }
        private List<T> Items
        {
            get { return _cache[typeof (T).Name] as List<T>; }
        }

        public T GetById(object id)
        {
            return Items.FirstOrDefault(i => TheIdOf(i) == id);
        }
        private object TheIdOf(object thing)
        {
            return thing.GetType().GetProperty("Id").GetValue(thing, null);
        }
        public IEnumerable<T> GetAll()
        {
            return Items;
        }

        public void Save(T entity)
        {
            if(Items.Contains(entity))
            {
                Items.Remove(entity);
            }
            Items.Add(entity);
        }
    }
}