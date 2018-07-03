using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Internal;

namespace PartnerNode.Controllers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequestTrack: ActionFilterAttribute {
        private static readonly ILog Logger = LogManager.GetLogger(Log4NetCore.CoreRepository,"Track");
        private static readonly string timeKey = "enterTime";
        private static readonly string reqKey = "reqStr";

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
            
            context.HttpContext.Items[timeKey] = DateTime.Now.ToBinary();
            
            
            return base.OnActionExecutionAsync(context, next);
            
        }

        public override void OnActionExecuted(ActionExecutedContext context) {

            if (!context.HttpContext.Items.TryGetValue(timeKey, out var beginTime))
                return;
            
            var time = DateTime.FromBinary(Convert.ToInt64(beginTime));
            var request = context.HttpContext.Request;

            var contextActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

            var log = new Dictionary<string, object> {
                ["controller"] = context.Controller.GetType().Name,
                ["action"] = contextActionDescriptor?.ActionName,
                ["cost"] = (int) (DateTime.Now - time).TotalMilliseconds,
                ["ip"] = GetUserIp(context.HttpContext),
                ["req"] = GetRequestValues(context)
            };
            var requestUriQuery = request.GetUri().Query;
            if (!string.IsNullOrEmpty(requestUriQuery))
                log["q"] = requestUriQuery;
            

            log["resp"] = GetResponseValues(context);
            if (context.Exception != null)
                log["ex"] = context.Exception.Message;

            Logger.Debug(log);
            

            base.OnActionExecuted(context);
        }



        public static string GetUserIp(Microsoft.AspNetCore.Http.HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.ToString();
            }
            return ip;
        }

        public string GetResponseValues(ActionExecutedContext actionExecutedContext) {
            var result = actionExecutedContext.Result as ObjectResult;

            if (result == null) {
                return string.Empty;
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(result.Value);
        }

        public string GetRequestValues(ActionExecutedContext actionExecutedContext) {

            switch (actionExecutedContext.HttpContext.Request.ContentType?.ToLower()) {
                case "application/json": {
                    using (var sr = new StreamReader(actionExecutedContext.HttpContext.Request.Body)) {
                        var str = sr.ReadToEnd();
                        if (str.Length == 0) {
                            if (actionExecutedContext.HttpContext.Items.ContainsKey(reqKey))
                                str = actionExecutedContext.HttpContext.Items[reqKey].ToString();
                        }

                        return str;
                    }
                }
                case "application/x-www-form-urlencoded": {
                    var sb = new StringBuilder();
                    foreach (var item in actionExecutedContext.HttpContext.Request.Form) {
                        sb.Append($"{item.Key}={item.Value.Join(";")}");
                        return sb.ToString();
                    }
                }
                       break;
            }
            
            return string.Empty;
        }

        private static bool SkipLogging(ActionExecutingContext actionContext) {
            
            return actionContext.Controller.GetType().GetCustomAttributes<NoLogAttribute>().Any();
        }

        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
        public class NoLogAttribute : Attribute {
        }
    }
}
