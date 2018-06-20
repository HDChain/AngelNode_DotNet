using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Ipfs.Api;
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
        [Route("Get")]
        [HttpGet]
        public string Get(string account) {
            var client = new RpcClient(new Uri("http://ethereum:8545"));

            var web3 = new Web3(client);
            
            var resp = web3.Eth.GetBalance.SendRequestAsync(account).Result;

            return resp.HexValue;
        }

        [Route("GetFile")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetFile(string cid) {
            var client = new IpfsClient("http://ipfs:5001");

            var node = await client.DownloadBytesAsync("cat", default(CancellationToken), cid);

            return new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new ByteArrayContent(node)
            };
        }
    }
}