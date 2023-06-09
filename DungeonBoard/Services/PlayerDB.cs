﻿using DungeonBoard.Models;
using DungeonBoard.Models.Player;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data;
using System.Reflection.PortableExecutable;

namespace DungeonBoard.Services
{
    public interface IPlayerDB
    {
        Task<ErrorCode> CreatePlayerData(int userId);
        Task<ErrorCode> UpdatePlayerClass(int userId, int classId);
        Task<(ErrorCode, Player?)> LoadPlayerFromId(int userId);
    }

    public class PlayerDB : IPlayerDB, IDisposable
    {
        IDbConnection dbConnection;
        MySqlCompiler compiler;
        QueryFactory _queryFactory;
        IOptions<DbConfig> _dbConfig;
        private bool _disposed = false;

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

                if (effectedRow != 1)
                {
                    return ErrorCode.CannotCreatePlayer;
                }

                return ErrorCode.None;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ErrorCode.CannotConnectServer;
            }
        }

        async public Task<ErrorCode> UpdatePlayerClass(int userId, int classId)
        {
            try
            {
                int effectedRow = await _queryFactory.Query("players").Where("userId", userId).UpdateAsync(new {
                    classId = classId
                });

                if(effectedRow != 1)
                {
                    return ErrorCode.FailedUpdateClass;
                }

                return ErrorCode.None;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ErrorCode.CannotConnectServer;
            }
        }

        async public Task<(ErrorCode, Player?)> LoadPlayerFromId(int userId)
        {
            try
            {
                Player? player = await _queryFactory.Query("players").Where("userId", userId).FirstOrDefaultAsync<Player>();
                if(player == null)
                {
                    return (ErrorCode.NoneExistUserId, null);
                }
                return (ErrorCode.None, player); 
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (ErrorCode.CannotConnectServer, null);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
                dbConnection.Close();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
