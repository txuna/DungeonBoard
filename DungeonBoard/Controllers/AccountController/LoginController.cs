using DungeonBoard.Models;
using DungeonBoard.Models.Account;
using DungeonBoard.Models.Player;
using DungeonBoard.Models.Room;
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
    IPlayerDB _playerDB;
    public LoginController(IAccountDB accountDB, IMemoryDB memoryDB, IPlayerDB playerDB)
    {
        _accountDB = accountDB;
        _memoryDB = memoryDB;
        _playerDB = playerDB;
    }

    /**
     * @TODO
     * 로그인했을 시 해당 userId로 되어있는 key가 있을 떄 Stat 확인 있다면 이미 플레이중이므로 게임 화면으로 넘겨줌 (response에 추가)
     * 단) 토큰은 새로 발급
     */
    [Route("/Login")]
    [HttpPost]
    async public Task<LoginResponse> Login(LoginRequest loginRequest)
    {
        var (Result, user) = await _accountDB.VerifyAccount(loginRequest.Email, loginRequest.Password);
        if(Result != ErrorCode.None)
        {
            return new LoginResponse
            {
                Result = Result
            };
        }

        if(CheckPassword(user, loginRequest.Password) == false)
        {
            return new LoginResponse
            {
                Result = ErrorCode.InvalidPassword
            };
        }

        var authToken = Security.CreateAuthToken();

        // 이미 플레이어가 Redis에 있는지 확인 -> State 보존후 새로 저장 
        RedisUser redisUser;
        (Result, redisUser) = await _memoryDB.LoadRedisUser(user.UserId);
        int roomId = 0; 
        UserState state = UserState.Lobby;
        // 플레이어가 있다면 
        if (Result == ErrorCode.None)
        {
            state = redisUser.State;
            if(state == UserState.Playing)
            {
                RedisRoom redisRoom;
                (Result, redisRoom) = await _memoryDB.LoadRoomFromUserId(user.UserId); 
                if(Result  == ErrorCode.None)
                {
                    roomId = redisRoom.RoomId;
                }
                // 만약 방이 파괴되었다면 
                else
                {
                    state = UserState.Lobby;
                }
            }
        }

        Result = await StoreUserInfoInMemory(user, authToken, state);
        if(Result != ErrorCode.None)
        {
            return new LoginResponse
            {
                Result = Result
            };
        }

        Player? player;
        (Result, player) = await _playerDB.LoadPlayerFromId(user.UserId);

        if (Result != ErrorCode.None)
        {
            return new LoginResponse
            {
                Result = Result
            };
        }

        return new LoginResponse
        {
            Result = ErrorCode.None,
            AuthToken = authToken,
            UserId = user.UserId,
            ClassId = player.ClassId,
            State = state,
            RoomId = roomId 
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

    async Task<ErrorCode> StoreUserInfoInMemory(User user, string authToken, UserState state)
    {
        ErrorCode Result = await _memoryDB.StoreRedisUser(user.UserId, authToken, user.Email, state);
        return Result;
    }
}
