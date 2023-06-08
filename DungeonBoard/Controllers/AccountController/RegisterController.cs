using DungeonBoard.Models;
using DungeonBoard.ReqResModels.Account;
using DungeonBoard.Services;
using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.AccountController;

[ApiController]
public class RegisterController : Controller
{
    IAccountDB _accountDB;
    public RegisterController(IAccountDB accountDB)
    {
        _accountDB = accountDB;
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

        ErrorCode Result = await _accountDB.RegisterAccount(registerRequest.Email, registerRequest.Password);
        if(Result != ErrorCode.None)
        {
            return new RegisterResponse
            {
                Result = Result
            };
        }

        return new RegisterResponse
        {
            Result = Models.ErrorCode.None
        };
    }
}
