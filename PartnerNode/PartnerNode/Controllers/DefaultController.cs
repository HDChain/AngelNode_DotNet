using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
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

        
        
        [HttpGet]
        public string Sql() {
            using (var db = new SqlConnection("server=mssql;User Id=sa;password=mssqlP@ssw0rd;Database=test01;")) {

                var json = db.QueryFirstOrDefault<string>("select Json from table001 where Id = 1");

                return json;
            }
        }

        
    }
}