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

            GameResult gameResult;
            (Result, gameResult) = await UpdateRedisGame(redisGame, skillGameRequest.UserId, skillGameRequest.SkillId);

            if (Result != ErrorCode.None)
            {
                return new SkillGameResponse
                {
                    Result = Result
                };
            }

            if (gameResult != GameResult.GameProceeding)
            {
                redisRoom.State = RoomState.Ready;
                Result = await _memoryDB.UpdateRedisRoom(redisRoom);
            }

            return new SkillGameResponse
            {
                Result = Result
            };
        }

        int CalculateDamage(int attack, int defence, int magic, MasterSkillInfo skill)
        {
            int damage = skill.BaseValue + (int)(attack * (skill.Attack / 100)) + (int)(defence * (skill.Defence / 100)) + (int)(magic * (skill.Magic / 100));
            return damage;
        }

        int CalculateDefence(int defence, int damage)
        {
            int final = (int)(damage * ( 1 - (defence / (defence + 100))));
            return final;
        }

        RedisGame UpdateBossSkill(RedisGame redisGame)
        {
            MasterSkillInfo? masterSkillInfo = _masterDB.LoadSkillInfo(redisGame.BossInfo.SkillSet1);
            if (masterSkillInfo == null)
            {
                return redisGame;
            }

            int damage = CalculateDamage(redisGame.BossInfo.Attack, redisGame.BossInfo.Defence, redisGame.BossInfo.Magic, masterSkillInfo);

            for (int i = 0; i < redisGame.Players.Length; i++)
            {
                if (masterSkillInfo.Type == SkillType.Physic)
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

        async Task<(ErrorCode, GameResult)> UpdateRedisGame(RedisGame redisGame, int userId, int skillId)
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
                        return (ErrorCode.NoneExistClassId, redisGame.GameResult);
                    }

                    MasterSkillInfo? masterSkillInfo = _masterDB.LoadSkillInfo(skillId); 
                    if(masterSkillInfo == null)
                    {
                        return (ErrorCode.NoneExistSkill, redisGame.GameResult);
                    }

                    // 마나 체크 
                    if (redisGame.Players[i].Mp < masterSkillInfo.Mp)
                    {
                        return (ErrorCode.NotEnoughMp, redisGame.GameResult);
                    }
                    redisGame.Players[i].Mp -= masterSkillInfo.Mp;

                    // 물리피해, 고정피해, 힐 체크 
                    int damage = CalculateDamage(redisGame.Players[i].Attack, redisGame.Players[i].Defence, redisGame.Players[i].Magic, masterSkillInfo);

                    if(masterSkillInfo.Type != SkillType.Heal)
                    {
                        if(masterSkillInfo.Type == SkillType.Physic)
                        {
                            damage = CalculateDefence(redisGame.BossInfo.Defence, damage);
                        }
                        if(IsDead(redisGame.BossInfo.Hp, damage) == true)
                        {
                            redisGame.GameResult = GameResult.GameWin;
                            redisGame.BossInfo.Hp = 0;
                            break; 
                        }
                        else
                        {
                            redisGame.BossInfo.Hp -= damage;
                        }
                    }
                    else
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
                redisGame.Round += 1;
                redisGame = UpdateBossSkill(redisGame);
            }
            else
            {
                redisGame.WhoIsTurn.Index += 1;
            }

            redisGame.WhoIsTurn.UserId = redisGame.Players[redisGame.WhoIsTurn.Index].UserId;

            var Result = await _memoryDB.StoreRedisGame(redisGame);
            return (Result, redisGame.GameResult);
        }

        bool IsDead(int hp, int damage)
        {
            if (hp - damage <= 0)
            {
                return true;
            }

            return false;
        }
    }
}
