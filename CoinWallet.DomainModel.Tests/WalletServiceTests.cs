using Autofac.Extras.Moq;
using CoinWallet.DomainModel.Constants;
using CoinWallet.DomainModel.DataModel;
using CoinWallet.DomainModel.Repository.Contracts;
using CoinWallet.DomainModel.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;

namespace CoinWallet.DomainModel.Tests
{
    public class WalletServiceTests
    {
        [Theory]
        //[ClassData(typeof(IncorrectGuidData))]
        [MemberData(nameof(InvalidGuidData))]
        public void GetWalletDetailsById_InvalidWalletId_ReturnsNotFoundEmptyResponse(Guid walletId)
        {
            using (var mock = AutoMock.GetLoose())
            {
                IEnumerable<Wallet> response = Enumerable.Empty<Wallet>();
                var walletRepo = new Mock<IWalletRepository>();
                walletRepo.Setup(p => p.GetWalletById(It.IsAny<Guid>())).Returns(response);
                WalletService walletService = new WalletService(walletRepo.Object);

                Wallet expected = new Wallet
                {
                    Coins = 0,
                    TransactionId = null,
                    Version = 0
                };

                var actual = walletService.GetWalletDetailsById(walletId);

                Assert.Equal(expected.Coins, actual.Coins);
                Assert.Equal(expected.TransactionId, actual.TransactionId);
                Assert.Equal(expected.Version, actual.Version);
            }
        }


        [Theory]
        [InlineData(1000, "tx123", 1, HttpStatusCode.Created, 1000)]
        public void CreateAndCreditWallet_NewWalletWithCredit_ReturnsCreatedStatusCodeAndBalance(int coins, string transactionId, int version, HttpStatusCode expHttpCode, int expBalance)
        {
            using (var mock = AutoMock.GetLoose())
            {
                Wallet payload = new Wallet
                {
                    WalletId = new Guid("1d4e7d81-ce9d-457b-b056-0f883baa783d"),
                    TransactionType = Const.Credit,
                    Coins = coins,
                    TransactionId = transactionId,
                    Version = version
                };

                IEnumerable<Wallet> response = Enumerable.Empty<Wallet>();
                var walletRepo = new Mock<IWalletRepository>();
                walletRepo.Setup(p => p.GetWalletById(It.IsAny<Guid>())).Returns(response);
                walletRepo.Setup(p => p.AddWalletData(payload));
                WalletService walletService = new WalletService(walletRepo.Object);
                var actual = walletService.CreateAndCreditWallet(payload);

                Assert.Equal(expHttpCode, actual.Item1);
                Assert.Equal(expBalance, actual.Item2);
            }
        }

        [Theory]
        [MemberData(nameof(ValidGuidData))]
        public void GetWalletDetailsById_ValidWalletId_ReturnsBalanceForAnExistingWallet(Guid walletId, string expTransactionId, int expVersion, int expCoins)
        {
            using (var mock = AutoMock.GetLoose())
            {
                Wallet expected = new Wallet
                {
                    WalletId = new Guid("1d4e7d81-ce9d-457b-b056-0f883baa783d"),
                    TransactionType = Const.Credit,
                    Coins = expCoins,
                    TransactionId = expTransactionId,
                    Version = expVersion
                };

                IEnumerable<Wallet> response = new List<Wallet> { expected };
                var walletRepo = new Mock<IWalletRepository>();
                walletRepo.Setup(p => p.GetWalletById(It.IsAny<Guid>())).Returns(response);
                walletRepo.Setup(p => p.AddWalletData(expected));
                WalletService walletService = new WalletService(walletRepo.Object);

                walletService.CreateAndCreditWallet(expected);
                var actual = walletService.GetWalletDetailsById(walletId);

                Assert.Equal(expected.Coins, actual.Coins);
                Assert.Equal(expected.TransactionId, actual.TransactionId);
                Assert.Equal(expected.Version, actual.Version);
            }
        }


        [Theory]
        [InlineData(1000, "tx123", 1, HttpStatusCode.Accepted, 1000)]
        public void CreateAndCreditWallet_DuplicateCredit_ReturnsStatusCodeAcceptedAndBalance(int coins, string transactionId, int version, HttpStatusCode expHttpCode, int expBalance)
        {
            using (var mock = AutoMock.GetLoose())
            {
                Wallet payload = new Wallet
                {
                    WalletId = new Guid("1d4e7d81-ce9d-457b-b056-0f883baa783d"),
                    TransactionType = Const.Credit,
                    Coins = coins,
                    TransactionId = transactionId,
                    Version = version
                };

                IEnumerable<Wallet> response = new List<Wallet>() { payload };
                var walletRepo = new Mock<IWalletRepository>();
                walletRepo.Setup(p => p.GetWalletById(It.IsAny<Guid>())).Returns(response);
                walletRepo.Setup(p => p.AddWalletData(payload));
                WalletService walletService = new WalletService(walletRepo.Object);
                
                var actual = walletService.CreateAndCreditWallet(payload);

                Assert.Equal(expHttpCode, actual.Item1);
                Assert.Equal(expBalance, actual.Item2);
            }
        }

        [Theory]
        [InlineData(2000, "tx120", HttpStatusCode.BadRequest)]
        public void DebitWallet_MoreThanBalance_ReturnsStatusCodeBadRequest(int coins, string transactionId, HttpStatusCode expHttpCode)
        {
            using (var mock = AutoMock.GetLoose())
            {
                Wallet credit = new Wallet
                {
                    WalletId = new Guid("1d4e7d81-ce9d-457b-b056-0f883baa783d"),
                    TransactionType = Const.Credit,
                    Coins = 1000,
                    TransactionId = "tx123",
                    Version = 1
                };

                IEnumerable<Wallet> response = new List<Wallet>() { credit };
                var walletRepo = new Mock<IWalletRepository>();
                walletRepo.Setup(p => p.GetWalletById(It.IsAny<Guid>())).Returns(response);
                walletRepo.Setup(p => p.AddWalletData(credit));
                WalletService walletService = new WalletService(walletRepo.Object);

                walletService.CreateAndCreditWallet(credit);

                Wallet debit = new Wallet
                {
                    WalletId = new Guid("1d4e7d81-ce9d-457b-b056-0f883baa783d"),
                    TransactionType = Const.Debit,
                    Coins = coins,
                    TransactionId = transactionId
                };

                var actual = walletService.DebitWallet(debit);

                Assert.Equal(expHttpCode, actual.Item1);
            }
        }

        [Theory]
        [InlineData(456, "tx120", HttpStatusCode.Created,544)]
        public void DebitWallet_LessThanBalance_ReturnsStatusCodeCreatedAndBalance(int coins, string transactionId, HttpStatusCode expHttpCode,int expBalance)
        {
            using (var mock = AutoMock.GetLoose())
            {
                Wallet credit = new Wallet
                {
                    WalletId = new Guid("1d4e7d81-ce9d-457b-b056-0f883baa783d"),
                    TransactionType = Const.Credit,
                    Coins = 1000,
                    TransactionId = "tx123",
                    Version = 1
                };

                IEnumerable<Wallet> response = new List<Wallet>() { credit };
                var walletRepo = new Mock<IWalletRepository>();
                walletRepo.Setup(p => p.GetWalletById(It.IsAny<Guid>())).Returns(response);
                walletRepo.Setup(p => p.AddWalletData(credit));
                WalletService walletService = new WalletService(walletRepo.Object);

                walletService.CreateAndCreditWallet(credit);

                Wallet debit = new Wallet
                {
                    WalletId = new Guid("1d4e7d81-ce9d-457b-b056-0f883baa783d"),
                    TransactionType = Const.Debit,
                    Coins = coins,
                    TransactionId = transactionId
                };

                var actual = walletService.DebitWallet(debit);

                Assert.Equal(expHttpCode, actual.Item1);
                Assert.Equal(expBalance, actual.Item2);
            }
        }

        [Theory]
        [InlineData(1000, "tx123", HttpStatusCode.Accepted,1000)]
        public void DebitWallet_DuplicateDebit_ReturnsStatusCodeAcceptedAndBalance(int coins, string transactionId, HttpStatusCode expHttpCode, int expBalance)
        {
            using (var mock = AutoMock.GetLoose())
            {
                Wallet credit = new Wallet
                {
                    WalletId = new Guid("1d4e7d81-ce9d-457b-b056-0f883baa783d"),
                    TransactionType = Const.Credit,
                    Coins = 1000,
                    TransactionId = "tx123",
                    Version = 1
                };

                IEnumerable<Wallet> response = new List<Wallet>() { credit };
                var walletRepo = new Mock<IWalletRepository>();
                walletRepo.Setup(p => p.GetWalletById(It.IsAny<Guid>())).Returns(response);
                walletRepo.Setup(p => p.AddWalletData(credit));
                WalletService walletService = new WalletService(walletRepo.Object);

                walletService.CreateAndCreditWallet(credit);

                Wallet debit = new Wallet
                {
                    WalletId = new Guid("1d4e7d81-ce9d-457b-b056-0f883baa783d"),
                    TransactionType = Const.Debit,
                    Coins = coins,
                    TransactionId = transactionId
                };

                var actual = walletService.DebitWallet(debit);

                Assert.Equal(expHttpCode, actual.Item1);
                Assert.Equal(expBalance, actual.Item2);
            }
        }

        public static IEnumerable<object[]> InvalidGuidData =>
        new List<object[]>
        {
            new object[] { new Guid("770f74c1-c8ad-4b85-9b78-dfd13fb9a71a") },
            new object[] { new Guid("764006e6-3dcf-4401-b3d4-3f47f22097d0") }
        };

        public static IEnumerable<object[]> ValidGuidData =>
        new List<object[]>
        {
             new object[] { new Guid("1d4e7d81-ce9d-457b-b056-0f883baa783d"), "tx123", 2, 544 }
        };

    }
}
