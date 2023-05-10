using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace _00_Framework.Domain
{
    /// <summary>
    /// abstraction for avoid of repeating the common operation of repository Such as CRUD
    /// </summary>
    /// <typeparam name="TKey">Represent the type of key property</typeparam>
    /// <typeparam name="T">Represent the type of entity</typeparam>
    

    //ToDo : Change the IRepository to segregated interfaces
    public interface IBaseRepository<in TKey, T> where T : class
    {
        /// <summary>
        /// Add the <paramref name="entity"/> to the database
        /// </summary>
        /// <param name="entity"></param>
        void Create(T entity);

        /// <summary>
        /// Get the <see cref="T"/> From database by <paramref name="id"/>
        /// </summary>
        /// <param name="id">The id of searching record</param>
        /// <returns>Return the <see cref="T"/> with this <paramref name="id"/> </returns>
        /// <exception cref="NullReferenceException">if didn't find any<see cref="T"/> then return null</exception>
        T Get(TKey id);
        /// <summary>
        /// Get the all <see cref="T"/> entities from database
        /// </summary>
        /// <returns>all <see cref="T"/> entities</returns>
        List<T> Get();

        /// <summary>
        /// check there is any <see cref="T"/> entity in database with <paramref name="expression"/> condition
        /// </summary>
        /// <param name="expression">delegate that return a <see langword="bool"/> with <paramref name="expression"/> input</param>
        /// <returns><see langword="true"/> if there is any value and else <see langword="false"/></returns>
        bool IsExists(Expression<Func<T, bool>> expression);

        /// <summary>
        /// Save the changes to the database
        /// </summary>
        void SaveChanges();
    }
}
