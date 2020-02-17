using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CoinWallet.DomainModel.Repository.Contracts
{
    public interface IRepository<TEntity> where TEntity : class
    {
        void Add(TEntity entity);
        IEnumerable<TEntity> Find(Func<TEntity, bool> predicate);
        TEntity GetSingleOrDefault(Func<TEntity, bool> predicate);
    }
}
