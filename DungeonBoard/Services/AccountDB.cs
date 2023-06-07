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
        Task<ErrorCode> RegisterAccount(string email, string password);
        Task<(ErrorCode, User?)> VerifyAccount(string email, string password);
    }

    public class AccountDB : IAccountDB
    {
        IDbConnection dbConnection;
        MySqlCompiler compiler;
        QueryFactory _queryFactory;
        IOptions<DbConfig> _dbConfig;

        public AccountDB(IOptions<DbConfig> dbConfig)
        {
            _dbConfig = dbConfig;
            Open();
        }

        async public Task<ErrorCode> RegisterAccount(string email, string password)
        {
            try
            {
                var saltValue = Security.SaltString();
                var hashingPassword = Security.MakeHashingPassWord(saltValue, password);

                var count = await _queryFactory.Query("users").InsertAsync(new
                {
                    Email = email,
                    Salt = saltValue,
                    Password = hashingPassword
                });

                if(count != 1)
                {
                    return ErrorCode.AlreadyExistEmail;
                }
                return ErrorCode.None;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ErrorCode.CannotConnectServer;
            }
        }

        async public Task<(ErrorCode, User?)> VerifyAccount(string email, string password)
        {
            try
            {
                var user = await _queryFactory.Query("users").Where("email", email).FirstOrDefaultAsync<User>();
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
