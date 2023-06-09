using DungeonBoard.Models;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data;

namespace DungeonBoard.Services
{
    public interface IPlayerDB
    {
        Task<ErrorCode> CreatePlayerData(int userId);
        Task<ErrorCode> UpdatePlayerClass(int classId);
    }

    public class PlayerDB : IPlayerDB
    {
        IDbConnection dbConnection;
        MySqlCompiler compiler;
        QueryFactory _queryFactory;
        IOptions<DbConfig> _dbConfig;

        public PlayerDB(IOptions<DbConfig> dbConfig)
        {
            _dbConfig = dbConfig;
            Open();
        }

        async public Task<ErrorCode> CreatePlayerData(int userId)
        {
            try
            {
                int effectedRow = await _queryFactory.Query("players").InsertAsync(new
                {
                    userId = userId,
                    classId = 0,
                });
                return ErrorCode.None;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ErrorCode.CannotConnectServer;
            }
        }

        async public Task<ErrorCode> UpdatePlayerClass(int classId)
        {
            try
            {
                return ErrorCode.None;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ErrorCode.CannotConnectServer;
            }
        }

        void Dispose()
        {
            try
            {
                dbConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        void Open()
        {
            try
            {
                dbConnection = new MySqlConnection(_dbConfig.Value.GameDB);
                dbConnection.Open();
                compiler = new MySqlCompiler();
                _queryFactory = new QueryFactory(dbConnection, compiler);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
