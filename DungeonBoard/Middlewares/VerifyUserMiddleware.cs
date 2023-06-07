using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DungeonBoard.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class VerifyUserMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Services.IMemoryDB _memoryDB;

        public VerifyUserMiddleware(RequestDelegate next, Services.IMemoryDB memoryDB)
        {
            _next = next;
            _memoryDB = memoryDB;
        }

        public Task Invoke(HttpContext httpContext)
        {

            return _next(httpContext);
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
