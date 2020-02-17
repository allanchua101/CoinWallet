namespace CoinWallet.DomainModel.DTO
{
    public class WalletResponseDTO
    {
        public string TransactionId { get; set; }
        public int Version { get; set; }
        public int Coins { get; set; }
    }
}
