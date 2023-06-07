using CloudStructures;
using Microsoft.Extensions.Options;

namespace DungeonBoard.Services
{
    public interface IMemoryDB
    {

    }
    public class MemoryDB : IMemoryDB
    {
        RedisConnection _redisConn;
        public MemoryDB()
        {

        }
    }
}
