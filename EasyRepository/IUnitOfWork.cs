namespace EasyRepository
{
    using System;

    /// <summary>
    /// Unit of Work
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        #region methods

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// Rollsback the transaction.
        /// </summary>
        void RollbackTransaction();

        /// <summary>
        /// The find repository method locates a repository instance matching the provided type..
        /// </summary>
        /// <param name="type">
        /// The type of the IRepository instance to be retrieved.
        /// </param>
        /// <typeparam name="T">
        /// TKey value for the target repository.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IRepository{T}"/>.
        /// </returns>
        IRepository<T> FindRepository<T>(Type type);

        #endregion
    }
}