using DungeonBoard.Models;
using DungeonBoard.Models.Account;
using DungeonBoard.Models.Game;
using DungeonBoard.Models.Master;
using DungeonBoard.Models.Player;
using DungeonBoard.Models.Room;
using DungeonBoard.ReqResModels.Game;
using DungeonBoard.Services;
using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.GameController;

[ApiController]
public class StartGameController : Controller
{
    IMemoryDB _memoryDB;
    IMasterDB _masterDB;
    IPlayerDB _playerDB; 
    public StartGameController(IMemoryDB memoryDB, IMasterDB masterDB, IPlayerDB playerDB)
    {
        _memoryDB = memoryDB;
        _masterDB = masterDB;
        _playerDB = playerDB;
    }

    [Route("/Game/Start")]
    [HttpPost]
    async public Task<StartGameResponse> StartGame(StartGameRequest startGameRequest)
    {
        // 방에 참여하고 있는지 확인 
        RedisUser redisUser = (RedisUser)HttpContext.Items["Redis-User"];
        if(redisUser.State != UserState.Playing)
        {
            return new StartGameResponse
            {
                Result = ErrorCode.NoneExistInThisRoom
            };
        }

        var (Result, room) = await _memoryDB.LoadRoomFromId(startGameRequest.RoomId);
        if(Result != ErrorCode.None)
        {
            return new StartGameResponse
            {
                Result = Result
            };
        }

        // 해당 방의 호스트인지 확인
        if(room.HostUserId != redisUser.UserId)
        {
            return new StartGameResponse
            {
                Result = ErrorCode.NoneHostThisRoom
            };
        }

        // 해당 방의 상태가 Ready가 맞는지 확인
        if(room.State != RoomState.Ready)
        {
            return new StartGameResponse
            {
                Result = ErrorCode.AlreadyPlayRoom
            };
        }

        // Game Object Redis에 저장
        RedisGame? redisGame = await LoadRedisGame(room);

        if(redisGame == null)
        {
            return new StartGameResponse
            {
                Result = ErrorCode.FailedStoreGameInfoInMemory
            };
        }

        Result = await _memoryDB.StoreRedisGame(redisGame);
        if(Result != ErrorCode.None)
        {
            return new StartGameResponse
            {
                Result = Result
            };
        }

        // 해당 방의 상태 변경 
        room.State = RoomState.Playing;
        Result = await _memoryDB.UpdateRedisRoom(room);

        return new StartGameResponse
        {
            Result = Result,
        };
    }

    async Task<RedisGame?> LoadRedisGame(RedisRoom redisRoom)
    {
        var redisGame = new RedisGame();
        // GameId 세팅 
        redisGame.GameId = redisRoom.RoomId;
        redisGame.WhoIsTurn = 0;
        redisGame.Round = 0;

        // 보스 세팅 
        MasterBossInfo? bossInfo = _masterDB.LoadMasterBossInfo(redisRoom.BossId);
        if(bossInfo == null)
        {
            return null;
        }
        redisGame.BossInfo = bossInfo;

        // 플레이어 세팅 
        redisGame.Players = new GamePlayer[redisRoom.Users.Length];
        int index = 0;
        foreach(var userId in redisRoom.Users)
        {
            var (Result, player) = await _playerDB.LoadPlayerFromId(userId);
            if(Result != ErrorCode.None)
            {
                return null;
            }

            MasterClassInitStat? masterClassInitStat = _masterDB.LoadClassInitStat(player.ClassId);
            if(masterClassInitStat == null)
            {
                return null;
            }

            GamePlayer gamePlayer = new GamePlayer
            {
                UserId = player.UserId, 
                ClassId = player.ClassId,
                Hp = masterClassInitStat.Hp, 
                MaxHp = masterClassInitStat.Hp, 
                Mp = masterClassInitStat.Mp, 
                MaxMp = masterClassInitStat.Mp, 
                Attack = masterClassInitStat.Attack, 
                Defence = masterClassInitStat.Defence, 
                Magic = masterClassInitStat.Magic, 
                Level = 1, 
                PositionCard = 0,
            };

            redisGame.Players[index] = gamePlayer;
            index += 1;
        }

        return redisGame;
    }
}
