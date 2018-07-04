using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PartnerNode.Models;

namespace PartnerNode.Controllers
{
    [Route("api/[controller]/[action]")]
    [RequestTrack]
    public class AdminController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> AddPeer(ReqAddPeer req) {
            using (var c = new HttpClient()) {
                var resp = await c.PostAsync("http://ethereum:8545",
                    new StringContent(JsonConvert.SerializeObject(new JsonReq {
                            Method = "admin_addPeer",
                            Params = new []{ req.Peer}
                        }),
                        Encoding.UTF8,
                        "application/json"));

                var json = await resp.Content.ReadAsStringAsync();

                return new OkObjectResult(new RespAddPeer() {
                    Ret = Newtonsoft.Json.JsonConvert.DeserializeObject(json)
                });
            }
        }




    }
}