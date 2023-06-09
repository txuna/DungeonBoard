using DungeonBoard.Models;
using DungeonBoard.ReqResModels.Account;
using DungeonBoard.Services;
using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.AccountController;

[ApiController]
public class RegisterController : Controller
{
    IAccountDB _accountDB;
    IPlayerDB _playerDB;
    public RegisterController(IAccountDB accountDB, IPlayerDB playerDB)
    {
        _accountDB = accountDB;
        _playerDB = playerDB;
    }

    [Route("/Register")]
    [HttpPost]
    async public Task<RegisterResponse> Register(RegisterRequest registerRequest)
    {
        if(registerRequest.Password != registerRequest.ConfirmPassword)
        {
            return new RegisterResponse
            {
                Result = ErrorCode.MissmatchPassword
            };
        }

        var (Result, userId) = await _accountDB.RegisterAccount(registerRequest.Email, registerRequest.Password);
        if(Result != ErrorCode.None)
        {
            return new RegisterResponse
            {
                Result = Result
            };
        }

        Result = await _playerDB.CreatePlayerData(userId);
        if(Result != ErrorCode.None)
        {
            // UNDO !!
            var Error = await _accountDB.DeleteAccount(userId);
            if(Error != ErrorCode.None)
            {
                return new RegisterResponse
                {
                    Result = Error
                };
            }
            return new RegisterResponse
            {
                Result = Result
            };
        }

        return new RegisterResponse
        {
            Result = ErrorCode.None
        };
    }
}
