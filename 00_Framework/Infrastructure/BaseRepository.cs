using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using _00_Framework.Domain;

namespace _00_Framework.Infrastructure
{
    /// <summary>
    ///an Implement of <see cref="IBaseRepository{TKey,T}"/>
    /// <b/>
    /// The base implement
    /// </summary>
    /// <typeparam name="TKey">Represent the type of key property</typeparam>
    /// <typeparam name="T">Represent the type of entity</typeparam>
    public class BaseRepository<TKey, T> : IBaseRepository<TKey, T> where T : class
    {
        /// <summary>
        /// Represent the db context that include <see cref="T"/>
        /// </summary>
        private readonly DbContext _context;

        public BaseRepository(DbContext context)
        {
            _context = context;
        }

        public void Create(T entity)
        {
            _context.Add(entity);
        }

        public T Get(TKey id)
        {
            return _context.Set<T>().Find(id);
        }

        public List<T> Get()
        {
            return _context.Set<T>().ToList();
        }

        public bool IsExists(Expression<Func<T, bool>> expression)
        {
            //there is any entity with expression condition 
            return _context.Set<T>().Any(expression);
        }

        public void SaveChanges()
        {
             _context.SaveChanges();
            
        }
    }
}