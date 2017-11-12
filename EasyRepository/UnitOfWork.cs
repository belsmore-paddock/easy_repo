namespace EasyRepository
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Validation;
    using System.Text;
    using System.Transactions;

    /// <inheritdoc />
    /// <summary>
    ///   The unit of work.
    /// </summary>
    /// <typeparam name="TKey">
    ///   Type parameter to be used as the model identifier (i.e. Guid, int, etc).
    /// </typeparam>
    public class UnitOfWork<TKey> : IUnitOfWork
    {
        #region fields

        /// <summary>
        ///   The database context for use with this UoW.
        /// </summary>
        private readonly DbContext context;

        /// <summary>
        ///   The Transaction Scope
        /// </summary>
        private DbContextTransaction transactionScope;

        #endregion

        #region public constructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="UnitOfWork{TKey}"/> class.
        /// </summary>
        /// <param name="context">
        ///   The database context to be used for database interactions.
        /// </param>
        public UnitOfWork(DbContext context)
        {
            this.context = context;
            var repository = new Repository<TKey>(context);
            repository.BeforeAction += this.OnBeforeAction;
            this.Repository = repository;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the repository being used with the current UoW.
        /// </summary>
        public IRepository<TKey> Repository { get; }

        #endregion

        #region public methods

        /// <summary>
        ///   The dispose method destroys the database context used with the current UoW.
        /// </summary>
        public void Dispose()
        {
            if (this.context != null)
            {
                try
                {
                    this.context.Dispose();
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                    // Do nothing
                }
            }

            if (this.Repository is Repository<TKey> repository)
            {
                repository.BeforeAction -= this.OnBeforeAction;
            }
        }

        /// <summary>
        ///   Begins a transaction for the current UoW.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///   Thrown in cases that the data context ise not usable or a transaction is already open.
        /// </exception>
        /// <exception cref="Exception">
        ///   Thrown when an issue occurs opening a transaction scope.
        /// </exception>
        public void BeginTransaction()
        {
            this.EnsureContext();

            if (this.transactionScope != null)
            {
                throw new InvalidOperationException("There is already an open transaction");
            }

            try
            {
                this.transactionScope = this.context.Database.BeginTransaction();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occured while beginning transaction.\r\n{ex.Message}");
            }
        }

        /// <summary>
        ///   The commit transaction commits all pending context changes to the database.
        /// </summary>
        /// <exception cref="TransactionException">
        ///   Thrown when no transaction is present,
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   Thrown in cases that the data context ise not usable or disposed.
        /// </exception>
        /// <exception cref="Exception">
        ///   Thrown on error comitting transactions.
        /// </exception>
        public void CommitTransaction()
        {
            this.EnsureContext();

            if (this.transactionScope == null)
            {
                throw new TransactionException("There is no current transaction");
            }

            try
            {
                this.context.SaveChanges();
                this.transactionScope.Commit();
            }
            catch (Exception ex)
            {
                this.RollbackTransaction();
                if (ex is DbEntityValidationException validationException)
                {
                    this.HandleDbEntityValidationException(validationException);
                }
                else
                {
                    throw new Exception($"An error occured during the Commit transaction.\r\n{ex.Message}");
                }
            }
            finally
            {
                var scope = this.transactionScope;
                scope?.Dispose();

                this.transactionScope = null;
            }
        }

        /// <summary>
        ///   The rollback transaction returns the context to the inital state it
        ///   was when BeginTransaction() was called.
        /// </summary>
        /// <exception cref="Exception">
        ///   Thrown when an error occurs rolling back a transaction.
        /// </exception>
        public void RollbackTransaction()
        {
            try
            {
                if (this.transactionScope != null)
                {
                    this.transactionScope.Rollback();
                    this.transactionScope.Dispose();
                    this.transactionScope = null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"An error occured during the Rollback transaction.\r\n{ex.Message}");
            }
        }

        #endregion

        #region private methods

        /// <summary>
        ///   The on before action triggered when a child repository action is triggered.
        /// </summary>
        /// <param name="sender">
        ///   The sender.
        /// </param>
        /// <param name="e">
        ///   The event args.
        /// </param>
        private void OnBeforeAction(object sender, EventArgs e)
        {
            this.EnsureContext();
        }

        /// <summary>
        ///   Handles the provided DbEntityValidationException and iterates through validation errors
        ///   to rethrow a more detailed exception.
        /// </summary>
        /// <param name="entityValidationException">
        ///   The db entity validation exception being handled.
        /// </param>
        /// <exception cref="Exception">
        ///   Exception thrown after extracting validation errors from the provided exception.
        /// </exception>
        private void HandleDbEntityValidationException(DbEntityValidationException entityValidationException)
        {
            var sb = new StringBuilder();

            foreach (var failure in entityValidationException.EntityValidationErrors)
            {
                sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                foreach (var error in failure.ValidationErrors)
                {
                    sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                    sb.AppendLine();
                }
            }

            throw new Exception(sb.ToString(), entityValidationException);
        }

        /// <summary>
        ///   The ensure context method checks whether the context is available and
        ///   throws an exception if it isn't.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///   Thrown when a context is not available.
        /// </exception>
        private void EnsureContext()
        {
            if (this.context == null)
            {
                throw new InvalidOperationException("The data context is disposed and unusable");
            }
        }

        #endregion
    }
}