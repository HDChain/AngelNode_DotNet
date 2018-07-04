using System;
using System.IO;
using System.Text;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PartnerNode.Models;

namespace PartnerNode.Controllers {
    public static class ApiHelper {
        private static readonly ILog Logger = LogManager.GetLogger(Log4NetCore.CoreRepository, typeof(ApiHelper));

        public static OkObjectResult MakeExceptionResp(this Controller controller,Exception ex) {
            return new OkObjectResult(new BaseResp() {
                Error = ex.Message,
                Result = false
            });

        }

        public static OkObjectResult MakeBaseResp(this Controller controller, string msg) {
            return new OkObjectResult(new BaseResp() {
                Error = msg,
                Result = false
            });
        }

        public static void CheckJsonParams<T>(this Controller controller,ref T obj) {
            var request = controller.Request;

            if (request.ContentType?.Equals("application/json", StringComparison.OrdinalIgnoreCase) == true)
                try {
                    using (var sr = new StreamReader(request.Body)) {
                        var json = sr.ReadToEnd();
                        if (!string.IsNullOrEmpty(json))
                            obj = JsonConvert.DeserializeObject<T>(json);

                        request.HttpContext.Items["reqStr"] = json;
                    }
                } catch (Exception ex) {
                    Logger.Error(ex);
                }
        }

        
    }
}