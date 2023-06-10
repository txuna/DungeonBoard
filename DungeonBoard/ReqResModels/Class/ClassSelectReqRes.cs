using DungeonBoard.Models;
using System.ComponentModel.DataAnnotations;

namespace DungeonBoard.ReqResModels.Class
{
    public class ClassSelectRequest
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string AuthToken { get; set; }
        [Required]
        public int ClassId { get; set; }
        
    }

    public class ClassSelectResponse
    {
        public ErrorCode Result { get; set; }
    }
}
