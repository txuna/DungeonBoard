using CloudStructures.Structures;
using DungeonBoard.Models;
using DungeonBoard.Models.Account;
using DungeonBoard.ReqResModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DungeonBoard.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class VerifyUserMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Services.IMemoryDB _memoryDB;
        private readonly string[] exclusivePaths = { "/Login", "/Register" };

        int userId;
        string authToken;
        public VerifyUserMiddleware(RequestDelegate next, Services.IMemoryDB memoryDB)
        {
            _next = next;
            _memoryDB = memoryDB;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();

            using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 4096, true))
            {
                var bodyStr = await reader.ReadToEndAsync();

                if (await IsNullBodyDataThenSendError(httpContext, bodyStr))
                {
                    await WriteResponse(httpContext, ErrorCode.InValidRequestHttpBody);
                    return;
                }

                // else Login or Register
                if (CheckExclusivePath(httpContext) == false)
                {
                    var document = JsonDocument.Parse(bodyStr);
                    if (IsInvalidJsonFormatThenSendError(document, out userId, out authToken) == false)
                    {
                        await WriteResponse(httpContext, ErrorCode.InvalidJsonFormat);
                        return;
                    }

                    var (Result, redisUser) = await _memoryDB.LoadRedisUser(userId);
                    if (Result != ErrorCode.None)
                    {
                        await WriteResponse(httpContext, Result);
                        return;
                    }

                    if (VerifyAccount(redisUser) == false)
                    {
                        await WriteResponse(httpContext, ErrorCode.InvalidAuthToken);
                        return;
                    }

                    httpContext.Items["Redis-User"] = redisUser;
                }
            }

            httpContext.Request.Body.Position = 0;
            await _next(httpContext);
        }

        private bool IsInvalidJsonFormatThenSendError(JsonDocument document, out int userId, out string authToken)
        {
            try
            {
                userId = document.RootElement.GetProperty("UserId").GetInt32();
                authToken = document.RootElement.GetProperty("AuthToken").GetString();

                return true;
            }
            catch
            {
                userId = -1; authToken = "";
                return false;
            }
        }

        private bool VerifyAccount(RedisUser redisUser)
        {
            if(redisUser.AuthToken != authToken)
            {
                return false;
            }
            return true;
        }

        private bool CheckExclusivePath(HttpContext httpContext)
        {
            string rpath = httpContext.Request.Path;

            foreach (var path in exclusivePaths)
            {
                if (rpath.Contains(path))
                {
                    return true;
                }
            }
            return false;
        }

        private async Task<bool> IsNullBodyDataThenSendError(HttpContext context, string bodyStr)
        {
            if (string.IsNullOrEmpty(bodyStr) == false)
            {
                return false;
            }
            return true;
        }

        private async Task WriteResponse(HttpContext context, ErrorCode Result)
        {
            var errorJsonResponse = JsonConvert.SerializeObject(new MiddlewareResponse
            {
                Result = Result
            }); ;
            var bytes = Encoding.UTF8.GetBytes(errorJsonResponse);
            await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class VerifyUserMiddlewareExtensions
    {
        public static IApplicationBuilder UseVerifyUserMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<VerifyUserMiddleware>();
        }
    }
}
