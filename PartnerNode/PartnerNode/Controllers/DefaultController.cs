using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ipfs.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nethereum.JsonRpc.Client;
using Nethereum.Web3;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PartnerNode.Models;

namespace PartnerNode.Controllers
{
    [Route("api/[controller]/[action]")]
    public class DefaultController : Controller
    {
        [HttpGet]
        public string Get(string account) {
            var client = new RpcClient(new Uri("http://ethereum:8545"));

            var web3 = new Web3(client);
            
            var resp = web3.Eth.GetBalance.SendRequestAsync(account).Result;
            

            return resp.HexValue;
        }
        
        [HttpGet]
        public async Task<HttpResponseMessage> GetFile(string cid) {
            var client = new IpfsClient("http://ipfs:5001");

            var node = await client.DownloadBytesAsync("cat", default(CancellationToken), cid);

            return new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new ByteArrayContent(node)
            };
        }

        [HttpPost]
        [HttpGet]
        public async Task<string> AddPeer() {
            using (var c = new HttpClient()) {
                var resp = await c.PostAsync("http://ethereum:8545",
                    new StringContent(JsonConvert.SerializeObject(new JsonReq {
                            Method = "admin_addPeer",
                            Params = new []{ "enode://91f146ad02496bbc2cbb6c6b0619d242cb8b8a512dbafd612dc1320edb02b2b016db9e7df36280cae6a33d1f9985448abfee55934833f44750f827be75a06bb6@182.254.129.109:30333"}
                        }),
                        Encoding.UTF8,
                        "application/json"));

                var json = await resp.Content.ReadAsStringAsync();

                return json;
            }
        }
    }
}