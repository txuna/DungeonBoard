using CloudStructures;
using CloudStructures.Structures;
using DungeonBoard.Models;
using DungeonBoard.Models.Account;
using DungeonBoard.Models.Room;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
        Task<(ErrorCode, RedisRoom[]?)> LoadAllRoom();
        Task<(ErrorCode, RedisRoom?)> LoadRoomFromId(int roomId);
        Task<ErrorCode> DeleteRedisRoom(int roomId);
        Task<ErrorCode> UpdateRedisRoom(RedisRoom room);
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
                var redis = new RedisSet<RedisRoom>(_redisConn, "ROOMS", null);
                var redisRoom = await redis.MembersAsync();
                foreach(var room in redisRoom)
                {
                    if(room.RoomId == roomId)
                    {
                        return (ErrorCode.None, room);
                    }
                }

                return (ErrorCode.NoneExistRoom, null);
            }
            catch(Exception ex )
            {
                Console.WriteLine(ex.Message);
                return (ErrorCode.CannotConnectServer, null); 
            }
        }

        async public Task<(ErrorCode, RedisRoom[]?)> LoadAllRoom()
        {
            try
            {
                var redis = new RedisSet<RedisRoom>(_redisConn, "ROOMS", null);
                var redisRoom = await redis.MembersAsync();
                return (ErrorCode.None,  redisRoom);
            }
            catch (Exception ex)
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
local rooms = redis.call('SMEMBERS', KEYS[1])
for _, member in ipairs(rooms) do 
    local obj = cjson.decode(member)
    if tonumber(obj.RoomId) == tonumber(ARGV[1]) then 
        if #obj.Users < obj.HeadCount then
            redis.call('SREM', KEYS[1], member)
            table.insert(obj.Users, tonumber(ARGV[2]))     
            redis.call('SADD', KEYS[1], cjson.encode(obj))
            return 0
        else
            return 1
        end
    end
end 
return 2
";
                var redis = new RedisLua(_redisConn, "ROOMS");
                var keys = new RedisKey[] { "ROOMS" };
                var values = new RedisValue[] { roomId, userId };
                var result = await redis.ScriptEvaluateAsync<int>(script, keys, values);

                if(result.Value != 0)
                {
                    if(result.Value == 1)
                    {
                        return ErrorCode.AlreadyFullRoom;
                    }
                    else if(result.Value == 2)
                    {
                        return ErrorCode.NoneExistRoom;
                    }
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

                var redis = new RedisSet<RedisRoom>(_redisConn, "ROOMS", null);
                if(await redis.AddAsync(redisRoom) == false)
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

        async public Task<ErrorCode> DeleteRedisRoom(int roomId)
        {
            try
            {
                var script =
@"
local rooms = redis.call('SMEMBERS', KEYS[1])
for _, member in ipairs(rooms) do 
    local obj = cjson.decode(member)
    if tonumber(obj.RoomId) == tonumber(ARGV[1]) then 
        redis.call('SREM', KEYS[1], member)
    end
end 
";

                var redis = new RedisLua(_redisConn, "ROOMS");
                var keys = new RedisKey[] { "ROOMS" };
                var values = new RedisValue[] { roomId };
                var result = await redis.ScriptEvaluateAsync<int>(script, keys, values);
                return ErrorCode.None;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ErrorCode.CannotConnectServer;
            }
        }

        // 제거와 삽입을 원자적으로 하지 않으면 중간 폴링(방 정보 로드)에서 문제 발생할 수 있음
        async public Task<ErrorCode> UpdateRedisRoom(RedisRoom room)
        {
            try
            {
                var script =
@"
local rooms = redis.call('SMEMBERS', KEYS[1])
for _, member in ipairs(rooms) do 
    local obj = cjson.decode(member)
    if tonumber(obj.RoomId) == tonumber(ARGV[1]) then 
        redis.call('SREM', KEYS[1], member)
        redis.call('SADD', KEYS[1], ARGV[2])
    end
end 
";

                var redis = new RedisLua(_redisConn, "ROOMS");
                var keys = new RedisKey[] { "ROOMS" };
                var values = new RedisValue[] { room.RoomId, JsonConvert.SerializeObject(room) };
                var result = await redis.ScriptEvaluateAsync<int>(script, keys, values);

                return ErrorCode.None;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ErrorCode.CannotConnectServer;
            }
        }
    }
}
