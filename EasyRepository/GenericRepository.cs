namespace EasyRepository
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    ///   The generic repository implementation of IRepository.
    /// </summary>
    /// <typeparam name="TKey">
    ///   Type indicating the primary key type for the repo implementation.
    /// </typeparam>
    public class GenericRepository<TKey> : IRepository<TKey>
    {
        #region fields

        /// <summary>
        ///   The database context being used by this repository implementation.
        /// </summary>
        private readonly DbContext context;

        #endregion

        #region public constructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="GenericRepository{TKey}"/> class.
        /// </summary>
        /// <param name="context">
        ///   The context to be used by this repository.
        /// </param>
        public GenericRepository(DbContext context)
        {
            this.context = context;
        }

        #endregion

        #region events

        /// <summary>
        ///   The before action invoked before any database CRUD interaction.
        /// </summary>
        internal event EventHandler BeforeAction;

        #endregion

        #region public methods

        /// <inheritdoc />
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
        ///   The typed entity model of <see cref="T" /> with the provided id.
        /// </returns>
        public T FindById<T>(TKey id)
            where T : class
        {
            this.OnBeforeAction();
            return this.context.Set<T>().Find(id);
        }

        /// <summary>
        ///   Retrieves a full list of persisted entities.
        /// </summary>
        /// <typeparam name="T">
        ///   Target type of the persisted entity being found.
        /// </typeparam>
        /// <returns>
        ///   The <see cref="IList"/> of entities belonging to the provided target type.
        /// </returns>
        public IList<T> GetList<T>()
            where T : class
        {
            this.OnBeforeAction();
            return this.context.Set<T>().ToList();
        }

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
        public IList<T> GetList<T>(Expression<Func<T, bool>> query)
            where T : class
        {
            this.OnBeforeAction();
            return this.context.Set<T>().Where(query).ToList();
        }

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
        public IList<T> GetListIncluding<T>(Expression<Func<T, bool>> query, params Expression<Func<T, object>>[] includeProperties)
            where T : class
        {
            this.OnBeforeAction();
            return includeProperties.Aggregate(
                this.context.Set<T>()
                .Where(query),
                (current, includeProperty) => current.Include(includeProperty)).ToList();
        }

        /// <summary>
        ///   The add method persists a new entity.
        /// </summary>
        /// <param name="entity">
        ///   The entity being added to the pseristence layer.
        /// </param>
        /// <typeparam name="T">
        ///   Type of entity being added.
        /// </typeparam>
        /// <returns>
        ///   The <see cref="T"/> created entity.
        /// </returns>
        /// <exception cref="Exception">
        ///   Thrown if an error occurs trying to add an entity.
        /// </exception>
        public T Add<T>(T entity)
            where T : class
        {
            this.OnBeforeAction();
            try
            {
                this.context.Set<T>().Add(entity);
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occured while adding an entity.\r\n{ex.Message}");
            }
        }

        /// <summary>
        ///   The update method modifies an existing persisted entity.
        /// </summary>
        /// <param name="entity">
        ///   The entity being updated.
        /// </param>
        /// <typeparam name="T">
        ///   Type of entity being updated.
        /// </typeparam>
        /// <returns>
        ///   The <see cref="T"/> updated entity.
        /// </returns>
        /// <exception cref="Exception">
        ///   Thrown if an error occurs trying to update an entity.
        /// </exception>
        public T Update<T>(T entity)
            where T : class
        {
            this.OnBeforeAction();
            try
            {
                this.context.Set<T>().Attach(entity);
                this.context.Entry(entity).State = EntityState.Modified;
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occured while updating an entity.\r\n{ex.Message}");
            }
        }

        /// <summary>
        ///   The delete method removes an entity with the provided id from the persistence layer.
        /// </summary>
        /// <param name="id">
        ///   The id of the entity being deleted.
        /// </param>
        /// <typeparam name="T">
        ///   Type of entity being deleted.
        /// </typeparam>
        /// <exception cref="Exception">
        ///   Thrown if an error occurs trying to delete an entity.
        /// </exception>
        public void Delete<T>(TKey id)
            where T : class
        {
            this.OnBeforeAction();
            try
            {
                var entity = this.FindById<T>(id);
                this.context.Entry(entity).State = EntityState.Deleted;
                this.context.Set<T>().Remove(entity);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occured while deleting an entity.\r\n{ex.Message}");
            }
        }

        #endregion

        #region protected methods

        /// <summary>
        ///   The on before action invokes the BeforeAction event if specified.
        /// </summary>
        protected virtual void OnBeforeAction()
        {
            this.BeforeAction?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}