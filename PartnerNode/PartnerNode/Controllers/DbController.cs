using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Numerics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Ipfs.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.JsonRpc.Client;
using Nethereum.Web3;
using PartnerNode.Models;

namespace PartnerNode.Controllers
{
    [Route("api/[controller]/[action]")]
    public class DbController : Controller {

        private static readonly string MasterSqlConn = "server=mssql;User Id=sa;password=mssqlP@ssw0rd;Database=master;";
        private static readonly string ChainSqlConn = "server=mssql;User Id=sa;password=mssqlP@ssw0rd;Database=ChainDatabase;";


        private static readonly string FileContractAbi =
                "[{\"constant\":true,\"inputs\":[{\"name\":\"user\",\"type\":\"address\"},{\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"GetFileByAddressAndIndex\",\"outputs\":[{\"name\":\"fileName\",\"type\":\"string\"},{\"name\":\"desKey\",\"type\":\"string\"},{\"name\":\"displayname\",\"type\":\"string\"},{\"name\":\"createTime\",\"type\":\"uint32\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"name\",\"type\":\"string\"}],\"name\":\"GetFileByName\",\"outputs\":[{\"name\":\"desKey\",\"type\":\"string\"},{\"name\":\"fileDisplayName\",\"type\":\"string\"},{\"name\":\"createTime\",\"type\":\"uint32\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"ContractOwner\",\"outputs\":[{\"name\":\"\",\"type\":\"address\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"writer\",\"type\":\"address\"}],\"name\":\"AddWriter\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"\",\"type\":\"address\"}],\"name\":\"FileWriters\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"writer\",\"type\":\"address\"}],\"name\":\"RemoveWriter\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"user\",\"type\":\"address\"},{\"name\":\"fileName\",\"type\":\"string\"},{\"name\":\"displayname\",\"type\":\"string\"},{\"name\":\"key\",\"type\":\"string\"},{\"name\":\"createTime\",\"type\":\"uint32\"}],\"name\":\"AddFile\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"user\",\"type\":\"address\"}],\"name\":\"GetFileCountByAddress\",\"outputs\":[{\"name\":\"fileCount\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"writer\",\"type\":\"address\"},{\"indexed\":true,\"name\":\"filename\",\"type\":\"string\"}],\"name\":\"FileCreated\",\"type\":\"event\"}]"
            ;


        [HttpPost]
        public IActionResult InitDb() {
            using (var db = new SqlConnection(MasterSqlConn)) {

                db.Execute(@"
if exists (select * From master.dbo.sysdatabases where name='ChainDatabase')
begin
	return
end
else
begin
	CREATE DATABASE ChainDatabase
end

");

                db.Execute(@"

use ChainDatabase
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[IpfsContent]') AND type in (N'U'))
begin
    CREATE TABLE [dbo].[IpfsContent](
	    [Id] [int] IDENTITY(1,1) NOT NULL,
	    [Account] [varchar](50) NOT NULL,
	    [FileName] [varchar](500) NOT NULL,
	    [FileContent] [ntext] NULL,
	    [FileCreateTime] [bigint] NOT NULL,
	    [DesKey] [varchar](500) NOT NULL
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

    CREATE CLUSTERED INDEX [ClusteredIndex_IpfsContent] ON [dbo].[IpfsContent]
    (
	    [Account] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    
end
");

                return new OkResult();
            }
        }

        [HttpPost]
        public async Task<IActionResult> LoadUserFile(ReqLoadUserFile req) {
            var client = new RpcClient(new Uri("http://ethereum:8545"));
            var web3 = new Web3(client);
            var contract = web3.Eth.GetContract(FileContractAbi, req.ContractAddress);
            var function = contract.GetFunction("GetFileCountByAddress");
            var ret = await contract.Eth.Transactions.Call.SendRequestAsync(contract.GetFunction("GetFileCountByAddress").CreateCallInput(req.Account));

            var fileCount = BigInteger.Parse(ret.Replace("0x",""), NumberStyles.HexNumber);

            for (int i = 0; i < fileCount; i++) {
                ret = await contract.Eth.Transactions.Call.SendRequestAsync(contract.GetFunction("GetFileByAddressAndIndex").CreateCallInput(req.Account,i));

                var pd = new ParameterDecoder();

                var obj = new GetFileByAddressAndIndex();

                var declaredProperties = (typeof(GetFileByAddressAndIndex)).GetTypeInfo().DeclaredProperties;
                pd.DecodeAttributes(ret, obj, declaredProperties.ToArray());


                var dekey = CryptoHelper.RsaDecode(req.FileKey, obj.desKey);

                var ipfsClient = new IpfsClient("http://ipfs:5001");

                var node = await ipfsClient.DownloadBytesAsync("cat", default(CancellationToken), obj.fileName);

                var buff = CryptoHelper.DesDecode(dekey, node);

                var content = System.Text.Encoding.UTF8.GetString(buff);

                using (var db = new SqlConnection(ChainSqlConn)) {

                    db.Execute(@"
if not exists (select 0 from [dbo].[IpfsContent] where [FileName]=@FileName)
begin
    INSERT INTO [dbo].[IpfsContent]
           ([Account]
           ,[FileName]
           ,[FileContent]
           ,[FileCreateTime]
           ,[DesKey])
     VALUES
           (@Account
           ,@FileName
           ,@FileContent
           ,@FileCreateTime
           ,@DesKey)
end
",new {
                        Account = req.Account,
                        FileName = obj.fileName,
                        FileContent = content,
                        FileCreateTime = Convert.ToInt64(obj.createTime),
                        DesKey = dekey
                    });


                }




            }



            


            return new OkObjectResult(new RespLoadUserFile() {
                Ret = ret
            });
        }

        [FunctionOutput]
        public class GetFileByAddressAndIndex {
            [Parameter("string", "fileName", 1)]
            public string fileName { get; set; }

            [Parameter("string", "desKey", 2)]
            public string desKey { get; set; }
            
            [Parameter("string", "displayname", 3)]
            public string displayname { get; set; }

            [Parameter("uint32", "createTime", 4)]
            public UInt32 createTime { get; set; }
        }
    }
}