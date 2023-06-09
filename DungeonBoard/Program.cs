using CloudStructures;
using CloudStructures.Structures;
using DungeonBoard.Middlewares;
using DungeonBoard.Models;
using DungeonBoard.Services;
using Microsoft.AspNetCore.Components.Forms;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
/* 
 설정값 읽어 들이고 서비스 객체로 등록
 */
IConfiguration configuration = builder.Configuration;

builder.Services.Configure<DbConfig>(configuration.GetSection(nameof(DbConfig)));
builder.Services.AddTransient<IAccountDB, AccountDB>();
builder.Services.AddTransient<IPlayerDB, PlayerDB>();   

builder.Services.AddSingleton<IMemoryDB,  MemoryDB>();
builder.Services.AddSingleton<IMasterDB, MasterDB>();



var app = builder.Build();

app.UseRouting();
// 인증 확인 미들웨어
app.UseVerifyUserMiddleware();
//app.UseAuthenticationMiddleware(configuration.GetSection("DbConfig")["RedisDB"]);
//app.MapControllers();
#pragma warning disable ASP0014
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
#pragma warning restore ASP0014

if(await LoadRoomIndex() == false)
{
    return;
}

app.Run();


async Task<bool> LoadRoomIndex()
{
    var config = new RedisConfig("default", configuration.GetSection("DbConfig")["RedisDB"]);
    var _redisConn = new RedisConnection(config);
    int uniqueId = 1;

    var redis = new RedisString<int>(_redisConn, configuration.GetSection("DbConfig")["RedisUniqueRoomKey"], null);
    try
    {
        var roomIndex = await redis.GetAsync();
        if (roomIndex.HasValue)
        {
            uniqueId = roomIndex.Value;
        }

        await redis.SetAsync(uniqueId);
        return true;
    }
    catch(Exception ex)
    {
        Console.WriteLine(ex.Message);
        return false;
    }
}