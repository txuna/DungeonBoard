using DungeonBoard.Middlewares;
using DungeonBoard.Models;
using DungeonBoard.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
/* 
 설정값 읽어 들이고 서비스 객체로 등록
 */
IConfiguration configuration = builder.Configuration;

builder.Services.Configure<DbConfig>(configuration.GetSection(nameof(DbConfig)));
builder.Services.AddTransient<IAccountDB, AccountDB>();
builder.Services.AddSingleton<IMemoryDB,  MemoryDB>();

var app = builder.Build();

app.UseRouting();
// 인증 확인 미들웨어
app.UseVerifyUserMiddleware();
//app.UseAuthenticationMiddleware(configuration.GetSection("DbConfig")["RedisDB"]);
//app.MapControllers();
#pragma warning disable ASP0014
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
#pragma warning restore ASP0014


app.Run();
