using System;
using System.Net;
using CoinWallet.DomainModel.DTO;
using CoinWallet.DomainModel.DataModel;

namespace CoinWallet.DomainModel.Services.Contracts
{
    public interface IWalletService
    {
        WalletResponseDTO GetWalletDetailsById(Guid walletId);

        Tuple<HttpStatusCode, int> CreateAndCreditWallet(Wallet wallet);

        Tuple<HttpStatusCode, int> DebitWallet(Wallet wallet);
    }
}
