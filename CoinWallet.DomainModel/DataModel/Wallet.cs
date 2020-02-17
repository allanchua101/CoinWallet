using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoinWallet.DomainModel.DataModel
{
    public class Wallet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Guid WalletId { get; set; }

        public string TransactionId { get; set; }

        public string TransactionType { get; set; }

        public int Version { get; set; }

        public int Coins { get; set; }
    }
}
