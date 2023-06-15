using DungeonBoard.Models;
using DungeonBoard.Models.Master;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data;
using System.Data.Common;

namespace DungeonBoard.Services
{
    public interface IMasterDB
    {
        MasterBossInfo? LoadMasterBossInfo(int bossId);
        MasterBossInfo[] LoadAllMasterBossInfo();
        MasterClassInitStat? LoadClassInitStat(int classId);
        MasterClassLevelupStat? LoadClassLevelupStat(int classId);
        MasterSkillInfo? LoadSkillInfo(int skillId);
        MasterClassSkillInfo? LoadClassSkillInfo(int classId, int skillId);
    }

    public class MasterDB : IMasterDB
    {
        MasterBossInfo[] masterBossInfo;
        MasterClassInitStat[] masterClassInitStat;
        MasterClassLevelupStat[] masterClassLevelupStat;
        MasterClassSkillInfo[] masterClassSkillInfo;
        MasterSkillInfo[] masterSkillInfo;

        IOptions<DbConfig> _dbConfig;
        IDbConnection dbConnection;
        MySqlCompiler compiler;
        QueryFactory queryFactory;
        public MasterDB(IOptions<DbConfig> dbConfig)
        {
            _dbConfig = dbConfig;
            Open();
            Load();
        }

        public MasterClassSkillInfo? LoadClassSkillInfo(int classId, int skillId)
        {
            return masterClassSkillInfo.FirstOrDefault(e => e.SkillId == skillId && e.ClassId == classId);
        }

        public MasterSkillInfo? LoadSkillInfo(int skillId)
        {
            return masterSkillInfo.FirstOrDefault( e => e.SkillId == skillId);
        }

        public MasterBossInfo? LoadMasterBossInfo(int bossId)
        {
            return masterBossInfo.FirstOrDefault( e => e.BossId == bossId);
        }

        public MasterBossInfo[] LoadAllMasterBossInfo()
        {
            return masterBossInfo;
        }

        public MasterClassInitStat? LoadClassInitStat(int classId)
        {
            return masterClassInitStat.FirstOrDefault( e => e.ClassId == classId);
        }

        public MasterClassLevelupStat? LoadClassLevelupStat(int classId)
        {
            return masterClassLevelupStat.FirstOrDefault( e => e.ClassId == classId);
        }

        void Load()
        {
            try
            {
                // 보스 정보를 가지고 옴
                masterBossInfo = (queryFactory.Query("master_boss_info").Get<MasterBossInfo>()).ToArray();
                masterClassInitStat = (queryFactory.Query("master_class_init_stat").Get<MasterClassInitStat>()).ToArray();
                masterClassLevelupStat = (queryFactory.Query("master_class_levelup_stat").Get<MasterClassLevelupStat>()).ToArray();
                masterClassSkillInfo = (queryFactory.Query("master_class_skill_info").Get<MasterClassSkillInfo>()).ToArray();
                masterSkillInfo = (queryFactory.Query("master_skill_info").Get<MasterSkillInfo>()).ToArray();
            }

            catch(Exception ex)
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
                queryFactory = new QueryFactory(dbConnection, compiler);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
