using DungeonBoard.Middlewares;
using DungeonBoard.Models;
using DungeonBoard.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
/* 
 ������ �о� ���̰� ���� ��ü�� ���
 */
IConfiguration configuration = builder.Configuration;

builder.Services.Configure<DbConfig>(configuration.GetSection(nameof(DbConfig)));
builder.Services.AddTransient<IAccountDB, AccountDB>();
builder.Services.AddSingleton<IMemoryDB,  MemoryDB>();

var app = builder.Build();

app.UseRouting();
// ���� Ȯ�� �̵����
app.UseVerifyUserMiddleware();
//app.UseAuthenticationMiddleware(configuration.GetSection("DbConfig")["RedisDB"]);
//app.MapControllers();
#pragma warning disable ASP0014
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
#pragma warning restore ASP0014


app.Run();
