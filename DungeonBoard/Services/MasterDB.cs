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
    }

    public class MasterDB : IMasterDB
    {
        MasterBossInfo[] masterBossInfo; 

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

        public MasterBossInfo? LoadMasterBossInfo(int bossId)
        {
            return masterBossInfo.First( e => e.BossId == bossId);
        }

        public MasterBossInfo[] LoadAllMasterBossInfo()
        {
            return masterBossInfo;
        }

        void Load()
        {
            try
            {
                // 보스 정보를 가지고 옴
                masterBossInfo = (queryFactory.Query("master_boss_info").Get<MasterBossInfo>()).ToArray();
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
