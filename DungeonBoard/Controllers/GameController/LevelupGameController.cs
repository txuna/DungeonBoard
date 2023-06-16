using DungeonBoard.Models;
using DungeonBoard.Models.Account;
using DungeonBoard.Models.Game;
using DungeonBoard.Models.Master;
using DungeonBoard.ReqResModels.Game;
using DungeonBoard.Services;
using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.GameController
{
    [ApiController]
    public class LevelupGameController : Controller
    {

        IMemoryDB _memoryDB; 
        IMasterDB _masterDB;
        public LevelupGameController(IMemoryDB memoryDB, IMasterDB masterDB)
        {
            _memoryDB = memoryDB;
            _masterDB = masterDB;
        }

        [Route("/Game/Levelup")]
        [HttpPost]
        async public Task<LevelupGameResponse> GameLevelup(LevelupGameRequest levelupGameRequest)
        {
            // 해당 플레이어가 게임 중이고 해당 방이 게임상태라면 
            RedisUser redisUser = (RedisUser)HttpContext.Items["Redis-User"];
            if (redisUser.State != UserState.Playing)
            {
                return new LevelupGameResponse
                {
                    Result = ErrorCode.PlayerIsNotPlaying
                };
            }

            var (Result, redisRoom) = await _memoryDB.LoadRoomFromId(levelupGameRequest.GameId);
            if (Result != ErrorCode.None)
            {
                return new LevelupGameResponse
                {
                    Result = Result
                };
            }

            if (redisRoom.State != RoomState.Playing)
            {
                return new LevelupGameResponse
                {
                    Result = ErrorCode.IsNotPlayingThisRoom
                };
            }

            RedisGame? redisGame;
            (Result, redisGame) = await _memoryDB.LoadRedisGame(levelupGameRequest.GameId);
            if (Result != ErrorCode.None)
            {
                return new LevelupGameResponse
                {
                    Result = Result
                };
            }

            if (redisGame.WhoIsTurn.UserId != levelupGameRequest.UserId)
            {
                return new LevelupGameResponse
                {
                    Result = ErrorCode.IsNotDiceTurn
                };
            }

            GameResult gameResult;
            (Result, gameResult) = await UpdateRedisGame(redisGame, levelupGameRequest.UserId);

            if(Result != ErrorCode.None)
            {
                return new LevelupGameResponse
                {
                    Result = Result
                };
            }

            if(gameResult != GameResult.GameProceeding)
            {
                redisRoom.State = RoomState.Ready;
                Result = await _memoryDB.UpdateRedisRoom(redisRoom);
            }

            return new LevelupGameResponse
            {
                Result = Result
            };
        }

        async Task<(ErrorCode, GameResult)> UpdateRedisGame(RedisGame redisGame, int userId)
        {
            // Backup 덮기
            for (int i = 0; i < redisGame.Players.Length; i++)
            {
                if (redisGame.Players[i].UserId == userId)
                {
                    MasterClassLevelupStat? masterClassLevelupStat = _masterDB.LoadClassLevelupStat(redisGame.Players[i].ClassId);
                    if(masterClassLevelupStat == null)
                    {
                        return (ErrorCode.NoneExistClassId, redisGame.GameResult);
                    }

                    if (redisGame.Players[i].Level + 1 > 18)
                    {
                        return (ErrorCode.NoneExistClassId, redisGame.GameResult);
                    }

                    redisGame.Players[i].MaxHp += masterClassLevelupStat.Hp;
                    redisGame.Players[i].MaxMp += masterClassLevelupStat.Mp;
                    redisGame.Players[i].Attack += masterClassLevelupStat.Attack;
                    redisGame.Players[i].Defence += masterClassLevelupStat.Defence;
                    redisGame.Players[i].Magic += masterClassLevelupStat.Magic;
                    redisGame.Players[i].Level += 1;

                    break;
                }
            }

            // Set WhoIsTurn 
            if (redisGame.WhoIsTurn.Index + 1 >= redisGame.Players.Length)
            {
                // Round Over -> 보스 차례 
                redisGame.WhoIsTurn.Index = 0;
                redisGame.Round += 1;
                redisGame = UpdateBossSkill(redisGame);
            }
            else
            {
                redisGame.WhoIsTurn.Index += 1;
            }

            redisGame.WhoIsTurn.UserId = redisGame.Players[redisGame.WhoIsTurn.Index].UserId;


            var Result =  await _memoryDB.StoreRedisGame(redisGame);
            return (Result, redisGame.GameResult);
        }

        RedisGame UpdateBossSkill(RedisGame redisGame)
        {
            MasterSkillInfo? masterSkillInfo = _masterDB.LoadSkillInfo(redisGame.BossInfo.SkillSet1);
            if (masterSkillInfo == null)
            {
                return redisGame;
            }

            int damage = CalculateDamage(redisGame.BossInfo.Attack, redisGame.BossInfo.Defence, redisGame.BossInfo.Magic, masterSkillInfo);

            for(int i = 0; i<redisGame.Players.Length; i++)
            {
                if(masterSkillInfo.Type == SkillType.Physic)
                {
                    damage = CalculateDefence(redisGame.Players[i].Defence, damage);
                }

                // 플레이어 사망
                if (IsDead(redisGame.Players[i].Hp, damage) == true)
                {
                    redisGame.Players[i].Hp = 0;
                    redisGame.GameResult = GameResult.GameDefeat;
                    break; 
                }
                else
                {
                    redisGame.Players[i].Hp -= damage;
                }
            }

            return redisGame;
        }

        int CalculateDamage(int attack, int defence, int magic, MasterSkillInfo skill)
        {
            int damage = skill.BaseValue + (int)(attack * (skill.Attack / 100)) + (int)(defence * (skill.Defence / 100)) + (int)(magic * (skill.Magic / 100));
            return damage;
        }

        int CalculateDefence(int defence, int damage)
        {
            int final = (int)(damage * (1 - (defence / (defence + 100))));
            return final;
        }

        bool IsDead(int hp, int damage)
        {
            if(hp - damage <= 0)
            {
                return true; 
            }

            return false;
        }
    }
}
