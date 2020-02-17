using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CoinWallet.DomainModel.Repository.Contracts;
using CoinWallet.DomainModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CoinWallet.DomainModel.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntity> _entities;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _entities = context.Set<TEntity>();
        }

        public virtual void Add(TEntity entity)
        {
            _entities.Add(entity);
            _context.SaveChanges();
        }

        public virtual IEnumerable<TEntity> Find(Func<TEntity, bool> predicate)
        {
            return _entities.Where(predicate);
        }

        public virtual TEntity GetSingleOrDefault(Func<TEntity, bool> predicate)
        {
            return _entities.SingleOrDefault(predicate);
        }
    }
}
