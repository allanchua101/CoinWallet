using System;
using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using CoinWallet.DomainModel.DataModel;
using CoinWallet.DomainModel.Services.Contracts;
using CoinWallet.Web.Models;

namespace CoinWallet.Web.Controllers
{
    [Route("[controller]/{walletId:guid}")]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IWalletService _service;

        public WalletsController(IMapper mapper,IWalletService service)
        {
            _mapper = mapper;
            _service = service;
        }

        // GET wallets/{walletId}
        [HttpGet]
        public ActionResult Get(Guid walletId)
        {
            var walletResponse = _service.GetWalletDetailsById(walletId);
            if(string.IsNullOrEmpty(walletResponse.TransactionId))
            {
                return NotFound("Requested wallet not found");
            }
            else
            {
                return new JsonResult(walletResponse);
            }
        }

        // POST wallets/{walletId}/Credit
        [HttpPost]
        [Route("Credit")]
        public ActionResult Credit(Guid walletId,[FromBody] WalletRequest data)
        {
            data.WalletId = walletId;
            var walletData = _mapper.Map<WalletRequest,Wallet>(data);

            var result = _service.CreateAndCreditWallet(walletData);
            var statusCode = result.Item1;
            if(statusCode == HttpStatusCode.Accepted)
            {
                return Accepted(JObject.FromObject(new { balance = result.Item2 }));
            }
            else
            {
                return CreatedAtAction(nameof(Credit), JObject.FromObject(new { balance = result.Item2 }));
            }
        }

        // POST wallets/{walletId}/Debit
        [HttpPost]
        [Route("Debit")]
        public ActionResult Debit(Guid walletId, [FromBody] WalletRequest data)
        {
            data.WalletId = walletId;
            var walletData = _mapper.Map<WalletRequest, Wallet>(data);

            var result = _service.DebitWallet(walletData);
            var statusCode = result.Item1;
            if (statusCode == HttpStatusCode.Accepted)
            {
                return Accepted(JObject.FromObject(new { balance = result.Item2 }));
            }
            else if(statusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest();
            }
            else
            {
                return CreatedAtAction(nameof(Debit), JObject.FromObject(new { balance = result.Item2 }));
            }
        }
    }
}
