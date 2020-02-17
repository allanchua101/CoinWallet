using CoinWallet.DomainModel.DataModel;
using CoinWallet.DomainModel;
using CoinWallet.DomainModel.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinWallet.DomainModel.Repository
{
    public class WalletRepository : Repository<Wallet>, IWalletRepository
    {
        public WalletRepository(ApplicationDbContext context) : base(context)
        { }

        public IEnumerable<Wallet> GetWalletById(Guid Id)
        {
            Func<Wallet,bool> predicate = wallet => (wallet.WalletId.CompareTo(Id) == 0);
            return Find(predicate);
        }

        public void AddWalletData(Wallet wallet)
        {
            Add(wallet);
        }

        public int GetMaxId()
        {
            throw new NotImplementedException();
        }
    }
}
