using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Transactions;
using DocaLabs.EntityFrameworkStorage;
using DocaLabs.Storage.Core.Repositories;
using DocaLabs.Storage.Integration.Tests._Repositories._Scenarios;
using DocaLabs.Storage.Integration.Tests._Utils;

namespace DocaLabs.Storage.Integration.Tests._Repositories._EntityFramework._Utils
{
    class ScenarioProviderBase : IRepositoryScenarioProvider
    {
        protected RepositorySession Session;
        readonly Dictionary<Type, DbContext> _readingContexts = new Dictionary<Type, DbContext>();

        static ScenarioProviderBase()
        {
            RepositoryAggregateRoot<Book>.OnModelCreatingAction = OnModelCreating;
        }

        public void Dispose()
        {
            if (Session != null)
                Session.Dispose(); 

            foreach (var context in _readingContexts.Values)
                context.Dispose();
        }

        public IQueryableRepository<TEntity> CreateRepository<TEntity>() where TEntity : class
        {
            return new Repository<TEntity>(Session);
        }

        public TEntity GetEntity<TEntity>(object id) where TEntity : class
        {
            return GetContext<TEntity>().Set<TEntity>().Find(id);
        }

        public void Save<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            using (var context = new RepositoryAggregateRoot<TEntity>(MsSqlHelper.EfConnectionStringName))
            {
                foreach (var entity in entities)
                {
                    context.Entities.Add(entity);
                }

                context.SaveChanges();
                scope.Complete();
            }
        }

        public static void OnModelCreating(DbModelBuilder builder)
        {
            builder.Entity<Book>().HasKey(x => x.Id);
            builder.Entity<Book>().Property(x => x.Version)
                   .IsConcurrencyToken();
            builder.Entity<Book>().Property(x => x.Isbn)
                   .IsRequired()
                   .HasMaxLength(13);
            builder.Entity<Book>().Property(x => x.Title)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.Entity<Book>().HasMany(x => x.Prices)
                   .WithOptional()
                   .Map(x => x.MapKey("BookId"))
                   .WillCascadeOnDelete();

            builder.Entity<Price>().ToTable("BookPrices");
            builder.Entity<Price>().HasKey(x => x.Id);
            builder.Entity<Price>().Property(x => x.Country)
                   .IsRequired()
                   .HasMaxLength(2);
            builder.Entity<Price>().Property(x => x.Currency)
                   .IsRequired()
                   .HasMaxLength(3);
            builder.Entity<Price>().Property(x => x.Value)
                   .IsRequired();
        }

        DbContext GetContext<TEntity>() where TEntity : class
        {
            DbContext context;
            if (_readingContexts.TryGetValue(typeof(RepositoryAggregateRoot<TEntity>), out context))
                return context;

            context = new RepositoryAggregateRoot<TEntity>(MsSqlHelper.EfConnectionStringName);
            _readingContexts[typeof(RepositoryAggregateRoot<TEntity>)] = context;

            return context;
        }
    }
}