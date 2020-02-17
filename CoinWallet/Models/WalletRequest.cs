using System;

namespace CoinWallet.Web.Models
{
    public class WalletRequest
    {
        public Guid WalletId { get; set; }
        public string TransactionId { get; set; }
        public int Coins { get; set; }
    }
}
