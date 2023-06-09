using DungeonBoard.Models;
using DungeonBoard.Models.Account;
using DungeonBoard.ReqResModels.Account;
using DungeonBoard.Services;
using DungeonBoard.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.AccountController;

[ApiController]
public class LoginController : Controller
{
    IAccountDB _accountDB;
    IMemoryDB _memoryDB;
    public LoginController(IAccountDB accountDB, IMemoryDB memoryDB)
    {
        _accountDB = accountDB;
        _memoryDB = memoryDB;
    }

    [Route("/Login")]
    [HttpPost]
    async public Task<LoginResponse> Login(LoginRequest loginRequest)
    {
        var (Result, user) = await _accountDB.VerifyAccount(loginRequest.Email, loginRequest.Password);
        if(Result != ErrorCode.None)
        {
            return new LoginResponse
            {
                Result = Result,
                AuthToken = ""
            };
        }

        if(CheckPassword(user, loginRequest.Password) == false)
        {
            return new LoginResponse
            {
                Result = ErrorCode.InvalidPassword,
                AuthToken = ""
            };
        }

        var authToken = Security.CreateAuthToken();

        Result = await StoreUserInfoInMemory(user, authToken);
        if(Result != ErrorCode.None)
        {
            return new LoginResponse
            {
                Result = Result,
                AuthToken = ""
            };
        }

        return new LoginResponse
        {
            Result = Models.ErrorCode.None,
            AuthToken = authToken
        };
    }

    bool CheckPassword(User user, string requestPassword)
    {
        string requestHashPassword = Security.MakeHashingPassWord(user.Salt, requestPassword);
        if(requestHashPassword != user.Password)
        {
            return false;
        }
        return true;
    }

    async Task<ErrorCode> StoreUserInfoInMemory(User user, string authToken)
    {
        ErrorCode Result = await _memoryDB.StoreRedisUser(user.Userid, authToken, user.Email, UserState.Lobby);
        return Result;
    }
}
