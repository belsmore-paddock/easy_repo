namespace EasyRepository
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    ///   The Repository interface. Interface can be applied generically to any Model type.
    /// </summary>
    /// <typeparam name="TKey">
    ///   Value type of primary key.
    /// </typeparam>
    public interface IRepository<in TKey>
    {
        /// <summary>
        ///   Finds a single entity instance beloinging to the provided id. Returns null if not found.
        /// </summary>
        /// <param name="id">
        ///   The id of the entity to be returned.
        /// </param>
        /// <typeparam name="T">
        ///   Target type of the persisted entity being found.
        /// </typeparam>
        /// <returns>
        ///   The typed entity model of <see cref="T"/> with the provided id.
        /// </returns>
        T FindById<T>(TKey id) where T : class;

        /// <summary>
        ///   Retrieves a full list of persisted entities.
        /// </summary>
        /// <typeparam name="T">
        ///   Target type of the persisted entity being found.
        /// </typeparam>
        /// <returns>
        ///   The <see cref="IList"/> of entities belonging to the provided target type.
        /// </returns>
        IList<T> GetList<T>() where T : class;

        /// <summary>
        ///   Retrieves a list for the provided entity using the provided expression.
        /// </summary>
        /// <param name="query">
        ///   The query expression to filter the result set.
        /// </param>
        /// <typeparam name="T">
        ///   Target type of the persisted entity being found.
        /// </typeparam>
        /// <returns>
        ///   The <see cref="IList"/> of entities belonging to the provided target type.
        /// </returns>
        IList<T> GetList<T>(Expression<Func<T, bool>> query) where T : class;

        /// <summary>
        ///   Retrieves a list for the provided entity using the provided expression and allows for associated model properties
        ///   to be included (eager loaded) in the result.
        /// </summary>
        /// <param name="query">
        ///   The query expression to filter the result set.
        /// </param>
        /// <param name="includeProperties">
        ///   The included (associated) properties to be eager loaded with the result.
        /// </param>
        /// <typeparam name="T">
        ///   Target type of the persisted entity being found.
        /// </typeparam>
        /// <returns>
        ///   The <see cref="IList"/> of entities belonging to the provided target type.
        /// </returns>
        IList<T> GetListIncluding<T>(Expression<Func<T, bool>> query, params Expression<Func<T, object>>[] includeProperties) where T : class;

        /// <summary>
        ///   Adds an entity to the data context.
        /// </summary>
        /// <param name="entity">
        ///   The target entity being persisted.
        /// </param>
        /// <typeparam name="T">
        ///  Target type of the entity being persisted.
        /// </typeparam>
        /// <returns>
        ///   The <see cref="T"/> of entity once persisted.
        /// </returns>
        T Add<T>(T entity) where T : class;

        /// <summary>
        ///   Updates an existing entity in the data context.
        /// </summary>
        /// <param name="entity">
        ///   The target persisted entity being updated.
        /// </param>
        /// <typeparam name="T">
        ///   Target type of the entity being updated.
        /// </typeparam>
        /// <returns>
        ///   The <see cref="T"/> of updated entity.
        /// </returns>
        T Update<T>(T entity) where T : class;

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="id">
        ///   The id of the entity to be returned.
        /// </param>
        /// <typeparam name="T">
        ///   Target type of the entity being deleted.
        /// </typeparam>
        void Delete<T>(TKey id) where T : class;
    }
}