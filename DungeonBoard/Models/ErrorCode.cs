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

        /* ROOM  201 ~ 300 */
        AlreadyInRoom = 201,
        NoneExistRoom = 202,
        InvalidBossId = 203, 
        InvalidHeadCount = 204,
        AlreadyFullRoom = 205,
    }
}
