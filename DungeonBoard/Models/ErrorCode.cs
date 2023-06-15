namespace DungeonBoard.Models
{
    public enum ErrorCode
    {
        /* Common  0 ~ 100 */ 
        None = 0,
        CannotConnectServer = 1,
        InValidRequestHttpBody = 2,
        InvalidJsonFormat = 3,

        /* Account 101 ~ 200 */
        NoneExistEmail = 101, 
        InvalidPassword = 102,
        AlreadyExistEmail = 103,
        NoneExistUserIdInRedis = 104,
        InvalidAuthToken = 105,
        MissmatchPassword = 106,
        NoneExistUserId = 107,
        CannotCreatePlayer = 108,

        /* ROOM  201 ~ 300 */
        AlreadyInRoom = 201,
        NoneExistRoom = 202,
        InvalidBossId = 203, 
        InvalidHeadCount = 204,
        AlreadyFullRoom = 205,
        AlreadyPlayRoom = 206,
        NoneHostThisRoom = 207,
        NoneExistInThisRoom = 208,
        IsNotPlayingThisRoom = 209,

        /* MasterData  301 ~ 400 */
        NoneExistClassId = 301,

        /* Player  401 ~ 500 */
        PlayerAlreadyHasClass = 401,
        FailedUpdateClass = 402,
        PlayerIsNotInRoom = 403,
        PlayerIsNotPlaying = 404,

        /* Game 501 ~ 600 */ 
        FailedStoreGameInfoInMemory = 501,
        InvalidDiceNumber = 502,
        IsNotDiceTurn = 503,
        NoneExistSkill = 504,
        InvalidSkill = 505,
        NotEnoughMp = 506,
        
    }
}
