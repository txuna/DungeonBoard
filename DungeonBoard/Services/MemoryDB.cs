using CloudStructures;
using CloudStructures.Structures;
using DungeonBoard.Models;
using DungeonBoard.Models.Account;
using Microsoft.Extensions.Options;

namespace DungeonBoard.Services
{
    public interface IMemoryDB
    {
        Task<ErrorCode> StoreRedisUser(int userId, string authToken, string email, UserState state);
        Task<(ErrorCode, RedisUser?)> LoadRedisUser(int userId);
    }
    public class MemoryDB : IMemoryDB
    {
        RedisConnection _redisConn;
        string redisUserKey = string.Empty;
        public MemoryDB(IOptions<DbConfig> dbConfig)
        {
            var config = new RedisConfig("default", dbConfig.Value.RedisDB);
            redisUserKey = dbConfig.Value.RedisUserKey;
            _redisConn = new RedisConnection(config);
        }

        async public Task<ErrorCode> StoreRedisUser(int userId, string authToken, string email, UserState state)
        {
            try
            {
                TimeSpan expiration = TimeSpan.FromMinutes(60);

                RedisUser redisUser = new RedisUser
                {
                    UserId = userId,
                    AuthToken = authToken,
                    Email = email,
                    State = state,
                };

                var redis = new RedisString<RedisUser>(_redisConn, userId+redisUserKey, expiration);
                if(await redis.SetAsync(redisUser, expiration) == false)
                {
                    return ErrorCode.CannotConnectServer;
                }

                return ErrorCode.None;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ErrorCode.CannotConnectServer;
            }
        }

        async public Task<(ErrorCode, RedisUser?)> LoadRedisUser(int userId)
        {
            try
            {
                var redis = new RedisString<RedisUser>(_redisConn, userId+redisUserKey, null);
                var user = await redis.GetAsync(); 
                if(!user.HasValue)
                {
                    return (ErrorCode.NoneExistUserIdInRedis, null);
                }

                return (ErrorCode.None, user.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (ErrorCode.CannotConnectServer, null);
            }
        }
    }
}
