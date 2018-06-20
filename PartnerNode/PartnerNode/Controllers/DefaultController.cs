using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nethereum.JsonRpc.Client;
using Nethereum.Web3;

namespace PartnerNode.Controllers
{
    [Produces("application/json")]
    [Route("api/Default")]
    public class DefaultController : Controller
    {
        [HttpGet]
        public string Get(string account) {
            var client = new RpcClient(new Uri("http://ethereum:8545"));

            var web3 = new Web3(client);
            
            var resp = web3.Eth.GetBalance.SendRequestAsync(account).Result;

            return resp.HexValue;
        }
    }
}