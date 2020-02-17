using AutoMapper;
using CoinWallet.DomainModel.DataModel;
using CoinWallet.Web.Models;

namespace CoinWallet
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<WalletRequest, Wallet>();
        }
    }
}