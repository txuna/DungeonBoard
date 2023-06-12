using DungeonBoard.Models;
using DungeonBoard.Models.Account;
using DungeonBoard.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data;
using System.Data.Common;

namespace DungeonBoard.Services
{
    public interface IAccountDB
    {
        Task<(ErrorCode, int)> RegisterAccount(string email, string password);
        Task<(ErrorCode, User?)> VerifyAccount(string email, string password);
        Task<ErrorCode> DeleteAccount(int userId);
    }

    public class AccountDB : IAccountDB, IDisposable
    {
        IDbConnection dbConnection;
        MySqlCompiler compiler;
        QueryFactory _queryFactory;
        IOptions<DbConfig> _dbConfig;
        private bool _disposed = false;

        public AccountDB(IOptions<DbConfig> dbConfig)
        {
            _dbConfig = dbConfig;
            Open();
        }

        async public Task<ErrorCode> DeleteAccount(int userId)
        {
            try
            {
                int effectedRow = await _queryFactory.Query("accounts").Where("userId", userId).DeleteAsync();
                if(effectedRow == 0)
                {
                    return ErrorCode.NoneExistUserId;
                }

                return ErrorCode.None;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ErrorCode.CannotConnectServer;
            }
        }

        async public Task<(ErrorCode, int)> RegisterAccount(string email, string password)
        {
            try
            {
                var saltValue = Security.SaltString();
                var hashingPassword = Security.MakeHashingPassWord(saltValue, password);

                var userId = await _queryFactory.Query("accounts").InsertGetIdAsync<int>(new
                {
                    Email = email,
                    Salt = saltValue,
                    Password = hashingPassword
                });

                return (ErrorCode.None, userId);
            }
            catch(MySqlException ex)
            {
                Console.WriteLine(ex.Message);

                if(ex.Number == 1062)
                {
                    return (ErrorCode.AlreadyExistEmail, -1);
                }
                return (ErrorCode.CannotConnectServer, -1);
            }
        }

        async public Task<(ErrorCode, User?)> VerifyAccount(string email, string password)
        {
            try
            {
                var user = await _queryFactory.Query("accounts").Where("email", email).FirstOrDefaultAsync<User>();
                if (user == null)
                {
                    return (ErrorCode.NoneExistEmail, null);
                }

                return (ErrorCode.None, user);
            }
            catch (Exception ex)
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
