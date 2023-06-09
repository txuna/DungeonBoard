using CloudStructures;
using CloudStructures.Structures;
using DungeonBoard.Models;
using DungeonBoard.Models.Account;
using DungeonBoard.Models.Room;
using Microsoft.Extensions.Options;

namespace DungeonBoard.Services
{
    public interface IMemoryDB
    {
        Task<ErrorCode> StoreRedisUser(int userId, string authToken, string email, UserState state);
        Task<(ErrorCode, RedisUser?)> LoadRedisUser(int userId);
        Task<(ErrorCode, int)> CreateRoom(string roomTitle, int userId, int headCount, int bossId);
        Task<(ErrorCode, int)> LoadUniqueRoomId();
        Task<ErrorCode> EnterRoom(int userId, int roomId);
    }
    public class MemoryDB : IMemoryDB
    {
        RedisConnection _redisConn;
        string redisUserKey = string.Empty;
        IOptions<DbConfig> _dbConfig;
        public MemoryDB(IOptions<DbConfig> dbConfig)
        {
            var config = new RedisConfig("default", dbConfig.Value.RedisDB);
            redisUserKey = dbConfig.Value.RedisUserKey;
            _redisConn = new RedisConnection(config);
            _dbConfig = dbConfig;
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

        async public Task<(ErrorCode, RedisRoom?)> LoadRoomFromId(int roomId)
        {
            try
            {
                var redis = new RedisString<RedisRoom>(_redisConn, Convert.ToString(roomId) + _dbConfig.Value.RedisRoomKey, null);
                var redisRoom = await redis.GetAsync();
                if(redisRoom.HasValue == false)
                {
                    return (ErrorCode.NoneExistRoom, null);
                }

                return (ErrorCode.None,  redisRoom.Value);
            }
            catch(Exception ex )
            {
                Console.WriteLine(ex.Message);
                return (ErrorCode.CannotConnectServer, null); 
            }
        }

        async public Task<ErrorCode> EnterRoom(int userId, int roomId)
        {
            try
            {
                
                return ErrorCode.None;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ErrorCode.CannotConnectServer;
            }
        }

        async public Task<(ErrorCode, int)> CreateRoom(string roomTitle, int userId, int headCount, int bossId)
        {
            try
            {

                return (ErrorCode.None, 0);
            }
            catch(Exception ex )
            {
                Console.WriteLine(ex.Message);
                return (ErrorCode.CannotConnectServer, -1);
            }
        }

        async public Task<(ErrorCode, int)> LoadUniqueRoomId()
        {
            try
            {
                var redis = new RedisString<int>(_redisConn, _dbConfig.Value.RedisUniqueRoomKey, null);
                var roomId = await redis.GetAsync();
                return (ErrorCode.None, roomId.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (ErrorCode.CannotConnectServer, -1);
            }
        }
    }
}
