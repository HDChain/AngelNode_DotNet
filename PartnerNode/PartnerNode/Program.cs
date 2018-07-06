using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PartnerNode.Models;

namespace PartnerNode
{
    public class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));
        public static void Main(string[] args)
        {
            var fi = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config"));
            ILoggerRepository repository = LogManager.CreateRepository(Log4NetCore.CoreRepository);
            XmlConfigurator.Configure(repository, fi);

            var sqlInit = false;

            while (!sqlInit) {
                sqlInit = DbHelper.Instance.MsSqlInit();

                if (!sqlInit) {
                    Logger.Debug("sql is not ready");
                    Thread.Sleep(1000);
                }
                
            }
            

            FetchUserDataTask.Instance.Start();
            BuildWebHost(args).Run();
            FetchUserDataTask.Instance.Stop();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
