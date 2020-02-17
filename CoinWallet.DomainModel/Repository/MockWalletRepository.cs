using System;
using System.Collections.Generic;
using System.Linq;
using CoinWallet.DomainModel.Repository.Contracts;
using CoinWallet.DomainModel.DataModel;
using CoinWallet.DomainModel;

namespace CoinWallet.DomainModel.Repository
{
    public class MockWalletRepository : Repository<Wallet>, IWalletRepository
    {
        public MockWalletRepository(ApplicationDbContext context) : base(context)
        { }

        private List<Wallet> _wallets { get; set; } = new List<Wallet>();

        public IEnumerable<Wallet> GetWalletById(Guid walletId)
        {
            return _wallets.Where(i => i.WalletId.CompareTo(walletId) == 0)?.ToList();
        }

        public void AddWalletData(Wallet wallet)
        {
           _wallets.Add(wallet);
        }
        
        public int GetMaxId()
        {
            return _wallets.Max(i => i.Id);
        }
    }
}
