using Individual_Identity.Services;
using Individual_Identity.Services.Errors;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;

namespace Individual_Identity.Utils.Filter
{
    public class ErrorHandlingFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; set; } = int.MaxValue - 10;
        public void OnActionExecuting(ActionExecutingContext context) { }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            try
            {
                var ex = context.Exception;
                object errors = null;
                switch (ex)
                {

                    case RestException re:
                        errors = re.Errors;
                        context.HttpContext.Response.StatusCode = (int)re.Code;
                        break;

                    case Exception e:
                        errors = e.Message;
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }
                context.ExceptionHandled = true;
                context.HttpContext.Response.ContentType = "application/json";
                if (errors != null)
                {
                    RestResponse restResponse = new RestResponse();

                    var result = JsonConvert.SerializeObject(new RestResponse()
                    {
                        Status = RestResponse.Error,
                        Message = string.Join("," + Environment.NewLine, errors)
                    }, new JsonSerializerSettings
                    {
                        ContractResolver = new DefaultContractResolver
                        {
                            NamingStrategy = new CamelCaseNamingStrategy()
                        },
                        Formatting = Formatting.Indented
                    });

                    context.HttpContext.Response.WriteAsync(result).Wait();
                }
            }
            catch
            {
                return;
            }
        }
    }
}
