using System;
using System.Collections.Generic;
using CoinWallet.DomainModel.DataModel;

namespace CoinWallet.DomainModel.Repository.Contracts
{
    public interface IWalletRepository : IRepository<Wallet>
    {
        IEnumerable<Wallet> GetWalletById(Guid Id);
        void AddWalletData(Wallet wallet);
        int GetMaxId();
    }
}
