namespace EasyRepository
{
    using System;

    /// <summary>
    ///   Unit of Work
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        #region methods

        /// <summary>
        ///   Begins the transaction.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        ///   Commits the transaction.
        /// </summary>
        void CommitTransaction();

        /// <summary>
        ///   Rollsback the transaction.
        /// </summary>
        void RollbackTransaction();

        #endregion
    }
}