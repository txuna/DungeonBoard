using CloudStructures;
using CloudStructures.Structures;
using DungeonBoard.Models;
using DungeonBoard.Models.Account;
using DungeonBoard.Models.Room;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace DungeonBoard.Services
{
    public interface IMemoryDB
    {
        Task<ErrorCode> StoreRedisUser(int userId, string authToken, string email, UserState state);
        Task<ErrorCode> StoreRedisUser(int userId, RedisUser redisUser);
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

        async public Task<ErrorCode> StoreRedisUser(int userId, RedisUser redisUser)
        {
            try
            {
                TimeSpan expiration = TimeSpan.FromMinutes(60);
                var redis = new RedisString<RedisUser>(_redisConn, userId + redisUserKey, expiration);
                if (await redis.SetAsync(redisUser, expiration) == false)
                {
                    return ErrorCode.CannotConnectServer;
                }

                return ErrorCode.None;
            }
            catch (Exception ex)
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
                var script =
@"
local room = cjson.decode(redis.call('GET', KEYS[1]))
if #room.Users < room.HeadCount then 
    table.insert(room.Users, tonumber(ARGV[1]))
    redis.call('SET', KEYS[1], cjson.encode(room))
    return 1
else
    return 0
end
";
                var roomIdString = Convert.ToString(roomId) + _dbConfig.Value.RedisRoomKey;
                var redis = new RedisLua(_redisConn, roomIdString);
                var keys = new RedisKey[] { roomIdString };
                var values = new RedisValue[] { userId };
                var result = await redis.ScriptEvaluateAsync<int>(script, keys, values);

                if(result.Value != 1)
                {
                    return ErrorCode.AlreadyFullRoom;
                }
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
                var (Result, roomId) = await LoadUniqueRoomId();
                RedisRoom redisRoom = new RedisRoom
                {
                    RoomId = roomId,
                    Title = roomTitle,
                    HeadCount = headCount,
                    Users = new int[] { userId },
                    HostUserId = userId,
                    BossId = bossId,
                    State = RoomState.Ready
                };

                TimeSpan expiration = TimeSpan.FromHours(1);
                var redis = new RedisString<RedisRoom>(_redisConn, Convert.ToString(roomId) + _dbConfig.Value.RedisRoomKey, expiration);
                if(await redis.SetAsync(redisRoom, expiration) == false)
                {
                    return (ErrorCode.CannotConnectServer, -1);
                }

                return (ErrorCode.None, roomId);
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
                var script =
@"local roomId = tonumber(redis.call('get', KEYS[1])) 
redis.call('incr', KEYS[1])
return roomId
";
                var redis = new RedisLua(_redisConn, _dbConfig.Value.RedisUniqueRoomKey);
                var keys = new RedisKey[] { _dbConfig.Value.RedisUniqueRoomKey };
                var values = new RedisValue[] {  };
                var roomId = await redis.ScriptEvaluateAsync<int>(script, keys, values);
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
