using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CoinWallet.DomainModel.DTO;
using CoinWallet.DomainModel.DataModel;
using CoinWallet.DomainModel.Repository.Contracts;
using CoinWallet.DomainModel.Services.Contracts;
using CoinWallet.DomainModel.Constants;

namespace CoinWallet.DomainModel.Services
{
    public class WalletService : IWalletService
    {
        private IWalletRepository _repository { get; }

        public WalletService(IWalletRepository repository)
        {
            _repository = repository;
        }

        public WalletResponseDTO GetWalletDetailsById(Guid walletId)
        {
            var walletResponse = new WalletResponseDTO();
            
            try
            {
                var walletData = _repository.GetWalletById(walletId);

                if (walletData != null && walletData.Count() > 0)
                {
                    var coinBalance = CalculateBalance(walletData);
                    var lastTransaction = walletData.OrderByDescending(i => i.Id).FirstOrDefault();

                    walletResponse.TransactionId = lastTransaction.TransactionId;
                    walletResponse.Version = lastTransaction.Version;
                    walletResponse.Coins = coinBalance;
                }
            }
            catch(Exception Ex)
            {
                //Some Logging here (if required)
                throw;
            }

            return walletResponse;
        }

        public Tuple<HttpStatusCode,int> CreateAndCreditWallet(Wallet wallet)
        {
            int walletBalance = 0;
            var StatusCode = new HttpStatusCode();

            try
            {
                var existingTransactions = _repository.GetWalletById(wallet.WalletId);
                if (existingTransactions != null && existingTransactions.Count(i => i.TransactionId == wallet.TransactionId) > 0)
                {
                    walletBalance = CalculateBalance(existingTransactions);
                    StatusCode = HttpStatusCode.Accepted;
                }
                else
                {
                    wallet.TransactionType = Const.Credit;

                    if (existingTransactions != null && existingTransactions.Count() > 0)
                    {
                        wallet.Version = existingTransactions.Max(i => i.Version) + 1;
                        walletBalance = CalculateBalance(existingTransactions) + wallet.Coins;
                    }
                    else
                    {
                        wallet.Version = 1;
                        walletBalance = wallet.Coins;
                    }

                    _repository.AddWalletData(wallet);
                    StatusCode = HttpStatusCode.Created;
                }
            }
            catch (Exception Ex)
            {
                //Some Logging here (if required)
                throw;
            }

            return new Tuple<HttpStatusCode, int>(StatusCode,walletBalance);
        }

        public Tuple<HttpStatusCode, int> DebitWallet(Wallet wallet)
        {
            int walletBalance = 0;
            
            var StatusCode = new HttpStatusCode();

            try
            {
                var existingTransactions = _repository.GetWalletById(wallet.WalletId);
                var currentBalance = CalculateBalance(existingTransactions);
                if (existingTransactions != null && existingTransactions.Count(i => i.TransactionId == wallet.TransactionId) > 0)
                {
                    walletBalance = currentBalance;
                    StatusCode = HttpStatusCode.Accepted;
                }
                else if (existingTransactions != null && currentBalance < wallet.Coins)
                {
                    StatusCode = HttpStatusCode.BadRequest;
                }
                else
                {
                    wallet.TransactionType = Const.Debit;

                    if (existingTransactions != null && existingTransactions.Count() > 0)
                    {
                        wallet.Version = existingTransactions.Max(i => i.Version) + 1;
                        _repository.AddWalletData(wallet);

                        walletBalance = currentBalance - wallet.Coins;
                        StatusCode = HttpStatusCode.Created;
                    }
                    else
                    {
                        StatusCode = HttpStatusCode.BadRequest;
                    }
                }
            }
            catch
            {
                //Some Logging here (if required)
                throw;
            }

            return new Tuple<HttpStatusCode, int>(StatusCode, walletBalance);
        }

        public int CalculateBalance(IEnumerable<Wallet> walletTransactions)
        {
            if (walletTransactions != null && walletTransactions.Count() > 0)
            {
                var Credits = walletTransactions.Where(i => i.TransactionType == Const.Credit).Sum(i => i.Coins);
                var Debits = walletTransactions.Where(i => i.TransactionType == Const.Debit)?.Sum(i => i.Coins);

                return (Debits != null) ? (Credits - Convert.ToInt32(Debits)) : Credits;
            }

            return 0;
        }
    }
}
