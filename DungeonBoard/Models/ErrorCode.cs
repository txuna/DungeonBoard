﻿namespace DungeonBoard.Models
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
    }
}