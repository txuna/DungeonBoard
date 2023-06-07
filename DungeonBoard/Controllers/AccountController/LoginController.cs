using DungeonBoard.Models;
using DungeonBoard.ReqResModels.Account;
using DungeonBoard.Services;
using DungeonBoard.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.AccountController;

[ApiController]
public class LoginController : Controller
{
    IAccountDB _accountDB;
    public LoginController(IAccountDB accountDB)
    {
        _accountDB = accountDB;
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

        string userPasswordHash = Security.MakeHashingPassWord(user.Salt, loginRequest.Password);
        if(userPasswordHash != user.Password)
        {
            return new LoginResponse
            {
                Result = ErrorCode.InvalidPassword,
                AuthToken = ""
            };
        }

        var authToken = Security.CreateAuthToken();

        return new LoginResponse
        {
            Result = Models.ErrorCode.None,
            AuthToken = authToken
        };
    }
}
