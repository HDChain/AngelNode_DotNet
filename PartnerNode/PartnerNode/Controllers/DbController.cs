using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Ipfs.Api;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.JsonRpc.Client;
using Nethereum.Web3;
using PartnerNode.Models;

namespace PartnerNode.Controllers {
    [Route("api/[controller]/[action]")]
    [EnableCors("any")]
    public class DbController : Controller {
       
        [HttpPost]
        public IActionResult InitDb() {
            DbHelper.Instance.MsSqlInit();

            return new OkObjectResult(new BaseResp {
                Result = true
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAccountList() {
            using (var db = new SqlConnection(DbHelper.Instance.ChainSqlConn)) {

                return new OkObjectResult(await db.QueryAsync("select * from [dbo].[DataAccount]"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(ReqDeleteAccount req) {
            using (var db = new SqlConnection(DbHelper.Instance.ChainSqlConn)) {
                
                await db.ExecuteAsync("delete from  [dbo].[DataAccount] where Id=@Id",
                    new {
                        Id = req.Id
                    });

            }

            return new OkObjectResult(new BaseResp {
                Result = true
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddUserDataAccount(ReqAddUserDataAccount req) {

            using (var db = new SqlConnection(DbHelper.Instance.ChainSqlConn)) {
                await db.ExecuteAsync(@"
            
if not exists( select * from [dbo].[DataAccount] where Account=@Account and ContractAddress=@ContractAddress) 
begin
    INSERT INTO [dbo].[DataAccount]
   ([Account]
   ,[FileKey]
   ,[ContractAddress]
   ,[LastSyncTime])
     VALUES
   (@Account
   ,@FileKey
   ,@ContractAddress
   ,getdate())
end
" , new {
                    req.Account,
                    req.FileKey,
                    req.ContractAddress
                });
            }

            return new OkObjectResult(new BaseResp {
                Result = true
            });

        }

        [HttpPost]
        public async Task<IActionResult> QueryHealthPressure(ReqQueryHealthPressure req) {
            using (var db = new SqlConnection(DbHelper.Instance.ChainSqlConn)) {

                var ret = new RespQueryHealthPressure();
                ret.Items.AddRange(await db.QueryAsync<RespQueryHealthPressure.Item>("select [BuildTime],[HighPressure],[LowPressure],[Pulse] from [dbo].[View_HealthPressure] where Account=@Account and BuildTime between @DateStart and @DateEnd order by BuildTime",new {
                    req.Account,
                    req.DateStart,
                    req.DateEnd
                }));

                return new OkObjectResult(ret);

            }

            
        }
        
    }
}