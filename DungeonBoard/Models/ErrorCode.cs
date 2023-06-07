namespace DungeonBoard.Models
{
    public enum ErrorCode
    {
        /* Common  0 ~ 100 */ 
        None = 0,
        CannotConnectServer = 1,

        /* Account 101 ~ 200 */
        NoneExistEmail = 101, 
        InvalidPassword = 102,
    }
}
