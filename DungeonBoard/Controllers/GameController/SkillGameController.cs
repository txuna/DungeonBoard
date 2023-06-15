using DungeonBoard.Models.Account;
using DungeonBoard.Models.Game;
using DungeonBoard.Models;
using DungeonBoard.ReqResModels.Game;
using DungeonBoard.Services;
using Microsoft.AspNetCore.Mvc;
using DungeonBoard.Models.Master;

namespace DungeonBoard.Controllers.GameController
{
    [ApiController]
    public class SkillGameController : Controller
    {
        IMemoryDB _memoryDB; 
        IMasterDB _masterDB;
        public SkillGameController(IMemoryDB memoryDB, IMasterDB masterDB)
        {
            _memoryDB = memoryDB;
            _masterDB = masterDB;
        }

        [Route("/Game/Skill")]
        [HttpPost]
        async public Task<SkillGameResponse> GameSkill(SkillGameRequest skillGameRequest)
        {
            // 해당 플레이어가 게임 중이고 해당 방이 게임상태라면 
            RedisUser redisUser = (RedisUser)HttpContext.Items["Redis-User"];
            if (redisUser.State != UserState.Playing)
            {
                return new SkillGameResponse
                {
                    Result = ErrorCode.PlayerIsNotPlaying
                };
            }

            var (Result, redisRoom) = await _memoryDB.LoadRoomFromId(skillGameRequest.GameId);
            if (Result != ErrorCode.None)
            {
                return new SkillGameResponse
                {
                    Result = Result
                };
            }

            if (redisRoom.State != RoomState.Playing)
            {
                return new SkillGameResponse
                {
                    Result = ErrorCode.IsNotPlayingThisRoom
                };
            }

            RedisGame? redisGame;
            (Result, redisGame) = await _memoryDB.LoadRedisGame(skillGameRequest.GameId);
            if (Result != ErrorCode.None)
            {
                return new SkillGameResponse
                {
                    Result = Result
                };
            }

            if (redisGame.WhoIsTurn.UserId != skillGameRequest.UserId)
            {
                return new SkillGameResponse
                {
                    Result = ErrorCode.IsNotDiceTurn
                };
            }

            Result = await UpdateRedisGame(redisGame, skillGameRequest.UserId, skillGameRequest.SkillId);

            return new SkillGameResponse
            {
                Result = Result
            };
        }

        int CalculateDamage(GamePlayer player, MasterSkillInfo skill)
        {
            int damage = skill.BaseValue + (int)(player.Attack * (skill.Attack / 100)) + (int)(player.Defence * (skill.Defence / 100)) + (int)(player.Magic * (skill.Magic / 100));
            return damage;
        }

        int CalculateDefence(int defence, int damage)
        {
            int final = (int)(damage * ( 1 - (defence / (defence + 100))));
            Console.WriteLine(final);
            return final;
        }

        async Task<ErrorCode> UpdateRedisGame(RedisGame redisGame, int userId, int skillId)
        {
            // Backup 덮기
            for (int i = 0; i < redisGame.Players.Length; i++)
            {
                if (redisGame.Players[i].UserId == userId)
                {
                    // 유효성 검사 
                    MasterClassSkillInfo? masterClassSkillInfo = _masterDB.LoadClassSkillInfo(redisGame.Players[i].ClassId, skillId);
                    if (masterClassSkillInfo == null)
                    {
                        return ErrorCode.InvalidSkill;
                    }

                    MasterSkillInfo? masterSkillInfo = _masterDB.LoadSkillInfo(skillId); 
                    if(masterSkillInfo == null)
                    {
                        return ErrorCode.NoneExistSkill;
                    }

                    // 마나 체크 
                    if (redisGame.Players[i].Mp < masterSkillInfo.Mp)
                    {
                        return ErrorCode.NotEnoughMp;
                    }
                    redisGame.Players[i].Mp -= masterSkillInfo.Mp;

                    // 물리피해, 고정피해, 힐 체크 
                    int damage = CalculateDamage(redisGame.Players[i], masterSkillInfo);
                    if (masterSkillInfo.Type == SkillType.Fixed)
                    {
                        redisGame.BossInfo.Hp -= damage;
                    }

                    else if(masterSkillInfo.Type == SkillType.Physic)
                    {
                        redisGame.BossInfo.Hp -= CalculateDefence(redisGame.BossInfo.Defence, damage);
                    }

                    else if(masterSkillInfo.Type == SkillType.Heal)
                    {
                        for(int heal_indx=0; heal_indx<redisGame.Players.Length; heal_indx++)
                        {
                            if(redisGame.Players[heal_indx].Hp + damage > redisGame.Players[heal_indx].MaxHp)
                            {
                                redisGame.Players[heal_indx].Hp += damage;
                            }
                            else
                            {
                                redisGame.Players[heal_indx].Hp = redisGame.Players[heal_indx].MaxHp;
                            }
                        }
                    }
                    break;
                }
            }

            // Set WhoIsTurn 
            if (redisGame.WhoIsTurn.Index + 1 >= redisGame.Players.Length)
            {
                redisGame.WhoIsTurn.Index = 0;
            }
            else
            {
                redisGame.WhoIsTurn.Index += 1;
            }

            redisGame.WhoIsTurn.UserId = redisGame.Players[redisGame.WhoIsTurn.Index].UserId;

            return await _memoryDB.StoreRedisGame(redisGame);
        }
    }
}
